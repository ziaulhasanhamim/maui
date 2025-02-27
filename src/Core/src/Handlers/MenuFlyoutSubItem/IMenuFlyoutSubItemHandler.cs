﻿#if IOS || MACCATALYST
using PlatformView = UIKit.UIMenu;
#elif MONOANDROID
using PlatformView = Android.Views.View;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.Controls.MenuFlyoutSubItem;
#elif NETSTANDARD || (NET6_0 && !IOS && !ANDROID)
using PlatformView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
	public interface IMenuFlyoutSubItemHandler : IElementHandler
	{
		void Add(IMenuElement view);
		void Remove(IMenuElement view);
		void Clear();
		void Insert(int index, IMenuElement view);
		new PlatformView PlatformView { get; }
		new IMenuFlyoutSubItem VirtualView { get; }
	}
}
