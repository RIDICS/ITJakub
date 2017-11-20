using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.Core.Parameter;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Controllers
{
    [Route("api/[controller]")]
    public class ProjectController : BaseController
    {
        private readonly ProjectManager m_projectManager;
        private readonly ProjectMetadataManager m_projectMetadataManager;
        private readonly ProjectInfoManager m_projectInfoManager;
        private readonly PageManager m_pageManager;

        public ProjectController(ProjectManager projectManager, ProjectMetadataManager projectMetadataManager, ProjectInfoManager projectInfoManager, PageManager pageManager)
        {
            m_projectManager = projectManager;
            m_projectMetadataManager = projectMetadataManager;
            m_projectInfoManager = projectInfoManager;
            m_pageManager = pageManager;
        }
        
        [HttpGet]
        public List<ProjectDetailContract> GetProjectList([FromQuery] int? start, [FromQuery] int? count, [FromQuery] bool? fetchPageCount)
        {
            var result = m_projectManager.GetProjectList(start, count, fetchPageCount ?? false);

            SetTotalCountHeader(result.TotalCount);

            return result.List;
        }

        [HttpGet("{projectId}")]
        [ProducesResponseType(typeof(ProjectDetailContract), StatusCodes.Status200OK)]
        public IActionResult GetProject(long projectId, [FromQuery] bool? fetchPageCount)
        {
            var projectData = m_projectManager.GetProject(projectId, fetchPageCount ?? false);
            if (projectData == null)
                return NotFound();

            return Ok(projectData);
        }

        [HttpPost]
        public long CreateProject([FromBody] ProjectContract project)
        {
            return m_projectManager.CreateProject(project);
        }

        [HttpPut("{projectId}")]
        public void UpdateProject(long projectId, [FromBody] ProjectContract data)
        {
            m_projectManager.UpdateProject(projectId, data);
        }

        [HttpDelete("{projectId}")]
        public void DeleteProject(long projectId)
        {
            m_projectManager.DeleteProject(projectId);
        }

        [HttpGet("{projectId}/metadata")]
        [ProducesResponseType(typeof(ProjectMetadataResultContract), StatusCodes.Status200OK)]
        public IActionResult GetProjectMetadata(long projectId, [FromQuery] bool includeAuthor, [FromQuery] bool includeResponsiblePerson,
            [FromQuery] bool includeKind, [FromQuery] bool includeGenre, [FromQuery] bool includeOriginal)
        {
            var parameters = new GetProjectMetadataParameter
            {
                IncludeKind = includeKind,
                IncludeGenre = includeGenre,
                IncludeOriginal = includeOriginal,
                IncludeResponsiblePerson = includeResponsiblePerson,
                IncludeAuthor = includeAuthor
            };
            var resultData = m_projectMetadataManager.GetProjectMetadata(projectId, parameters);

            if (resultData == null)
                return NotFound();

            return Ok(resultData);
        }

        [HttpPost("{projectId}/metadata")]
        public long CreateNewProjectMetadataVersion(long projectId, [FromBody] ProjectMetadataContract metadata)
        {
            return m_projectMetadataManager.CreateNewProjectMetadataVersion(projectId, metadata);
        }

        [HttpPut("{projectId}/literary-kind")]
        public void SetLiteraryKinds(long projectId, [FromBody] IntegerIdListContract kindIdList)
        {
            m_projectInfoManager.SetLiteraryKinds(projectId, kindIdList);
        }

        [HttpPut("{projectId}/literary-genre")]
        public void SetLiteraryGenres(long projectId, [FromBody] IntegerIdListContract genreIdList)
        {
            m_projectInfoManager.SetLiteraryGenres(projectId, genreIdList);
        }

        [HttpPut("{projectId}/literary-original")]
        public void SetLiteraryOriginal(long projectId, [FromBody] IntegerIdListContract litOriginalIdList)
        {
            m_projectInfoManager.SetLiteraryOriginals(projectId, litOriginalIdList);
        }

        [HttpPut("{projectId}/keyword")]
        public void SetKeywords(long projectId, [FromBody] IntegerIdListContract keywordIdList)
        {
            m_projectInfoManager.SetKeywords(projectId, keywordIdList);
        }

        [HttpPut("{projectId}/category")]
        public void SetCategories(long projectId, [FromBody] IntegerIdListContract categoryIdList)
        {
            m_projectInfoManager.SetCategories(projectId, categoryIdList);
        }

        [HttpPut("{projectId}/author")]
        public void SetAuthors(long projectId, [FromBody] IntegerIdListContract authorIdList)
        {
            m_projectInfoManager.SetAuthors(projectId, authorIdList);
        }

        [HttpPut("{projectId}/responsible-person")]
        public void SetResponsiblePersons(long projectId, [FromBody] List<ProjectResponsiblePersonIdContract> projectResposibleIdList)
        {
            m_projectInfoManager.SetResponsiblePersons(projectId, projectResposibleIdList);
        }

        [HttpGet("{projectId}/page")]
        public List<PageContract> GetPageList(long projectId)
        {
            var result = m_pageManager.GetPageList(projectId);
            return result;
        }

        [HttpGet("{projectId}/text")]
        public List<TextWithPageContract> GetTextResourceList(long projectId, [FromQuery] long? resourceGroupId)
        {
            var result = m_pageManager.GetTextResourceList(projectId, resourceGroupId);
            return result;
        }

        [HttpGet("{projectId}/image")]
        public List<ImageWithPageContract> GetImageList(long projectId)
        {
            var result = m_pageManager.GetImageResourceList(projectId);
            return result;
        }
        
        [HttpGet("text/{textId}")]
        public FullTextContract GetTextResource(long textId, [FromQuery] TextFormatEnumContract? format)
        {
            if (format == null)
                format = TextFormatEnumContract.Html;

            var result = m_pageManager.GetTextResource(textId, format.Value);
            return result;
        }

        [HttpPost("text/{textId}")]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        public IActionResult CreateNewTextResourceVersion([FromBody] CreateTextRequestContract request)
        {
            //TODO add logic
            return StatusCode(StatusCodes.Status409Conflict); // Version conflict
        }

        [HttpGet("text/{textId}/comment")]
        public List<GetTextCommentContract> GetCommentsForText(long textId)
        {
            var result = m_pageManager.GetCommentsForText(textId);
            return result;
        }

        [HttpPost("text/{textId}/comment")]
        public long CreateComment(long textId, [FromBody] CreateTextCommentContract request)
        {
            var resultId = m_pageManager.CreateNewComment(textId, request);
            return resultId;
        }

        [HttpPut("text/{textId}/comment")]
        public long UpdateComment(long textId, [FromBody] CreateTextCommentContract request)
        {
            //TODO add logic
            throw new NotImplementedException();
        }

        [HttpDelete("text/comment/{commentId}")]
        public void DeleteComment(long commentId)
        {
            m_pageManager.DeleteComment(commentId);
        }

        [HttpGet("image/{imageId}")]
        public IActionResult GetImage(long imageId)
        {
            var result = m_pageManager.GetImageResource(imageId);
            if (result == null)
                return NotFound();

            Response.ContentLength = result.FileSize;
            return File(result.Stream, result.MimeType, result.FileName);
        }
    }
}