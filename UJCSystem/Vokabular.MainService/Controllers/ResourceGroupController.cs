using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    [Obsolete("Resource groups are not used at all")]
    public class ResourceGroupController : BaseController
    {
        //private readonly NamedResourceGroupManager m_namedResourceGroupManager;

        //public ResourceGroupController(NamedResourceGroupManager namedResourceGroupManager)
        //{
        //    m_namedResourceGroupManager = namedResourceGroupManager;
        //}

        [HttpGet("Project/{projectId}/resource-group")]
        public List<NamedResourceGroupContract> GetResourceGroupList(long projectId, [FromQuery] ResourceTypeEnumContract? filterResourceType)
        {
            throw new MainServiceException(MainServiceErrorCode.UserBookAccessForbidden, "Resource groups are unsupported");

            //var result = m_namedResourceGroupManager.GetResourceGroupList(projectId, filterResourceType);
            //return result;
        }

        [HttpPost("Project/{projectId}/resource-group")]
        public long CreateResourceGroup(long projectId, [FromBody] NamedResourceGroupContract request)
        {
            throw new MainServiceException(MainServiceErrorCode.UserBookAccessForbidden, "Resource groups are unsupported");

            //var resultId = m_namedResourceGroupManager.CreateResourceGroup(projectId, request);
            //return resultId;
        }

        [HttpPut("ResourceGroup/{resourceGroupId}")]
        public void UpdateResourceGroup(long resourceGroupId, [FromBody] NamedResourceGroupContract request)
        {
            throw new MainServiceException(MainServiceErrorCode.UserBookAccessForbidden, "Resource groups are unsupported");

            //m_namedResourceGroupManager.UpdateResourceGroup(resourceGroupId, request);
        }

        [HttpDelete("ResourceGroup/{resourceGroupId}")]
        public void DeleteResourceGroup(long resourceGroupId)
        {
            throw new MainServiceException(MainServiceErrorCode.UserBookAccessForbidden, "Resource groups are unsupported");

            //m_namedResourceGroupManager.DeleteResourceGroup(resourceGroupId);
        }
    }
}