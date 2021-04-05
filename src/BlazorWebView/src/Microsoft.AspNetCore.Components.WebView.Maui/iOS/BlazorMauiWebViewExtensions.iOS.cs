using System;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;
using UIKit;
using WebKit;
using RectangleF = CoreGraphics.CGRect;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
    public static class BlazorMauiWebViewExtensions
	{
		public static void UpdateSource(this WKWebView nativeWebView, IBlazorWebView webView)
		{
			nativeWebView.UpdateSource(webView, null);
		}

		public static void UpdateSource(this WKWebView nativeWebView, IBlazorWebView webView, IWebViewDelegate? webViewDelegate)
		{
			if (nativeWebView is null)
			{
				throw new ArgumentNullException(nameof(nativeWebView));
			}

			if (webViewDelegate != null)
				webView.Source = nativeWebView.Url?.AbsoluteString;
		}
	}
}
