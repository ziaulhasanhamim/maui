using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace Maui.Controls.Sample.Controls
{
	class BordelessEntryServiceBuilder : IMauiServiceBuilder
	{
		static IMauiHandlersCollection HandlersCollection;
		static readonly Dictionary<Type, Type> PendingHandlers = new();

		public static void TryAddHandler<TType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeRender>()
			where TType : IElement
			where TTypeRender : IElementHandler
		{
			if (HandlersCollection == null)
				PendingHandlers[typeof(TType)] = typeof(TTypeRender);
			else
				HandlersCollection.TryAddHandler<TType, TTypeRender>();
		}

		void IMauiServiceBuilder.ConfigureServices(HostBuilderContext context, IServiceCollection services)
		{
			// No-op
		}

		void IMauiServiceBuilder.Configure(HostBuilderContext context, IServiceProvider services)
		{
			HandlersCollection ??= services.GetRequiredService<IMauiHandlersServiceProvider>().GetCollection();

			if (PendingHandlers.Count > 0)
			{
				foreach (var handler in PendingHandlers)
				{
					HandlersCollection.TryAddHandler(handler.Key, handler.Value);
				}

				PendingHandlers.Clear();
			}
		}
	}
}