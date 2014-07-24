﻿using System;
using ITJakub.MobileApps.Client.Shared;

namespace ITJakub.MobileApps.Client.SampleApp
{
    [MobileApplication(ApplicationType.SampleApp)]
    public class AppInfo : ApplicationBase
    {
        public override string Name
        {
            get { return "Sample application name"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new SampleViewModel(); }
        }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (SampleView); }
        }

        public override bool IsChatSupported
        {
            get { return true; }
        }
    }
}