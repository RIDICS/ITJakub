using System;
using ITJakub.MobileApps.Client.Books.Manager;
using ITJakub.MobileApps.Client.Books.Service;
using ITJakub.MobileApps.Client.Books.Service.Client;
using Microsoft.Practices.Unity;

namespace ITJakub.MobileApps.Client.Books
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

        private static void RegisterTypes(UnityContainer container)
        {
            container.RegisterType<IDataService, DataService>(WithLifetime.ContainerControlled(typeof (DataService)));
            container.RegisterType<INavigationService, NavigationService>(WithLifetime.ContainerControlled(typeof (NavigationService)));
            container.RegisterType<IServiceClient,MockServiceClient>(WithLifetime.ContainerControlled(typeof (IServiceClient)));
            container.RegisterType<BookManager>(WithLifetime.ContainerControlled(typeof (BookManager)));
        }
    }
}
