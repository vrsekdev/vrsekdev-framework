# Blazor Communication Foundation

Blazor Communication Foundation is a simple library that provides communication between Blazor WebAsembly and ASP.NET Server using typed contracts defined through C# interfaces.

For a sample usage with authorization, you can refer to the [sample project](https://github.com/vrsekdev/vrsekdev-framework/tree/master/src/Blazor.BlazorCommunicationFoundation.Sample)

## Configuration

### Shared

First, you need to define contracts inside your `Shared` project. Contract is a C# interface that defines an API between client and server. Only methods that return Task are supported.

Sample contract
```csharp
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.BlazorCommunicationFoundation.Sample.Shared
{
    public interface IWeatherForecastContract
    {
        Task<WeatherForecast[]> GetAsync();

        Task<WeatherForecast[]> GetAsync(WeatherForecastGetFilter filter);

        Task AddAsync(WeatherForecast weatherForecast);
    }
}
```

### Client
Then you can add client configuration into client's Main method.
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
...

builder.Services.AddBCFClient(builder =>
{
    builder.Contracts.AddContract<IWeatherForecastContract>();
});

...
```

Also, you need to make sure that you registered HttpClient or that you are using custom implementation of `IHttpClientResolver`.

### Server
On server, you need to specify configuration for services. All extension methods for `IServiceCollection` can be used, including `AddTransient`, `AddScoped`, `AddSingleton` and implementation factories. 
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;
...

services.AddBCFServer(builder =>
{
    builder.Contracts.AddTransient<IWeatherForecastContract, WeatherForecastService>();
});

...
```

Then add middleware after `UseAuthentication` and `UseAuthorization`
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Middlewares;
...

app.UseBlazorCommunicationFoundation();

...
```

## Usage

You can use DI on your client to resolve contracts and execute methods as you would normally would.

Inside blazor components, you can use attribute `[Inject]`

```csharp
@inject IWeatherForecastContract WeatherForecastContract

@code {
    private WeatherForecast[] forecasts;

    protected override async Task OnInitializedAsync()
    {
        forecasts = await WeatherForecastContract.GetAsync();
    }
}
```

Or you can use it using constructor injection inside your client services registered in `ServiceProvider`

## Authorization

Refer to a page [Authorization](authorization.md)