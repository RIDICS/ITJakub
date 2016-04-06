﻿using System;
using Windows.UI.Xaml.Media.Imaging;
using ITJakub.MobileApps.Client.Fillwords2.DataService;
using ITJakub.MobileApps.Client.Fillwords2.View;
using ITJakub.MobileApps.Client.Fillwords2.ViewModel;
using ITJakub.MobileApps.Client.Shared;
using ITJakub.MobileApps.Client.Shared.Communication;
using ITJakub.MobileApps.Client.Shared.Enum;
using ITJakub.MobileApps.Client.Shared.ViewModel;

namespace ITJakub.MobileApps.Client.Fillwords2
{
    [MobileApplication(ApplicationType.Fillwords2)]
    public class AppInfo : ApplicationBase
    {
        private readonly FillwordsDataService m_dataService;

        public AppInfo(ISynchronizeCommunication applicationCommunication) : base(applicationCommunication)
        {
            m_dataService = new FillwordsDataService(applicationCommunication);
        }

        public override string Name
        {
            get { return "Doplňovačky 2"; }
        }

        public override ApplicationBaseViewModel ApplicationViewModel
        {
            get { return new FillwordsViewModel(m_dataService); }
        }

        public override Type ApplicationDataTemplate
        {
            get { return typeof (FillwordsView); }
        }

        public override EditorBaseViewModel EditorViewModel
        {
            get { return new FillwordsEditorViewModel(m_dataService); }
        }

        public override Type EditorDataTemplate
        {
            get { return typeof (FillwordsEditorView); }
        }

        public override AdminBaseViewModel AdminViewModel
        {
            get { return new FillwordsAdminViewModel(m_dataService); }
        }

        public override Type AdminDataTemplate
        {
            get { return typeof(FillwordsAdminView); }
        }

        public override TaskPreviewBaseViewModel TaskPreviewViewModel
        {
            get { return new FillwordsTaskPreviewViewModel(m_dataService); }
        }

        public override Type TaskPreviewDataTemplate
        {
            get { return typeof(FillwordsPreviewView); }
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
            get { return true; }
        }

        public override BitmapImage Icon
        {
            get { return new BitmapImage(new Uri(BaseUri, "Icon/ball_point_pen-100.png")); }
        }
    }
}