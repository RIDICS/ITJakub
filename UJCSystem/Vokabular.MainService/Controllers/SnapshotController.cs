using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    public class SnapshotController : Controller
    {
        [HttpPost("project/{projectId}/snapshot")]
        public long CreateSnapshot(long projectId)
        {
            return 73;
        }

        [HttpGet("project/{projectId}/snapshot")]
        public List<SnapshotContract> GetSnapshotList(long projectId)
        {
            return null;
        }
    }
}
