using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    public class ResourceGroupController : BaseController
    {
        private readonly NamedResourceGroupManager m_namedResourceGroupManager;

        public ResourceGroupController(NamedResourceGroupManager namedResourceGroupManager)
        {
            m_namedResourceGroupManager = namedResourceGroupManager;
        }

        [HttpGet("Project/{projectId}/resource-group")]
        public List<NamedResourceGroupContract> GetResourceGroupList(long projectId, [FromQuery] ResourceTypeEnumContract? filterResourceType)
        {
            var result = m_namedResourceGroupManager.GetResourceGroupList(projectId, filterResourceType);
            return result;
        }

        [HttpPost("Project/{projectId}/resource-group")]
        public long CreateResourceGroup(long projectId, [FromBody] NamedResourceGroupContract request)
        {
            var resultId = m_namedResourceGroupManager.CreateResourceGroup(projectId, request);
            return resultId;
        }

        [HttpPut("ResourceGroup/{resourceGroupId}")]
        public void UpdateResourceGroup(long resourceGroupId, [FromBody] NamedResourceGroupContract request)
        {
            m_namedResourceGroupManager.UpdateResourceGroup(resourceGroupId, request);
        }

        [HttpDelete("ResourceGroup/{resourceGroupId}")]
        public void DeleteResourceGroup(long resourceGroupId)
        {
            m_namedResourceGroupManager.DeleteResourceGroup(resourceGroupId);
        }
    }
}