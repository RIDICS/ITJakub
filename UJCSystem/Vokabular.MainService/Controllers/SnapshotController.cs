using Microsoft.AspNetCore.Mvc;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class SnapshotController : BaseController
    {
        private readonly SnapshotManager m_snapshotManager;
        private readonly AuthorizationManager m_authorizationManager;

        public SnapshotController(SnapshotManager snapshotManager, AuthorizationManager authorizationManager)
        {
            m_snapshotManager = snapshotManager;
            m_authorizationManager = authorizationManager;
        }

        [HttpPost]
        public long CreateSnapshot([FromBody] CreateSnapshotContract data)
        {
            m_authorizationManager.AuthorizeBook(data.ProjectId, PermissionFlag.EditProject);

            return m_snapshotManager.CreateSnapshot(data);
        }

        [HttpGet("{snapshotId}")]
        public SnapshotDetailContract GetSnapshot(long snapshotId)
        {
            m_authorizationManager.AuthorizeSnapshot(snapshotId, PermissionFlag.ReadProject);

            return m_snapshotManager.GetSnapshotDetail(snapshotId);
        }
    }
}
