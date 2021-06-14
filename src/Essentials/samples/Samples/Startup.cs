using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Hosting;

namespace Samples
{
	public class Startup : IStartup
	{
		public void Configure(IAppHostBuilder appBuilder)
		{
			appBuilder
				.UseFormsCompatibility(scanAllAssemblies:true)
				.UseMauiControlsHandlers()
				.UseMauiApp<App>();

		}
	}
}
