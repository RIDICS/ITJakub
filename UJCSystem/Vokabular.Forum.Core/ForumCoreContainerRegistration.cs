﻿using Vokabular.ForumSite.Core.Helpers;
using Vokabular.ForumSite.Core.Managers;
using Vokabular.ForumSite.DataEntities;
using Vokabular.Shared.Container;

namespace Vokabular.ForumSite.Core
{
    public class ForumCoreContainerRegistration : IContainerInstaller
    {
        public void Install(IIocContainer container)
        {
            container.AddPerWebRequest<ForumManager>();
            container.AddPerWebRequest<SubForumManager>();

            container.AddPerWebRequest<ForumSiteUrlHelper>();
            container.AddPerWebRequest<MessageGenerator>();

            container.Install<ForumDataEntitiesContainerRegistration>();
        }
    }
}
