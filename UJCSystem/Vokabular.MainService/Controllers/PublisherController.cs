using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class PublisherController : Controller
    {
        private readonly ProjectMetadataManager m_projectMetadataManager;

        public PublisherController(ProjectMetadataManager projectMetadataManager)
        {
            m_projectMetadataManager = projectMetadataManager;
        }

        [HttpPost("")]
        public int CreatePublisher([FromBody] PublisherContract publisher)
        {
            var resultId = m_projectMetadataManager.CreatePublisher(publisher);
            return resultId;
        }
    }
}
