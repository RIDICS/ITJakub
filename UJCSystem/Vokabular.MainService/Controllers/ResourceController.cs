using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    public class ResourceController : Controller
    {
        [HttpPost("session/{sessionId}/resource")]
        public void UploadResource(string sessionId)
        {
            // TODO Request.Body;
        }

        [HttpPost("project/{projectId}/resource")]
        public void ProcessUploadedResources(long projectId, [FromBody] NewResourceContract resourceInfo)
        {
            
        }

        [HttpPost("resource/{resourceId}/version")]
        public void ProcessUploadedResourceVersion(long resourceId, [FromBody] NewResourceContract resourceInfo)
        {
            
        }

        [HttpGet("project/{projectId}/resource")]
        public List<ResourceContract> GetResourceList(long projectId)
        {
            return new List<ResourceContract>
            {
                MockData.GetResourceContract(1),
                MockData.GetResourceContract(2),
                MockData.GetResourceContract(3)
            };
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
        public void GetResourceVersionHistory(long resourceId)
        {
            
        }
    }

    public class MockData
    {
        public static ResourceContract GetResourceContract(int id)
        {
            return new ResourceContract
            {
                Id = id,
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