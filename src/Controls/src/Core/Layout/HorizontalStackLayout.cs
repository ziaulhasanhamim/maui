using Microsoft.Maui.Layouts;

namespace Microsoft.Maui.Controls
{
	public class HorizontalStackLayout : StackBase
	{
		protected override ILayoutManager CreateLayoutManager() => new HorizontalStackLayoutManager(this);
	}
}
