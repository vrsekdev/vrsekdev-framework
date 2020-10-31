using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.DependencyInjection;
using Blazor.BlazorCommunicationFoundation.Sample.Shared;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;

namespace Blazor.BlazorCommunicationFoundation.Sample.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddBlazorCommunicationFoundation();
            builder.Services.AddBCFContract<IWeatherForecastContract>();

            await builder.Build().RunAsync();
        }
    }
}
