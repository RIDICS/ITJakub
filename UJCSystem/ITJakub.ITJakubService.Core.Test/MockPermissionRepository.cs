using System.Collections.Generic;
using Castle.Facilities.NHibernateIntegration;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;

namespace ITJakub.ITJakubService.Core.Test
{
    public class MockPermissionRepository : PermissionRepository
    {
        public MockPermissionRepository(ISessionManager sessManager) : base(sessManager)
        {
        }

        public bool IsAdmin { get; set; }

        public override IList<SpecialPermission> GetSpecialPermissionsByUserAndType(int userId, SpecialPermissionCategorization type)
        {
            return IsAdmin ? GetSpecialPermissions() : new List<SpecialPermission>();
        }
    }
}