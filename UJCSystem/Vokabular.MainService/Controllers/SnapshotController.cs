using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class SnapshotController : BaseController
    {
        private readonly SnapshotManager m_snapshotManager;

        public SnapshotController(SnapshotManager snapshotManager)
        {
            m_snapshotManager = snapshotManager;
        }

        [HttpPost]
        public long CreateSnapshot([FromBody] CreateSnapshotContract data)
        {
            return m_snapshotManager.CreateSnapshot(data);
        }

        [HttpGet("{snapshotId}")]
        public SnapshotDetailContract GetSnapshot(long snapshotId)
        {
            return m_snapshotManager.GetSnapshotDetail(snapshotId);
        }
    }
}
