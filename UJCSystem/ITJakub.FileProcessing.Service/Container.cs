using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using log4net;
using log4net.Config;
using Log4net.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Vokabular.Core;
using Vokabular.DataEntities;
using Vokabular.Shared;
using Vokabular.Shared.Container;

namespace ITJakub.FileProcessing.Service
{
    ///<summary>
    ///Container for IOC
    ///</summary>
    public class Container : WindsorContainer, IIocContainer
    {
        private static readonly Lazy<WindsorContainer> m_current = new Lazy<WindsorContainer>(() => new Container());

        private const string ConfigSuffix = ".Container.config";
        private const string CodeBasePrefix = "file:///";

        private static ILog m_log;

        public static WindsorContainer Current
        {
            get { return m_current.Value; }
        }


        private Container()
        {
            //configure log4net
            XmlConfigurator.Configure();
            m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            ApplicationLogging.LoggerFactory = new LoggerFactory();
            ApplicationLogging.LoggerFactory.AddLog4Net();

            AddSubresolvers();

            InstallComponents();
            
            ConfigureAutoMapper();

            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Configuration Castle Windsor is completed");
        }

        private void InstallComponents()
        {
            Install(FromAssembly.InThisApplication());
            Install(Configuration.FromAppConfig());
            //Install(Configuration.FromXml(GetConfigResource()));

            Install<CoreContainerRegistration>();
            Install<DataEntitiesContainerRegistration>();

            AddSingleton<IOptions<PathConfiguration>, PathConfigImplementation>(); // TODO after switch to ASP.NET Core use framework options handler
        }

        private void ConfigureAutoMapper()
        {
            var profiles = ResolveAll<Profile>();
            Mapper.Reset();
            foreach (var profile in profiles)
            {
                Mapper.AddProfile(profile);
            }
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
            return typeof(Container).Assembly;
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
            Register(Component.For<TService>().LifestylePerWebRequest());
        }

        public void AddPerWebRequest<TService, TImplementation>() where TService : class where TImplementation : class, TService
        {
            Register(Component.For<TService>().ImplementedBy<TImplementation>().LifestylePerWebRequest());
        }

        public void AddInstance<TImplementation>(TImplementation instance) where TImplementation : class
        {
            Register(Component.For<TImplementation>().Instance(instance));
        }

        public void AddInstance<TService, TImplementation>(TImplementation instance) where TService : class where TImplementation : class, TService
        {
            Register(Component.For<TService>().Instance(instance));
        }

        public void AddAllSingletonBasedOn<TService>(Assembly assembly) where TService : class
        {
            Register(Classes.FromAssembly(assembly)
                .BasedOn<TService>()
                .LifestyleSingleton()
                .WithServiceSelf()
                .WithServiceBase());
        }

        public void AddAllTransientBasedOn<TService>(Assembly assembly) where TService : class
        {
            Register(Classes.FromAssembly(assembly)
                .BasedOn<TService>()
                .LifestyleTransient()
                .WithServiceSelf()
                .WithServiceBase());
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
            throw new NotSupportedException();
        }
    }
}