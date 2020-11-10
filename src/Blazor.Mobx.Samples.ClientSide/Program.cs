using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VrsekDev.Blazor.Mobx.Extensions;
using VrsekDev.Blazor.Mobx.Samples.Shared.Stores;

namespace VrsekDev.Blazor.Mobx.Samples.ClientSide
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
            services.AddDefaultMobxProperties();
            services.AddMobxStore<AppStore>().LifestyleScoped();
            /*services.AddMobxStore<IHomeStore>().LifestyleScoped();
            services.AddMobxStore<ICounterStore>().LifestyleTransient();
            services.AddMobxStore<ITodoStore>().LifestyleCascading();*/
        }
    }
}
