﻿using System;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.Client.Shared.ViewModel;
using ITJakub.MobileApps.Client.SynchronizedReading.DataService;
using ITJakub.MobileApps.Client.SynchronizedReading.View;
using ITJakub.MobileApps.Client.SynchronizedReading.ViewModel;
using ITJakub.MobileApps.Client.SynchronizedReading.ViewModel.Reading;

namespace ITJakub.MobileApps.Client.SynchronizedReading
{
    [MobileApplication(ApplicationType.SynchronizedReading)]
    public class AppInfo : ApplicationBase
    {
        private readonly ReaderDataService m_dataService;

        public AppInfo(ISynchronizeCommunication applicationCommunication) : base(applicationCommunication)
        {
            m_dataService = new ReaderDataService(applicationCommunication);
        }

        public override string Name
        {
            get { return "Synchronizované čtení"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new ReadingViewModel(m_dataService); }
        }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (ReadingView); }
        }

        public override EditorBaseViewModel EditorViewModel
        {
            get { return new ReadingEditorViewModel(m_dataService); }
        }

        public override Type EditorDataTemplate
        {
            get { return typeof (ReadingEditorView); }
        }

        public override ApplicationRoleType ApplicationRoleType
        {
            get { return ApplicationRoleType.MainApp; }
        }

        public override ApplicationCategory ApplicationCategory
        {
            get { return ApplicationCategory.Education; }
        }

        public override bool IsChatSupported
        {
            get { return false; }
        }

        public override BitmapImage Icon
        {
            get { return new BitmapImage(new Uri(BaseUri, "Icon/literature-128.png")); }
        }
    }
}