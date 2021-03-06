﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.Controllers
{
    [Route("api/Project")]
    public class ProjectChapterController : BaseController
    {
        private readonly ProjectItemManager m_projectItemManager;
        private readonly AuthorizationManager m_authorizationManager;

        public ProjectChapterController(ProjectItemManager projectItemManager, AuthorizationManager authorizationManager)
        {
            m_projectItemManager = projectItemManager;
            m_authorizationManager = authorizationManager;
        }

        [HttpGet("{projectId}/chapter")]
        public IList<ChapterHierarchyDetailContract> GetChapterList(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.ReadProject);

            var result = m_projectItemManager.GetChapterList(projectId);
            return result;
        }

        [HttpGet("chapter/{chapterId}")]
        public GetChapterContract GetChapterResource(long chapterId)
        {
            m_authorizationManager.AuthorizeResource(chapterId, PermissionFlag.ReadProject);

            var result = m_projectItemManager.GetChapterResource(chapterId);
            return result;
        }

        [HttpPost("{projectId}/chapter")]
        public IActionResult CreateChapterResource(long projectId, [FromBody] CreateChapterContract chapterData)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            var chapterId = m_projectItemManager.CreateChapterResource(projectId, chapterData);
            return Ok(chapterId);
        }

        [HttpPut("chapter/{chapterId}")]
        public IActionResult UpdateChapterResource(long chapterId, [FromBody] CreateChapterContract chapterData)
        {
            m_authorizationManager.AuthorizeResource(chapterId, PermissionFlag.EditProject);

            m_projectItemManager.UpdateChapterResource(chapterId, chapterData);
            return Ok();
        }

        [HttpPut("{projectId}/chapter")]
        public IActionResult UpdateChapterList(long projectId, [FromBody] IList<CreateOrUpdateChapterContract> chapterData)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectItemManager.UpdateChapters(projectId, chapterData);
            return Ok();
        }

        [HttpPost("{projectId}/chapter/generator")]
        public IActionResult GenerateChapters(long projectId)
        {
            m_authorizationManager.AuthorizeBook(projectId, PermissionFlag.EditProject);

            m_projectItemManager.GenerateChapters(projectId);
            return Ok();
        }
    }
}
