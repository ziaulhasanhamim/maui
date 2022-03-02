﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Maui.Controls.Sample.Controls;
using Maui.Controls.Sample.Pages;
using Maui.Controls.Sample.Services;
using Maui.Controls.Sample.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility;
using Microsoft.Maui.Controls.Hosting;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.LifecycleEvents;

#if NET6_0_OR_GREATER
using Microsoft.AspNetCore.Components.WebView.Maui;
#endif

namespace Maui.Controls.Sample
{
	public class CustomButton : Button { }

	public static class MauiProgram
	{
		static bool UseMauiGraphicsSkia = false;

		enum PageType { Main, Blazor, Shell, Template, FlyoutPage, TabbedPage }
		readonly static PageType _pageType = PageType.Main;

		public static MauiApp CreateMauiApp()
		{
			var appBuilder = MauiApp.CreateBuilder();

			appBuilder.UseMauiApp<XamlApp>();
			var services = appBuilder.Services;

			if (UseMauiGraphicsSkia)
			{
				/*
				appBuilder.ConfigureMauiHandlers(handlers =>
				{
					handlers.AddHandler<GraphicsView, SkiaGraphicsViewHandler>();
					handlers.AddHandler<BoxView, SkiaShapeViewHandler>();
					handlers.AddHandler<Microsoft.Maui.Controls.Shapes.Ellipse, SkiaShapeViewHandler>();
					handlers.AddHandler<Microsoft.Maui.Controls.Shapes.Line, SkiaShapeViewHandler>();
					handlers.AddHandler<Microsoft.Maui.Controls.Shapes.Path, SkiaShapeViewHandler>();
					handlers.AddHandler<Microsoft.Maui.Controls.Shapes.Polygon, SkiaShapeViewHandler>();
					handlers.AddHandler<Microsoft.Maui.Controls.Shapes.Polyline, SkiaShapeViewHandler>();
					handlers.AddHandler<Microsoft.Maui.Controls.Shapes.Rectangle, SkiaShapeViewHandler>();
					handlers.AddHandler<Microsoft.Maui.Controls.Shapes.RoundRectangle, SkiaShapeViewHandler>();
				});
				*/
			}

			appBuilder
				.ConfigureMauiHandlers(handlers =>
				{
#if __ANDROID__
					handlers.AddCompatibilityRenderer(typeof(CustomButton),
						typeof(Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat.ButtonRenderer));
#elif __IOS__
					handlers.AddCompatibilityRenderer(typeof(CustomButton),
						typeof(Microsoft.Maui.Controls.Compatibility.Platform.iOS.ButtonRenderer));
#elif WINDOWS
					handlers.AddCompatibilityRenderer(typeof(CustomButton),
						typeof(Microsoft.Maui.Controls.Compatibility.Platform.UWP.ButtonRenderer));
#endif
				});

			// Use a "third party" library that brings in a massive amount of controls
			appBuilder.UseBordelessEntry();
			appBuilder.ConfigureEffects(builder =>
			{
				builder.Add<FocusRoutingEffect, FocusPlatformEffect>();
			});

			appBuilder.Configuration.AddInMemoryCollection(
				new Dictionary<string, string>
					{
						{"MyKey", "Dictionary MyKey Value"},
						{":Title", "Dictionary_Title"},
						{"Position:Name", "Dictionary_Name" },
						{"Logging:LogLevel:Default", "Warning"}
					});

#if NET6_0_OR_GREATER
			appBuilder
				.RegisterBlazorMauiWebView();
			services.AddBlazorWebView();
#endif

			services.AddLogging(logging =>
			{
#if WINDOWS
				logging.AddDebug();
#else
				logging.AddConsole();
#endif
			});

			services.AddSingleton<ITextService, TextService>();
			services.AddTransient<MainViewModel>();

			services.AddTransient<IWindow, Window>();
			services.AddTransient<CustomFlyoutPage, CustomFlyoutPage>();
			services.AddTransient<CustomNavigationPage, CustomNavigationPage>();

			services.AddTransient(
				serviceType: typeof(Page),
				implementationType: _pageType switch
				{
					PageType.Template => typeof(TemplatePage),
					PageType.Shell => typeof(AppShell),
					PageType.Main => typeof(CustomNavigationPage),
					PageType.FlyoutPage => typeof(CustomFlyoutPage),
					PageType.TabbedPage => typeof(Pages.TabbedPageGallery),
					PageType.Blazor =>
#if NET6_0_OR_GREATER
						typeof(BlazorPage),
#else
						throw new NotSupportedException("Blazor requires .NET 6 or higher."),
#endif
					_ => throw new Exception(),
				});

			appBuilder
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("Dokdo-Regular.ttf", "Dokdo");
					fonts.AddFont("LobsterTwo-Regular.ttf", "Lobster Two");
					fonts.AddFont("LobsterTwo-Bold.ttf", "Lobster Two Bold");
					fonts.AddFont("LobsterTwo-Italic.ttf", "Lobster Two Italic");
					fonts.AddFont("LobsterTwo-BoldItalic.ttf", "Lobster Two BoldItalic");
					fonts.AddFont("ionicons.ttf", "Ionicons");
					fonts.AddFont("SegoeUI.ttf", "Segoe UI");
					fonts.AddFont("SegoeUI-Bold.ttf", "Segoe UI Bold");
					fonts.AddFont("SegoeUI-Italic.ttf", "Segoe UI Italic");
					fonts.AddFont("SegoeUI-Bold-Italic.ttf", "Segoe UI Bold Italic");
				})
				.ConfigureEssentials(essentials =>
				{
					essentials
						.UseMapServiceToken("YOUR-KEY-HERE")
						.AddAppAction("test_action", "Test App Action")
						.AddAppAction("second_action", "Second App Action")
						.OnAppAction(appAction =>
						{
							Debug.WriteLine($"You seem to have arrived from a special place: {appAction.Title} ({appAction.Id})");
						});

					// TODO: Unpackaged apps need to know the package ID and local data locations
					if (AppInfo.PackagingModel == AppPackagingModel.Packaged)
						essentials.UseVersionTracking();
				})
				.ConfigureLifecycleEvents(events =>
				{
					events.AddEvent<Action<string>>("CustomEventName", value => LogEvent("CustomEventName"));

#if __ANDROID__
					// Log everything in this one
					events.AddAndroid(android => android
						.OnActivityResult((a, b, c, d) => LogEvent(nameof(AndroidLifecycle.OnActivityResult), b.ToString()))
						.OnBackPressed((a) => LogEvent(nameof(AndroidLifecycle.OnBackPressed)) && false)
						.OnConfigurationChanged((a, b) => LogEvent(nameof(AndroidLifecycle.OnConfigurationChanged)))
						.OnCreate((a, b) => LogEvent(nameof(AndroidLifecycle.OnCreate)))
						.OnDestroy((a) => LogEvent(nameof(AndroidLifecycle.OnDestroy)))
						.OnNewIntent((a, b) => LogEvent(nameof(AndroidLifecycle.OnNewIntent)))
						.OnPause((a) => LogEvent(nameof(AndroidLifecycle.OnPause)))
						.OnPostCreate((a, b) => LogEvent(nameof(AndroidLifecycle.OnPostCreate)))
						.OnPostResume((a) => LogEvent(nameof(AndroidLifecycle.OnPostResume)))
						.OnRequestPermissionsResult((a, b, c, d) => LogEvent(nameof(AndroidLifecycle.OnRequestPermissionsResult)))
						.OnRestart((a) => LogEvent(nameof(AndroidLifecycle.OnRestart)))
						.OnRestoreInstanceState((a, b) => LogEvent(nameof(AndroidLifecycle.OnRestoreInstanceState)))
						.OnResume((a) => LogEvent(nameof(AndroidLifecycle.OnResume)))
						.OnSaveInstanceState((a, b) => LogEvent(nameof(AndroidLifecycle.OnSaveInstanceState)))
						.OnStart((a) => LogEvent(nameof(AndroidLifecycle.OnStart)))
						.OnStop((a) => LogEvent(nameof(AndroidLifecycle.OnStop))));

					// Add some cool features/things
					var shouldPreventBack = 1;
					events.AddAndroid(android => android
						.OnResume(a =>
						{
							LogEvent(nameof(AndroidLifecycle.OnResume), "shortcut");
						})
						.OnBackPressed(a => LogEvent(nameof(AndroidLifecycle.OnBackPressed), "shortcut") && (shouldPreventBack-- > 0))
						.OnRestoreInstanceState((a, b) =>
						{
							LogEvent(nameof(AndroidLifecycle.OnRestoreInstanceState), "shortcut");

							Debug.WriteLine($"{b.GetString("test2", "fail")} == {b.GetBoolean("test", false)}");
						})
						.OnSaveInstanceState((a, b) =>
						{
							LogEvent(nameof(AndroidLifecycle.OnSaveInstanceState), "shortcut");

							b.PutBoolean("test", true);
							b.PutString("test2", "yay");
						}));
#elif __IOS__
					// Log everything in this one
					events.AddiOS(ios => ios
						.ContinueUserActivity((a, b, c) => LogEvent(nameof(iOSLifecycle.ContinueUserActivity)) && false)
						.DidEnterBackground((a) => LogEvent(nameof(iOSLifecycle.DidEnterBackground)))
						.FinishedLaunching((a, b) => LogEvent(nameof(iOSLifecycle.FinishedLaunching)) && true)
						.OnActivated((a) => LogEvent(nameof(iOSLifecycle.OnActivated)))
						.OnResignActivation((a) => LogEvent(nameof(iOSLifecycle.OnResignActivation)))
						.OpenUrl((a, b, c) => LogEvent(nameof(iOSLifecycle.OpenUrl)) && false)
						.PerformActionForShortcutItem((a, b, c) => LogEvent(nameof(iOSLifecycle.PerformActionForShortcutItem)))
						.WillEnterForeground((a) => LogEvent(nameof(iOSLifecycle.WillEnterForeground)))
						.WillTerminate((a) => LogEvent(nameof(iOSLifecycle.WillTerminate))));
#elif WINDOWS
					// Log everything in this one
					events.AddWindows(windows => windows
						//.OnNativeMessage((a, b) => LogEvent(nameof(WindowsLifecycle.OnNativeMessage)))
						.OnActivated((a, b) => LogEvent(nameof(WindowsLifecycle.OnActivated)))
						.OnClosed((a, b) => LogEvent(nameof(WindowsLifecycle.OnClosed)))
						.OnLaunched((a, b) => LogEvent(nameof(WindowsLifecycle.OnLaunched)))
						.OnVisibilityChanged((a, b) => LogEvent(nameof(WindowsLifecycle.OnVisibilityChanged))));
#endif

					static bool LogEvent(string eventName, string type = null)
					{
						Debug.WriteLine($"Lifecycle event: {eventName}{(type == null ? "" : $" ({type})")}");
						return true;
					}
				});

			// If someone wanted to completely turn off the CascadeInputTransparent behavior in their application, this next line would be an easy way to do it
			// Microsoft.Maui.Controls.Layout.ControlsLayoutMapper.ModifyMapping(nameof(Microsoft.Maui.Controls.Layout.CascadeInputTransparent), (_, _, _) => { });

			return appBuilder.Build();
		}
	}
}
