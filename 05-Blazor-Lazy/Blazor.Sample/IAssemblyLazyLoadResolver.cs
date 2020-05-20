using System.Threading.Tasks;

namespace Blazor.Sample
{
	public interface IAssemblyLazyLoadResolver
	{
		Task ResolveAsync(string uri, bool isInterceptedLink);
	}
}