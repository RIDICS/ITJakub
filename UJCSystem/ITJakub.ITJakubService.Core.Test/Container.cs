using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Castle.Core.Resource;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Installer;
using ITJakub.DataEntities.Database.Repositories;
using log4net;
using log4net.Config;

namespace ITJakub.ITJakubService.Core.Test
{
    ///<summary>
    ///Container for IOC
    ///</summary>
    public class Container : WindsorContainer
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
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            //Add subresolvers
            AddSubresolvers();

            InstallComponents();

            ////Configure Nhibernate
            //InstallFacilities();

            //configure AutoMapper
            ConfigureAutoMapper();
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("Configuration castle is completed");
        }

        private void InstallComponents()
        {
            Install(FromAssembly.InThisApplication());
            Install(Configuration.FromXml(GetConfigResource()));

            Register(Component.For<PermissionRepository>().ImplementedBy<MockPermissionRepository>().IsDefault());
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
    }
}