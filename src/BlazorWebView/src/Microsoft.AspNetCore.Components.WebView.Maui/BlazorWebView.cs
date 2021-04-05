using System;
using System.Collections.ObjectModel;
using Microsoft.Maui;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	public class BlazorWebView : Microsoft.Maui.Controls.View, IBlazorWebView
	{
		private string? _source;

		public string? Source
		{
			get
			{
#if ANDROID
				Android.Util.Log.Info("eilon", "get_Source = " + _source);
#endif
				return _source;
			}
			set
			{
#if ANDROID
				Android.Util.Log.Info("eilon", "set_Source: " + value);
#endif
				_source = value;
			}
		}

		public string? HostPage
		{
			get;
			set;
		}

		public ObservableCollection<RootComponent> RootComponents { get; } = new();

		public IServiceProvider? Services { get; set; }
	}
}
