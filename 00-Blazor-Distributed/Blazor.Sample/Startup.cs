using Blazor.LibA;
using Blazor.LibB;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Sample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
            app.AddLibrary<ComponentA>();
            app.AddLibrary<ComponentB>();
        }
    }
}
