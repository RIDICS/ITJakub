using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.Shared.Container;

namespace Vokabular.Shared.AspNetCore.Container
{
    public class DryIocContainer
    {
        private readonly IContainer m_container;

        public DryIocContainer()
        {
            m_container = new DryIoc.Container().WithDependencyInjectionAdapter();
        }
        
        public void Dispose()
        {
            m_container.Dispose();
        }

        public void AddAllSingletonBasedOn<TService>(Assembly assembly) where TService : class
        {
            var serviceType = typeof(TService);
            var implTypes = assembly.GetImplementationTypes(type => serviceType.IsAssignableFrom(type));
            foreach (var implType in implTypes)
            {
                m_container.RegisterMany(new[] { serviceType, implType }, implType, Reuse.Singleton);
            }
        }

        public void AddAllTransientBasedOn<TService>(Assembly assembly) where TService : class
        {
            var serviceType = typeof(TService);
            var implTypes = assembly.GetImplementationTypes(type => serviceType.IsAssignableFrom(type));
            foreach (var implType in implTypes)
            {
                m_container.RegisterMany(new[] { serviceType, implType }, implType, Reuse.Transient);
            }
        }

        public void Install<T>() where T : IContainerInstaller
        {
            var services = new ServiceCollection();
            var installer = Activator.CreateInstance<T>();
            installer.Install(services);

            m_container.Populate(services);
        }

        public T Resolve<T>()
        {
            return m_container.Resolve<T>();
        }

        public T[] ResolveAll<T>()
        {
            return m_container.ResolveMany<T>().ToArray();
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            m_container.Populate(services);
            return m_container.Resolve<IServiceProvider>();
        }
    }
}