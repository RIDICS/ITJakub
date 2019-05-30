using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using Vokabular.Authentication.Client;
using Vokabular.Authentication.Client.Configuration;
using Vokabular.Core;
using Vokabular.MainService.Authorization;
using Vokabular.MainService.Core;
using Vokabular.MainService.Middleware;
using Vokabular.MainService.Utils;
using Vokabular.Shared;
using Vokabular.Shared.AspNetCore.Container;
using Vokabular.Shared.AspNetCore.Container.Extensions;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;
using Vokabular.Shared.Const;
using Vokabular.Shared.DataContracts.Search.Criteria;
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
        private DryIocContainer Container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var openIdConnectConfig = Configuration.GetSection("OpenIdConnect").Get<OpenIdConnect>();

            // Configuration options
            services.AddOptions();
            services.Configure<List<EndpointOption>>(Configuration.GetSection("Endpoints"));
            services.Configure<List<CredentialsOption>>(Configuration.GetSection("Credentials"));
            services.Configure<PathConfiguration>(Configuration.GetSection("PathConfiguration"));

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
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = openIdConnectConfig.Url;
                    options.Audience = "auth_api";

                    // Disable validation (client-app is already authenticated)
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.ValidateIssuer = false;
                    options.TokenValidationParameters.ValidateLifetime = false;

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            AddClaimsToUser(ref context, openIdConnectConfig.UserInfoEndpoint);
                            return Task.CompletedTask;
                        }
                    };
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
                AuthServiceScopeName = openIdConnectConfig.AuthServiceScopeName,
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
            var container = new DryIocContainer();
            container.RegisterLogger();
            container.Install<MainServiceContainerRegistration>();
            container.Install<NHibernateInstaller>();
            Container = container;

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

        private void AddClaimsToUser(ref TokenValidatedContext context, string userInfoEndpoint)
        {
            using (var client = new HttpClient())
            {
                var jwtToken = (JwtSecurityToken) context.SecurityToken;
                client.SetBearerToken(jwtToken.RawData);

                var content = client.GetStringAsync(userInfoEndpoint).Result;
                IList<Claim> claims = new List<Claim>();

                var arrayName = new[] {CustomClaimTypes.Permission, CustomClaimTypes.ResourcePermissionType, CustomClaimTypes.ResourcePermission};

                foreach (var property in JToken.Parse(content).Children<JProperty>())
                {
                    if (arrayName.Contains(property.Name))
                    {
                        try
                        {
                            var permArray = JArray.Parse(property.Value.ToString());
                            foreach (var permission in permArray)
                            {
                                claims.Add(new Claim(property.Name, permission.ToString(), context.SecurityToken.Issuer));
                            }
                        }
                        catch (JsonReaderException)
                        {
                            claims.Add(new Claim(property.Name, property.Value.ToString()));
                        } 

                    }
                    else
                    {
                        claims.Add(new Claim(property.Name, property.Value.ToString()));
                    }
                }

                context.Principal.AddIdentity(new ClaimsIdentity(claims));
            }
        }
    }
}