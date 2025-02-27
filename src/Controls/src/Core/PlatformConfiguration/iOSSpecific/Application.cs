namespace Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific
{
	using FormsElement = Maui.Controls.Application;

	/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="Type[@FullName='Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Application']/Docs" />
	public static class Application
	{
		#region PanGestureRecognizerShouldRecognizeSimultaneously
		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='PanGestureRecognizerShouldRecognizeSimultaneouslyProperty']/Docs" />
		public static readonly BindableProperty PanGestureRecognizerShouldRecognizeSimultaneouslyProperty = BindableProperty.Create("PanGestureRecognizerShouldRecognizeSimultaneously", typeof(bool), typeof(Application), false);

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='GetPanGestureRecognizerShouldRecognizeSimultaneously'][0]/Docs" />
		public static bool GetPanGestureRecognizerShouldRecognizeSimultaneously(BindableObject element)
		{
			return (bool)element.GetValue(PanGestureRecognizerShouldRecognizeSimultaneouslyProperty);
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='SetPanGestureRecognizerShouldRecognizeSimultaneously'][0]/Docs" />
		public static void SetPanGestureRecognizerShouldRecognizeSimultaneously(BindableObject element, bool value)
		{
			element.SetValue(PanGestureRecognizerShouldRecognizeSimultaneouslyProperty, value);
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='GetPanGestureRecognizerShouldRecognizeSimultaneously']/Docs" />
		public static bool GetPanGestureRecognizerShouldRecognizeSimultaneously(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetPanGestureRecognizerShouldRecognizeSimultaneously(config.Element);
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='SetPanGestureRecognizerShouldRecognizeSimultaneously']/Docs" />
		public static IPlatformElementConfiguration<iOS, FormsElement> SetPanGestureRecognizerShouldRecognizeSimultaneously(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetPanGestureRecognizerShouldRecognizeSimultaneously(config.Element, value);
			return config;
		}
		#endregion

		#region HandleControlUpdatesOnMainThread
		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='HandleControlUpdatesOnMainThreadProperty']/Docs" />
		public static readonly BindableProperty HandleControlUpdatesOnMainThreadProperty = BindableProperty.Create("HandleControlUpdatesOnMainThread", typeof(bool), typeof(Application), false);

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='GetHandleControlUpdatesOnMainThread'][0]/Docs" />
		public static bool GetHandleControlUpdatesOnMainThread(BindableObject element)
		{
			return (bool)element.GetValue(HandleControlUpdatesOnMainThreadProperty);
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='SetHandleControlUpdatesOnMainThread'][0]/Docs" />
		public static void SetHandleControlUpdatesOnMainThread(BindableObject element, bool value)
		{
			element.SetValue(HandleControlUpdatesOnMainThreadProperty, value);
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='GetHandleControlUpdatesOnMainThread']/Docs" />
		public static bool GetHandleControlUpdatesOnMainThread(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetHandleControlUpdatesOnMainThread(config.Element);
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='SetHandleControlUpdatesOnMainThread']/Docs" />
		public static IPlatformElementConfiguration<iOS, FormsElement> SetHandleControlUpdatesOnMainThread(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetHandleControlUpdatesOnMainThread(config.Element, value);
			return config;
		}
		#endregion

		#region EnableAccessibilityScalingForNamedFontSizes
		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='EnableAccessibilityScalingForNamedFontSizesProperty']/Docs" />
		public static readonly BindableProperty EnableAccessibilityScalingForNamedFontSizesProperty = BindableProperty.Create("EnableAccessibilityScalingForNamedFontSizes", typeof(bool), typeof(Application), true);

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='GetEnableAccessibilityScalingForNamedFontSizes'][0]/Docs" />
		public static bool GetEnableAccessibilityScalingForNamedFontSizes(BindableObject element)
		{
			return (bool)element.GetValue(EnableAccessibilityScalingForNamedFontSizesProperty);
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='SetEnableAccessibilityScalingForNamedFontSizes'][0]/Docs" />
		public static void SetEnableAccessibilityScalingForNamedFontSizes(BindableObject element, bool value)
		{
			element.SetValue(EnableAccessibilityScalingForNamedFontSizesProperty, value);
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='GetEnableAccessibilityScalingForNamedFontSizes']/Docs" />
		public static bool GetEnableAccessibilityScalingForNamedFontSizes(this IPlatformElementConfiguration<iOS, FormsElement> config)
		{
			return GetEnableAccessibilityScalingForNamedFontSizes(config.Element);
		}

		/// <include file="../../../../docs/Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific/Application.xml" path="//Member[@MemberName='SetEnableAccessibilityScalingForNamedFontSizes']/Docs" />
		public static IPlatformElementConfiguration<iOS, FormsElement> SetEnableAccessibilityScalingForNamedFontSizes(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
		{
			SetEnableAccessibilityScalingForNamedFontSizes(config.Element, value);
			return config;
		}
		#endregion
	}
}
