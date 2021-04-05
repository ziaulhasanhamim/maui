using AWebView = Android.Webkit.WebView;
using Microsoft.Maui.Controls;
using Android.Util;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public static class BlazorWebViewExtensions
	{
		public static void UpdateSource(this AWebView nativeWebView, IBlazorWebView webView)
		{
			nativeWebView.UpdateSource(webView, null);
		}

		public static void UpdateSource(this AWebView nativeWebView, IBlazorWebView webView, IWebViewDelegate? webViewDelegate)
		{
			Log.Info("eilon", $"UpdateSource from native={nativeWebView.Url} to new={webView.Source}");
			if (webViewDelegate != null && webView.Source != null)
			{
				nativeWebView.LoadUrl(webView.Source);
			}
		}
	}
}
