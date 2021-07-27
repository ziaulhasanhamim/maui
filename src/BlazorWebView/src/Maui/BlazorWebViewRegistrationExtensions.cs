using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public static class BlazorWebViewRegistrationExtensions
	{
		public static MauiAppBuilder RegisterBlazorMauiWebView(this MauiAppBuilder appHostBuilder)
		{
			var entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly == null)
			{
				throw new InvalidOperationException(
					$"The entry assembly for this application could not be found for loading static web assets. " +
					$"Call the overload of {nameof(RegisterBlazorMauiWebView)} that accepts a parameter to specify the assembly in the project that contains static web assets.");
			}
			return RegisterBlazorMauiWebView(appHostBuilder, entryAssembly);
		}

		public static MauiAppBuilder RegisterBlazorMauiWebView(this MauiAppBuilder appHostBuilder, Assembly assetsAssembly)
		{
			if (appHostBuilder is null)
			{
				throw new ArgumentNullException(nameof(appHostBuilder));
			}
			if (assetsAssembly is null)
			{
				throw new ArgumentNullException(nameof(assetsAssembly));
			}

			appHostBuilder.Services.AddSingleton(new BlazorAssetsAssemblyConfiguration(assetsAssembly));
			appHostBuilder.ConfigureMauiHandlers((_, handlers) => handlers.AddHandler<IBlazorWebView, BlazorWebViewHandler>());

			return appHostBuilder;
		}
	}
}
