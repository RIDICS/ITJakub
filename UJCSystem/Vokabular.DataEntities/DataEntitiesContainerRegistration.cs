﻿using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;
using Vokabular.Shared.Container;

namespace Vokabular.DataEntities
{
    public class DataEntitiesContainerRegistration : IContainerInstaller
    {
        public void Install(IContainer container)
        {
            container.AddPerWebRequest<IUnitOfWork, UnitOfWork>();
            container.AddPerWebRequest<MetadataRepository>();
            container.AddPerWebRequest<ProjectRepository>();
            container.AddPerWebRequest<UserRepository>();
        }
    }
}
