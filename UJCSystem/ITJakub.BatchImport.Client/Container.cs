using System.IO;
using System.Reflection;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using log4net.Config;

namespace ITJakub.BatchImport.Client
{
    public class Container : WindsorContainer
    {
        private const string ContainerConfigName = "ITJakub.BatchImport.Client.Container.Config";

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
                            m_current = new Container(ContainerConfigName);
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
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            //XmlConfigurator.Configure();
            //configure AutoMapper         
        }        

    }
}