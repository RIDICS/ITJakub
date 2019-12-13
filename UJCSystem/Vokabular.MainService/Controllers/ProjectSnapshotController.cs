using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.RestClient.Headers;
using Vokabular.Shared.AspNetCore.WebApiUtils.Documentation;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectSnapshotController : BaseController
    {
        private readonly SnapshotManager m_snapshotManager;
        private readonly AuthorizationManager m_authorizationManager;

        public ProjectSnapshotController(SnapshotManager snapshotManager, AuthorizationManager authorizationManager)
        {
            m_snapshotManager = snapshotManager;
            m_authorizationManager = authorizationManager;
        }

        [HttpGet("{projectId}/snapshot")]
        [ProducesResponseTypeHeader(StatusCodes.Status200OK, CustomHttpHeaders.TotalCount, ResponseDataType.Integer, "Total count")]
        public IList<SnapshotAggregatedInfoContract> GetSnapshotList(long projectId, [FromQuery] int? start, [FromQuery] int? count, [FromQuery] string filterByComment)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            var result = m_snapshotManager.GetPublishedSnapshotWithAggregatedInfo(projectId, start, count, filterByComment);

            SetTotalCountHeader(result.TotalCount);
            return result.List;
        }

        [HttpGet("{projectId}/snapshot/latest")]
        public ActionResult<SnapshotContract> GetLatestPublishedSnapshot(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            var snapshot = m_snapshotManager.GetLatestPublishedSnapshot(projectId);

            return Ok(snapshot);
        }
    }
}
