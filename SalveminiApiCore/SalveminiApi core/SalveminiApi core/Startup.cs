using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SalveminiApi_core.Models;
namespace SalveminiApi_core
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

           
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Lighter response
            services.AddResponseCompression();

            //Add db connection string
            services.AddDbContext<Salvemini_DBContext>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("Default")));

            //Used to get ip
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            //Clear session values after 30 minutes
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
            });

            services.AddRazorPages().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

            //SignalR
            services.AddSignalR();

            //To store session
            services.AddMemoryCache();

           
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Set error pages
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //Used for razor pages
                app.UseExceptionHandler("/Error");

                //Used for api requests
                app.UseWhen(context => context.Request.Path.StartsWithSegments("/api"), subApp =>
                {
                    subApp.UseExceptionHandler(builder =>
                    {
                        builder.Run(async context =>
                        {
                            var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                            var exception = feature.Error;

                            //Get endpoint
                            var url = Microsoft.AspNetCore.Http.Extensions.UriHelper.GetDisplayUrl(context.Request);
                            //Get headers
                            var headers = context.Request.Headers;
                            var headersList = "";
                            foreach (var header in headers)
                            {
                                headersList += header.Key + ": " + header.Value + Environment.NewLine;
                            }

                            //Return error
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        });
                    });
                });

                //Idk
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseResponseCompression();
            app.UseSession();
            app.UseRouting();
            //app.UseCors();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(name: "Api", pattern: "api/{controller}/{id}");
                //endpoints.MapHub<ChatHub>("/chathub");
            });

            //Set italian as default language
            var cultureInfo = new CultureInfo("it-IT");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}

