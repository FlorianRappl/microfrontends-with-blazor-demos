using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.Sample
{
	#region AssemblyProviderBase
	public abstract class AssemblyProviderBase : IAssemblyProvider
	{
		readonly HttpClient _httpClient;

		public AssemblyProviderBase(HttpClient httpClient)
			=> _httpClient = httpClient;

		public abstract Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName);

		public Task<byte[]> DownloadFileBytes(string filename)
			=> _httpClient.GetByteArrayAsync("_framework/_bin/" + filename);

		public async Task<byte[]> TryDownloadFileBytes(string filename)
		{
			try
			{
				return await DownloadFileBytes(filename);
			}
			catch
			{ }
			return null;
		}
	}
	#endregion AssemblyProviderBase
}
