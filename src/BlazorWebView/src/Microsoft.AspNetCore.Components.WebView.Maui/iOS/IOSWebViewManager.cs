using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using Foundation;
using Microsoft.Extensions.FileProviders;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using UIKit;
using WebKit;
using RectangleF = CoreGraphics.CGRect;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public partial class IOSWebViewManager : WebViewManager, IWKScriptMessageHandler
	{

		private sealed class WebViewScriptMessageHandler : NSObject, IWKScriptMessageHandler
		{
			private Action<Uri, string> _messageReceivedAction;
			
			public WebViewScriptMessageHandler(Action<Uri, string> messageReceivedAction)
			{
				_messageReceivedAction = messageReceivedAction ?? throw new ArgumentNullException(nameof(messageReceivedAction));
			}

			public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
			{
				if (message is null)
				{
					throw new ArgumentNullException(nameof(message));
				}
				_messageReceivedAction(new Uri(AppOrigin), ((NSString)message.Body).ToString());
			}
		}

		private const string AppOrigin = "app://0.0.0.0/";

		private readonly BlazorWebViewHandler _blazorMauiWebViewHandler;
		private readonly WKWebView _webview;

		// TODO: Is this an OK handle to use for IWKScriptMessageHandler.Handle?
		public IntPtr Handle => NSObject.FromObject(this).Handle;

		public IOSWebViewManager(BlazorWebViewHandler blazorMauiWebViewHandler, WKWebView webview, IServiceProvider services, Dispatcher dispatcher, IFileProvider fileProvider, string hostPageRelativePath)
			: base(services, dispatcher, new Uri(AppOrigin), fileProvider, hostPageRelativePath)
		{
			_blazorMauiWebViewHandler = blazorMauiWebViewHandler ?? throw new ArgumentNullException(nameof(blazorMauiWebViewHandler));
			_webview = webview ?? throw new ArgumentNullException(nameof(webview));

			InitializeWebView();
		}

		/// <inheritdoc />
		protected override void NavigateCore(Uri absoluteUri)
		{
			using var nsUrl = new NSUrl(absoluteUri.ToString());
			using var request = new NSUrlRequest(nsUrl);
			_webview.LoadRequest(request);
		}

		/// <inheritdoc />
		protected override void SendMessage(string message)
		{
			var messageJSStringLiteral = JavaScriptEncoder.Default.Encode(message);
			_webview.EvaluateJavaScript(
				javascript: $"__dispatchMessageCallback(\"{messageJSStringLiteral}\")",
				completionHandler: (NSObject result, NSError error) => { });
		}

		private void InitializeWebView()
		{
			var config = _webview.Configuration;
			config.Preferences.SetValueForKey(NSObject.FromObject(true), new NSString("developerExtrasEnabled"));

			var frameworkScriptSource = @"
		Blazor.start();

        (function () {
	        window.onpageshow = function(event) {
		        if (event.persisted) {
			        window.location.reload();
		        }
	        };
        })();
                    ";

			config.UserContentController.AddScriptMessageHandler(new WebViewScriptMessageHandler(MessageReceived), "webwindowinterop");
			config.UserContentController.AddUserScript(new WKUserScript(
				new NSString(frameworkScriptSource), WKUserScriptInjectionTime.AtDocumentEnd, true));

			// iOS WKWebView doesn't allow handling 'http'/'https' schemes, so we use the fake 'app' scheme
			config.SetUrlSchemeHandler(new SchemeHandler(this), urlScheme: "app");

			_webview.NavigationDelegate = new WebViewNavigationDelegate(_blazorMauiWebViewHandler);
		}

		public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
		{
			if (message is null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			MessageReceived(new Uri(AppOrigin), ((NSString)message.Body).ToString());
		}

		private class SchemeHandler : NSObject, IWKUrlSchemeHandler
		{
			private readonly IOSWebViewManager _webViewManager;

			public SchemeHandler(IOSWebViewManager webViewManager)
			{
				_webViewManager = webViewManager;
			}

			[Export("webView:startURLSchemeTask:")]
			public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
			{
				var responseBytes = GetResponseBytes(urlSchemeTask.Request.Url.AbsoluteString, out var contentType, statusCode: out var statusCode);
				if (statusCode == 200)
				{
					using (var dic = new NSMutableDictionary<NSString, NSString>())
					{
						dic.Add((NSString)"Content-Length", (NSString)(responseBytes.Length.ToString(CultureInfo.InvariantCulture)));
						dic.Add((NSString)"Content-Type", (NSString)contentType);
						// Disable local caching. This will prevent user scripts from executing correctly.
						dic.Add((NSString)"Cache-Control", (NSString)"no-cache, max-age=0, must-revalidate, no-store");
						using var response = new NSHttpUrlResponse(urlSchemeTask.Request.Url, statusCode, "HTTP/1.1", dic);
						urlSchemeTask.DidReceiveResponse(response);
					}
					urlSchemeTask.DidReceiveData(NSData.FromArray(responseBytes));
					urlSchemeTask.DidFinish();
				}
			}

			private byte[] GetResponseBytes(string url, out string contentType, out int statusCode)
			{
				if (_webViewManager.TryGetResponseContent(url, allowFallbackOnHostPage: true, out statusCode, out var statusMessage, out var content, out var headers))
				{
					statusCode = 200;
					using var ms = new MemoryStream();
					var uri = new Uri(url);

					content.CopyTo(ms);
					content.Dispose();

					var headersDict =
						new Dictionary<string, string>(
						headers
							.Split(Environment.NewLine)
							.Select(headerString =>
								new KeyValuePair<string, string>(
									headerString.Substring(0, headerString.IndexOf(':')),
									headerString.Substring(headerString.IndexOf(':') + 2))));

					contentType = headersDict["Content-Type"];

					return ms.ToArray();
				}
				else
				{
					statusCode = 404;
					contentType = string.Empty;
					return Array.Empty<byte>();
				}
			}

			[Export("webView:stopURLSchemeTask:")]
			public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
			{
			}
		}


		internal class WebViewNavigationDelegate : WKNavigationDelegate
		{
			private readonly BlazorWebViewHandler _webView;

			private WKNavigation? _currentNavigation;
			private Uri? _currentUri;

			public WebViewNavigationDelegate(BlazorWebViewHandler webView)
			{
				_webView = webView ?? throw new ArgumentNullException(nameof(webView));
			}

			public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
			{
				_currentNavigation = navigation;
			}

			public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
			{
				if (navigationAction.TargetFrame!.MainFrame)
				{
					_currentUri = navigationAction.Request.Url;
				}
				decisionHandler(WKNavigationActionPolicy.Allow);
			}

			public override void DidReceiveServerRedirectForProvisionalNavigation(WKWebView webView, WKNavigation navigation)
			{
				// We need to intercept the redirects to the app scheme because Safari will block them.
				// We will handle these redirects through the Navigation Manager.
				if (_currentUri?.Host == "0.0.0.0")
				{
					var uri = _currentUri;
					_currentUri = null;
					_currentNavigation = null;
					var request = new NSUrlRequest(uri);
					webView.LoadRequest(request);
				}
			}

			public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
			{
				_currentUri = null;
				_currentNavigation = null;
			}

			public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
			{
				_currentUri = null;
				_currentNavigation = null;
			}

			public override void DidCommitNavigation(WKWebView webView, WKNavigation navigation)
			{
				if (_currentUri != null && _currentNavigation == navigation)
				{
					//_webView.HandleNavigationStarting(_currentUri);
				}
			}

			public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
			{
				if (_currentUri != null && _currentNavigation == navigation)
				{
					//_webView.HandleNavigationFinished(_currentUri);
					_currentUri = null;
					_currentNavigation = null;
				}
			}
		}
	}
}
