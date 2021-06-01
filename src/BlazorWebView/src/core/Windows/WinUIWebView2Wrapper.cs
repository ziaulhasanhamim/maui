using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebView.WebView2;
using Microsoft.Web.WebView2.Core;
using Windows.Foundation;
using Windows.Storage.Streams;
using WebView2Control = Microsoft.UI.Xaml.Controls.WebView2;

namespace Microsoft.AspNetCore.Components.WebView.Maui
{
	internal class WinUIWebView2Wrapper : IWebView2Wrapper
	{
		private readonly WinUICoreWebView2Wrapper _coreWebView2Wrapper;

		public WinUIWebView2Wrapper(WebView2Control webView2)
		{
			if (webView2 is null)
			{
				throw new ArgumentNullException(nameof(webView2));
			}

			WebView2 = webView2;
			_coreWebView2Wrapper = new WinUICoreWebView2Wrapper(this);
		}

		public ICoreWebView2Wrapper CoreWebView2 => _coreWebView2Wrapper;

		public Uri Source
		{
			get => WebView2.Source;
			set => WebView2.Source = value;
		}

		public WebView2Control WebView2 { get; }

		public CoreWebView2Environment? Environment { get; set; }

		public Action AddAcceleratorKeyPressedHandler(EventHandler<ICoreWebView2AcceleratorKeyPressedEventArgsWrapper> eventHandler)
		{
			// This event is not supported in WinUI, so we ignore it
			return () => { };
		}

		public async Task CreateEnvironmentAsync()
		{
			Environment = await CoreWebView2Environment.CreateAsync();
		}

		public Task EnsureCoreWebView2Async()
		{
			// NOTE: WinUI WebView2 doesn't have a way to pass in a custom created environment.
			return WebView2.EnsureCoreWebView2Async().AsTask();
		}
	}

	internal class WinUICoreWebView2AcceleratorKeyPressedEventArgsWrapper : ICoreWebView2AcceleratorKeyPressedEventArgsWrapper
	{
		private readonly CoreWebView2AcceleratorKeyPressedEventArgs _eventArgs;

		public WinUICoreWebView2AcceleratorKeyPressedEventArgsWrapper(CoreWebView2AcceleratorKeyPressedEventArgs eventArgs)
		{
			_eventArgs = eventArgs;
		}
		public uint VirtualKey => _eventArgs.VirtualKey;

		public int KeyEventLParam => _eventArgs.KeyEventLParam;

		public bool Handled
		{
			get => _eventArgs.Handled;
			set => _eventArgs.Handled = value;
		}
	}

	internal class WinUICoreWebView2Wrapper : ICoreWebView2Wrapper
	{
		private readonly WinUIWebView2Wrapper _webView2;
		private WinUICoreWebView2SettingsWrapper? _settings;

		public WinUICoreWebView2Wrapper(WinUIWebView2Wrapper webView2)
		{
			if (webView2 is null)
			{
				throw new ArgumentNullException(nameof(webView2));
			}

			_webView2 = webView2;
		}

		public ICoreWebView2SettingsWrapper Settings
		{
			get
			{
				if (_settings == null)
				{
					_settings = new WinUICoreWebView2SettingsWrapper(_webView2.WebView2.CoreWebView2.Settings);
				}
				return _settings;
			}
		}

		public Task AddScriptToExecuteOnDocumentCreatedAsync(string javaScript)
		{
			return _webView2.WebView2.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(javaScript).AsTask();
		}

		public Action AddWebMessageReceivedHandler(Action<WebMessageReceivedEventArgs> messageReceivedHandler)
		{
			void nativeEventHandler(object sender, CoreWebView2WebMessageReceivedEventArgs e)
			{
				messageReceivedHandler(new WebMessageReceivedEventArgs(e.Source, e.TryGetWebMessageAsString()));
			}

			_webView2.WebView2.CoreWebView2.WebMessageReceived += nativeEventHandler;

			// Return removal callback
			return () =>
			{
				_webView2.WebView2.CoreWebView2.WebMessageReceived -= nativeEventHandler;
			};
		}

		public void AddWebResourceRequestedFilter(string uri, CoreWebView2WebResourceContextWrapper resourceContext)
		{
			_webView2.WebView2.CoreWebView2.AddWebResourceRequestedFilter(uri, (CoreWebView2WebResourceContext)resourceContext);
		}

		public Action AddWebResourceRequestedHandler(EventHandler<ICoreWebView2WebResourceRequestedEventArgsWrapper> webResourceRequestedHandler)
		{
			void nativeEventHandler(object sender, CoreWebView2WebResourceRequestedEventArgs e)
			{
				webResourceRequestedHandler(_webView2.WebView2, new WinUICoreWebView2WebResourceRequestedEventArgsWrapper(_webView2.Environment!, e));
			}

			_webView2.WebView2.CoreWebView2.WebResourceRequested += nativeEventHandler;

			// Return removal callback
			return () =>
			{
				_webView2.WebView2.CoreWebView2.WebResourceRequested -= nativeEventHandler;
			};
		}

		public void PostWebMessageAsString(string message)
		{
			_webView2.WebView2.CoreWebView2.PostWebMessageAsString(message);
		}
	}

	internal class WinUICoreWebView2SettingsWrapper : ICoreWebView2SettingsWrapper
	{
		private CoreWebView2Settings _settings;

		public WinUICoreWebView2SettingsWrapper(CoreWebView2Settings settings)
		{
			_settings = settings;
		}

		public bool IsScriptEnabled
		{
			get => _settings.IsScriptEnabled;
			set => _settings.IsScriptEnabled = value;
		}
		public bool IsWebMessageEnabled
		{
			get => _settings.IsWebMessageEnabled;
			set => _settings.IsWebMessageEnabled = value;
		}
		public bool AreDefaultScriptDialogsEnabled
		{
			get => _settings.AreDefaultScriptDialogsEnabled;
			set => _settings.AreDefaultScriptDialogsEnabled = value;
		}
		public bool IsStatusBarEnabled
		{
			get => _settings.IsStatusBarEnabled;
			set => _settings.IsStatusBarEnabled = value;
		}
		public bool AreDevToolsEnabled
		{
			get => _settings.AreDevToolsEnabled;
			set => _settings.AreDevToolsEnabled = value;
		}
		public bool AreDefaultContextMenusEnabled
		{
			get => _settings.AreDefaultContextMenusEnabled;
			set => _settings.AreDefaultContextMenusEnabled = value;
		}
		public bool AreHostObjectsAllowed
		{
			get => _settings.AreHostObjectsAllowed;
			set => _settings.AreHostObjectsAllowed = value;
		}
		public bool IsZoomControlEnabled
		{
			get => _settings.IsZoomControlEnabled;
			set => _settings.IsZoomControlEnabled = value;
		}
		public bool IsBuiltInErrorPageEnabled
		{
			get => _settings.IsBuiltInErrorPageEnabled;
			set => _settings.IsBuiltInErrorPageEnabled = value;
		}
	}

	internal class WinUICoreWebView2WebResourceRequestedEventArgsWrapper : ICoreWebView2WebResourceRequestedEventArgsWrapper
	{
		private readonly CoreWebView2Environment _env;
		private readonly CoreWebView2WebResourceRequestedEventArgs _e;

		public WinUICoreWebView2WebResourceRequestedEventArgsWrapper(CoreWebView2Environment env, CoreWebView2WebResourceRequestedEventArgs e)
		{
			Request = new WinUICoreWebView2WebResourceRequestWrapper(e);
			ResourceContext = (CoreWebView2WebResourceContextWrapper)e.ResourceContext;
			_env = env;
			_e = e;
		}

		public ICoreWebView2WebResourceRequestWrapper Request { get; }

		public CoreWebView2WebResourceContextWrapper ResourceContext { get; }

		public void SetResponse(Stream content, int statusCode, string statusMessage, string headerString)
		{
			// NOTE: This is stream copying is to work around a hanging bug in WinRT with managed streams
			var memStream = new MemoryStream();
			content.CopyTo(memStream);
			var ms = new InMemoryRandomAccessStream();
			ms.WriteAsync(memStream.GetWindowsRuntimeBuffer()).AsTask().Wait();

			_e.Response = _env.CreateWebResourceResponse(ms, statusCode, statusMessage, headerString);
		}
	}

	internal class WinUICoreWebView2WebResourceRequestWrapper : ICoreWebView2WebResourceRequestWrapper
	{
		private CoreWebView2WebResourceRequestedEventArgs _e;

		public WinUICoreWebView2WebResourceRequestWrapper(CoreWebView2WebResourceRequestedEventArgs e)
		{
			_e = e;
		}

		public string Uri
		{
			get => _e.Request.Uri;
			set => _e.Request.Uri = value;
		}
		public string Method
		{
			get => _e.Request.Method;
			set => _e.Request.Method = value;
		}
		// TODO: If we want to implement this then we need to convert to/from IRandomAccessStream. For now it's not needed by BlazorWebView.
		//public Stream Content
		//{
		//	get => _e.Request.Content;
		//	set => _e.Request.Content = value;
		//}
	}
}
