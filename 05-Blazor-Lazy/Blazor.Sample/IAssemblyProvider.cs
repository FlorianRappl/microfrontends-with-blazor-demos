using System.Threading.Tasks;

namespace Blazor.Sample
{
	public interface IAssemblyProvider
	{
		Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName);
	}
}
