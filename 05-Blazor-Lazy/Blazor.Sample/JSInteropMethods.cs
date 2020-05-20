using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Blazor.Sample
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class JSInteropMethods
	{
		internal static IServiceProvider ServiceProvider;
		internal static IServiceCollection Services;
		internal static IWebAssemblyHost Host;
		internal static IRouterEnvelope Router;

		[JSInvokable("NotifyLocationChanged")]
		public static async Task NotifyLocationChanged(string uri, bool isInterceptedLink)
		{
			await ServiceProvider.GetRequiredService<IAssemblyLazyLoadResolver>().ResolveAsync(uri, isInterceptedLink);

			//Would it be possible to send original parameters and call it via DotNetDispatcher? It'd need to get ServiceProvider.GetRequiredService<IJSRuntime>() instance.
			await JSInteropMethods.NotifyLocationChanged(uri, isInterceptedLink);
		}
	}
}
