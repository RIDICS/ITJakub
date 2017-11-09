using System;
using System.Collections.Generic;
using ITJakub.Web.Hub.Areas.Admin.Models.Response;
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
                    var mainComment = new CommentStructureResponse
                    {
                        Order = order,
                        Time = ((DateTimeOffset)pageComments.CreateTime).ToUnixTimeMilliseconds(),
                        Text = pageComments.Text,
                        Picture = pageComments.User.AvatarUrl,
                        Id = pageComments.Id,
                        Nested = false,
                        TextId = textId,
                        TextReferenceId = pageComments.TextReferenceId,
                        Name = pageComments.User.FirstName,
                        Surname = pageComments.User.LastName
                    };
                    parts.Add(mainComment);
                    if (pageComments.TextComments.Count > 0)
                    {
                        foreach (var textComment in pageComments.TextComments)
                        {
                            order++;
                            var nestedComment = new CommentStructureResponse
                            {
                                Order = order,
                                Time = ((DateTimeOffset)textComment.CreateTime).ToUnixTimeMilliseconds(),
                                Text = textComment.Text,
                                Picture = textComment.User.AvatarUrl,
                                Id = textComment.Id,
                                Nested = true,
                                TextId = textId,
                                TextReferenceId = textComment.TextReferenceId,
                                Name = textComment.User.FirstName,
                                Surname = textComment.User.LastName
                            };
                            parts.Add(nestedComment);
                        }
                    }
                }
            }
            return Json(parts);
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

        [HttpPost]
        public IActionResult GetPageImage(long pageId)
        {
            using (var client = GetRestClient())
            {
                var result = client.GetPageImage(pageId);
                return new FileStreamResult(result.Stream, "image/jpeg");
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
    }
}
