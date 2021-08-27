using BookMarket.Api;
using BookMarket.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace BookMarket
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //Save if running in debug or release
            Costants.IsDebug = Debugger.IsAttached;

        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Add db connection string
            string DbContext = Configuration.GetConnectionString("Production");

            services.AddDbContext<BookMarket_DBContext>(options => options.UseLazyLoadingProxies().UseSqlServer(DbContext));

            // Add SQL stored session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
            });
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = DbContext;
                options.SchemaName = "dbo";
                options.TableName = "SessionCache";
            });

            //Add mail configurations
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.AddTransient<IMailService, MailService>();

            services.AddMemoryCache();
            services.AddRazorPages().AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 314572800;
            });
        }

        public IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }




            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });


            //Set italian as default language
            var cultureInfo = new CultureInfo("it-IT");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }
    }
}
