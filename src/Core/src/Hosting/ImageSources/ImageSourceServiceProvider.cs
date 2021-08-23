#nullable enable

using System;
using System.Collections.Concurrent;
using Microsoft.Maui.Hosting.Internal;

namespace Microsoft.Maui.Hosting
{
	class ImageSourceServiceProvider : MauiServiceProvider, IImageSourceServiceProvider
	{
		static readonly string ImageSourceInterface = typeof(IImageSource).FullName!;

		readonly ConcurrentDictionary<Type, Type> _imageSourceCache = new ConcurrentDictionary<Type, Type>();
		readonly ConcurrentDictionary<Type, Type> _serviceCache = new ConcurrentDictionary<Type, Type>();

		public ImageSourceServiceProvider(IMauiServiceCollection collection, IServiceProvider hostServiceProvider)
			: base(collection, false)
		{
			HostServiceProvider = hostServiceProvider;
		}

		public IServiceProvider HostServiceProvider { get; }

		public IImageSourceService? GetImageSourceService(Type imageSource) =>
			(IImageSourceService?)GetService(GetImageSourceServiceType(imageSource));

		public Type GetImageSourceServiceType(Type imageSource) =>
			_serviceCache.GetOrAdd(imageSource, type =>
			{
				var genericConcreteType = typeof(IImageSourceService<>).MakeGenericType(type);

				if (genericConcreteType != null && GetServiceDescriptor(genericConcreteType) != null)
					return genericConcreteType;

				var imageSourceType = GetImageSourceType(type);
				return typeof(IImageSourceService<>).MakeGenericType(imageSourceType);
			});

		public Type GetImageSourceType(Type imageSource) =>
			_imageSourceCache.GetOrAdd(imageSource, type =>
			{
				if (type.IsInterface)
				{
					if (type.GetInterface(ImageSourceInterface) != null)
						return type;
				}
				else
				{
					foreach (var directInterface in type.GetInterfaces())
					{
						if (directInterface.GetInterface(ImageSourceInterface) != null)
							return directInterface;
					}
				}

				throw new InvalidOperationException($"Unable to find the image source type because none of the interfaces on {type.Name} were derived from {nameof(IImageSource)}.");
			});
	}
}