using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Havit.Blazor.StateManagement.Mobx.Extensions;
using Havit.Blazor.StateManagement.Mobx.PropertyObservables.Dynamic.Extensions;
using Havit.Blazor.StateManagement.Mobx.ObservableProperties.Default.Extensions;
using Havit.Blazor.StateManagement.Mobx.Samples.Shared.Stores;

namespace Havit.Blazor.StateManagement.Mobx.Samples
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.UseDefaultObservableProperties();
            services.UseDynamicPropertyObservables();
            services.AddMobxStore<IAppStore>().WithDefaultState(new DefaultAppStore()).AsSingleton();
            services.AddMobxStore<IHomeStore>().WithActions<HomeStoreActions>().AsSingleton();
            services.AddMobxStore<ICounterStore>().AsTransient();
            services.AddMobxStore<ITodoStore>().WithActions<TodoStoreActions>().Cascading();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
