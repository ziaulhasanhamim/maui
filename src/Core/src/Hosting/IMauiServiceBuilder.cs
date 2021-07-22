using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Maui.Hosting
{
	public interface IMauiServiceBuilder
	{
		void ConfigureServices(HostBuilderContext context, IServiceCollection services);
	}

	public interface IMauiInitializeService
	{
		void Initialize(HostBuilderContext context, IServiceProvider services);
	}

	public static class InitServiceExtensions
	{
		public static void InitThis(this IServiceCollection services, IMauiInitializeService initService)
		{
			services.AddSingleton<IMauiInitializeService>(initService);
		}
	}
}