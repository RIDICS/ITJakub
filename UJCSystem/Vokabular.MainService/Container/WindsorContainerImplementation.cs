using System;
using System.IO;
using System.Reflection;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Windsor.MsDependencyInjection;
using Log4net.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Vokabular.Shared.Container;
using Vokabular.Shared.Extensions;

namespace Vokabular.MainService.Container
{
    ///<summary>
    ///Container for IOC
    ///</summary>
    public class WindsorContainerImplementation : WindsorContainer, IContainer
    {
        private const string ConfigSuffix = ".Container.config";
        private const string CodeBasePrefix = "file:///";

        private static ILogger m_log;

        public WindsorContainerImplementation()
        {
            m_log = new Log4NetAdapter(MethodBase.GetCurrentMethod().DeclaringType.FullName);

            AddSubresolvers();

            InstallComponents();
            
            if (m_log.IsDebugEnabled())
                m_log.LogDebug("Configuration Castle is completed");
        }

        private void InstallComponents()
        {
            Install(FromAssembly.InThisApplication());
        }
        
        private void AddSubresolvers()
        {
            Kernel.Resolver.AddSubResolver(new CollectionResolver(Kernel, true));
        }
        
        private static IResource GetConfigResource()
        {
            var assembly = GetAssembly();

            string fileConfigPath = GetFileConfigPath(assembly);
            if (File.Exists(fileConfigPath))
            {
                if (m_log.IsDebugEnabled())
                    m_log.LogDebug("Using assembly location config succeded. Using config at location: {0}", fileConfigPath);

                return new FileResource(fileConfigPath);
            }
            else
            {
                if (m_log.IsDebugEnabled())
                    m_log.LogDebug("Using assembly location config failed. Search location was: {0}", fileConfigPath);
            }


            fileConfigPath = GetCodebasePath(assembly);
            if (File.Exists(fileConfigPath))
            {
                if (m_log.IsDebugEnabled())
                    m_log.LogDebug("Using codeBase location config succeded. Using config at location: {0}", fileConfigPath);
                return new FileResource(fileConfigPath);
            }
            else
            {
                if (m_log.IsDebugEnabled())
                    m_log.LogDebug("Using codeBase location config failed.  Search location was: {0}", fileConfigPath);
            }

            if (m_log.IsWarningEnabled())
                m_log.LogWarning("Using embedded resource config");

            return new AssemblyResource(GetEmbeddedConfigPath(assembly));
        }

        private static string GetCodebasePath(Assembly assembly)
        {
            var assemblyLocation = assembly.CodeBase;
            if (assemblyLocation.StartsWith(CodeBasePrefix))
            {
                assemblyLocation = assemblyLocation.Substring(CodeBasePrefix.Length);
            }
            var directory = Path.GetDirectoryName(assemblyLocation);
            var configName = GetConfigName(assembly);
            return string.Format(@"{0}\{1}", directory, configName);
        }

        private static string GetFileConfigPath(Assembly assembly)
        {
            var directory = Path.GetDirectoryName(assembly.Location);
            var configName = GetConfigName(assembly);
            return string.Format(@"{0}\{1}", directory, configName);
        }

        private static string GetEmbeddedConfigPath(Assembly assembly)
        {
            string configName = string.Format(@"assembly://{0}/{1}", assembly.GetName().Name, GetConfigName(assembly));
            return configName;
        }

        private static string GetConfigName(Assembly assembly)
        {
            return string.Format(@"{0}{1}", assembly.GetName().Name, ConfigSuffix);
        }

        private static Assembly GetAssembly()
        {
            //return System.Reflection.Assembly.GetExecutingAssembly();
            return typeof(WindsorContainerImplementation).Assembly;
        }

        public void AddSingleton<TService>() where TService : class
        {
            Register(Component.For<TService>().LifestyleSingleton());
        }

        public void AddSingleton<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Register(Component.For<TService>().ImplementedBy<TImplementation>().LifestyleSingleton());
        }

        public void AddTransient<TService>() where TService : class
        {
            Register(Component.For<TService>().LifestyleTransient());
        }

        public void AddTransient<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Register(Component.For<TService>().ImplementedBy<TImplementation>().LifestyleTransient());
        }

        public void AddPerWebRequest<TService>() where TService : class
        {
            Register(Component.For<TService>().LifestyleCustom<MsScopedLifestyleManager>());
        }

        public void AddPerWebRequest<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Register(Component.For<TService>().ImplementedBy<TImplementation>().LifestyleCustom<MsScopedLifestyleManager>());
        }

        public void AddInstance<TImplementation>(TImplementation instance) where TImplementation : class
        {
            Register(Component.For<TImplementation>().Instance(instance));
        }

        public void AddInstance<TService, TImplementation>(TImplementation instance) where TService : class where TImplementation : class, TService
        {
            Register(Component.For<TService>().Instance(instance));
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

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            return WindsorRegistrationHelper.CreateServiceProvider(this, services);
        }
    }
}