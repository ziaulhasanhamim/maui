using System;
using Microsoft.Maui.Controls.Internals;
using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui.Controls.Platform
{
	public static class Extensions
	{
		public static UIModalPresentationStyle ToPlatformModalPresentationStyle(this PlatformConfiguration.iOSSpecific.UIModalPresentationStyle style)
		{
			switch (style)
			{
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FormSheet:
					return UIModalPresentationStyle.FormSheet;
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.FullScreen:
					return UIModalPresentationStyle.FullScreen;
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.Automatic:
					return UIModalPresentationStyle.Automatic;
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.OverFullScreen:
					return UIModalPresentationStyle.OverFullScreen;
				case PlatformConfiguration.iOSSpecific.UIModalPresentationStyle.PageSheet:
					return UIModalPresentationStyle.PageSheet;
				default:
					throw new ArgumentOutOfRangeException(nameof(style));
			}
		}

		internal static UISearchBarStyle ToPlatformSearchBarStyle(this PlatformConfiguration.iOSSpecific.UISearchBarStyle style)
		{
			switch (style)
			{
				case PlatformConfiguration.iOSSpecific.UISearchBarStyle.Default:
					return UISearchBarStyle.Default;
				case PlatformConfiguration.iOSSpecific.UISearchBarStyle.Prominent:
					return UISearchBarStyle.Prominent;
				case PlatformConfiguration.iOSSpecific.UISearchBarStyle.Minimal:
					return UISearchBarStyle.Minimal;
				default:
					throw new ArgumentOutOfRangeException(nameof(style));
			}
		}

		internal static bool IsHorizontal(this Button.ButtonContentLayout layout) =>
			layout.Position == Button.ButtonContentLayout.ImagePosition.Left ||
			layout.Position == Button.ButtonContentLayout.ImagePosition.Right;
	}
}