using System;
using Microsoft.Maui.Controls.Core.UnitTests;
using Microsoft.Maui.Essentials;
using NUnit.Framework;

namespace Microsoft.Maui.Controls.Xaml.UnitTests
{
	[TestFixture]
	public class Issue1594
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

		[Test]
		public void OnPlatformForButtonHeight()
		{
			var xaml = @"
				<Button 
					xmlns=""http://schemas.microsoft.com/dotnet/2021/maui"" 
					xmlns:x=""http://schemas.microsoft.com/winfx/2009/xaml"" 
					xmlns:sys=""clr-namespace:System;assembly=mscorlib""
					x:Name=""activateButton"" Text=""ACTIVATE NOW"" TextColor=""White"" BackgroundColor=""#00A0FF"">
				        <Button.HeightRequest>
				           <OnPlatform x:TypeArguments=""sys:Double"">
				                   <On Platform=""iOS"">33</On>
				                   <On Platform=""Android"">44</On>
				                   <On Platform=""UWP"">44</On>
				         	</OnPlatform>
				         </Button.HeightRequest>
				 </Button>";

			mockDeviceInfo.Platform = DevicePlatform.iOS;
			var button = new Button().LoadFromXaml(xaml);
			Assert.AreEqual(33, button.HeightRequest);

			mockDeviceInfo.Platform = DevicePlatform.Android;
			button = new Button().LoadFromXaml(xaml);
			Assert.AreEqual(44, button.HeightRequest);

			mockDeviceInfo.Platform = DevicePlatform.UWP;
			button = new Button().LoadFromXaml(xaml);
			Assert.AreEqual(44, button.HeightRequest);
		}
	}
}