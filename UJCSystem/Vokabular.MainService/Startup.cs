using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.HttpClient;
using Ridics.Authentication.HttpClient.Configuration;
using Ridics.Core.HttpClient.Config;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Vokabular.CardFile.Core;
using Vokabular.Core;
using Vokabular.ForumSite.Core;
using Vokabular.ForumSite.Core.Options;
using Vokabular.FulltextService.DataContracts;
using Vokabular.MainService.Authorization;
using Vokabular.MainService.Core;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.Middleware;
using Vokabular.MainService.Options;
using Vokabular.MainService.Utils;
using Vokabular.ProjectImport;
using Vokabular.ProjectImport.Shared.Options;
using Vokabular.Shared;
using Vokabular.Shared.AspNetCore;
using Vokabular.Shared.AspNetCore.Container;
using Vokabular.Shared.AspNetCore.Container.Extensions;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.DataContracts.Search.Criteria;
using Vokabular.Shared.Options;

namespace Vokabular.MainService
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
            var openIdConnectConfig = Configuration.GetSection("OpenIdConnect").Get<OpenIdConnectConfiguration>();
            var endpointsConfiguration = Configuration.GetSection("Endpoints").Get<EndpointOption>();
            var credentialsConfiguration = Configuration.GetSection("Credentials").Get<List<CredentialsOption>>();
            
            // Configuration options
            services.AddOptions();
            services.Configure<EndpointOption>(Configuration.GetSection("Endpoints"));
            services.Configure<List<CredentialsOption>>(Configuration.GetSection("Credentials"));
            services.Configure<PathConfiguration>(Configuration.GetSection("PathConfiguration"));
            services.Configure<OaiPmhClientOption>(Configuration.GetSection("OaiPmhClientOption"));
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
                options.IncludeXmlComments(ServiceUtils.GetAppXmlDocumentationPath());
                options.IncludeXmlComments(ServiceUtils.GetAppXmlDocumentationPath(typeof(BookContract)));
                options.OperationFilter<AddResponseHeadersFilter>();
                options.OperationFilter<FileOperationFilter>();

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

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Authority = openIdConnectConfig.Url;
                    options.Audience = "auth_api";

                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.ValidateLifetime = true;
                })
                // Required for app authentication against the Auth service (for background services)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = openIdConnectConfig.Url;
                    options.ClientSecret = openIdConnectConfig.ClientSecret;
                    options.ClientId = openIdConnectConfig.ClientId;

                    options.Scope.Clear();
                    options.Scope.Add("auth_api.Internal");
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

            services.RegisterFulltextServiceClientComponents(new FulltextServiceClientConfiguration
            {
                Url = new Uri(endpointsConfiguration.Addresses["FulltextService"]),
                CreateCustomHandler = false
            });

            var credentials = credentialsConfiguration.FirstOrDefault(x => x.Type == "CardFiles");
            if (credentials == null)
            {
                throw new ArgumentException("Credentials for Card files not found");
            }

            services.RegisterCardFileClientComponents(new CardFilesCommunicationConfiguration
            {
                Url = new Uri(endpointsConfiguration.Addresses["CardFilesService"]),
                CreateCustomHandler = true,
                Username = credentials.Username,
                Password = credentials.Password
            });

            services.AddProjectImportServices();

            // IoC
            var container = new DryIocContainerWrapper();
            container.RegisterLogger();
            Container = container;

            container.InnerContainer.AddNHibernateDefaultDatabase();
            container.InnerContainer.AddNHibernateForumDatabase();
            
            container.Install<MainServiceContainerRegistration>();
            container.Install<ForumCoreContainerRegistration>();

            return container.CreateServiceProvider(services);
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
    }
}