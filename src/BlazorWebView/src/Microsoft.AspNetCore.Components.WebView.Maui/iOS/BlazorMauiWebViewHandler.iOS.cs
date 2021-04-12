using System;
using System.Collections.ObjectModel;
using System.IO;
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
	public partial class BlazorWebViewHandler : ViewHandler<IBlazorWebView, WKWebView>
	{
		protected override WKWebView CreateNativeView()
		{
			return new WKWebView(RectangleF.Empty, new WKWebViewConfiguration())
			{
				BackgroundColor = UIColor.Clear,
				AutosizesSubviews = true
			};
		}

		public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
		{
			SetDesiredSize(widthConstraint, heightConstraint);

			return base.GetDesiredSize(widthConstraint, heightConstraint);
		}

		void SetDesiredSize(double width, double height)
		{
			if (NativeView != null)
			{
				var x = NativeView.Frame.X;
				var y = NativeView.Frame.Y;

				NativeView.Frame = new RectangleF(x, y, width, height);
			}
		}




		private IOSWebViewManager? _webviewManager;

		protected override void DisconnectHandler(WKWebView nativeView)
		{
			//nativeView.StopLoading();

			//_webViewClient?.Dispose();
			//_webChromeClient?.Dispose();
		}

		private bool RequiredStartupPropertiesSet =>
			//_webview != null &&
			HostPage != null &&
			Services != null;

		public string? HostPage { get; private set; }
		public ObservableCollection<RootComponent>? RootComponents { get; private set; }
		public new IServiceProvider? Services { get; private set; }

		private void StartWebViewCoreIfPossible()
		{
			if (!RequiredStartupPropertiesSet ||
				false)//_webviewManager != null)
			{
				return;
			}
			if (NativeView == null)
			{
				throw new InvalidOperationException($"Can't start {nameof(BlazorWebView)} without native web view instance.");
			}

			var resourceAssembly = RootComponents?[0]?.ComponentType?.Assembly;
			if (resourceAssembly == null)
			{
				throw new InvalidOperationException($"Can't start {nameof(BlazorWebView)} without a component type assembly.");
			}

			// We assume the host page is always in the root of the content directory, because it's
			// unclear there's any other use case. We can add more options later if so.
			var contentRootDir = Path.GetDirectoryName(HostPage) ?? string.Empty;
			var hostPageRelativePath = Path.GetRelativePath(contentRootDir, HostPage!);
			var fileProvider = new ManifestEmbeddedFileProvider(resourceAssembly, root: contentRootDir);

			_webviewManager = new IOSWebViewManager(this, NativeView, Services!, MauiDispatcher.Instance, fileProvider, hostPageRelativePath);
			if (RootComponents != null)
			{
				foreach (var rootComponent in RootComponents)
				{
					// Since the page isn't loaded yet, this will always complete synchronously
					_ = rootComponent.AddToWebViewManagerAsync(_webviewManager);
				}
			}
			_webviewManager.Navigate("/");
		}

		public static void MapHostPage(BlazorWebViewHandler handler, IBlazorWebView webView)
		{
			handler.HostPage = webView.HostPage;
			handler.StartWebViewCoreIfPossible();
		}

		public static void MapRootComponents(BlazorWebViewHandler handler, IBlazorWebView webView)
		{
			handler.RootComponents = webView.RootComponents;
			handler.StartWebViewCoreIfPossible();
		}

		public static void MapServices(BlazorWebViewHandler handler, IBlazorWebView webView)
		{
			handler.Services = webView.Services;
			handler.StartWebViewCoreIfPossible();
		}
	}
}
