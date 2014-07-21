using System;

namespace ITJakub.MobileApps.Client.Shared
{
    public class MobileApplicationAttribute : Attribute
    {
        public MobileApplicationAttribute(ApplicationType applicationType)
        {
            ApplicationType = applicationType;
        }

        public ApplicationType ApplicationType { get; private set; }
    }
}