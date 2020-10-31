using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using Blazor.BlazorCommunicationFoundation.Sample.Shared;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Core.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.Handlers;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.DependencyInjection;

namespace Blazor.BlazorCommunicationFoundation.Sample.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("Blazor.BlazorCommunicationFoundation.Sample.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler(sp => sp.GetRequiredService<BlazorCommunicationFoundationHandler>().ConfigureHandler(returnUrl: "/authentication/login"));

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Blazor.BlazorCommunicationFoundation.Sample.ServerAPI"));

            builder.Services.AddApiAuthorization();

            builder.Services.AddBlazorCommunicationFoundation();
            builder.Services.AddBFCAuthentication();
            builder.Services.AddBCFContract<IWeatherForecastContract>();

            await builder.Build().RunAsync();
        }
    }
}
