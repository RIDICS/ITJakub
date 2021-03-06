﻿using System;
using System.Linq;
using System.Reflection;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Vokabular.Shared.Container;

namespace Vokabular.MainService.Test.Containers
{
    public class DryIocContainer
    {
        private readonly IContainer m_container;

        public DryIocContainer()
        {
            //m_container = new Container().WithDependencyInjectionAdapter();
            m_container = new Container();
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
            //m_container.Register<TService>(Reuse.InCurrentScope);
            m_container.Register<TService>(Reuse.Singleton);
        }

        public void AddPerWebRequest<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            //m_container.Register<TService, TImplementation>(Reuse.InCurrentScope);
            m_container.Register<TService, TImplementation>(Reuse.Singleton);
        }

        public void AddInstance<TImplementation>(TImplementation instance, string serviceKey = null) where TImplementation : class
        {
            m_container.UseInstance(instance, serviceKey: serviceKey);
        }

        public void AddInstance<TService, TImplementation>(TImplementation instance, string serviceKey = null) where TService : class where TImplementation : class, TService
        {
            m_container.UseInstance(typeof(TService), instance, serviceKey: serviceKey);
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
            throw new NotSupportedException();
            //m_container.Populate(services);
            //return m_container.Resolve<IServiceProvider>();
        }

        public void ReplacePerWebRequest<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            m_container.Register<TService, TImplementation>(Reuse.Singleton, ifAlreadyRegistered: IfAlreadyRegistered.Replace);
        }

        public void Populate(IServiceCollection services)
        {
            m_container.Populate(services);
        }
    }
}