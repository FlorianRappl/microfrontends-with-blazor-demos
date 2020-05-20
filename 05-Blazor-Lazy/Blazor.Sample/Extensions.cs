using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blazor.Sample
{
	public static class Extensions
	{
		public static IServiceCollection AddLazyLoadCore(this IServiceCollection services)
		{
			JSInteropMethods.Services = services;
			JSInteropMethods.ServiceProvider = services.BuildServiceProvider();
			JSInteropMethods.Host = JSInteropMethods.ServiceProvider.GetService<IWebAssemblyHost>();
			return services;
		}

		public static IServiceCollection AddLazyLoad<TAssemblyLazyLoadResolver>(this IServiceCollection services) where TAssemblyLazyLoadResolver : class, IAssemblyLazyLoadResolver
			=> AddLazyLoad<TAssemblyLazyLoadResolver, MetadataAssemblyDependencyResolver, NonGZippedAssemblyDownloader>(services);

		public static IServiceCollection AddLazyLoad<TAssemblyLazyLoadResolver, TAssemblyDependencyResolver, TAssemblyProvider>(this IServiceCollection services)
					where TAssemblyLazyLoadResolver : class, IAssemblyLazyLoadResolver
					where TAssemblyDependencyResolver : class, IAssemblyDependencyResolver
					where TAssemblyProvider : class, IAssemblyProvider
		{
			services.AddSingleton<IAssemblyLazyLoadResolver, TAssemblyLazyLoadResolver>();
			services.AddSingleton<IAssemblyDependencyResolver, TAssemblyDependencyResolver>();
			services.AddSingleton<IAssemblyProvider, TAssemblyProvider>();
			return services.AddLazyLoadCore();
		}
	}
}
