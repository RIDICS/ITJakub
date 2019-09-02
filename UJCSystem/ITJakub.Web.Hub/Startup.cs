using System;
using System.Collections.Generic;
using ITJakub.Web.Hub.Models.Config;
using System.Security.Claims;
using System.Threading.Tasks;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Helpers;
using ITJakub.Web.Hub.Middleware;
using ITJakub.Web.Hub.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Ridics.Authentication.HttpClient;
using Ridics.Authentication.HttpClient.Configuration;
using Ridics.Authentication.TicketStore;
using Ridics.Authentication.TicketStore.Store;
using Ridics.Core.HttpClient.Config;
using Scalesoft.Localization.AspNetCore;
using Scalesoft.Localization.AspNetCore.IoC;
using Scalesoft.Localization.Core.Configuration;
using Scalesoft.Localization.Core.Util;
using Scalesoft.Localization.Database.NHibernate;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient;
using Vokabular.Shared;
using Vokabular.Shared.AspNetCore.Container;
using Vokabular.Shared.AspNetCore.Container.Extensions;
using Vokabular.Shared.AspNetCore.Extensions;
using Vokabular.Shared.Const;
using Vokabular.Shared.Options;

namespace ITJakub.Web.Hub
{
    public class Startup
    {
        private readonly TimeSpan m_cookieExpireTimeSpan = TimeSpan.FromMinutes(60);

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ApplicationConfig.Configuration = Configuration;
        }

        private IConfiguration Configuration { get; }
        private DryIocContainerWrapper Container { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //IdentityModelEventSource.ShowPII = true; // Enable to debug authentication problems

            var openIdConnectConfig = Configuration.GetSection("OpenIdConnect").Get<OpenIdConnectConfiguration>();
            var portalConfig = Configuration.GetSection("PortalConfig").Get<PortalOption>();
            var endpointsConfiguration = Configuration.GetSection("Endpoints").Get<EndpointOption>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.ExpireTimeSpan = m_cookieExpireTimeSpan;
                    options.Cookie.Name = $"identity{portalConfig.PortalType}";
                    options.AccessDeniedPath = "/Error/403/";
                    options.LoginPath = "/Account/Login";
                })
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = openIdConnectConfig.Url;
                    options.ClientSecret = openIdConnectConfig.ClientSecret;
                    options.ClientId = openIdConnectConfig.ClientId;

                    options.ResponseType = "code id_token";

                    options.Scope.Clear();
                    options.Scope.Add("openid");
                    options.Scope.Add("profile");
                    options.Scope.Add("offline_access");
                    options.Scope.Add("auth_api.Internal");

                    options.ClaimActions.MapJsonKey(ClaimTypes.Role, "role");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                    options.ClaimActions.MapJsonKey(CustomClaimTypes.EmailConfirmed, "email_confirmed");
                    options.ClaimActions.MapJsonKey(CustomClaimTypes.Permission, CustomClaimTypes.Permission);
                    options.ClaimActions.MapJsonKey(CustomClaimTypes.ResourcePermission, CustomClaimTypes.ResourcePermission);
                    options.ClaimActions.MapJsonKey(CustomClaimTypes.ResourcePermissionType, CustomClaimTypes.ResourcePermissionType);

                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.SaveTokens = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.Name,
                        RoleClaimType = ClaimTypes.Role
                    };

                    //Adds return url address in case of user clicking on "Leave" button on login page
                    options.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            var returnUrl = context.Request.GetAppBaseUrl();
                            context.ProtocolMessage.SetParameter("returnUrlOnCancel", returnUrl.ToString());

                            var culture = context.HttpContext.RequestServices.GetRequiredService<ILocalizationService>()
                                .GetRequestCulture();
                            context.ProtocolMessage.SetParameter("culture", culture.Name);

                            return Task.CompletedTask;
                        },
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            var culture = context.HttpContext.RequestServices.GetRequiredService<ILocalizationService>()
                                .GetRequestCulture();
                            context.ProtocolMessage.SetParameter("culture", culture.Name);

                            return Task.CompletedTask;
                        },
                        OnUserInformationReceived = context =>
                        {
                            var communicationProvider = context.HttpContext.RequestServices.GetRequiredService<CommunicationProvider>();
                            var client = communicationProvider.GetMainServiceUserClient();

                            client.CreateUserIfNotExist(context.Principal.GetIdOrDefault().GetValueOrDefault());

                            return Task.CompletedTask;
                        },
                    };
                });

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

            //store claims in memory:
            services.SetAuthenticationTicketStore<MemoryCacheTicketStore>(new CacheTicketStoreConfig
            {
                SlidingExpiration = m_cookieExpireTimeSpan
            });
            services.RegisterAutomaticTokenManagement();

            // Register Auth service client, because contains components for obtaining access token (for user and also for app)
            services.RegisterAuthorizationHttpClientComponents<AuthServiceClientLocalization>(new AuthServiceCommunicationConfiguration
            {
                TokenName = null, // not required
                ApiAccessToken = null, // not required
                AuthenticationServiceAddress = openIdConnectConfig.Url,
            }, new OpenIdConnectConfig
            {
                Url = openIdConnectConfig.Url,
                Scopes = new List<string> {openIdConnectConfig.AuthServiceScopeName},
                ClientId = openIdConnectConfig.ClientId,
                ClientSecret = openIdConnectConfig.ClientSecret,
            }, new AuthServiceControllerBasePathsConfiguration( /*Not required to fill because client is not used*/));

            services.RegisterMainServiceClientComponents<AuthTokenProvider, MainServiceClientLocalization>(new MainServiceClientConfiguration
            {
                PortalType = (PortalTypeContract) portalConfig.PortalType,
                Url = new Uri(endpointsConfiguration.Addresses["MainService"]),
                CreateCustomHandler = false
            });

            // Configuration options
            services.AddOptions();
            services.AddSingleton(openIdConnectConfig);
            services.Configure<EndpointOption>(Configuration.GetSection("Endpoints"));
            services.Configure<GoogleCalendarConfiguration>(Configuration.GetSection("GoogleCalendar"));
            services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 1048576000; });
            services.Configure<PortalOption>(Configuration.GetSection("PortalConfig"));
            services.Configure<AutoLoginCookieConfiguration>(Configuration.GetSection("AutoLoginCookie"));
            services.Configure<ForumOption>(Configuration.GetSection("Forum"));
            services.PostConfigure<AutoLoginCookieConfiguration>(config =>
            {
                config.CookieName = $"{AutoLoginCookieConfiguration.CookieNamePrefix}{portalConfig.PortalType}";
            });

            // Localization
            var localizationConfiguration = Configuration.GetSection("Localization").Get<LocalizationConfiguration>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddLocalizationService(localizationConfiguration, new NHibernateDatabaseConfiguration());

            services.AddMvc()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        return factory
                            .Create(type.Name, LocTranslationSource.File.ToString());
                    };
                });

            // IoC
            var container = new DryIocContainerWrapper();
            container.RegisterLogger();
            container.Install<WebHubContainerRegistration>();
            container.Install<NHibernateInstaller>();
            Container = container;


            return container.CreateServiceProvider(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime applicationLifetime)
        {
            ApplicationLogging.LoggerFactory = loggerFactory;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error/500");
            }

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseAuthentication();

            app.ConfigureAutoMapper();

            app.UseMiddleware<ErrorHandlingMiddleware>();

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
                    .MapAreaRoute("professionalLiteratureDefault", "ProfessionalLiterature",
                        "{controller=ProfessionalLiterature}/{action=Index}")
                    .MapAreaRoute("bibliographiesDefault", "Bibliographies", "{controller=Bibliographies}/{action=Index}")
                    .MapAreaRoute("cardFilesDefault", "CardFiles", "{controller=CardFiles}/{action=Index}")
                    .MapAreaRoute("audioBooksDefault", "AudioBooks", "{controller=AudioBooks}/{action=Index}")
                    .MapAreaRoute("toolsDefault", "Tools", "{controller=Tools}/{action=Index}");
            });

            // Update missing permissions on Auth service:
            var communicationProvider = app.ApplicationServices.GetRequiredService<CommunicationProvider>();
            communicationProvider.GetMainServicePermissionClient().EnsureAuthServiceHasRequiredPermissions();

            applicationLifetime.ApplicationStopped.Register(OnShutdown);
        }

        private void OnShutdown()
        {
            Container.Dispose();
        }
    }
}