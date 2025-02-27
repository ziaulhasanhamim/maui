#nullable enable
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.Maui.Handlers
{
	public partial class LabelHandler : ViewHandler<ILabel, TextBlock>
	{
		protected override TextBlock CreatePlatformView() => new TextBlock();

		public override bool NeedsContainer =>
			VirtualView?.Background != null ||
			base.NeedsContainer;

		public static void MapBackground(ILabelHandler handler, ILabel label)
		{
			handler.UpdateValue(nameof(IViewHandler.ContainerView));

			handler.ToPlatform().UpdateBackground(label);
		}

		public static void MapOpacity(ILabelHandler handler, ILabel label)
		{
			handler.UpdateValue(nameof(IViewHandler.ContainerView));
			handler.PlatformView.UpdateOpacity(label);
			handler.ToPlatform().UpdateOpacity(label);
		}

		public static void MapText(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdateText(label);

		public static void MapTextColor(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdateTextColor(label);

		public static void MapCharacterSpacing(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdateCharacterSpacing(label);

		public static void MapFont(ILabelHandler handler, ILabel label)
		{
			var fontManager = handler.GetRequiredService<IFontManager>();

			handler.PlatformView?.UpdateFont(label, fontManager);
		}

		public static void MapHorizontalTextAlignment(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdateHorizontalTextAlignment(label);

		public static void MapVerticalTextAlignment(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdateVerticalTextAlignment(label);

		public static void MapLineBreakMode(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdateLineBreakMode(label);

		public static void MapTextDecorations(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdateTextDecorations(label);

		public static void MapMaxLines(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdateMaxLines(label);

		public static void MapPadding(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdatePadding(label);

		public static void MapLineHeight(ILabelHandler handler, ILabel label) =>
			handler.PlatformView?.UpdateLineHeight(label);
	}
}
