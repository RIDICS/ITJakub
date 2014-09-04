using System;
using ITJakub.MobileApps.Client.Shared.Enum;

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