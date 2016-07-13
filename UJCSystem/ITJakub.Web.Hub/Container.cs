using System.IO;
using System.Reflection;
using System.Web.Mvc;
using AutoMapper;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using ITJakub.Web.Hub.App_Start;

namespace ITJakub.Web.Hub
{
    public class Container : WindsorContainer
    {
        private volatile static WindsorContainer m_current;

        public static WindsorContainer Current
        {
            get
            {
                if (m_current == null)
                {
                    lock (typeof(Container))
                    {
                        if (m_current == null)
                        {
                            m_current = new Container(string.Format(@"{0}.Container.Config", GetAssemblyNamePrefixForWebservice()));
                        }
                    }
                }
                return m_current;
            }
            set { m_current = value; }
        }

        public Container(string configFile)
            : base(new XmlInterpreter(configFile))
        {
            //configure log4net
            //XmlConfigurator.Configure(new FileInfo("log4net.config"));
            //XmlConfigurator.Configure();
            //configure AutoMapper
            ConfigureControllerFactory();
            ConfigureAutoMapper();
        }
        
        private void ConfigureAutoMapper()
        {
            foreach (var profile in ResolveAll<Profile>()) Mapper.AddProfile(profile);
        }

        static string GetAssembly()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        static string GetAssemblyNamePrefixForWebservice()
        {
            return "ITJakub.Web.Hub";
        }

        static string GetAssemblyNamePrefix()
        {
            var assembly = GetAssembly();
            var directory = Path.GetDirectoryName(assembly);
            var prefix = Path.GetFileNameWithoutExtension(assembly);
            return string.Format(@"{0}\{1}", directory, prefix);
        }

        private void ConfigureControllerFactory()
        {
            Register(Classes.FromThisAssembly()
                .BasedOn<IController>()
                .LifestyleTransient());

            var controllerFactory = new WindsorControllerFactory(Kernel);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);
        }
    }
}