# Blazor Communication Foundation <!-- omit in toc -->

Blazor Communication Foundation is a simple library that provides communication between Blazor WebAsembly and ASP.NET Server using typed contracts defined through C# interfaces.

For a sample usage with authorization, you can refer to the [sample project](https://github.com/vrsekdev/vrsekdev-framework/tree/master/src/Blazor.BlazorCommunicationFoundation.Sample)

- [Installation](#installation)
- [Configuration](#configuration)
  - [Shared](#shared)
  - [Client](#client)
  - [Server](#server)
- [Usage](#usage)
- [Authorization](#authorization)
- [Advanced](#advanced)
  - [Custom serializer](#custom-serializer)
  - [Custom HttpClient](#custom-httpclient)
  - [Scopes](#scopes)
- [Exception handling](#exception-handling)
- [Swagger integration](#swagger-integration)
- [Debugging](#debugging)
  - [Request](#request)
  - [Response](#response)


## Installation

Into your client application, install package
```
Install-Package VrsekDev.Blazor.BlazorCommunicationFoundation.Client
```

Into your server application (ASP.NET Core hosted), install package
```
Install-Package VrsekDev.Blazor.BlazorCommunicationFoundation.Server
```

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
Add client configuration into client's Main method.
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
...

builder.Services.AddBCFClient(builder =>
{
    builder.Contracts.AddContract<IWeatherForecastContract>();
});

...
```

You can also register contracts by attribute `ContractAttribute`. Following code will register all contracts from assembly containing `IUserActionContract`.
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
...

builder.Services.AddBCFClient(builder =>
{
    scope.Contracts.AddContractsByAttribute(typeof(IUserActionContract).Assembly);
});

...
```

By specifying `Area` into `ContractAttribute`, you can then register some contracts into one scope and different ones into a different scope. 

The following code will register all contracts from assembly containing `IUserActionContract` and specified Area `WithAuth`.
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Client.DependencyInjection;
...

builder.Services.AddBCFClient(builder =>
{
    builder.CreateScope(scope =>
    {
        scope.UseNamedHttpClient("WithAuth");

        scope.Contracts.AddContractsByAttribute(typeof(IUserActionContract).Assembly, areaName: "WithAuth");
    });

    builder.Contracts.AddContract<IWeatherForecastContract>();
});

...
```

Also, you need to make sure that you registered HttpClient or that you are using custom implementation of `IHttpClientResolver` ([more](#custom-httpclient))

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

Then register endpoints through method `MapBlazorCommunicationFoundation()`. This will register all contracts to a unique endpoint. **You can then use any OpenAPI generator and export your definitions into other applications that do not use `.NET`, like mobile `Java` applications or any other platform that supports HTTP requests.**
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;
...

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorCommunicationFoundation();
    ...
});
```

You can also register contract implementations by attribute `ContractImplementationAttribute` and `ContractAttribute`. Class implementing any contract has to have `ContractImplementationAttribute` and the contract has to have `ContractAttribute`. You can also specify `ServiceLifetime` with `serviceLifetime` argument or by specifying `Lifetime` on a `ContractImplementationAttribute` to use a lifetime for a specific contract implementation. Default lifetime is transient. 

The following code will register all contract implementations from assembly containing `UserActionService` with scoped lifetime.
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;
...

services.AddBCFServer(builder =>
{
    builder.Contracts.AddTransient<IWeatherForecastContract, WeatherForecastService>();

    builder.Contracts.AddContractsByAttribute(typeof(UserActionService).Assembly, serviceLifetime: ServiceLifetime.Scoped);
});

...
```

By specifying argument `areaName`, you can filter services by the value of `Area` on `ContractAttribute` of implemented contract. 

The following code will register all contract implementations that implement contract with `Area` of value `WithAuth`.
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;
...

services.AddBCFServer(builder =>
{
    builder.Contracts.AddTransient<IWeatherForecastContract, WeatherForecastService>();

    builder.Contracts.AddContractsByAttribute(typeof(UserActionService).Assembly, areaName: "WithAuth", serviceLifetime: ServiceLifetime.Transient);
});

...
```

## Usage

You can use DI on your client to resolve contracts and execute methods as you normally would.

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

## Advanced

### Custom serializer

Blazor Communication Foundation allows using different serializers for serializing data between client and server. By default, MessagePack is used, but JSON is also available.
To use JSON serializer, you need to install new package into your server and client application
```
Install-Package VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.Json
```

**Server can handle multiple types of serializers and uses HTTP headers Accept and ContentType.**
That means, that you can have multiple clients using different serializers with one server.

Into your server configuration, add this new line

```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.Json;
...

services.AddBCFServer(builder =>
{
    ...

    builder.AddSerializer<JsonInvocationSerializer>();
    
    ...
});
```

Into your client configuration, add this new line
```csharp
using VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.Json;
...

builder.Services.AddBCFClient(builder =>
{
    ...

    builder.UseSerializer<JsonInvocationSerializer>();

    ...
});
```

I recommend using it just for debugging by using `#if DEBUG` directive.

You can also provide custom serializer by implementing interface `IInvokeSerializer`

### Custom HttpClient

To use custom HttpClient to execute remote methods, you can use method `UseNamedHttpClient(string)` for using named HttpClients or `UseHttpClientResolver<TResolver>()` for custom implementations of interface `IHttpClientResolver`.

```csharp
builder.Services.AddHttpClient("WithAuth", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<BlazorCommunicationFoundationHandler>());

builder.Services.AddBCFClient(builder =>
{
    builder.UseNamedHttpClient("WithAuth");
    // IWeatherForecastContract will use HttpClient registered by name "WithAuth"
    builder.Contracts.AddContract<IWeatherForecastContract>();
});
```

By default, instance of `HttpClient` will be obtained from IServiceProvider by type `HttpClient`.

```csharp
builder.Services.AddHttpClient("NoAuth", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Register type HttpClient by instances supplied from factory
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NoAuth"));

builder.Services.AddBCFClient(builder =>
{
    // IWeatherForecastContract will use HttpClient registered from factory
    builder.Contracts.AddContract<IWeatherForecastContract>();
});
```

### Scopes

You can create scopes for contracts that enable you to combine multiple types of `IHttpClientResolver`. All contracts registered inside the scope will have the defined `HttpClient`. You can also register contracts by attribute, see [Configuration](#configuration).
```csharp
builder.Services.AddHttpClient("WithAuth", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(sp => sp.GetRequiredService<BlazorCommunicationFoundationHandler>());

builder.Services.AddHttpClient("NoAuth", 
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

// Register type HttpClient by instances supplied from factory
builder.Services.AddScoped(
    sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NoAuth"));

builder.Services.AddBCFClient(builder =>
{
    builder.CreateScope(scope =>
    {
        scope.UseNamedHttpClient("WithAuth");
        // IUserActionContract will use HttpClient registered by name "WithAuth"
        scope.Contracts.AddContract<IUserActionContract>();
    });
    // IWeatherForecastContract will use HttpClient registered from factory
    builder.Contracts.AddContract<IWeatherForecastContract>();
});
```

## Exception handling

When a status code `500 Internal Server Error` occurs, contents of the response will be dumped into console as string. This is compliant with developer exception page of .NET Core 3+, see [Handle errors in ASP.NET Core web APIs](https://docs.microsoft.com/en-us/aspnet/core/web-api/handle-errors?view=aspnetcore-5.0).

![exception_detailed](images/exception_detailed.jpg)

If you want to customize exception message, you can use a custom middleware placed after `app.UseDeveloperExceptionPage()` and before the `app.UseBlazorCommunicationFoundation()` and send the desired output as a body of the error response with status code `500 Internal Server Error`.

You can use the prepared `ExceptionHandlerMiddlewareBase` that will handle only exceptions that occured while handling BCF requests. Example usage can be found in sample server: `BCFExceptionHandlerMiddleware`.

```csharp
namespace Blazor.BlazorCommunicationFoundation.Sample.Server.Middlewares
{
    public class BCFExceptionHandlerMiddleware : ExceptionHandlerMiddlewareBase
    {
        public BCFExceptionHandlerMiddleware(RequestDelegate next) : base(next)
        {
        }

        protected override string SerializeException(Exception e)
        {
            return e.ToString();
        }
    }
}
```

## Swagger integration

BCF is fully integrated into Swagger through API Explorer, so any library for API gerenating from API Explorer would work.

![swagger-integration](images/swagger-integration.png)

## Debugging

By default, Blazor Communication Foundation uses MessagePack to serialize communication between client and server, which is not human-readable.

To have an insight into the communication, I recommend using JSON serializer for `DEBUG` using `#if` directive. See [Custom serializer](#custom-serializer).

### Request
Request contains binding information with binding identifier and name and value of the arguments. You can use query string to see what method on what contract is being called, but it has only informational value for the developer. Internally, `BindingIdentifier` is used. 
![request](images/json_request.jpg)

### Response
Response contains only serialized values by the selected serializer.
![response](images/json_response.jpg)