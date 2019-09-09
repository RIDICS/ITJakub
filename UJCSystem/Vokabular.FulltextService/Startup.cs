using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Vokabular.FulltextService.Core.Options;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;
using Vokabular.Shared.AspNetCore;
using Vokabular.Shared.AspNetCore.Container;
using Vokabular.Shared.AspNetCore.Container.Extensions;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.Options;

namespace Vokabular.FulltextService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ApplicationConfig.Configuration = Configuration;
        }

        private IConfiguration Configuration { get; }

        private DryIocContainerWrapper Container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Configuration options
            services.AddOptions();
            services.Configure<EndpointOption>(Configuration.GetSection("Endpoints"));
            services.Configure<IndicesOption>(Configuration.GetSection("ElasticsearchIndices"));
            services.Configure<SpecialCharsOption>(Configuration.GetSection("SpecialChars"));

            // Add framework services
            services.AddMvc();

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Vokabular FulltextService API",
                    Version = "v1",
                });
                options.DescribeAllEnumsAsStrings();
                options.IncludeXmlComments(ServiceUtils.GetAppXmlDocumentationPath());
                options.IncludeXmlComments(ServiceUtils.GetAppXmlDocumentationPath(typeof(FulltextSearchCorpusResultContract)));

                options.DocumentFilter<PolymorphismDocumentFilter<SearchCriteriaContract>>();
                options.SchemaFilter<PolymorphismSchemaFilter<SearchCriteriaContract>>();
            });

            // AutoMapper
            services.AddAutoMapper();

            // IoC
            var container = new DryIocContainerWrapper();
            container.Install<FulltextServiceContainerRegistration>();
            Container = container;

            return container.CreateServiceProvider(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            ApplicationLogging.LoggerFactory = loggerFactory;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Vokabular FulltextService API v1"); // using relative address to Swagger UI
            });

            applicationLifetime.ApplicationStopped.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            Container.Dispose();
        }
    }
}
