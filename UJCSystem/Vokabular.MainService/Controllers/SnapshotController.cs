using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;

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
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public IList<SnapshotAggregatedInfoContract> GetUserList(long projectId, [FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByComment)
        {
            var result = m_snapshotManager.GetPublishedSnapshotWithAggregatedInfo(projectId, start, count, filterByComment);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }
    }
}
