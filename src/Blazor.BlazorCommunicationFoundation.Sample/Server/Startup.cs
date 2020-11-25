using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Blazor.BlazorCommunicationFoundation.Sample.Server.Data;
using Blazor.BlazorCommunicationFoundation.Sample.Server.Models;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.DependencyInjection;
using Blazor.BlazorCommunicationFoundation.Sample.Shared;
using Blazor.BlazorCommunicationFoundation.Sample.Server.Services;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Server.Middlewares;
using VrsekDev.Blazor.BlazorCommunicationFoundation.Serializers.Json;
using Blazor.BlazorCommunicationFoundation.Sample.Server.Middlewares;

namespace Blazor.BlazorCommunicationFoundation.Sample.Server
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddBCFServer(builder =>
            {
#if DEBUG
                //builder.UseSerializer<JsonInvocationSerializer>();
#endif
                builder.Contracts.AddTransient<IWeatherForecastContract, WeatherForecastService>();

                builder.Contracts.AddContractsByAttribute(typeof(UserActionService).Assembly, serviceLifetime: ServiceLifetime.Transient);
            });

            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMiddleware<BCFExceptionHandlerMiddleware>();

                app.UseDatabaseErrorPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseBlazorCommunicationFoundation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
