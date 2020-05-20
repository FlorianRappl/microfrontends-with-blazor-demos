using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace Blazor.Sample
{
	public class MetadataAssemblyDependencyResolver : IAssemblyDependencyResolver
	{
		static AssemblyNameComparer _assemblyNameComparer = new AssemblyNameComparer();
		readonly IAssemblyProvider _assemblyProvider;

		public MetadataAssemblyDependencyResolver(IAssemblyProvider assemblyProvider)
			=> _assemblyProvider = assemblyProvider;

		public async Task<IEnumerable<Assembly>> ResolveAsync(string assemblyName)
		{
			Dictionary<string, (byte[] DllBytes, byte[] PdbBytes)> assemblies = new Dictionary<string, (byte[] DllBytes, byte[] PdbBytes)>(_assemblyNameComparer);
			await ProcessAssemblyNameAsync(assemblyName, assemblies);

			//AppDomain.CurrentDomain.AssemblyResolve+=(sender,args) => { return null; args. };
			return assemblies.Select(x => { (byte[] dllBytes, byte[] pdbBytes) = x.Value; Assembly asm = pdbBytes == null ? Assembly.Load(dllBytes) : Assembly.Load(dllBytes, pdbBytes); return asm; }).ToArray();
		}

		async Task ProcessAssemblyNameAsync(string assemblyName, Dictionary<string, (byte[] DllBytes, byte[] PdbBytes)> assemblies)
		{
			//Get assembly
			(byte[] DllBytes, byte[] PdbBytes) bytes = await _assemblyProvider.GetAssemblyAsync(assemblyName);
			assemblies[assemblyName] = bytes;

			//Resolve referenced assemblies
			string[] assemblyReferences;

			using (PEReader pEReader = new PEReader(ImmutableArray.Create(bytes.DllBytes)))
			{
				MetadataReader mdReader = pEReader.GetMetadataReader(MetadataReaderOptions.None);
				assemblyReferences = mdReader.AssemblyReferences.Select(x => mdReader.GetAssemblyReference(x).GetAssemblyName().Name).ToArray();
			}

			//Filter out referenced assemblies already managed or loaded
			lock (assemblies)
			{
				assemblyReferences = assemblyReferences.Where(x => !assemblies.ContainsKey(x))
					.Except(AppDomain.CurrentDomain.GetAssemblies().Select(x => x.GetName().Name), _assemblyNameComparer)
					.ToArray();
				foreach (string item in assemblyReferences)
					assemblies.Add(item, default);
			}

			//Find deeper references
			await Task.WhenAll(assemblyReferences.Select(x => ProcessAssemblyNameAsync(x, assemblies)));
		}

		class AssemblyNameComparer : IEqualityComparer<string>
		{
			public bool Equals(string x, string y)
				=> string.Equals(x, y, StringComparison.OrdinalIgnoreCase);

			public int GetHashCode(string obj)
				=> 0;
		}
	}
}
