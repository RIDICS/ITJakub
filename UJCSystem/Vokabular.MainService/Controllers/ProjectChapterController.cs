using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectChapterController : BaseController
    {
        private readonly ProjectItemManager m_projectItemManager;

        public ProjectChapterController(ProjectItemManager projectItemManager)
        {
            m_projectItemManager = projectItemManager;
        }

        [HttpGet("{projectId}/chapter")]
        public List<ChapterHierarchyContract> GetChapterList(long projectId)
        {
            var result = m_projectItemManager.GetChapterList(projectId);
            return result;
        }

        [HttpGet("chapter/{chapterId}")]
        public GetChapterContract GetChapterResource(long chapterId)
        {
            var result = m_projectItemManager.GetChapterResource(chapterId);
            return result;
        }

        [HttpPost("{projectId}/chapter")]
        public IActionResult CreateChapterResource(long projectId, [FromBody] CreateChapterContract chapterData)
        {
            var chapterId = m_projectItemManager.CreateChapterResource(projectId, chapterData);
            return Ok(chapterId);
        }

        [HttpPut("chapter/{chapterId}")]
        public IActionResult UpdateChapterResource(long chapterId, [FromBody] CreateChapterContract chapterData)
        {
            m_projectItemManager.UpdateChapterResource(chapterId, chapterData);
            return Ok();
        }
    }
}
