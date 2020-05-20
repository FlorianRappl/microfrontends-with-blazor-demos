using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Sample
{
	public class LazyLoadResolver : AssemblyLazyLoadResolverBase
	{
		readonly IAssemblyDependencyResolver _assemblyDependencyResolver;

		public LazyLoadResolver(IAssemblyDependencyResolver assemblyDependencyResolver)
		{
			_assemblyDependencyResolver = assemblyDependencyResolver;
		}

		public override async Task ResolveAsync(string uri, bool isInterceptedLink)
		{
			//Get requested assembly based on the first path segment. This is highly specific, other applications might use different strategy.
			string[] segments = new Uri(uri, UriKind.Absolute).Segments.Select(x => x.Trim('/')).Where(x => x.Length > 0).ToArray();
			if (segments.Length < 2)
				return;
			string assemblyName = segments[0];

			//We need to inject new assembly to the router because it resolves which page to display.
			IRouterEnvelope router = base.Router;

			IEnumerable<Assembly> additionalAssemblies = router.AdditionalAssemblies ?? Enumerable.Empty<Assembly>();
			//Don't inject the assembly multiple times.
			if (additionalAssemblies.Any(x => string.Equals(x.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase)))
				return;

			//Load assembly including its dependencies
			IEnumerable<Assembly> newAssemblies = await _assemblyDependencyResolver.ResolveAsync(assemblyName);
			if (!newAssemblies.Any())
				return;

			//Register also services
			foreach (Assembly asm in newAssemblies)
				LoadServices(asm);

			//Inject the assembly to the router.
			ParameterView pv = ParameterView.FromDictionary(router.GetType().GetProperties()
				.Where(x => x.CustomAttributes.Any(y => y.AttributeType == typeof(ParameterAttribute)))
				.ToDictionary(pi => pi.Name, pi => string.Equals(pi.Name, nameof(IRouterEnvelope.AdditionalAssemblies), StringComparison.Ordinal)
					 ? additionalAssemblies.Concat(newAssemblies).ToArray()
					 : pi.GetValue(router)));
			await router.SetParametersAsync(pv);
		}
	}
}
