using System;
using System.Collections.Generic;
using System.Net;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Areas.Admin.Models.Request;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;
using ITJakub.Web.Hub.Options;
using Vokabular.RestClient.Errors;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [LimitedAccess(PortalType.CommunityPortal)]
    [Area("Admin")]
    public class ContentEditorController : BaseController
    {
        public ContentEditorController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        [HttpPost]
        public IActionResult GetGuid()
        {
            var guid = Guid.NewGuid();
            return Json(guid);
        }

        [HttpPost]
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

        [HttpGet]
        public IActionResult GetPageImage(long pageId)
        {
            var client = GetBookClient();
            var result = client.GetPageImage(pageId);
            return new FileStreamResult(result.Stream, result.MimeType);
        }

        [HttpGet]
        public IActionResult GetPageDetail(long pageId)
        {
            var client = GetBookClient();
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

            try
            {
                var result = client.GetPageImage(pageId);
                model.Image = new FileStreamResult(result.Stream, result.MimeType);
            }
            catch (HttpErrorCodeException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    throw;
            }

            return PartialView("../Project/Work/_PageListDetail", model);
        }

        [HttpPost]
        public IActionResult GetProjectContent(long projectId, long? resourceGroupId)
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
        public IActionResult GetTextResource(long textId, TextFormatEnumContract? format)
        {
            var client = GetProjectClient();
            var result = client.GetTextResource(textId, format);
            return Json(result);
        }

        [HttpPost]
        public IActionResult SetTextResource(long textId, CreateTextRequestContract request)
        {
            var client = GetProjectClient();
            var result = client.CreateTextResourceVersion(textId, request);
            return Json(result);
        }

        [HttpGet]
        public IActionResult GetEditionNote(long projectId, TextFormatEnumContract format)
        {
            var client = GetBookClient();
            var result = client.GetEditionNote(projectId, format);
            return Json(result);
        }

        [HttpPost]
        public IActionResult SetEditionNote([FromBody] CreateEditionNoteRequest request)
        {
            //var result = client.
            var result = "TODO"; //TODO add logic
            return Json(result);
        }
    }
}