using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using VrsekDev.Blazor.Mobx.Extensions;
using VrsekDev.Blazor.Templates.Frest.Stores;

namespace VrsekDev.Blazor.Templates.Frest.Registrations
{
    public static class ServiceCollectionRegistrations
    {
        public static void AddFrestTheme(this IServiceCollection services)
        {
            services.AddMobxStore<MainMenuStore>().LifestyleSingleton();
            services.AddMobxStore<MenuContainerStore>().LifestyleCascading();
        }
    }
}
