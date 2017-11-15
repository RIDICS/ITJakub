using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class SnapshotController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<TextController>();

        private readonly SnapshotResourceManager m_snapshotResourceManager;
        private readonly TextResourceManager m_textResourceManager;

        public SnapshotController(SnapshotResourceManager snapshotResourceManager, TextResourceManager textResourceManager)
        {
            m_snapshotResourceManager = snapshotResourceManager;
            m_textResourceManager = textResourceManager;
        }

        [HttpPost]
        public ResultContract CreateSnapshot([FromBody] SnapshotPageIdsResourceContract snapshotPageIdsResourceContract)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var pageId in snapshotPageIdsResourceContract.PageIds)
            {
                var textResource = m_textResourceManager.GetTextResource(pageId);
                builder.Append(textResource.PageText);
            }
            var snapShotResource = new SnapshotResourceContract{SnapshotId = snapshotPageIdsResourceContract.SnapshotId, ProjectId = snapshotPageIdsResourceContract.ProjectId, SnapshotText = builder.ToString()};
            var result = m_snapshotResourceManager.CreateSnapshotResource(snapShotResource);
            return result;

        }
    }
}