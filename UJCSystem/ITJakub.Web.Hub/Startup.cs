using System;
using System.Collections.Generic;
using Localization.AspNetCore.Service.Extensions;
using Localization.CoreLibrary.Dictionary.Factory;
using Localization.CoreLibrary.Util;
using Localization.Database.EFCore.Data.Impl;
using Localization.Database.EFCore.Factory;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vokabular.Shared;
using Vokabular.Shared.AspNetCore.Container;
using Vokabular.Shared.AspNetCore.Container.Extensions;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace ITJakub.Web.Hub
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            ApplicationConfig.Configuration = Configuration;
        }

        private IConfiguration Configuration { get; }
        private IIocContainer Container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.AccessDeniedPath = "/Account/AccessDenied/";
                    o.LoginPath = "/Account/Login";
                });


            // Configuration options
            services.AddOptions();
            services.Configure<List<EndpointOption>>(Configuration.GetSection("Endpoints"));

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1048576000;
            });

            // Localization
            var connectionString = Configuration.GetConnectionString(SettingKeys.WebConnectionString) ??
                                   throw new ArgumentException("Connection string not found");
            services.AddDbContext<StaticTextsContext>(options => options
                .UseSqlServer(connectionString));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddLocalizationService();


            services.AddMvc()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        return factory
                            .Create(type.Name, LocTranslationSource.File.ToString());
                    };
                })
                .AddRazorOptions(options =>
                {
                    var previous = options.CompilationCallback;
                    options.CompilationCallback = context =>
                    {
                        previous?.Invoke(context);

                        context.Compilation = context.Compilation.AddReferences(Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(typeof(Localization.AspNetCore.Service.ILocalization).Assembly.Location));
                        context.Compilation = context.Compilation.AddReferences(Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(typeof(Localization.CoreLibrary.Localization).Assembly.Location));
                    };
                });

            // IoC
            IIocContainer container = new DryIocContainer();
            container.Install<WebHubContainerRegistration>();
            Container = container;
            

            return container.CreateServiceProvider(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            ApplicationLogging.LoggerFactory = loggerFactory;

            var configuration = app.ApplicationServices.GetService<TelemetryConfiguration>();
            configuration.DisableTelemetry = true; // Workaround for disabling telemetry

            // Localization
            Localization.CoreLibrary.Localization.Init(
                @"localizationsettings.json",
                new DatabaseServiceFactory(Container.Resolve<StaticTextsContext>()),
                new JsonDictionaryFactory());
            Localization.CoreLibrary.Localization.AttachLogger(loggerFactory);

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

            app.UseAuthentication();

            app.ConfigureAutoMapper();

            app.UseStaticFiles();

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
