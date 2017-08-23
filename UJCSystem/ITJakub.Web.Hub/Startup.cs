using System;
using System.Collections.Generic;
using ITJakub.Web.Hub.AppStart;
using ITJakub.Web.Hub.AppStart.Containers;
using ITJakub.Web.Hub.AppStart.Extensions;
using ITJakub.Web.Hub.AppStart.Middleware;
using Log4net.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vokabular.Shared;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;

namespace ITJakub.Web.Hub
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var globalbuilder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("globalsettings.json");
            var globalConfiguration = globalbuilder.Build();

            var environmentConfiguration = globalConfiguration["EnvironmentConfiguration"];

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentConfiguration}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            ApplicationConfig.Configuration = Configuration;

            env.ConfigureLog4Net("log4net.config");
        }

        private IConfigurationRoot Configuration { get; }
        private IIocContainer Container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Authorization
            services.AddCustomAuthServices();
            
            // Configuration options
            services.AddOptions();
            services.Configure<List<EndpointOption>>(Configuration.GetSection("Endpoints"));

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1048576000;
            });

            services.AddMvc();

            // IoC
            IIocContainer container = new DryIocContainer();
            container.Install<WebHubContainerRegistration>();
            Container = container;
            
            return container.CreateServiceProvider(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            // Logging
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddLog4Net();
            ApplicationLogging.LoggerFactory = loggerFactory;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStatusCodePages();

            app.ConfigureAuth();

            app.ConfigureAutoMapper();

            app.UseStaticFiles();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areaRoute",
                    template: "{area:exists}/{controller=Home}/{action=Index}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes
                    .MapAreaRoute("dictionariesDefault", "Dictionaries", "{controller=Dictionaries}/{action=Index}")
                    .MapAreaRoute("editionsDefault", "Editions", "{controller=Editions}/{action=Index}")
                    .MapAreaRoute("bohemianTextBankDefault", "BohemianTextBank", "{controller=BohemianTextBank}/{action=Index}")
                    .MapAreaRoute("oldGrammarDefault", "OldGrammar", "{controller=OldGrammar}/{action=Index}")
                    .MapAreaRoute("professionalLiteratureDefault", "ProfessionalLiterature", "{controller=ProfessionalLiterature}/{action=Index}")
                    .MapAreaRoute("bibliographiesDefault", "Bibliographies", "{controller=Bibliographies}/{action=Index}")
                    .MapAreaRoute("cardFilesDefault", "CardFiles", "{controller=CardFiles}/{action=Index}")
                    .MapAreaRoute("audioBooksDefault", "AudioBooks", "{controller=AudioBooks}/{action=Index}")
                    .MapAreaRoute("toolsDefault", "Tools", "{controller=Tools}/{action=Index}");
            });
            
            applicationLifetime.ApplicationStopped.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            Container.Dispose();
        }
    }
}
