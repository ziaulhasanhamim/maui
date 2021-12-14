using System;
using Microsoft.Maui.Graphics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Microsoft.Maui.Handlers
{
	public partial class LayoutHandler : ViewHandler<ILayout, LayoutPanel>
	{
		public void Add(IView child)
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			var targetIndex = VirtualView.GetLayoutHandlerIndex(child);
			NativeView.Children.Insert(targetIndex, child.ToNative(MauiContext, true));
		}

		public override void SetVirtualView(IView view)
		{
			base.SetVirtualView(view);

			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			NativeView.CrossPlatformMeasure = VirtualView.CrossPlatformMeasure;
			NativeView.CrossPlatformArrange = VirtualView.CrossPlatformArrange;

			NativeView.Children.Clear();

			foreach (var child in VirtualView.OrderByZIndex())
			{
				NativeView.Children.Add(child.ToNative(MauiContext, true));
			}
		}

		public void Remove(IView child)
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");

			if (child?.GetNative(true) is UIElement view)
			{
				NativeView.Children.Remove(view);
			}
		}

		public void Clear() 
		{
			NativeView?.Children.Clear();
		}

		public void Insert(int index, IView child)
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			var targetIndex = VirtualView.GetLayoutHandlerIndex(child);
			NativeView.Children.Insert(targetIndex, child.ToNative(MauiContext, true));
		}

		public void Update(int index, IView child) 
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			NativeView.Children[index] = child.ToNative(MauiContext, true);
			EnsureZIndexOrder(child);
		}

		public void UpdateZIndex(IView child) 
		{
			_ = NativeView ?? throw new InvalidOperationException($"{nameof(NativeView)} should have been set by base class.");
			_ = VirtualView ?? throw new InvalidOperationException($"{nameof(VirtualView)} should have been set by base class.");
			_ = MauiContext ?? throw new InvalidOperationException($"{nameof(MauiContext)} should have been set by base class.");

			EnsureZIndexOrder(child);
		}

		protected override LayoutPanel CreateNativeView()
		{
			if (VirtualView == null)
			{
				throw new InvalidOperationException($"{nameof(VirtualView)} must be set to create a LayoutViewGroup");
			}

			var view = new LayoutPanel
			{
				CrossPlatformMeasure = VirtualView.CrossPlatformMeasure,
				CrossPlatformArrange = VirtualView.CrossPlatformArrange,
			};

			return view;
		}

		protected override void DisconnectHandler(LayoutPanel nativeView)
		{
			// If we're being disconnected from the xplat element, then we should no longer be managing its children
			Clear();
			base.DisconnectHandler(nativeView);
		}

		void EnsureZIndexOrder(IView child) 
		{
			if (NativeView.Children.Count == 0)
			{
				return;
			}

			var currentIndex = NativeView.Children.IndexOf(child.ToNative(MauiContext!, true));

			if (currentIndex == -1)
			{
				return;
			}

			var targetIndex = VirtualView.GetLayoutHandlerIndex(child);
			
			if (currentIndex != targetIndex)
			{
				NativeView.Children.Move((uint)currentIndex, (uint)targetIndex);
			}
		}

		bool _clip = false;

		public override bool NeedsContainer =>
			_clip == false ||
			base.NeedsContainer;

		protected override void SetupContainer()
		{
			if (NativeView == null || ContainerView != null)
				return;

			var oldParent = (Panel?)NativeView.Parent;

			var oldIndex = oldParent?.Children.IndexOf(NativeView);
			oldParent?.Children.Remove(NativeView);

			ContainerView = new Canvas();
			ContainerView.Children.Add(NativeView);

			if (oldIndex is int idx && idx >= 0)
				oldParent?.Children.Insert(idx, ContainerView);
			else
				oldParent?.Children.Add(ContainerView);
		}

		//protected override void RemoveContainer()
		//{
		//	if (NativeView == null || ContainerView == null || NativeView.Parent != ContainerView)
		//		return;

		//	var oldParent = (Canvas?)ContainerView.Parent;

		//	var oldIndex = oldParent?.Children.IndexOf(ContainerView);
		//	oldParent?.Children.Remove(ContainerView);

		//	ContainerView.Child = null;
		//	ContainerView = null;

		//	if (oldIndex is int idx && idx >= 0)
		//		oldParent?.Children.Insert(idx, NativeView);
		//	else
		//		oldParent?.Children.Add(NativeView);
		//}
	}
}
