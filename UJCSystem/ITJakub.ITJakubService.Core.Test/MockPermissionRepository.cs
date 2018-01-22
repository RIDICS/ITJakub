using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.DataEntities.Database.Repositories;
using Vokabular.DataEntities.Database.UnitOfWork;

namespace ITJakub.ITJakubService.Core.Test
{
    public class MockPermissionRepository : PermissionRepository
    {
        public MockPermissionRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public bool IsAdmin { get; set; }

        public override IList<SpecialPermission> GetSpecialPermissionsByUserAndType(int userId, SpecialPermissionCategorization type)
        {
            return IsAdmin ? GetSpecialPermissions() : new List<SpecialPermission>();
        }
    }
}