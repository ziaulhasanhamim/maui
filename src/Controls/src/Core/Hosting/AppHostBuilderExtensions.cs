using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls.Hosting
{
	public static partial class AppHostBuilderExtensions
	{
		public static IMauiHandlersCollection AddMauiControlsHandlers(this IMauiHandlersCollection handlers)
		{
			return handlers
#if WINDOWS || __ANDROID__
				.AddHandler<Shell, ShellHandler>()
#endif
#if __ANDROID__ || __IOS__
				.AddHandler<RefreshView, RefreshViewHandler>()
#endif
				//.AddHandler<NavigationPage, NavigationPageHandler>()
				.AddHandler<ActivityIndicator, ActivityIndicatorHandler>()
				.AddHandler<Button, ButtonHandler>()
				.AddHandler<CheckBox, CheckBoxHandler>()
				.AddHandler<DatePicker, DatePickerHandler>()
				.AddHandler<Editor, EditorHandler>()
				.AddHandler<Entry, EntryHandler>()
				.AddHandler<GraphicsView, GraphicsViewHandler>()
				.AddHandler<Image, ImageHandler>()
				.AddHandler<Label, LabelHandler>()
				.AddHandler<Layout, LayoutHandler>()
				.AddHandler<Picker, PickerHandler>()
				.AddHandler<ProgressBar, ProgressBarHandler>()
				.AddHandler<ScrollView, ScrollViewHandler>()
				.AddHandler<SearchBar, SearchBarHandler>()
				.AddHandler<Slider, SliderHandler>()
				.AddHandler<Stepper, StepperHandler>()
				.AddHandler<Switch, SwitchHandler>()
				.AddHandler<TimePicker, TimePickerHandler>()
				.AddHandler<Page, PageHandler>()
				.AddHandler<Shapes.Ellipse, ShapeViewHandler>()
				.AddHandler<Shapes.Line, ShapeViewHandler>()
				.AddHandler<Shapes.Path, ShapeViewHandler>()
				.AddHandler<Shapes.Polygon, ShapeViewHandler>()
				.AddHandler<Shapes.Polyline, ShapeViewHandler>()
				.AddHandler<Shapes.Rectangle, ShapeViewHandler>()
				.AddHandler<Window, WindowHandler>();
		}
	}
}
