# Blazor Communication Foundation - Authorization  <!-- omit in toc -->

Authentication package for Blazor Communication Foundation includes both authorization and authentication. It uses authorization attributes from `Microsoft.AspNetCore.Authorization` and thereby simplifies use of this package.

Blazor Communication Foundation uses underlying authentication between client and server, so you need to make sure, that the authentication is already working before you try to implement authentication for Blazor Communication Foundation. For more information about authentication and authorization, see [official documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-5.0).

## Installation

Into your client application, install package
```
Install-Package VrsekDev.Blazor.BlazorCommunicationFoundation.Client.Authentication
```

## Configuration

You can apply `AuthorizeAttribute` into your contract implementation on the server the same way as you would normally do on your MVC controller and actions.

```csharp
public class UserActionService : IUserActionContract
{
    [Authorize]
    public Task PerformActionAsync()
    {
        return Task.CompletedTask;
    }
}
```

or directly into your contract interface

```csharp
public interface IUserActionContract
{
    [Authorize]
    Task PerformActionAsync();
}
```

Also, `AuthorizeAttribute` can be added onto class/interface, which then requires authentication for all methods declared inside that class/interface

```csharp
[Authorize]
public class UserActionService : IUserActionContract
{
    public Task PerformActionAsync()
    {
        return Task.CompletedTask;
    }
}
```

### Handling unathorized users

To handle unathorized users trying to access protected resources, you can use `BlazorCommunicationFoundationHandler` as a `DelegatingHandler`. It intercepts `401 Unauthorized` status codes from **BCF handler** and then throws exception of type `UnauthorizedException`. You can then catch this exception and redirect user onto the login page or you can specify a `loginUrl` param and then you can redirect user by calling method `RedirectToLogin` in a caught exception.

Configurating `HttpClient`
```csharp
builder.Services.AddHttpClient("WithAuth", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler(
        sp => sp.GetRequiredService<BlazorCommunicationFoundationHandler>()
            .ConfigureHandler(loginUrl: "/authentication/login"));
```

Handling unauthenticated users in a blazor component
```csharp
try
{
    await UserActionContract.PerformActionAsync();
}
catch (UnauthorizedException exception)
{
    exception.RedirectToLogin();
}
```

### Role-based authorization

See [official documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-5.0)

### Claims-based authorization

See [official documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/claims?view=aspnetcore-5.0)

### Policy-based authorization

See [official documentation](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0#apply-policies-to-mvc-controllers)

