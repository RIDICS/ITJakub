using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.MainService.DataContracts.ServiceContracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    public class ResourceController : Controller, IResourceMainService
    {
        [HttpPost("session/{sessionId}/resource")]
        public void UploadResource(string sessionId)
        {
            // TODO Request.Body;
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

        [HttpGet("project/{projectId}/resource?type={resourceType}")]
        public List<ResourceContract> GetResourceList(long projectId, ResourceTypeContract? resourceType)
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
                    ResourceTypeContract type = (ResourceTypeContract)random.Next(4);
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
        public void RenameResource(long resourceId, ResourceContract resource)
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
        public ResourceMetadataContract GetResourceMetadata(long resourceId)
        {
            return new ResourceMetadataContract
            {
                Editor = "Jan Novák",
                Editor2 = "Josef Novák",
                LastModification = DateTime.Now,
                EditionNote = "xxxxxxx"
            };
        }
    }

    public class MockResourceData
    {
        public static ResourceContract GetResourceContract(int id, ResourceTypeContract resourceType)
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