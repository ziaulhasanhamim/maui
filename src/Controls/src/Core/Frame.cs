using System;
using System.Globalization;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Layouts;

namespace Microsoft.Maui.Controls
{
	[ContentProperty(nameof(Content))]
	public class Frame : ContentView, IElementConfiguration<Frame>, IPaddingElement, IBorderElement
	{
		static ControlTemplate s_defaultTemplate;
		static bool? s_rendererAvailable;
		public const string TemplateRootName = "Root";

		public static readonly BindableProperty BorderColorProperty = BorderElement.BorderColorProperty;

		public static readonly BindableProperty HasShadowProperty = BindableProperty.Create("HasShadow", typeof(bool), typeof(Frame), true);

		public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(nameof(CornerRadius), typeof(float), typeof(Frame), -1.0f,
									validateValue: (bindable, value) => ((float)value) == -1.0f || ((float)value) >= 0f);

		readonly Lazy<PlatformConfigurationRegistry<Frame>> _platformConfigurationRegistry;

		public Frame()
		{
			_platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<Frame>>(() => new PlatformConfigurationRegistry<Frame>(this));
			ControlTemplate = DefaultTemplate;
		}

		Thickness IPaddingElement.PaddingDefaultValueCreator()
		{
			return 20d;
		}

		public bool HasShadow
		{
			get { return (bool)GetValue(HasShadowProperty); }
			set { SetValue(HasShadowProperty, value); }
		}

		public Color BorderColor
		{
			get { return (Color)GetValue(BorderElement.BorderColorProperty); }
			set { SetValue(BorderElement.BorderColorProperty, value); }
		}

		public float CornerRadius
		{
			get { return (float)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		int IBorderElement.CornerRadius => (int)CornerRadius;

		// not currently used by frame
		double IBorderElement.BorderWidth => -1d;

		int IBorderElement.CornerRadiusDefaultValue => (int)CornerRadiusProperty.DefaultValue;

		Color IBorderElement.BorderColorDefaultValue => (Color)BorderColorProperty.DefaultValue;

		double IBorderElement.BorderWidthDefaultValue => -1d;

		public IPlatformElementConfiguration<T, Frame> On<T>() where T : IConfigPlatform
		{
			return _platformConfigurationRegistry.Value.On<T>();
		}

		void IBorderElement.OnBorderColorPropertyChanged(Color oldValue, Color newValue)
		{
		}

		bool IBorderElement.IsCornerRadiusSet() => IsSet(CornerRadiusProperty);

		bool IBorderElement.IsBackgroundColorSet() => IsSet(BackgroundColorProperty);

		bool IBorderElement.IsBackgroundSet() => IsSet(BackgroundProperty);

		bool IBorderElement.IsBorderColorSet() => IsSet(BorderColorProperty);

		bool IBorderElement.IsBorderWidthSet() => false;

		public override ControlTemplate ResolveControlTemplate()
		{
			_ = base.ResolveControlTemplate();

			if (!RendererAvailable)
			{
				ControlTemplate = DefaultTemplate;
			}

			return ControlTemplate;
		}

		static bool RendererAvailable
		{
			get
			{
				if (!s_rendererAvailable.HasValue)
				{
					s_rendererAvailable = Microsoft.Maui.Controls.Internals.Registrar.Registered.GetHandlerType(typeof(RadioButton)) != null;
				}

				return s_rendererAvailable.Value;
			}
		}

		public static ControlTemplate DefaultTemplate
		{
			get
			{
				if (s_defaultTemplate == null)
				{
					s_defaultTemplate = new ControlTemplate(() => BuildDefaultTemplate());
				}

				return s_defaultTemplate;
			}
		}

		public new static void RemapForControls()
		{
			// Could have done this

			//IPropertyMapper<IContentView, ContentViewHandler> ControlsContentViewMapper = new PropertyMapper<Frame, ContentViewHandler>(ContentViewHandler.ContentViewMapper)
			//{
			//	[nameof(Background)] = IgnoreBackground, // Empty method
			//};

			//ContentViewHandler.ContentViewMapper = ControlsContentViewMapper;

			// But what about something like this?
			ContentViewHandler.ContentViewMapper.ModifyMapping(BackgroundProperty.PropertyName, IgnoreBackgroundForFrame);
		}

		static void IgnoreBackgroundForFrame(ContentViewHandler handler, IContentView contentView, Action<IElementHandler, Maui.IElement> oldMapping)
		{
			if (contentView is not Controls.Frame)
			{
				oldMapping?.Invoke(handler, contentView);
			}
		}

		static View BuildDefaultTemplate()
		{
			var grid = new GridLayout
			{
				Background = new SolidColorBrush(Colors.Transparent),
				ColumnDefinitions = new ColumnDefinitionCollection {
					new ColumnDefinition { Width = GridLength.Star }
				},
				RowDefinitions = new RowDefinitionCollection {
					new RowDefinition { Height = GridLength.Star }
				}
			};

			BindTemplatedParentProperties(grid, HorizontalOptionsProperty, HasShadowProperty,
				 OpacityProperty, RotationProperty, ScaleProperty, ScaleXProperty, ScaleYProperty,
				TranslationYProperty, TranslationXProperty, VerticalOptionsProperty);

			var rectangle = new Shapes.Rectangle
			{
				//Aspect = Stretch.Fill
				Aspect = Stretch.Fill,
				Background = new SolidColorBrush(Colors.Transparent)
			};

			BindBrushToTemplatedParentColor(rectangle, Shapes.Shape.StrokeProperty, BorderColorProperty);
			BindToTemplatedParentProperty(rectangle, Shapes.Shape.FillProperty, BackgroundProperty);
			//BindToTemplatedParentProperty(rectangle, Shapes.Shape.StrokeThicknessProperty, BorderElement.BorderWidthProperty);
			rectangle.StrokeThickness = 10;
			BindToTemplatedParentProperty(rectangle, Shapes.Rectangle.RadiusXProperty, CornerRadiusProperty);
			BindToTemplatedParentProperty(rectangle, Shapes.Rectangle.RadiusYProperty, CornerRadiusProperty);

			grid.Add(rectangle);

			var contentPresenter = new ContentPresenter();
			//BindToTemplatedParentProperty(contentPresenter, MarginProperty, BorderElement.BorderWidthProperty);
			contentPresenter.Margin = 1;

			grid.Add(contentPresenter);

			INameScope nameScope = new NameScope();
			NameScope.SetNameScope(grid, nameScope);
			nameScope.RegisterName(TemplateRootName, grid);
			nameScope.RegisterName("ContentPresenter", contentPresenter);

			return grid;
		}
	}
}