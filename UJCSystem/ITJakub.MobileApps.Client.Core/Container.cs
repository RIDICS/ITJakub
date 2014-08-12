using System;
using ITJakub.MobileApps.Client.Core.Manager;
using ITJakub.MobileApps.Client.Core.Manager.Authentication;
using ITJakub.MobileApps.Client.Core.Manager.Authentication.AuthenticationProviders;
using ITJakub.MobileApps.Client.Core.Manager.Communication;
using ITJakub.MobileApps.Client.Core.Manager.Groups;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Core
{
    public class Container
    {
        #region Unity Container

        private static readonly Lazy<IUnityContainer> m_container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        ///     Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer Current
        {
            get { return m_container.Value; }
        }

        #endregion

        private static void RegisterTypes(IUnityContainer container)
        {
            
            //// Your registration logic ...
            //container.RegisterTypes(AllClasses.From,
            //    WithMappings.FromMatchingInterface,
            //    WithName.Default,
            //    WithLifetime.ContainerControlled
            //    );



            RegisterLoginProviders(container);
            container.RegisterType<ApplicationManager>();
            container.RegisterType<AuthenticationManager>(WithLifetime.ContainerControlled(typeof(AuthenticationManager)));
            container.RegisterType<MobileAppsServiceManager>(WithLifetime.ContainerControlled(typeof(MobileAppsServiceManager)));
            container.RegisterType<UserAvatarCache>(WithLifetime.ContainerControlled(typeof(UserAvatarCache)));
            container.RegisterType<GroupManager>(WithLifetime.ContainerControlled(typeof(GroupManager)));


            //container.RegisterTypes(AllClasses.FromApplication())
        }

        private static void RegisterLoginProviders(IUnityContainer container)
        {
            container.RegisterType<ILoginProvider, FacebookProvider>(WithName.TypeName(typeof(FacebookProvider)));
            container.RegisterType<ILoginProvider, GoogleProvider>(WithName.TypeName(typeof(GoogleProvider)));
            container.RegisterType<ILoginProvider, LiveIdProvider>(WithName.TypeName(typeof(LiveIdProvider)));
            container.RegisterType<ILoginProvider, ItJakubProvider>(WithName.TypeName(typeof(ItJakubProvider)));

            //container.RegisterTypes(
            //    AllClasses.FromApplication().Where(x => x.GetTypeInfo().IsSubclassOf(typeof(ILoginProvider))),
            //    WithMappings.None,
            //    WithName.TypeName,
            //    WithLifetime.ContainerControlled
            //    );
        }
    }
}