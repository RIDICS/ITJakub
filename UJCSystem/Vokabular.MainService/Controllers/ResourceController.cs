using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    public class ResourceController : Controller
    {
        private readonly ProjectResourceManager m_resourceManager;
        private readonly NamedResourceGroupManager m_namedResourceGroupManager;

        public ResourceController(ProjectResourceManager resourceManager, NamedResourceGroupManager namedResourceGroupManager)
        {
            m_resourceManager = resourceManager;
            m_namedResourceGroupManager = namedResourceGroupManager;
        }

        [Authorize(PermissionNames.UploadBook)]
        [HttpPost("session/{sessionId}/resource")]
        public void UploadResource(string sessionId, [FromQuery] string fileName)
        {
            m_resourceManager.UploadResource(sessionId, Request.Body, fileName);
        }

        [Authorize(PermissionNames.UploadBook)]
        [HttpPost("session/{sessionId}")]
        public void ProcessSessionAsImport(string sessionId, [FromBody] NewBookImportContract info)
        {
            m_resourceManager.ProcessSessionAsImport(sessionId, info.ProjectId, info.Comment);
        }

        [HttpPost("project/{projectId}/resource")]
        public long ProcessUploadedResources(long projectId, [FromBody] NewResourceContract resourceInfo)
        {
            return 22;
        }

        [HttpPost("resource/{resourceId}/version")]
        public long ProcessUploadedResourceVersion(long resourceId, [FromBody] NewResourceContract resourceInfo)
        {
            return 231;
        }

        [HttpGet("project/{projectId}/resource")]
        public List<ResourceContract> GetResourceList(long projectId, [FromQuery] ResourceTypeEnumContract? resourceType)
        {
            var list = new List<ResourceContract>();
            if (resourceType != null)
            {
                for (int i = 5; i >= 0; i--)
                {
                    list.Add(MockResourceData.GetResourceContract(i, resourceType.Value));
                }
            }
            else
            {
                var random = new Random();
                for (int i = 11; i >= 0; i--)
                {
                    ResourceTypeEnumContract type = (ResourceTypeEnumContract)random.Next(4);
                    list.Add(MockResourceData.GetResourceContract(i, type));
                }
            }
            return list;
        }

        [HttpDelete("resource/{resourceId}")]
        public void DeleteResource(long resourceId)
        {
            
        }

        [HttpPut("resource/{resourceId}")]
        public void RenameResource(long resourceId, [FromBody] ResourceContract resource)
        {
            
        }

        [HttpPost("resource/{resourceId}/duplicate")]
        public long DuplicateResource(long resourceId)
        {
            return 45;
        }

        [HttpGet("resource/{resourceId}/version")]
        public List<ResourceVersionContract> GetResourceVersionHistory(long resourceId)
        {
            return new List<ResourceVersionContract>
            {
                MockResourceData.GetResourceVersionContract(5),
                MockResourceData.GetResourceVersionContract(4),
                MockResourceData.GetResourceVersionContract(3),
                MockResourceData.GetResourceVersionContract(2),
                MockResourceData.GetResourceVersionContract(1)
            };
        }

        [HttpGet("resource/{resourceId}/metadata")]
        [ProducesResponseType(typeof(ResourceMetadataContract), StatusCodes.Status200OK)]
        public IActionResult GetResourceMetadata(long resourceId)
        {
            var resultData = new ResourceMetadataContract
            {
                Editor = "Jan Novák",
                Editor2 = "Josef Novák",
                LastModification = DateTime.Now,
                EditionNote = "xxxxxxx"
            };
            return Ok(resultData);
        }

        [HttpGet("project/{projectId}/resource/group")]
        public List<NamedResourceGroupContract> GetResourceGroupList(long projectId, [FromQuery] ResourceTypeEnumContract? filterResourceType)
        {
            var result = m_namedResourceGroupManager.GetResourceGroupList(projectId, filterResourceType);
            return result;
        }

        [HttpPost("project/{projectId}/resource/group/")]
        public long CreateResourceGroup(long projectId, [FromBody] NamedResourceGroupContract request)
        {
            var resultId = m_namedResourceGroupManager.CreateResourceGroup(projectId, request);
            return resultId;
        }

        [HttpPut("resource/group/{resourceGroupId}")]
        public void UpdateResourceGroup(long resourceGroupId, [FromBody] NamedResourceGroupContract request)
        {
            m_namedResourceGroupManager.UpdateResourceGroup(resourceGroupId, request);
        }

        [HttpDelete("resource/group/{resourceGroupId}")]
        public void DeleteResourceGroup(long resourceGroupId)
        {
            m_namedResourceGroupManager.DeleteResourceGroup(resourceGroupId);
        }
    }

    public class MockResourceData
    {
        public static ResourceContract GetResourceContract(int id, ResourceTypeEnumContract resourceType)
        {
            return new ResourceContract
            {
                Id = id,
                ResourceType = resourceType,
                Name = string.Format("Zdroj {0}", id)
            };
        }

        public static ResourceVersionContract GetResourceVersionContract(int versionNumber)
        {
            return new ResourceVersionContract
            {
                Id = versionNumber,
                Author = "Jan Novák",
                Comment = "První verze dokumentu [název díla]",
                CreateDate = DateTime.Now,
                VersionNumber = versionNumber
            };
        }
    }
}