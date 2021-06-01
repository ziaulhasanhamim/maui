using System;
using Microsoft.AspNetCore.Components.WebView.WebView2;
using Microsoft.Extensions.FileProviders;
using WebView2Control = Microsoft.UI.Xaml.Controls.WebView2;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	internal sealed class WinUIWebViewManager : WebView2WebViewManager
	{
		private readonly WebView2Control _nativeWebView2;

		public WinUIWebViewManager(WebView2Control nativeWebView2, IWebView2Wrapper webview, IServiceProvider services, Dispatcher dispatcher, IFileProvider fileProvider, string hostPageRelativePath)
			: base(webview, services, dispatcher, fileProvider, hostPageRelativePath)
		{
			_nativeWebView2 = nativeWebView2;
		}

		protected override void QueueBlazorStart()
		{
			// In .NET MAUI Blazor is initialized explicitly (using autostart="false" on the script tag), so this
			// is where it's queued up to start after everything else loads.
			_nativeWebView2.CoreWebView2.DOMContentLoaded += async (_, __) =>
			{
				await _nativeWebView2.CoreWebView2!.ExecuteScriptAsync(@"
					Blazor.start();
					");
			};
		}
	}
}
