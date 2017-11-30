using System;
using System.Collections.Generic;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
using ITJakub.Web.Hub.Areas.Admin.Models.Type;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
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
            using (var client = GetRestClient())
            {
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
            }
            return Json(parts);
        }

        private static CommentStructureResponse CreateComment(int order, GetTextCommentContract textComment, bool nested, long textId)
        {
            var comment = new CommentStructureResponse
            {
                Order = order,
                Time = ((DateTimeOffset)textComment.CreateTime).ToUnixTimeMilliseconds(),
                Text = textComment.Text,
                Picture = textComment.User.AvatarUrl,
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
            using (var client = GetRestClient())
            {
                var result = client.CreateComment(textId, comment);
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult UpdateComment(CreateTextCommentContract comment, long textId)
        {
            using (var client = GetRestClient())
            {
                var result = client.UpdateComment(textId, comment);
                return Json(result);
            }
        }

        [HttpPost]
        public void DeleteComment(long commentId)//TODO needs response code
        {
            using (var client = GetRestClient())
            {
                client.DeleteComment(commentId);
            }
        }

        [HttpGet]
        public IActionResult GetPageImage(long pageId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetPageImage(pageId);
                return new FileStreamResult(result.Stream, result.MimeType);
            }
        }

        [HttpPost]
        public IActionResult GetProjectContent(long projectId, long? resourceGroupId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetAllTextResourceList(projectId, resourceGroupId);
                return Json(result);
            }
        }

        [HttpGet]
        public IActionResult GetPageList(long projectId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetAllPageList(projectId);
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult SavePageList(string[] pageList)
        {
            using (var client = GetRestClient())
            {
                var result = client.SetAllPageList(pageList);
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult GetTextResource(long textId, TextFormatEnumContract? format)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetTextResource(textId, format);
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult SetTextResource(long textId, CreateTextRequestContract request)
        {
            using (var client = GetRestClient())
            {
                var result = client.CreateTextResourceVersion(textId, request);
                return Json(result);
            }
        }

        [HttpGet]
        public IActionResult GetEditionNote(long projectId, TextFormatEnumContract format)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetEditionNote(projectId, format);
                return Json(result);
            }
        }

        [HttpPost]
        public IActionResult SetEditionNote(EditionNote request)
        {
            using (var client = GetRestClient())
            {
                //var result = client.
                var result = "TODO";//TODO add logic
                return Json(result);
            }
        }
    }
}
