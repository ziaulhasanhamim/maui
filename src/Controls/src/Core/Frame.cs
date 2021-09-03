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

		// Normally we'd have to account for padding in the measure/arrange steps. But since we're doing the weird thing
		// of shifting the padding to the content's margin, it's already accounted for by the measure/arrange of the Content
		// So we need to override CPM and CPA here to deliberately ignore Padding

		Size IContentView.CrossPlatformMeasure(double widthConstraint, double heightConstraint)
		{
			var content = (this as IContentView).PresentedContent;

			var contentSize = Size.Zero;

			if (content != null)
			{
				contentSize = content.Measure(widthConstraint, heightConstraint);
			}

			return contentSize;
		}

		Size IContentView.CrossPlatformArrange(Rectangle bounds)
		{
			var contentView = this as IContentView;

			if (contentView.PresentedContent == null)
			{
				return bounds.Size;
			}

			_ = contentView.PresentedContent.Arrange(bounds);

			return bounds.Size;
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
				Aspect = Stretch.Fill,
				Background = new SolidColorBrush(Colors.Transparent)
			};

			BindBrushToTemplatedParentColor(rectangle, Shapes.Shape.StrokeProperty, BorderColorProperty);
			BindToTemplatedParentProperty(rectangle, Shapes.Shape.FillProperty, BackgroundProperty);
			BindToTemplatedParentProperty(rectangle, Shapes.Shape.StrokeThicknessProperty, BorderElement.BorderWidthProperty);
			BindToTemplatedParentProperty(rectangle, Shapes.Rectangle.RadiusXProperty, CornerRadiusProperty);
			BindToTemplatedParentProperty(rectangle, Shapes.Rectangle.RadiusYProperty, CornerRadiusProperty);

			grid.Add(rectangle);

			var contentPresenter = new ContentPresenter();
			BindToTemplatedParentProperty(contentPresenter, MarginProperty, PaddingProperty);

			// To achieve the padding for the Frame inside of the rectangle, we substitute the Padding from the Frame
			// for the Margin of the Frame's Content. But we also need to account for the BorderWidth. So we'll bind the Content
			// Margin to both the Padding and the BorderWidth and combine them.
			var paddingAndBorderWidthBinding = new MultiBinding()
			{
				Bindings = new Collection<BindingBase>() {
						new Binding(PaddingProperty.PropertyName, source: RelativeBindingSource.TemplatedParent),
						new Binding(BorderWidthProperty.PropertyName, source: RelativeBindingSource.TemplatedParent)
					},
				Converter = new PaddingConverter()
			};

			contentPresenter.SetBinding(MarginProperty, paddingAndBorderWidthBinding);

			grid.Add(contentPresenter);

			INameScope nameScope = new NameScope();
			NameScope.SetNameScope(grid, nameScope);
			nameScope.RegisterName(TemplateRootName, grid);
			nameScope.RegisterName("ContentPresenter", contentPresenter);

			return grid;
		}

		class PaddingConverter : IMultiValueConverter
		{
			public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
			{
				// values[0] is the Padding 
				// values[1] is the BorderWidth

				if (values[0] == null || values[1] == null)
				{
					return Thickness.Zero;
				}

				var padding = (Thickness)values[0];
				var borderWidth = (double)values[1];

				var fullPadding = new Thickness(padding.Left + borderWidth, padding.Top + borderWidth,
					padding.Right + borderWidth, padding.Bottom + borderWidth);

				return fullPadding;
			}

			public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
	}
}