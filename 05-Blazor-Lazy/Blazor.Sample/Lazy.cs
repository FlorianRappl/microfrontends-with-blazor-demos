using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Sample
{
	public class Lazy : ComponentBase
	{
		Type _innerComponentType;
		KeyValuePair<string, object>[] _parms;

		[Inject] IAssemblyDependencyResolver AssemblyDependencyResolver { get; set; }

		public async override Task SetParametersAsync(ParameterView parameters)
		{
			IReadOnlyDictionary<string, object> parms = parameters.ToDictionary();

			if (!(parms.TryGetValue("_Assembly", out object asmVal) && (asmVal is string asm) && parms.TryGetValue("_Type", out object typeVal) && typeVal is string type))
				throw new ArgumentException("Specify _Assembly and _Type attributes as string values identifying a valid component class.");
			_parms = parms.Where(x => string.Equals(x.Key, "_Assembly", StringComparison.OrdinalIgnoreCase) == string.Equals(x.Key, "_Type", StringComparison.OrdinalIgnoreCase)).ToArray();

			_innerComponentType = await EnsureComponentAsync(asm, type);
			if (_innerComponentType == null)
				throw new ArgumentException($"Assembly {asm} can't be resolved.");

			StateHasChanged();
		}

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{
			builder.OpenComponent(1, _innerComponentType);
			int a = 1;
			foreach (KeyValuePair<string, object> parm in _parms)
				builder.AddAttribute(++a, parm.Key, parm.Value);
			builder.CloseComponent();
		}

		async Task<Type> EnsureComponentAsync(string assemblyName, string type)
		{
			//We need to inject new assembly to the router because it resolves which page to display.
			IRouterEnvelope router = JSInteropMethods.Router;
			IEnumerable<Assembly> additionalAssemblies = router.AdditionalAssemblies ?? Enumerable.Empty<Assembly>();

			//Don't inject the assembly multiple times.
			Assembly asm = additionalAssemblies.FirstOrDefault(x => string.Equals(x.GetName().Name, assemblyName, StringComparison.OrdinalIgnoreCase));
			if (asm == default)
			{
				//Load assembly including its dependencies
				IEnumerable<Assembly> newAssemblies = await AssemblyDependencyResolver.ResolveAsync(assemblyName);
				if (!newAssemblies.Any())
					return null;

				LoadServices(newAssemblies);

				//Inject the assembly to the router.
				ParameterView pv = ParameterView.FromDictionary(new Dictionary<string, object>() { { nameof(IRouterEnvelope.AdditionalAssemblies), additionalAssemblies.Concat(newAssemblies).ToArray() } });
				await router.SetParametersAsync(pv);

				asm = newAssemblies.First();
			}

			return asm.GetType(type, true);
		}

		protected virtual void LoadServices(IEnumerable<Assembly> newAssemblies)
		{
			foreach (Assembly asm in newAssemblies)
				AssemblyLazyLoadResolverBase.LoadServices(asm);
		}
	}
}
