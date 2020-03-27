using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using AutoMapper;
using Domain.Models.WebModels;
using Domain.Models.WebModels.BroadcastDiagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebAdmin
{
    public class Startup
    {
        private AppSettings _appSettings = new AppSettings();
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(env.ContentRootPath)
            //    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            //    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            //builder.AddEnvironmentVariables();
            //Configuration = builder.Build();
            Configuration = configuration;
            //Mapper.Initialize(cfg =>
            //{
            //    cfg.CreateMap<MainDiagnostics, BroadcastDiagnosticsRow1>();
            //});

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            // Add functionality to inject IOptions<T>
            //services.AddOptions();
            // Add our Config object so it can be injected
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            services.Configure<Connections>(Configuration.GetSection("ConnectionStrings"));
            // *If* you need access to generic IConfiguration this is **required**
            // services.AddSingleton<IConfiguration>(Configuration)
            //services.AddSingleton(Configuration);

            //appSettingsSection.Bind(_appSettings);
            // Allow injection of our strongly-type AppSettings...
            //services.AddSingleton<AppSettings>(_appSettings);

            services.AddMvc();

            //services.AddScoped<AFDbContext>(_ => new AFDbContext(Configuration.GetConnectionString("AFTM-DB")));

            //services.AddTransient<IEmailSender, Email>();
            //services.Configure<IISOptions>(options => { });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
