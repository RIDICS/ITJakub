﻿using System;
using System.Collections.Generic;
using Log4net.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Vokabular.MainService.Containers.Extensions;
using Vokabular.MainService.Containers;
using Vokabular.MainService.Containers.Installers;
using Vokabular.Shared;
using Vokabular.Shared.Container;
using Vokabular.Shared.Options;

namespace Vokabular.MainService
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            ApplicationConfig.Configuration = Configuration;

            env.ConfigureLog4Net("log4net.config");
        }

        private IConfiguration Configuration { get; }
        private IIocContainer Container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Configuration options
            services.AddOptions();
            services.Configure<List<EndpointOption>>(Configuration.GetSection("Endpoints"));

            // Add framework services.
            services.AddMvc();

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "Vokabular MainService API",
                    Version = "v1",
                });
                options.DescribeAllEnumsAsStrings();
            });

            // IoC
            IIocContainer container = new DryIocContainer();
            container.Install<MainServiceContainerRegistration>();
            container.Install<NHibernateInstaller>();
            Container = container;

            return container.CreateServiceProvider(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime applicationLifetime)
        {
            // Configure logging
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            loggerFactory.AddLog4Net();
            ApplicationLogging.LoggerFactory = loggerFactory;

            app.ConfigureAutoMapper();

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vokabular MainService API v1");
            });

            applicationLifetime.ApplicationStopped.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            Container.Dispose();
        }
    }
}
