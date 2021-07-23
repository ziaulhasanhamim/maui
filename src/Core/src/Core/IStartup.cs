using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Hosting.Internal;

namespace Microsoft.Maui
{
	public sealed class MauiAppBuilder
	{
		private MauiAppBuilder()
		{
			Host = new ConfigureHostBuilder(Services);
		}

		public static MauiAppBuilder CreateBuilder() => new MauiAppBuilder(); // <-- perhaps set some defaults here?

		/// <summary>
		/// A collection of services for the application to compose. This is useful for adding user provided or framework provided services.
		/// </summary>
		public IServiceCollection Services { get; } = new ServiceCollection();

		/// <summary>
		/// An <see cref="IHostBuilder"/> for configuring host specific properties, but not building.
		/// </summary>
		public ConfigureHostBuilder Host { get; }

		//public FontCollection CustomFonts { get; set; } // <-- Would something like this be useful?
		//public ControlHandlerCollection CustomHandlers { get; set; } // <-- Or this? More usable than just registering magical 'services'?



		readonly Dictionary<Type, List<Action<HostBuilderContext, IMauiServiceBuilder>>> _configureServiceBuilderActions = new();
		public MauiAppBuilder ConfigureMauiHandlers(Action<IMauiHandlersCollection> configureDelegate)
		{
			ConfigureServices<HandlerCollectionBuilder>((_, handlers) => configureDelegate(handlers));
			return this;
		}

		public MauiAppBuilder ConfigureMauiHandlers(Action<HostBuilderContext, IMauiHandlersCollection> configureDelegate)
		{
			ConfigureServices<HandlerCollectionBuilder>(configureDelegate);
			return this;
		}

		public MauiAppBuilder ConfigureFonts(Action<IFontCollection> configureDelegate)
		{
			ConfigureServices<FontCollectionBuilder>((_, fonts) => configureDelegate(fonts));
			return this;
		}

		readonly List<Action<HostBuilderContext, IConfigurationBuilder>> _configureAppConfigActions = new List<Action<HostBuilderContext, IConfigurationBuilder>>();
		readonly List<Action<IConfigurationBuilder>> _configureHostConfigActions = new List<Action<IConfigurationBuilder>>();

		public MauiAppBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
		{
			_configureAppConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
			return this;
		}

		public MauiAppBuilder ConfigureAppConfiguration(Action<IConfigurationBuilder> configureDelegate)
		{
			ConfigureAppConfiguration((_, config) => configureDelegate(config));
			return this;
		}

		public MauiAppBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
		{
			_configureHostConfigActions.Add(configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate)));
			return this;
		}




		public MauiAppBuilder ConfigureImageSources()
		{
			ConfigureImageSources(services =>
			{
				services.AddService<IFileImageSource>(svcs => new FileImageSourceService(svcs.GetService<IImageSourceServiceConfiguration>(), svcs.CreateLogger<FileImageSourceService>()));
				services.AddService<IFontImageSource>(svcs => new FontImageSourceService(svcs.GetRequiredService<IFontManager>(), svcs.CreateLogger<FontImageSourceService>()));
				services.AddService<IStreamImageSource>(svcs => new StreamImageSourceService(svcs.CreateLogger<StreamImageSourceService>()));
				services.AddService<IUriImageSource>(svcs => new UriImageSourceService(svcs.CreateLogger<UriImageSourceService>()));
			});
			return this;
		}

		public MauiAppBuilder ConfigureImageSources(Action<IImageSourceServiceCollection> configureDelegate)
		{
			ConfigureServices<ImageSourceServiceBuilder>((_, services) => configureDelegate(services));
			return this;
		}

		public MauiAppBuilder ConfigureImageSources(Action<HostBuilderContext, IImageSourceServiceCollection> configureDelegate)
		{
			ConfigureServices<ImageSourceServiceBuilder>(configureDelegate);
			return this;
		}

		class ImageSourceServiceBuilder : MauiServiceCollection, IImageSourceServiceCollection, IMauiServiceBuilder
		{
			public void ConfigureServices(HostBuilderContext context, IServiceCollection services)
			{
				services.AddSingleton<IImageSourceServiceConfiguration, ImageSourceServiceConfiguration>();
				services.AddSingleton<IImageSourceServiceProvider>(svcs => new ImageSourceServiceProvider(this, svcs));
			}
		}



		class FontCollectionBuilder : FontCollection, IMauiServiceBuilder, IMauiInitializeService
		{
			public void ConfigureServices(HostBuilderContext context, IServiceCollection services)
			{
				services.AddSingleton<IEmbeddedFontLoader>(svc => new EmbeddedFontLoader(svc.CreateLogger<EmbeddedFontLoader>()));
				services.AddSingleton<IFontRegistrar>(svc => new FontRegistrar(svc.GetRequiredService<IEmbeddedFontLoader>(), svc.CreateLogger<FontRegistrar>()));
				services.AddSingleton<IFontManager>(svc => new FontManager(svc.GetRequiredService<IFontRegistrar>(), svc.CreateLogger<FontManager>()));
				services.InitThis(this);
			}

			public void Initialize(HostBuilderContext context, IServiceProvider services)
			{
				var fontRegistrar = services.GetService<IFontRegistrar>();
				if (fontRegistrar == null)
					return;

				foreach (var font in this)
				{
					if (font.Assembly == null)
						fontRegistrar.Register(font.Filename, font.Alias);
					else
						fontRegistrar.Register(font.Filename, font.Alias, font.Assembly);
				}
			}
		}

		class HandlerCollectionBuilder : MauiHandlersCollection, IMauiServiceBuilder
		{
			public void ConfigureServices(HostBuilderContext context, IServiceCollection services)
			{
				var provider = new MauiHandlersServiceProvider(this);

				services.AddSingleton<IMauiHandlersServiceProvider>(provider);
			}
		}


		public MauiAppBuilder ConfigureServices<TBuilder>(Action<HostBuilderContext, TBuilder> configureDelegate)
			where TBuilder : IMauiServiceBuilder, new()
		{
			_ = configureDelegate ?? throw new ArgumentNullException(nameof(configureDelegate));

			var key = typeof(TBuilder);
			if (!_configureServiceBuilderActions.TryGetValue(key, out var list))
			{
				list = new List<Action<HostBuilderContext, IMauiServiceBuilder>>();
				_configureServiceBuilderActions.Add(key, list);
			}

			list.Add((context, builder) => configureDelegate(context, (TBuilder)builder));

			return this;
		}

		public IServiceProvider FinalizeInternals()
		{
			// AppConfig
			BuildHostConfiguration();
			BuildAppConfiguration();
			if (_appConfiguration != null)
				Services.AddSingleton(_appConfiguration);

			// ConfigureServices
			var properties = new Dictionary<object, object>();
			var builderContext = new HostBuilderContext(properties); // TODO: Should get this from somewhere...

			foreach (var pair in _configureServiceBuilderActions)
			{
				var instance = (IMauiServiceBuilder)Activator.CreateInstance(pair.Key)!;

				foreach (var action in pair.Value)
				{
					action(builderContext, instance);
				}

				instance.ConfigureServices(builderContext, Services);
			}

			var serviceProvider = Services.BuildServiceProvider();

			var initServices = serviceProvider.GetService<IEnumerable<IMauiInitializeService>>();
			if (initServices != null)
			{
				foreach (var instance in initServices)
				{
					instance.Initialize(builderContext, serviceProvider);
				}
			}

			return serviceProvider;
		}

		IConfiguration? _hostConfiguration;
		IConfiguration? _appConfiguration;

		void BuildHostConfiguration()
		{
			var configBuilder = new ConfigurationBuilder();
			foreach (var buildAction in _configureHostConfigActions)
			{
				buildAction(configBuilder);
			}
			_hostConfiguration = configBuilder.Build();
		}

		void BuildAppConfiguration()
		{
			var properties = new Dictionary<object, object>();
			var builderContext = new HostBuilderContext(properties); // TODO: Should get this from somewhere...

			var configBuilder = new ConfigurationBuilder();
			configBuilder.AddConfiguration(_hostConfiguration);
			foreach (var buildAction in _configureAppConfigActions)
			{
				buildAction(builderContext, configBuilder);
			}
			_appConfiguration = configBuilder.Build();

			builderContext.Configuration = _appConfiguration;
		}
	}

	/// <summary>
	/// A non-buildable <see cref="IHostBuilder"/> for <see cref="MauiAppBuilder"/>.
	/// </summary>
	public sealed class ConfigureHostBuilder : IHostBuilder
	{
		/// <inheritdoc />
		public IDictionary<object, object> Properties { get; } = new Dictionary<object, object>();

		private readonly IServiceCollection _services;
		private readonly HostBuilderContext _context;

		internal ConfigureHostBuilder(IServiceCollection services)
		{
			_services = services;
			_context = new HostBuilderContext(Properties)
			{
			};
		}

		internal bool ConfigurationEnabled { get; set; }

		IHost IHostBuilder.Build()
		{
			throw new NotSupportedException($"This object is not buildable.");
		}

		/// <inheritdoc />
		public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
		{
			throw new NotImplementedException("TODO: This");
		}

		/// <inheritdoc />
		public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
		{
			throw new NotImplementedException("TODO: This");
		}

		/// <inheritdoc />
		public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
		{
			throw new NotImplementedException("TODO: This");
		}

		/// <inheritdoc />
		public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
		{
			configureDelegate(_context, _services);
			return this;
		}

		/// <inheritdoc />
		public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory) where TContainerBuilder : notnull
		{
			throw new NotImplementedException("TODO: This");
		}

		/// <inheritdoc />
		public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory) where TContainerBuilder : notnull
		{
			throw new NotImplementedException("TODO: This");
		}
	}
}
