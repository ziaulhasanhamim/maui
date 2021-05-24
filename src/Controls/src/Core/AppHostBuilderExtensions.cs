using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls.Hosting
{
	public static class AppHostBuilderExtensions
	{
		static readonly Dictionary<Type, Type> DefaultMauiControlHandlers = new()
		{
			{ typeof(NavigationPage), typeof(NavigationPageHandler) },
			{ typeof(Shell), typeof(ShellHandler) },
			{ typeof(ActivityIndicator), typeof(ActivityIndicatorHandler) },
			{ typeof(Button), typeof(ButtonHandler) },
			{ typeof(CheckBox), typeof(CheckBoxHandler) },
			{ typeof(DatePicker), typeof(DatePickerHandler) },
			{ typeof(Editor), typeof(EditorHandler) },
			{ typeof(Entry), typeof(EntryHandler) },
			{ typeof(Image), typeof(ImageHandler) },
			{ typeof(Label), typeof(LabelHandler) },
			{ typeof(Layout), typeof(LayoutHandler) },
			{ typeof(Picker), typeof(PickerHandler) },
			{ typeof(ProgressBar), typeof(ProgressBarHandler) },
			{ typeof(SearchBar), typeof(SearchBarHandler) },
			{ typeof(Slider), typeof(SliderHandler) },
			{ typeof(Stepper), typeof(StepperHandler) },
			{ typeof(Switch), typeof(SwitchHandler) },
			{ typeof(TimePicker), typeof(TimePickerHandler) },
			{ typeof(Page), typeof(PageHandler) },
		};

		public static IAppHostBuilder UseMauiControlsHandlers(this IAppHostBuilder builder)
		{
			return builder
				.ConfigureMauiHandlers((_, handlersCollection) =>
				{
					handlersCollection.AddHandlers(DefaultMauiControlHandlers);
				})
				.ConfigureServices(ConfigureNativeServices);
		}

		private static void ConfigureNativeServices(HostBuilderContext arg1, IServiceCollection arg2)
		{
#if WINDOWS
			if (!UI.Xaml.Application.Current.Resources.ContainsKey("MauiControlsPageControlStyle"))
			{
				var myResourceDictionary = new Microsoft.UI.Xaml.ResourceDictionary();
				myResourceDictionary.Source = new Uri("ms-appx:///Microsoft.Maui.Controls/Platform/Windows/Styles/Resources.xbf");
				Microsoft.UI.Xaml.Application.Current.Resources.MergedDictionaries.Add(myResourceDictionary);
			}
#endif
		}
	}
}
