using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectChapterController : BaseController
    {

        [HttpGet("{projectId}/chapter")]
        public List<ChapterHierarchyContract> GetChapterList(long projectId)
        {
            throw new NotImplementedException();
        }

        [HttpGet("chapter/{chapterId}")]
        public ChapterContractBase GetChapterResource(long chapterId)
        {
            throw new NotImplementedException();
        }

        [HttpPost("{projectId}/chapter")]
        public IActionResult CreateChapterResource(long projectId, [FromBody] CreateChapterContract chapterData)
        {
            throw new NotImplementedException();
        }

        [HttpPut("chapter/{chapterId}")]
        public IActionResult UpdateChapterResource(long chapterId, CreateChapterContract chapterData)
        {
            throw new NotImplementedException();
        }
    }
}
