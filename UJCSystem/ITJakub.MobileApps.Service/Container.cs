﻿using System.IO;
using System.Reflection;
using AutoMapper;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using log4net.Config;

namespace ITJakub.MobileApps.Service
{
    ///<summary>
    ///Container for IOC
    ///</summary>
    public class Container : WindsorContainer
    {
        private static volatile WindsorContainer m_current;


        public static WindsorContainer Current
        {
            get
            {
                if (m_current == null)
                {
                    lock (typeof (Container))
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
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            //configure AutoMapper
            ConfigureAutoMapper();
        }

        private void ConfigureAutoMapper()
        {
            foreach (var profile in ResolveAll<Profile>()) Mapper.AddProfile(profile);
        }

        private static string GetAssembly()
        {
            return Assembly.GetEntryAssembly().Location;
        }

        private static string GetAssemblyNamePrefixForWebservice()
        {
            return "ITJakub.MobileApps.Service";
        }

        private static string GetAssemblyNamePrefix()
        {
            var assembly = GetAssembly();
            var directory = Path.GetDirectoryName(assembly);
            var prefix = Path.GetFileNameWithoutExtension(assembly);
            return string.Format(@"{0}\{1}", directory, prefix);
        }
    }
}