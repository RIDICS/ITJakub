using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITJakub.MobileApps.Client.Core.Configuration;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.Core
{
    public class ApplicationLoader
    {
        private readonly Dictionary<ApplicationType, ApplicationBase> m_applications = new Dictionary<ApplicationType, ApplicationBase>();

        public ApplicationLoader()
        {
            LoadAllDiffingWrappers(ApplicationConfigLoader.Instance.CurrentConfig.ApplicationAssemblies);
        }

        private static readonly ApplicationLoader m_instance = new ApplicationLoader();

        public static ApplicationLoader Instance
        {
            get { return m_instance; }
        }


        private void LoadAllDiffingWrappers(IEnumerable<string> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                Assembly ass = Assembly.Load(new AssemblyName(assembly));

                foreach (TypeInfo type in ass.DefinedTypes)
                {
                    var mobileApplicationAttribute = type.GetCustomAttributes(typeof(MobileApplicationAttribute), false).FirstOrDefault() as MobileApplicationAttribute;
                    if (mobileApplicationAttribute != null)
                    {
                        var applicationBase = Activator.CreateInstance(type.AsType()) as ApplicationBase;
                        if (applicationBase != null)
                        {
                            applicationBase.Assembly = assembly;
                            m_applications.Add(mobileApplicationAttribute.ApplicationType, applicationBase);
                        }
                        else
                            throw new InvalidOperationException(string.Format("Type {0} does not implement {1}",
                                type.FullName, typeof (ApplicationBase).FullName));
                    }
                }
            }
        }

        public ApplicationBase GetApplicationByType(ApplicationType applicationType)
        {
            return m_applications[applicationType];
        }

        public Dictionary<ApplicationType, ApplicationBase> GetAllApplications()
        {
            return new Dictionary<ApplicationType, ApplicationBase>(m_applications);
        }
        
    }
}
