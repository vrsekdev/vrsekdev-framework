using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Havit.Blazor.StateManagement.Mobx.ObservableProperties.Default.Extensions;
using Havit.Blazor.StateManagement.Mobx.Extensions;
using Havit.Blazor.StateManagement.Mobx.PropertyObservables.Dynamic.Extensions;

namespace Havit.Blazor.StateManagement.Mobx.Samples.ClientSide
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            ConfigureServices(builder.Services);

            await builder.Build().RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.UseDefaultObservableProperties();
            services.UseDynamicPropertyObservables();
            services.AddMobxStore<IAppStore>().WithDefaultState(new DefaultAppStore()).AsSingleton();
            services.AddMobxStore<IHomeStore>().AsSingleton();
            services.AddMobxStore<ICounterStore>().AsTransient();
            services.AddMobxStore<ITodoStore>().Cascading();
        }
    }
}
