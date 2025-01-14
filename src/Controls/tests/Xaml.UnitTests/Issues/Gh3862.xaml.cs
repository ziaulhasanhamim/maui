using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Core.UnitTests;
using Microsoft.Maui.Essentials;
using Microsoft.Maui.Graphics;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	public partial class Gh3862 : ContentPage
	{
		public Gh3862()
		{
			InitializeComponent();
		}

		public Gh3862(bool useCompiledXaml)
		{
			//this stub will be replaced at compile time
		}

		[TestFixture]
		class Tests
		{
			MockDeviceInfo mockDeviceInfo;

			[SetUp]
			public void Setup()
			{
				DeviceInfo.SetCurrent(mockDeviceInfo = new MockDeviceInfo());
			}

			[TearDown]
			public void TearDown()
			{
				DeviceInfo.SetCurrent(null);
			}

			[TestCase(false), TestCase(true)]
			public void OnPlatformMarkupInStyle(bool useCompiledXaml)
			{
				mockDeviceInfo.Platform = DevicePlatform.iOS;
				var layout = new Gh3862(useCompiledXaml);
				Assert.That(layout.label.TextColor, Is.EqualTo(Colors.Pink));
				Assert.That(layout.label.IsVisible, Is.False);

				mockDeviceInfo.Platform = DevicePlatform.Android;

				layout = new Gh3862(useCompiledXaml);
				Assert.That(layout.label.IsVisible, Is.True);

			}
		}
	}
}
