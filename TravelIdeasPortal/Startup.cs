using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

using SujaySarma.Sdk.AspNetCore.Mvc;

using System.IO;

namespace TravelIdeasPortalWeb
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IConfiguration config, IWebHostEnvironment env)
        {
            app.UseAppSettingsJson(config, env);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                
                // This is not working! Throws weird exceptions. Do not enable until it is fixed properly.
                app.UseSimpleDomainRedirectionMiddleware("flighthoteltravelideas.com", true);
            }

            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles(
                    new StaticFileOptions()
                    {
                        FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "StaticContent")),
                        RequestPath = "/StaticContent",
                        HttpsCompression = Microsoft.AspNetCore.Http.Features.HttpsCompressionMode.Compress
                    }
                );

            app.UseSession();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            System.Environment.SetEnvironmentVariable("WIKIPEDIA_CLIENT_API_EMAIL", "flighthoteltravelideas.com");
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.None;
                options.ConsentCookie = new Microsoft.AspNetCore.Http.CookieBuilder()
                {
                    IsEssential = true
                };
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = new System.TimeSpan(0, 30, 0);
                options.Cookie.IsEssential = true;
            });

            services.AddControllersWithViews();
            services.AddApplicationInsightsTelemetry();
        }
    }
}
