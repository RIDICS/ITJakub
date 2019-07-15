using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.Models;
using ITJakub.Web.Hub.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.Shared.AspNetCore.Helpers;
using Vokabular.Shared.Const;

namespace ITJakub.Web.Hub.Controllers
{
    [Authorize(VokabularPermissionNames.UploadBook)]
    public class UploadController : BaseController
    {
        public UploadController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public ActionResult Upload()
        {
            return View(new UploadViewModel {SessionId = Guid.NewGuid().ToString()});
        }

        //Dropzone upload method
        [HttpPost]
        public async Task<ActionResult> UploadFile()
        {
            var boundary = UploadHelper.GetBoundary(Request.ContentType);
            var reader = new MultipartReader(boundary, Request.Body, UploadHelper.MultipartReaderBufferSize);

            var valuesByKey = new Dictionary<string, string>();
            MultipartSection section;

            while ((section = await reader.ReadNextSectionAsync()) != null)
            {
                var contentDispo = section.GetContentDispositionHeader();

                if (contentDispo.IsFileDisposition())
                {
                    if (!valuesByKey.TryGetValue("sessionId", out var sessionId))
                    {
                        return BadRequest();
                    }

                    var fileSection = section.AsFileSection();
                    
                    using (var client = GetRestClient())
                    {
                        client.UploadResource(sessionId, fileSection.FileStream, fileSection.FileName);
                    }
                }
                else if (contentDispo.IsFormDisposition())
                {
                    var formSection = section.AsFormDataSection();
                    var value = await formSection.GetValueAsync();
                    valuesByKey.Add(formSection.Name, value);
                }
            }

            return Json(new {});
        }

        [HttpPost]
        public ActionResult ProcessUploadedFiles([FromBody] ProcessUploadedFilesRequest request)
        {
            using (var client = GetRestClient())
            {
                client.ProcessSessionAsImport(request.SessionId, new NewBookImportContract
                {
                    Comment = request.UploadMessage,
                    ProjectId = request.ProjectId
                });
                return Json(new {success = true});
            }
        }
    }
}