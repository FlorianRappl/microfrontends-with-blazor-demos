using Microsoft.Extensions.DependencyInjection;

namespace Blazor.LibA
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//this entrypoint is requested only during project build. Otherwise, it's useless.
		}

		public static void ConfigureServices(IServiceCollection services)
		{
		}
	}
}
