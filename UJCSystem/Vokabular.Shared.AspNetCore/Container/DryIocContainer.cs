using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.Shared.Container;

namespace Vokabular.Shared.AspNetCore.Container
{
    public class DryIocContainer : IIocContainer
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

        public void AddSingleton<TService>() where TService : class
        {
            m_container.Register<TService>(Reuse.Singleton);
        }

        public void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            m_container.Register<TService, TImplementation>(Reuse.Singleton);
        }

        public void AddTransient<TService>() where TService : class
        {
            m_container.Register<TService>(Reuse.Transient);
        }

        public void AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            m_container.Register<TService, TImplementation>(Reuse.Transient);
        }

        public void AddPerWebRequest<TService>() where TService : class
        {
            m_container.Register<TService>(Reuse.InCurrentScope);
        }

        public void AddPerWebRequest<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            m_container.Register<TService, TImplementation>(Reuse.InCurrentScope);
        }

        public void AddInstance<TImplementation>(TImplementation instance) where TImplementation : class
        {
            m_container.UseInstance(instance);
        }

        public void AddInstance<TService, TImplementation>(TImplementation instance) where TService : class where TImplementation : class, TService
        {
            m_container.UseInstance(typeof(TService), instance);
        }

        public void AddAllSingletonBasedOn<TService>(Assembly assembly) where TService : class
        {
            var serviceType = typeof(TService);
            m_container.RegisterMany(new[] { assembly }, (registrator, types, type) =>
            {
                if (serviceType.IsAssignableFrom(type))
                {
                    registrator.RegisterMany(new[] { type, serviceType }, type, Reuse.Singleton);
                }
            });
        }

        public void AddAllTransientBasedOn<TService>(Assembly assembly) where TService : class
        {
            //m_container.RegisterMany(new[] { typeof(App).Assembly },
            //    serviceTypeCondition: type => type.IsSubclassOf(typeof(TService)),
            //    reuse: Reuse.Transient);

            var serviceType = typeof(TService);
            m_container.RegisterMany(new[] { assembly }, (registrator, types, type) =>
            {
                if (serviceType.IsAssignableFrom(type))
                {
                    registrator.RegisterMany(new[] { type, serviceType }, type, Reuse.Transient);
                }
            });
        }

        public void Install<T>() where T : IContainerInstaller
        {
            var installer = Activator.CreateInstance<T>();
            installer.Install(this);
        }

        public void Install(params IContainerInstaller[] installers)
        {
            foreach (var installer in installers)
            {
                installer.Install(this);
            }
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