using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
using Blazor.BlazorCommunicationFoundation.Sample.Shared;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.Handlers;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication.DependencyInjection;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.Json;

namespace Blazor.BlazorCommunicationFoundation.Sample.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("WithAuth", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler(sp => sp.GetRequiredService<BlazorCommunicationFoundationHandler>().ConfigureHandler(loginUrl: "/authentication/login"));

            builder.Services.AddHttpClient("NoAuth", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NoAuth"));

            builder.Services.AddApiAuthorization();

            builder.Services.AddBFCAuthentication();
            builder.Services.AddBCFClient(builder =>
            {
#if DEBUG
                //builder.UseSerializer<JsonInvocationSerializer>();
#endif
                builder.CreateScope(scope =>
                {
                    scope.UseNamedHttpClient("WithAuth");

                    scope.Contracts.AddContractsByAttribute(typeof(IUserActionContract).Assembly, areaName: "WithAuth");
                });

                builder.Contracts.AddContract<IWeatherForecastContract>();
            });

            await builder.Build().RunAsync();
        }
    }
}
