using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.Sample
{
	public class NonGZippedAssemblyDownloader : AssemblyProviderBase
	{
		public NonGZippedAssemblyDownloader(HttpClient httpClient) : base(httpClient)
		{ }

		public override async Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName)
		{
			Task<byte[]> dllBytes = DownloadFileBytes(assemblyName + ".dll");
			return (await dllBytes, await TryDownloadFileBytes(assemblyName + ".pdb"));
		}
	}
}
