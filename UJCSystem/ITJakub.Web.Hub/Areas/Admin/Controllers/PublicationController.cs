using System.Collections.Generic;
using System.Linq;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Authorization;
using ITJakub.Web.Hub.Controllers;
using ITJakub.Web.Hub.Core.Communication;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using ITJakub.Web.Hub.Options;
using Scalesoft.Localization.AspNetCore;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers
{
    [LimitedAccess(PortalType.CommunityPortal)]
    [Area("Admin")]
    public class PublicationController : BaseController
    {
        private const int SnapshotListPageSize = 10;
        private readonly ILocalizationService m_localization;

        public PublicationController(CommunicationProvider communicationProvider, ILocalizationService localization) : base(communicationProvider)
        {
            m_localization = localization;
        }
       
    
        public IActionResult NewSnapshot(long projectId)
        {
            var model = CreateNewPublicationViewModel(projectId);
            return View("PublicationsNew", model);
        }

        public IActionResult DuplicateSnapshot(long snapshotId)
        {
            var client = GetSnapshotClient();
            var snapshot = client.GetSnapshot(snapshotId);
            var model = CreateNewPublicationViewModel(snapshot.ProjectId);

            model.DefaultBookType = snapshot.DefaultBookType;
            model.Comment = snapshot.Comment;

            foreach (var selectableBookType in model.PublishBookTypes)
            {
                selectableBookType.IsSelected = snapshot.BookTypes.Any(x => x == selectableBookType.BookType);
            }

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

        public IActionResult VersionList(long resourceId)
        {
            var client = GetResourceClient();
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


        public IActionResult GetText(long textId)
        {
            var client = GetProjectClient();
            var result = client.GetTextResourceVersion(textId, TextFormatEnumContract.Html);
            return Json(result);
        }

        public IActionResult GetImage(long imageId)
        {
            var client = GetProjectClient();
            var result = client.GetImageResourceVersion(imageId);
            return new FileStreamResult(result.Stream, result.MimeType);
        }

        public IActionResult GetRecordings(long trackId)
        {
            var client = GetProjectClient();
            var result = client.GetAudioTrackRecordings(trackId);
            return PartialView("_AudioResourcePreview", result);
        }

        public FileResult DownloadAudio(long audioId)
        {
            var client = GetProjectClient();
            var fileData = client.GetAudio(audioId);
            Response.ContentLength = fileData.FileSize;
            return File(fileData.Stream, fileData.MimeType, fileData.FileName);
        }
        
        private NewPublicationViewModel CreateNewPublicationViewModel(long projectId)
        {
            var client = GetProjectClient();
            var audio = client.GetResourceList(projectId, ResourceTypeEnumContract.Audio);
            var images = client.GetResourceList(projectId, ResourceTypeEnumContract.Image);
            var text = client.GetResourceList(projectId, ResourceTypeEnumContract.Text);
            var availableBookTypes = new List<BookTypeEnumContract>
            {
                BookTypeEnumContract.Edition,
                BookTypeEnumContract.TextBank,
                BookTypeEnumContract.Grammar,
                BookTypeEnumContract.AudioBook
            };

            return new NewPublicationViewModel
            {
                ProjectId = projectId,
                ResourceTypes = new List<ResourceTypeViewModel>
                {
                    new ResourceTypeViewModel
                    {
                        ResourceList = Mapper.Map<IList<ResourceViewModel>>(text),
                        ResourceType = ResourceTypeEnumContract.Text,
                        Title = m_localization.Translate("TextSources", "Admin"),
                    },
                    new ResourceTypeViewModel
                    {
                        ResourceList = Mapper.Map<IList<ResourceViewModel>>(images),
                        ResourceType = ResourceTypeEnumContract.Image,
                        Title = m_localization.Translate("ImageSources", "Admin"),
                    },
                    new ResourceTypeViewModel
                    {
                        ResourceList = Mapper.Map<IList<ResourceViewModel>>(audio),
                        ResourceType = ResourceTypeEnumContract.Audio,
                        Title = m_localization.Translate("AudioSources", "Admin"),
                    }
                },
                AvailableBookTypes = availableBookTypes,
                PublishBookTypes = availableBookTypes.Select(x => new SelectableBookType { BookType = x }).ToList()
             };
        }
    }
}