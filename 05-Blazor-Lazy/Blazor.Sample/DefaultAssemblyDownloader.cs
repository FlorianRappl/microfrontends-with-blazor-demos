using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazor.Sample
{
	public class DefaultAssemblyDownloader : AssemblyProviderBase
	{
		public DefaultAssemblyDownloader(HttpClient httpClient) : base(httpClient)
		{ }

		public override async Task<(byte[] DllBytes, byte[] PdbBytes)> GetAssemblyAsync(string assemblyName)
		{
			//Ungzip works with streams but it's still worth to download data as byte[] because HttpClient do various memory optimizations and content length checks when downloading as byte[].
			Task<byte[]> dllBytes = Ungzip(DownloadFileBytes(assemblyName + ".dll.gz"));
			return (await dllBytes, await Ungzip(TryDownloadFileBytes(assemblyName + ".pdb.gz")));
		}

		async Task<byte[]> Ungzip(Task<byte[]> source)
		{
			var gzipBytes = await source;

			if (gzipBytes == null)
				return null;

			using (Stream gzipStream = new MemoryStream(gzipBytes))
			using (GZipStream decompressStream = new GZipStream(gzipStream, CompressionMode.Decompress))
			using (MemoryStream rawStream = new MemoryStream())
			{
				decompressStream.CopyTo(rawStream);
				return rawStream.GetBuffer();
			}
		}
	}
}
