using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazor.Sample
{
	public interface IAssemblyDependencyResolver
	{
		Task<IEnumerable<Assembly>> ResolveAsync(string assemblyName);
	}
}
