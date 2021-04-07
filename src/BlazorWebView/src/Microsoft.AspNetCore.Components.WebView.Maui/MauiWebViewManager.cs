using System;
using Microsoft.Extensions.FileProviders;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public abstract class MauiWebViewManager : WebViewManager
	{
		public MauiWebViewManager(IServiceProvider services, Dispatcher dispatcher, Uri appOrigin, IFileProvider fileProvider, string hostPageRelativePath)
			: base(services, dispatcher, appOrigin, fileProvider, hostPageRelativePath)
		{
		}
	}
}
