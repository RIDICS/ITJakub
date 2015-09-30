using System.IO;
using System.Reflection;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Installer;
using GalaSoft.MvvmLight.Views;
using log4net.Config;

namespace ITJakub.BatchImport.Client
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
                            m_current = new Container();
                        }
                    }
                }
                return m_current;
            }
            set { m_current = value; }
        }
                
        private Container()
        {
            //configure log4net
            XmlConfigurator.Configure(new FileInfo("log4net.config"));
            //XmlConfigurator.Configure();              

            InstallComponents();
        }

        private void InstallComponents()
        {
            Install(FromAssembly.InThisApplication());
        }
    }
}