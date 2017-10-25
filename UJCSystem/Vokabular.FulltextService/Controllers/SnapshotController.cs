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

        public SnapshotController(SnapshotResourceManager snapshotResourceManager)
        {
            m_snapshotResourceManager = snapshotResourceManager;
        }

        [HttpPost]
        public ResultContract CreateSnapshot([FromBody] SnapshotResource snapshotResource)
        {
            var result = m_snapshotResourceManager.CreateSnapshotResource(snapshotResource);
            return result;

        }
    }
}