using System;
using ITJakub.MobileApps.Client.Books.Service;
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
            container.RegisterType<DataService>(WithLifetime.ContainerControlled(typeof (DataService)));
            container.RegisterType<NavigationService>(WithLifetime.ContainerControlled(typeof (NavigationService)));
        }
    }
}
