#nullable enable
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;
using Microsoft.Maui.Graphics.Win2D;
using BoxRenderer = Microsoft.Maui.Controls.Compatibility.Platform.UWP.BoxViewBorderRenderer;
using CellRenderer = Microsoft.Maui.Controls.Compatibility.Platform.UWP.TextCellRenderer;
using ResourcesProvider = Microsoft.Maui.Controls.Compatibility.Platform.UWP.WindowsResourcesProvider;
using StreamImagesourceHandler = Microsoft.Maui.Controls.Compatibility.Platform.UWP.StreamImageSourceHandler;
using ImageLoaderSourceHandler = Microsoft.Maui.Controls.Compatibility.Platform.UWP.UriImageSourceHandler;
using DefaultRenderer = Microsoft.Maui.Controls.Compatibility.Platform.UWP.DefaultRenderer;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.Maui.Controls.Compatibility;
using System;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls.Hosting
{
	public static partial class AppHostBuilderExtensions
	{
		internal static MauiAppBuilder ConfigureCompatibilityLifecycleEvents(this MauiAppBuilder builder) =>
			builder.ConfigureLifecycleEvents(events => events.AddWindows(OnConfigureLifeCycle));

		static void OnConfigureLifeCycle(IWindowsLifecycleBuilder windows)
		{
			windows.OnLaunching((app, args) =>
			{
				// This is the initial Init to set up any system services registered by
				// Forms.Init(). This happens before any UI has appeared.
				// This creates a dummy MauiContext.
				// We need to call this so the Window and Root Page can new up successfully
				// The dispatcher that's inside of Forms.Init needs to be setup before the initial
				// window and root page start creating.
				// Inside OnLaunched we grab the MauiContext that's on the window so we can have the correct
				// MauiContext inside Forms

				var services = MauiWinUIApplication.Current.Services;
				var mauiContext = new MauiContext(services);
				var state = new ActivationState(mauiContext, args);
				Forms.Init(state, new InitializationOptions { Flags = InitializationFlags.SkipRenderers });
			})
			.OnMauiContextCreated((mauiContext) =>
			{
				// This is the final Init that sets up the real context from the application.

				var state = new ActivationState(mauiContext);
				Forms.Init(state);
			});
		}
	}
}
