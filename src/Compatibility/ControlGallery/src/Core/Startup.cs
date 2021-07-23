using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls.Compatibility.ControlGallery
{
	public static class MauiProgram
	{
		internal static bool UseBlazor = false;

		public static MauiAppBuilder CreateAppBuilder()
		{
			var builder = MauiAppBuilder.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureMauiHandlers(handlers =>
				{
					handlers.AddCompatibilityRenderers(Device.GetAssemblies());
				})
				.ConfigureImageSources(sources =>
				{
					sources.AddCompatibilityServices(Device.GetAssemblies());
				})
				.ConfigureFonts(fonts =>
				{
					fonts.AddCompatibilityFonts(Device.GetAssemblies());
				})
				.ConfigureEffects(effects =>
				{
					effects.AddCompatibilityEffects(Device.GetAssemblies());
				});

			// TODO: This used to be called inside a callback from ConfigureServices, but doesn't actually use any services. Was it just to delay execution to be later in app startup?
			DependencyService.Register(Device.GetAssemblies());

			return builder;
		}
	}
}
