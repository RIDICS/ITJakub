using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Castle.Windsor.MsDependencyInjection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;

namespace Vokabular.MainService.Container
{
    ///<summary>
    ///Container for IOC
    ///</summary>
    public class WindsorContainerImplementation : WindsorContainer, IContainer
    {
        private readonly IServiceCollection m_services;
        private const string ConfigSuffix = ".Container.config";
        private const string CodeBasePrefix = "file:///";

        private static ILog m_log;

        public WindsorContainerImplementation(IServiceCollection services)
        {
            m_services = services;

            //configure log4net
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            //Add subresolvers
            AddSubresolvers();

            InstallComponents();
            
            //configure AutoMapper
            ConfigureAutoMapper();
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Configuration castle is completed");
        }

        private void InstallComponents()
        {
            Install(FromAssembly.InThisApplication());
        }

        private void ConfigureAutoMapper()
        {
            var profiles = ResolveAll<Profile>();

            Mapper.Initialize(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });
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
                if (m_log.IsDebugEnabled)
                    m_log.DebugFormat("Using assembly location config succeded. Using config at location: {0}", fileConfigPath);

                return new FileResource(fileConfigPath);
            }
            else
            {
                if (m_log.IsDebugEnabled)
                    m_log.DebugFormat("Using assembly location config failed. Search location was: {0}", fileConfigPath);
            }


            fileConfigPath = GetCodebasePath(assembly);
            if (File.Exists(fileConfigPath))
            {
                if (m_log.IsDebugEnabled)
                    m_log.DebugFormat("Using codeBase location config succeded. Using config at location: {0}", fileConfigPath);
                return new FileResource(fileConfigPath);
            }
            else
            {
                if (m_log.IsDebugEnabled)
                    m_log.DebugFormat("Using codeBase location config failed.  Search location was: {0}", fileConfigPath);
            }

            if (m_log.IsWarnEnabled)
                m_log.WarnFormat("Using embedded resource config");

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
            //Register(Component.For<TService>().LifestylePerWebRequest());
            m_services.AddScoped<TService>();
        }

        public void AddPerWebRequest<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            //Register(Component.For<TService>().LifestylePerWebRequest());
            m_services.AddScoped<TService, TImplementation>();
        }

        public void AddInstance<TImplementation>(TImplementation instance) where TImplementation : class
        {
            Register(Component.For<TImplementation>().Instance(instance));
        }

        public void AddInstance<TService, TImplementation>(TImplementation instance) where TService : class where TImplementation : class, TService
        {
            Register(Component.For<TService>().Instance(instance));
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection services)
        {
            return WindsorRegistrationHelper.CreateServiceProvider(this, services);
        }
    }
}