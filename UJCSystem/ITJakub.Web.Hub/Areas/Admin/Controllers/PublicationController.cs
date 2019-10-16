using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using ITJakub.Web.Hub.Options;
using Scalesoft.Localization.AspNetCore;
using Vokabular.RestClient.Errors;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [LimitedAccess(PortalType.CommunityPortal)]
    [RequestFormLimits(ValueCountLimit = 20000)]
    [Area("Admin")]
    public class PublicationController : BaseController
    {
        private readonly ILocalizationService m_localization;

        public PublicationController(ControllerDataProvider controllerDataProvider, ILocalizationService localization) : base(controllerDataProvider)
        {
            m_localization = localization;
        }
       
    
        public IActionResult NewSnapshot(long projectId)
        {
            var client = GetProjectClient();
            var snapshot = client.GetLatestPublishedSnapshot(projectId);

            var model = CreateNewPublicationViewModel(projectId, snapshot);
            return View("PublicationsNew", model);
        }

        public IActionResult DuplicateSnapshot(long snapshotId)
        {
            var client = GetSnapshotClient();
            var snapshot = client.GetSnapshot(snapshotId);
            var model = CreateNewPublicationViewModel(snapshot.ProjectId, snapshot);

            foreach (var resourceList in model.ResourceTypes)
            {
                foreach (var resource in resourceList.ResourceList)
                {
                    var versionResource = snapshot.ResourceVersions.FirstOrDefault(x => x.ResourceType == resourceList.ResourceType && x.ResourceId == resource.Id);
                    if (versionResource != null)
                    {
                        resource.IsSelected = true;
                        resource.ResourceVersionId = versionResource.Id;
                        resource.VersionNumber = versionResource.VersionNumber;
                    }
                }
            }
            
            return View("PublicationsNew", model);
        }

        public IActionResult VersionList(long resourceId, int? higherVersion, int? lowerVersion)
        {
            if (lowerVersion == null)
            {
                return BadRequest();
            }

            var client = GetProjectClient();
            var resourceVersionList = client.GetResourceVersionHistory(resourceId);
            var viewModel = Mapper.Map<List<ResourceVersionViewModel>>(resourceVersionList);
            
            return Json(viewModel);
        }
        
        [HttpPost]
        public IActionResult NewSnapshot(NewPublicationViewModel viewModel)
        {
            var client = GetSnapshotClient();

            var versionIds = new List<long>();
            foreach (var resource in viewModel.ResourceTypes)
            {
                if (resource.ResourceList != null)
                    versionIds.AddRange(resource.ResourceList.Where(x => x.IsSelected).Select(x => x.ResourceVersionId).ToList());
            }
            
            var createSnapshotContract = new CreateSnapshotContract
            {
                BookTypes = viewModel.PublishBookTypes.Where(x => x.IsSelected).Select(x => x.BookType).ToList(),
                DefaultBookType = viewModel.DefaultBookType,
                ResourceVersionIds = versionIds,
                Comment = viewModel.Comment,
                ProjectId = viewModel.ProjectId
            };

            client.CreateSnapshot(createSnapshotContract);
            return RedirectToAction("Project", "Project", new { id = viewModel.ProjectId });
        }


        public IActionResult GetText(long textVersionId)
        {
            var client = GetProjectClient();
            var result = client.GetTextResourceVersion(textVersionId, TextFormatEnumContract.Html);
            return Json(result);
        }

        public IActionResult GetImage(long imageVersionId)
        {
            var client = GetProjectClient();
            var result = client.GetImageResourceVersion(imageVersionId);
            return new FileStreamResult(result.Stream, result.MimeType);
        }

        public IActionResult GetRecordings(long trackVersionId)
        {
            throw new InvalidOperationException("Wrong implementation: Resource and ResourceVersion identifier mismatch.");
            //var client = GetProjectClient();
            //var result = client.GetAudioTrackRecordings(trackId);
            //return PartialView("_AudioResourcePreview", result);
        }

        public FileResult DownloadAudio(long audioId)
        {
            var client = GetProjectClient();
            var fileData = client.GetAudio(audioId);
            Response.ContentLength = fileData.FileSize;
            return File(fileData.Stream, fileData.MimeType, fileData.FileName);
        }
        
        private NewPublicationViewModel CreateNewPublicationViewModel(long projectId, SnapshotContract snapshot)
        {
            var client = GetProjectClient();
            var project = client.GetProject(projectId);
            //var audio = client.GetResourceList(projectId, ResourceTypeEnumContract.Audio);
            var images = client.GetResourceList(projectId, ResourceTypeEnumContract.Image);
            var text = client.GetResourceList(projectId, ResourceTypeEnumContract.Text);
            EditionNoteContract editionNote;
            try
            {
                editionNote = client.GetLatestEditionNote(projectId, TextFormatEnumContract.Html);
            }
            catch (HttpErrorCodeException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    editionNote = null;
                }
                else
                {
                    throw;
                }
            }


            var availableBookTypes = new List<BookTypeEnumContract>
            {
                BookTypeEnumContract.Edition,
                BookTypeEnumContract.TextBank,
                BookTypeEnumContract.Grammar,
                //BookTypeEnumContract.AudioBook,
            };
            
            return new NewPublicationViewModel
            {
                ProjectId = projectId,
                ProjectName = project.Name,
                ResourceTypes = new List<ResourceTypeViewModel>
                {
                    new ResourceTypeViewModel
                    {
                        ResourceList = Mapper.Map<IList<ResourceViewModel>>(text.OrderBy(x => x.LatestVersion.RelatedResource.Sequence)),
                        ResourceType = ResourceTypeEnumContract.Text,
                        Title = m_localization.Translate("TextSources", "Admin"),
                    },
                    new ResourceTypeViewModel
                    {
                        ResourceList = Mapper.Map<IList<ResourceViewModel>>(images.OrderBy(x => x.LatestVersion.RelatedResource.Sequence)),
                        ResourceType = ResourceTypeEnumContract.Image,
                        Title = m_localization.Translate("ImageSources", "Admin"),
                    },
                    //new ResourceTypeViewModel
                    //{
                    //    ResourceList = Mapper.Map<IList<ResourceViewModel>>(audio.OrderBy(x => x.LatestVersion.RelatedResource.Sequence)),
                    //    ResourceType = ResourceTypeEnumContract.Audio,
                    //    Title = m_localization.Translate("AudioSources", "Admin"),
                    //},
                },
                AvailableBookTypes = availableBookTypes,
                PublishBookTypes = availableBookTypes.Select(availableBookType => new SelectableBookType
                {
                    BookType = availableBookType,
                    IsSelected = snapshot != null && snapshot.BookTypes.Any(y => y == availableBookType),
                }).ToList(),
                EditionNoteText = editionNote?.Text,
                DefaultBookType = snapshot?.DefaultBookType ?? BookTypeEnumContract.Edition,
             };
        }
    }
}