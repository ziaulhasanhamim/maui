using System;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.AspNetCore.Components.WebView.WebView2;
using Microsoft.Extensions.FileProviders;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using Microsoft.UI.Xaml.Controls;
using WebView2Control = Microsoft.UI.Xaml.Controls.WebView2;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
    public partial class BlazorWebViewHandler : AbstractViewHandler<IBlazorWebView, WebView2Control>
    {
		private WebView2WebViewManager? _webviewManager;

		protected override WebView2Control CreateNativeView()
		{

			//_webViewClient = GetWebViewClient();
			//aWebView.SetWebViewClient(_webViewClient);

			//_webChromeClient = GetWebChromeClient();
			//aWebView.SetWebChromeClient(_webChromeClient);
			return new WebView2Control();
		}

		protected override void DisconnectHandler(WebView2Control nativeView)
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
			if (TypedNativeView == null)
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

			_webviewManager = new WebView2WebViewManager(new WinUIWebView2Wrapper(TypedNativeView), Services!, MauiDispatcher.Instance, fileProvider, hostPageRelativePath);
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
			// TODO: Do OnImportantPropertyChanged event here
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
