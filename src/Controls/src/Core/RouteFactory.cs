using System;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../docs/Microsoft.Maui.Controls/RouteFactory.xml" path="Type[@FullName='Microsoft.Maui.Controls.RouteFactory']/Docs" />
	public abstract class RouteFactory
	{
		/// <include file="../../docs/Microsoft.Maui.Controls/RouteFactory.xml" path="//Member[@MemberName='GetOrCreate']/Docs" />
		public abstract Element GetOrCreate();
		/// <include file="../../docs/Microsoft.Maui.Controls/RouteFactory.xml" path="//Member[@MemberName='GetOrCreate']/Docs" />
		public abstract Element GetOrCreate(IServiceProvider services);
	}
}