using System;
using System.Threading.Tasks;
using CoreGraphics;
using Microsoft.Maui.DeviceTests.Stubs;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Handlers;
using ObjCRuntime;
using UIKit;
using Xunit;

namespace Microsoft.Maui.DeviceTests
{
	public partial class ImageButtonHandlerTests
	{
		[Fact(DisplayName = "Stroke Color Initializes Correctly")]
		public async Task StrokeColorInitializesCorrectly()
		{
			var xplatStrokeColor = Colors.CadetBlue;

			var imageButton = new ImageButtonStub
			{
				Height = 50,
				Width = 100,
				StrokeThickness = 2,
				StrokeColor = xplatStrokeColor
			};

			var expectedValue = xplatStrokeColor.ToPlatform();

			var values = await GetValueAsync(imageButton, (handler) =>
			{
				return new
				{
					ViewValue = imageButton.StrokeColor,
					PlatformViewValue = GetNativeStrokeColor(handler)
				};
			});

			Assert.Equal(xplatStrokeColor, values.ViewValue);
			Assert.Equal(expectedValue, values.PlatformViewValue);
		}

		UIButton GetPlatformImageButton(ImageButtonHandler imageButtonHandler) =>
			   imageButtonHandler.PlatformView;

		UIColor GetNativeStrokeColor(ImageButtonHandler imageButtonHandler)
		{
			var platformButton = GetPlatformImageButton(imageButtonHandler);

			if (platformButton.Layer != null)
				return new UIColor(platformButton.Layer.BorderColor);

			return UIColor.Clear;
		}

		Task PerformClick(IButton button)
		{
			return InvokeOnMainThreadAsync(() =>
			{
				GetPlatformImageButton(CreateHandler(button)).SendActionForControlEvents(UIControlEvent.TouchUpInside);
			});
		}
	}
}