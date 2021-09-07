using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Layouts;

namespace Microsoft.Maui.Controls
{
	[ContentProperty(nameof(Content))]
	public class Frame : ContentView, IElementConfiguration<Frame>, IPaddingElement, IBorderElement, IContentView
	{
		static ControlTemplate s_defaultTemplate;
		static bool? s_rendererAvailable;
		public const string TemplateRootName = "Root";

		public static readonly BindableProperty BorderColorProperty = BorderElement.BorderColorProperty;

		public static readonly BindableProperty BorderWidthProperty = BorderElement.BorderWidthProperty;

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

		public double BorderWidth
		{
			get { return (double)GetValue(BorderElement.BorderWidthProperty); }
			set { SetValue(BorderElement.BorderWidthProperty, value); }
		}

		public float CornerRadius
		{
			get { return (float)GetValue(CornerRadiusProperty); }
			set { SetValue(CornerRadiusProperty, value); }
		}

		int IBorderElement.CornerRadius => (int)CornerRadius;

		double IBorderElement.BorderWidth => BorderWidth;

		int IBorderElement.CornerRadiusDefaultValue => (int)CornerRadiusProperty.DefaultValue;

		Color IBorderElement.BorderColorDefaultValue => (Color)BorderColorProperty.DefaultValue;

		double IBorderElement.BorderWidthDefaultValue => 1d;

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

		bool IBorderElement.IsBorderWidthSet() => IsSet(BorderWidthProperty);

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
			ContentViewHandler.ContentViewMapper.ModifyMapping(BackgroundProperty.PropertyName, IgnoreForFrame);
		}

		static void IgnoreForFrame(ContentViewHandler handler, IContentView contentView, Action<IElementHandler, Maui.IElement> oldMapping)
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
				Background = new SolidColorBrush(Colors.Orange),
				ColumnDefinitions = new ColumnDefinitionCollection {
					new ColumnDefinition { Width = GridLength.Auto }
				},
				RowDefinitions = new RowDefinitionCollection {
					new RowDefinition { Height = GridLength.Auto }
				}
			};

			BindTemplatedParentProperties(grid, HorizontalOptionsProperty, HasShadowProperty,
				 OpacityProperty, RotationProperty, ScaleProperty, ScaleXProperty, ScaleYProperty,
				TranslationYProperty, TranslationXProperty, VerticalOptionsProperty);

			grid.SetBinding(MarginProperty, new Binding(PaddingProperty.PropertyName,
				source: RelativeBindingSource.TemplatedParent, converter: new PaddingInverter()));

			grid.SetBinding(PaddingProperty, new Binding(PaddingProperty.PropertyName,
				source: RelativeBindingSource.TemplatedParent));

			var rectangle = new DecoratorRectangle
			{
				Background = new SolidColorBrush(Colors.Transparent)
			};

			BindBrushToTemplatedParentColor(rectangle, Shapes.Shape.StrokeProperty, BorderColorProperty);
			BindToTemplatedParentProperty(rectangle, Shapes.Shape.FillProperty, BackgroundProperty);
			BindToTemplatedParentProperty(rectangle, Shapes.Shape.StrokeThicknessProperty, BorderElement.BorderWidthProperty);
			BindToTemplatedParentProperty(rectangle, DecoratorRectangle.RadiusXProperty, CornerRadiusProperty);
			BindToTemplatedParentProperty(rectangle, DecoratorRectangle.RadiusYProperty, CornerRadiusProperty);

			//rectangle.SetBinding(MarginProperty, new Binding(PaddingProperty.PropertyName,
			//	source: RelativeBindingSource.TemplatedParent, converter: new PaddingInverter()));

			var contentPresenter = new ContentPresenter();
			BindToTemplatedParentProperty(contentPresenter, MarginProperty, BorderElement.BorderWidthProperty);

			grid.Add(rectangle);
			grid.Add(contentPresenter);

			INameScope nameScope = new NameScope();
			NameScope.SetNameScope(grid, nameScope);
			nameScope.RegisterName(TemplateRootName, grid);
			nameScope.RegisterName("ContentPresenter", contentPresenter);

			return grid;
		}

		// Specialized rectangle for use here in Frame; it ignores measure calls (always Size.Zero)
		// so it doesn't affect the size of the Frame; only the content counts for measurement
		class DecoratorRectangle : Shapes.Shape
		{
			public DecoratorRectangle() : base()
			{
				Aspect = Stretch.Fill;
			}

			public static readonly BindableProperty RadiusXProperty =
				BindableProperty.Create(nameof(RadiusX), typeof(double), typeof(Rectangle), 0.0d);

			public static readonly BindableProperty RadiusYProperty =
				BindableProperty.Create(nameof(RadiusY), typeof(double), typeof(Rectangle), 0.0d);

			public double RadiusX
			{
				set { SetValue(RadiusXProperty, value); }
				get { return (double)GetValue(RadiusXProperty); }
			}

			public double RadiusY
			{
				set { SetValue(RadiusYProperty, value); }
				get { return (double)GetValue(RadiusYProperty); }
			}

			public override PathF GetPath()
			{
				var path = new PathF();

				float x = (float)StrokeThickness / 2;
				float y = (float)StrokeThickness / 2;
				float w = (float)(Width - StrokeThickness);
				float h = (float)(Height - StrokeThickness);
				float cornerRadius = (float)Math.Max(RadiusX, RadiusY);

				path.AppendRoundedRectangle(x, y, w, h, cornerRadius);
				return path;
			}

			protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
			{
				return Size.Zero;
			}
		}

		class PaddingInverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				var padding = (Thickness)value;

				return new Thickness(-padding.Left, -padding.Top, -padding.Right, -padding.Bottom);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
	}
}