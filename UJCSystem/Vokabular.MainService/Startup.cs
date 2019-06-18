﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DryIoc;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Vokabular.Authentication.Client;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Authentication.Client.SharedClient.Config;
using Vokabular.Core;
using Vokabular.ForumSite.Core;
using Vokabular.ForumSite.Core.Options;
using Vokabular.MainService.Authorization;
using Vokabular.MainService.Core;
using Vokabular.MainService.Middleware;
using Vokabular.MainService.Options;
using Vokabular.MainService.Utils;
using Vokabular.Shared;
using Vokabular.Shared.AspNetCore.Container;
using Vokabular.Shared.AspNetCore.Container.Extensions;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
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
        private DryIocContainerWrapper Container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var openIdConnectConfig = Configuration.GetSection("OpenIdConnect").Get<OpenIdConnectConfiguration>();

            // Configuration options
            services.AddOptions();
            services.Configure<List<EndpointOption>>(Configuration.GetSection("Endpoints"));
            services.Configure<List<CredentialsOption>>(Configuration.GetSection("Credentials"));
            services.Configure<PathConfiguration>(Configuration.GetSection("PathConfiguration"));
            services.Configure<ForumOption>(Configuration.GetSection("ForumOptions"));

            services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 1048576000; });

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

                options.AddSecurityDefinition("implicit-oauth2", new OAuth2Scheme
                {
                    Flow = "implicit",
                    AuthorizationUrl = $"{openIdConnectConfig.Url}connect/authorize",
                    TokenUrl = $"{openIdConnectConfig.Url}connect/token",
                    Scopes = new Dictionary<string, string> {
                        { "auth_api.Internal", "API - internal" },
                    }
                });

                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    {"implicit-oauth2", new[] {"auth_api.Internal"}}
                });
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = openIdConnectConfig.Url;
                    options.Audience = "auth_api";

                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.ValidateLifetime = true;
                });

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

            services.RegisterAuthorizationHttpClientComponents<AuthServiceClientLocalization>(new AuthServiceCommunicationConfiguration
            {
                TokenName = null, // not required
                ApiAccessToken = null, // not required
                AuthenticationServiceAddress = openIdConnectConfig.Url,
            }, new OpenIdConnectConfig
            {
                Url = openIdConnectConfig.Url,
                Scopes = new List<string> { openIdConnectConfig.AuthServiceScopeName },
                ClientId = null, // not required
                ClientSecret = null, // not required
            }, new AuthServiceControllerBasePathsConfiguration
            {
                PermissionBasePath = "api/v1/permission/",
                RoleBasePath = "api/v1/role/",
                UserBasePath = "api/v1/user/",
                RegistrationBasePath = "api/v1/registration/",
                ExternalLoginProviderBasePath = "api/v1/externalLoginProvider/",
                FileResourceBasePath = "api/v1/fileResource/",
                NonceBasePath = "api/v1/nonce/",
                ContactBasePath = "api/v1/contact/",
                LoginCheckBasePath = "Account/CheckLogin",
            });

            // IoC
            var container = new DryIocContainerWrapper();
            container.RegisterLogger();
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime applicationLifetime)
        {
            ApplicationLogging.LoggerFactory = loggerFactory;

            app.ConfigureAutoMapper();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Vokabular MainService API v1"); // using relative address to Swagger UI
                c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete, SubmitMethod.Head);

                c.OAuthConfigObject.ClientId = string.Empty;
                c.OAuthConfigObject.ClientSecret = string.Empty;
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