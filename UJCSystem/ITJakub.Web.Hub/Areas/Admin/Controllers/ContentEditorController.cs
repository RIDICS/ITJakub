﻿using System;
using System.Collections.Generic;
using System.Net;
using ITJakub.Web.Hub.Areas.Admin.Core;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Areas.Admin.Models.Request;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;
using ITJakub.Web.Hub.Options;
using Microsoft.AspNetCore.Http;
using Vokabular.RestClient.Errors;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [LimitedAccess(PortalType.CommunityPortal)]
    [Area("Admin")]
    public class ContentEditorController : BaseController
    {
        private readonly TextManager m_textManager;

        public ContentEditorController(CommunicationProvider communicationProvider, TextManager textManager) : base(communicationProvider)
        {
            m_textManager = textManager;
        }

        public IActionResult LoadCommentFile(long textId)
        {
            var parts = new List<CommentStructureResponse>();
            var client = GetProjectClient();

            var result = client.GetCommentsForText(textId);
            if (result.Count <= 0)
            {
                return Json(parts);
            }

            foreach (var pageComments in result)
            {
                var order = 0;
                var mainComment = CreateComment(order, pageComments, false, textId);
                parts.Add(mainComment);
                if (pageComments.TextComments.Count > 0)
                {
                    foreach (var textComment in pageComments.TextComments)
                    {
                        order++;
                        var nestedComment = CreateComment(order, textComment, true, textId);
                        parts.Add(nestedComment);
                    }
                }
            }

            return Json(parts);
        }

        private static CommentStructureResponse CreateComment(int order, GetTextCommentContract textComment, bool nested, long textId)
        {
            var comment = new CommentStructureResponse
            {
                Order = order,
                Time = ((DateTimeOffset) textComment.CreateTime).ToUnixTimeMilliseconds(),
                Text = textComment.Text,
                Picture = null, // Picture is not supported
                Id = textComment.Id,
                Nested = nested,
                TextId = textId,
                TextReferenceId = textComment.TextReferenceId,
                Name = textComment.User.FirstName,
                Surname = textComment.User.LastName
            };
            return comment;
        }

        [HttpPost]
        public IActionResult SaveComment(CreateTextCommentContract comment, long textId)
        {
            var client = GetProjectClient();
            var result = client.CreateComment(textId, comment);
            return Json(result);
        }

        [HttpPost]
        public void UpdateComment(CreateTextCommentContract comment, long commentId)
        {
            var client = GetProjectClient();
            client.UpdateComment(commentId, comment);
        }

        [HttpPost]
        public void DeleteComment(long commentId)
        {
            var client = GetProjectClient();
            client.DeleteComment(commentId);
        }

        [HttpPost]
        public IActionResult DeleteRootComment(long commentId)
        {
            var result = m_textManager.DeleteRootComment(commentId);
            return Json(result);
        }

        [HttpPost]
        public IActionResult GenerateCommentId(long textId)
        {
            var client = GetProjectClient();
            var comments = client.GetCommentsForText(textId);
            var maxId = 0;
            foreach (var comment in comments)
            {
                var isNumber = int.TryParse(comment.TextReferenceId.Split('-')[1], out var value);
                if (isNumber && value > maxId)
                {
                    maxId = value;
                }
            }

            return Json(maxId + 1);
        }

        [HttpGet]
        public IActionResult GetPageImage(long pageId)
        {
            var client = GetProjectClient();
            try
            {
                var result = client.GetPageImage(pageId);
                return new FileStreamResult(result.Stream, result.MimeType);
            }
            catch (HttpErrorCodeException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    return NotFound();

                throw;
            }
        }

        [HttpGet]
        public IActionResult GetImageResourceByPageId(long pageId)
        {
            var client = GetProjectClient();
            try
            {
                var result = client.GetImageResourceByPageId(pageId);
                if (result == null)
                {
                    return NotFound();
                }

                result.ImageUrl = Url.Action("GetPageImage", "ContentEditor", new {Area = "Admin", pageId = pageId});
                return Json(result);
            }
            catch (HttpErrorCodeException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                    return NotFound();

                throw;
            }
        }

        [HttpGet]
        public IActionResult GetPageDetail(long pageId)
        {
            var client = GetProjectClient();
            var model = new PageDetailViewModel();
            try
            {
                model.Text = client.GetPageText(pageId, TextFormatEnumContract.Html);
            }
            catch (HttpErrorCodeException e) 
            {
                if(e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            model.HasImage = client.HasPageImage(pageId);
            model.PageId = pageId;
            
            return PartialView("../Project/Work/SubView/_PageListDetail", model);
        }

        [HttpPost]
        public IActionResult GetTextPages(long projectId, long? resourceGroupId)
        {
            var client = GetProjectClient();
            var result = client.GetAllTextResourceList(projectId, resourceGroupId);
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetPageList(long projectId)
        {
            var client = GetProjectClient();
            var result = client.GetAllPageList(projectId);
            return Json(result);
        }

        [HttpPost]
        public IActionResult SavePageList(long projectId, IList<CreateOrUpdatePageContract> pageList)
        {
            var client = GetProjectClient();
            client.SetAllPageList(projectId, pageList);
            return Ok();
        }

        [HttpPost]
        public IActionResult GetTextResourceByPageId(long pageId, TextFormatEnumContract? format)
        {
            var client = GetProjectClient();
            var result = client.GetTextResourceByPageId(pageId, format);
            return Json(result);
        }

        [HttpPost]
        public IActionResult SetTextResource(long textId, CreateTextRequestContract request, SaveTextModeType mode)
        {
            SaveTextResponse response;
            switch (mode)
            {
                case SaveTextModeType.FullValidateOrDeny:
                    response = m_textManager.SaveTextFullValidate(textId, request);
                    break;
                case SaveTextModeType.FullValidateAndRepair:
                    response = m_textManager.SaveTextFullValidateAndRepair(textId, request);
                    break;
                case SaveTextModeType.ValidateOnlySyntax:
                    response = m_textManager.SaveTextValidateSyntax(textId, request);
                    break;
                case SaveTextModeType.NoValidation:
                    response = m_textManager.SaveWithoutValidation(textId, request);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }

            return Json(response);
        }

        [HttpPost]
        public IActionResult CreateTextResource(long pageId)
        {
            var client = GetProjectClient();
            var resourceId = client.CreateTextResource(pageId);
            return Json(resourceId);
        }

        [HttpPost]
        public IActionResult CreateImageResource([FromForm] SaveImageResourceRequest request)
        {
            var client = GetProjectClient();
            var data = new CreateImageContract
            {
                ImageId = request.ImageId,
                OriginalVersionId = request.ResourceVersionId,
                ResourcePageId = request.PageId,
                Comment = request.Comment,
                FileName = request.File.FileName,
            };
            var result = client.CreateImageResource(data, request.File.OpenReadStream());

            return Json(new ImageContract
            {
                Id = result.ResourceId,
                VersionId = result.ResourceVersionId,
                VersionNumber = result.VersionNumber,
                ImageUrl = Url.Action("GetPageImage", "ContentEditor", new { Area = "Admin", pageId = request.PageId }),
            });
        }

        [HttpGet]
        public IActionResult GetEditionNote(long projectId, TextFormatEnumContract format)
        {
            var client = GetProjectClient();
            var result = client.GetLatestEditionNote(projectId, format);
            return Json(result);
        }

        [HttpPost]
        public IActionResult SetEditionNote(CreateEditionNoteRequest request)
        {
            var client = GetProjectClient();
            var data = new CreateEditionNoteContract
            {
                Text = request.Content,
                OriginalVersionId = request.OriginalVersionId
            };
            var resourceVersionId = client.CreateEditionNote(request.ProjectId, data);
            return Json(resourceVersionId);
        }
    }
}