# Blazor Mobx <!-- omit in toc -->

Blazor Mobx is a mutable state management for Blazor based on a popular Mobx.js. It is trying to provide abstraction over a state management and ease the development by automatically updating components inside your application based just on mutating your state.

- [How it works](#how-it-works)
- [Installation](#installation)
- [Configuration](#configuration)

## How it works

Internally, all properties of your store are overidden. Values of your properties are preserved based on the lifetime you chose for your stores. You get a new instance of `IStoreAccessor` each time you try to access your store which then tracks which properties you accessed and  when any of them gets updated, your instance of `IStoreAccessor` automatically calls `StateHasChanged` and leaves the rest on Blazor.

## Installation

If you are using Blazor WASM, install the package into your client application. In case you are using Blazor ServerSide, install the package into your ASP.NET Core application.
```
Install-Package VrsekDev.Blazor.Mobx
```

## Configuration

Register Blazor Mobx into IServiceCollection
```csharp
using VrsekDev.Blazor.Mobx.Extensions;
....

builder.Services.AddDefaultMobxProperties();
```