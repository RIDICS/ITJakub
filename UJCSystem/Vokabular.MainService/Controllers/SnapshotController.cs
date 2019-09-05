using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    public class SnapshotController : BaseController
    {
        private readonly SnapshotManager m_snapshotManager;

        public SnapshotController(SnapshotManager snapshotManager)
        {
            m_snapshotManager = snapshotManager;
        }

        [HttpPost("project/{projectId}/snapshot")]
        public long CreateSnapshot(long projectId, [FromBody] CreateSnapshotContract data)
        {
            return m_snapshotManager.CreateSnapshot(projectId, data);
        }

        [HttpGet("project/project/snapshot/{snapshotId}")]
        public SnapshotDetailContract GetSnapshot(long snapshotId)
        {
            return m_snapshotManager.GetSnapshotDetail(snapshotId);
        }

        [HttpGet("project/{projectId}/snapshot")]
        public IList<SnapshotAggregatedInfoContract> GetSnapshotList(long projectId)
        {
            return m_snapshotManager.GetPublishedSnapshotWithAggregatedInfo(projectId);
        } 
    }
}
