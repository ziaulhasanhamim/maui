using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public partial class BlazorMauiWebViewHandler
	{
		public static PropertyMapper<IBlazorWebView, BlazorMauiWebViewHandler> WebViewMapper = new PropertyMapper<IBlazorWebView, BlazorMauiWebViewHandler>(ViewHandler.ViewMapper)
		{
			[nameof(IBlazorWebView.Source)] = MapSource,
			[nameof(IBlazorWebView.HostPage)] = MapHostPage,
			[nameof(IBlazorWebView.RootComponents)] = MapRootComponents,
			[nameof(IBlazorWebView.Services)] = MapServices,
		};

		public BlazorMauiWebViewHandler() : base(WebViewMapper)
		{
		}

		public BlazorMauiWebViewHandler(PropertyMapper mapper) : base(mapper ?? WebViewMapper)
		{
		}
	}
}
