using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DryIoc;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Vokabular.Core;
using Vokabular.ForumSite.Core;
using Vokabular.ForumSite.Core.Options;
using Vokabular.MainService.Core;
using Vokabular.MainService.Middleware;
using Vokabular.Shared;
using Vokabular.Shared.AspNetCore.Container;
using Vokabular.Shared.AspNetCore.Container.Extensions;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.Container;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.DataEntities.UnitOfWork;
using Vokabular.Shared.Options;

namespace Vokabular.MainService
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
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Configuration options
            services.AddOptions();
            services.Configure<List<EndpointOption>>(Configuration.GetSection("Endpoints"));
            services.Configure<List<CredentialsOption>>(Configuration.GetSection("Credentials"));
            services.Configure<PathConfiguration>(Configuration.GetSection("PathConfiguration"));
            services.Configure<ForumOption>(Configuration.GetSection("ForumOptions"));

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 1048576000;
            });

            // Add framework services.
            services.AddMvc()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<MainServiceCoreContainerRegistration>());
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            
            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Vokabular MainService API",
                    Version = "v1",
                });
                options.DescribeAllEnumsAsStrings();
                options.IncludeXmlComments(GetXmlCommentsPath());
                options.OperationFilter<AddResponseHeadersFilter>();

                options.DocumentFilter<PolymorphismDocumentFilter<SearchCriteriaContract>>();
                options.SchemaFilter<PolymorphismSchemaFilter<SearchCriteriaContract>>();
            });

            // IoC
            var container = new DryIocContainerWrapper();
            Container = container;

            container.Install<MainServiceContainerRegistration>();
            container.InnerContainer.AddNHibernateDefaultDatabase();
            container.InnerContainer.AddNHibernateForumDatabase();

            container.InnerContainer.Register<IUnitOfWork>(Made.Of(
                () => GetCorrectUnitOfWork(Arg.Of<IResolver>(), Arg.Index<Type>(0)),
                request => request.Parent.ImplementationType));

            container.Install<ForumCoreContainerRegistration>();

            return container.CreateServiceProvider(services);
        }

        private static IUnitOfWork GetCorrectUnitOfWork(IResolver resolver, Type parentImplementationType)
        {
            // TODO better logic for selecting correct UnitOfWork
            if (parentImplementationType.Namespace != null && parentImplementationType.Namespace.StartsWith("Vokabular.ForumSite"))
            {
                return resolver.Resolve<UnitOfWork>("forum");
            }

            return resolver.Resolve<UnitOfWork>("default");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            ApplicationLogging.LoggerFactory = loggerFactory;

            app.ConfigureAutoMapper();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Vokabular MainService API v1"); // using relative address to Swagger UI
                c.SupportedSubmitMethods(new[] {"get", "post", "put", "delete", "head"});
            });

            applicationLifetime.ApplicationStopped.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            Container.Dispose();
        }

        private string GetXmlCommentsPath()
        {
            var appBasePath = AppContext.BaseDirectory;
            var appName = Assembly.GetEntryAssembly().GetName().Name;
            return Path.Combine(appBasePath, $"{appName}.xml");
        }
    }
}
