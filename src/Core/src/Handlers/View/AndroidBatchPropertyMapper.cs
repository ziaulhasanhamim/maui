using System.Collections.Generic;
#if __IOS__ || MACCATALYST
using NativeView = UIKit.UIView;
#elif __ANDROID__
using NativeView = Android.Views.View;
#elif WINDOWS
using NativeView = Microsoft.UI.Xaml.FrameworkElement;
#elif NETSTANDARD
using NativeView = System.Object;
#endif

namespace Microsoft.Maui.Handlers
{
#if ANDROID
	public class AndroidBatchPropertyMapper<TVirtualView, TViewHandler> : PropertyMapper<TVirtualView, TViewHandler>
		where TVirtualView : IElement
		where TViewHandler : IElementHandler
	{
		const string BatchUpdateMethod = "Initialize";

		// During mass property updates, this list of properties will be skipped
		public static List<string> SkipList = new()
		{ 
			nameof(IView.AutomationId),
			nameof(IView.Visibility),
			nameof(IView.MinimumHeight),
			nameof(IView.MinimumWidth),
			nameof(IView.IsEnabled),
			nameof(IView.Opacity),
			nameof(IView.TranslationX),
			nameof(IView.TranslationY),
			nameof(IView.Scale),
			nameof(IView.ScaleX),
			nameof(IView.ScaleY),
			nameof(IView.Rotation),
			nameof(IView.RotationX),
			nameof(IView.RotationY),
			nameof(IView.AnchorX),
			nameof(IView.AnchorY),
		};

		public AndroidBatchPropertyMapper(params IPropertyMapper[] chained) : base(chained)
		{
			// Add a mapping for Initialize 
			Add(BatchUpdateMethod, Initialize);
		}

		void Initialize(TViewHandler viewHandler, TVirtualView virtualView) 
		{
			if (virtualView is not IView view)
			{
				return;
			}

			((NativeView?)viewHandler.NativeView)?.Initialize(view);
		}

		public override IEnumerable<string> GetKeys()
		{
			foreach (var key in _mapper.Keys)
			{
				// When reporting the key list for mass updates up the chain, ignore the stuff in SkipList;
				// it'll be handled by the batch update method instead
				if (SkipList.Contains(key))
				{
					continue;
				}

				yield return key;
			}

			if (Chained is not null)
			{
				foreach (var chain in Chained)
					foreach (var key in chain.GetKeys())
						yield return key;
			}
		}
	}
#endif
}
