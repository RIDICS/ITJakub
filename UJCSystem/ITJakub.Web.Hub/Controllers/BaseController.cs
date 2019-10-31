using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using AutoMapper;
using ITJakub.Lemmatization.Shared.Contracts;
using ITJakub.Web.Hub.Areas.Admin.Models;
using ITJakub.Web.Hub.Core;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Scalesoft.Localization.AspNetCore;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Clients;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.RestClient.Errors;
using Vokabular.RestClient.Results;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class BaseController : Controller
    {
        private readonly ControllerDataProvider m_controllerDataProvider;
        private readonly CommunicationProvider m_communication;

        protected BaseController(ControllerDataProvider controllerDataProvider)
        {
            m_controllerDataProvider = controllerDataProvider;
            m_communication = controllerDataProvider.CommunicationProvider;
        }

        protected PortalTypeContract PortalTypeValue => m_controllerDataProvider.PortalType;

        protected IMapper Mapper => m_controllerDataProvider.Mapper;

        protected ILocalizationService Localizer => m_controllerDataProvider.Localizer;

        public ProjectTypeContract GetDefaultProjectType()
        {
            switch (PortalTypeValue)
            {
                case PortalTypeContract.Research:
                    return ProjectTypeContract.Research;
                case PortalTypeContract.Community:
                    return ProjectTypeContract.Community;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public MainServiceBookClient GetBookClient()
        {
            return m_communication.GetMainServiceBookClient();
        }

        public MainServiceCardFileClient GetCardFileClient()
        {
            return m_communication.GetMainServiceCardFileClient();
        }

        public MainServiceCodeListClient GetCodeListClient()
        {
            return m_communication.GetMainServiceCodeListClient();
        }

        public MainServiceExternalRepositoryClient GetExternalRepositoryClient()
        {
            return m_communication.GetMainServiceExternalRepositoryClient();
        }
        
        public MainServiceFavoriteClient GetFavoriteClient()
        {
            return m_communication.GetMainServiceFavoriteClient();
        }

        public MainServiceFeedbackClient GetFeedbackClient()
        {
            return m_communication.GetMainServiceFeedbackClient();
        }

        public MainServiceFilteringExpressionSetClient GetFilteringExpressionSetClient()
        {
            return m_communication.GetMainServiceFilteringExpressionSetClient();
        }

        public MainServiceMetadataClient GetMetadataClient()
        {
            return m_communication.GetMainServiceMetadataClient();
        }

        public MainServiceNewsClient GetNewsClient()
        {
            return m_communication.GetMainServiceNewsClient();
        }

        public MainServicePermissionClient GetPermissionClient()
        {   
            return m_communication.GetMainServicePermissionClient();
        }

        public MainServiceProjectClient GetProjectClient()
        {
            return m_communication.GetMainServiceProjectClient();
        }

        public MainServiceUserGroupClient GetRoleClient()
        {
            return m_communication.GetMainServiceRoleClient();
        }

        public MainServiceSessionClient GetSessionClient()
        {
            return m_communication.GetMainServiceSessionClient();
        }

        public MainServiceSnapshotClient GetSnapshotClient()
        {
            return m_communication.GetMainServiceSnapshotClient();
        }

        public MainServiceTermClient GetTermClient()
        {
            return m_communication.GetMainServiceTermClient();
        }

        public MainServiceUserClient GetUserClient()
        {
            return m_communication.GetMainServiceUserClient();
        }

        public LemmatizationServiceClient GetLemmationzationServiceClient()
        {
            return m_communication.GetLemmatizationClient();
        }

        protected bool IsUserLoggedIn()
        {
            return User.Identity.IsAuthenticated;            
        }

        protected string GetUserName()
        {
            return User.Identity.Name;
        }
        
        protected JsonSerializerSettings GetJsonSerializerSettingsForBiblModule()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver(),
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };
        }

        protected FileStreamResult File(Stream fileStream, string contentType, string fileDownloadName, long? fileSize)
        {
            Response.ContentLength = fileSize;
            return base.File(fileStream, contentType, fileDownloadName);
        }

        protected void AddErrors(HttpErrorCodeException exception)
        {
            if (exception.ValidationErrors == null)
            {
                // All errors with description should be propagated by MainServiceException so this is fallback:
                ModelState.AddModelError(string.Empty, Localizer.Translate("unknown-error-msg", "Error"));
                return;
            }

            foreach (var error in exception.ValidationErrors)
            {
                ModelState.AddModelError(string.Empty, error.Message);
            }
        }

        protected void AddErrors(MainServiceException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Description);
        }

        protected IActionResult AjaxOkResponse()
        {
            return new JsonResult(new {});
        }

        protected IActionResult AjaxErrorResponse(string message, HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError)
        {
            var result = new ErrorContract
            {
                ErrorMessage = message
            };

            return new JsonResult(result)
            {
                StatusCode = (int)httpStatusCode
            };
        }

        protected ListViewModel<TTarget> CreateListViewModel<TTarget, TSource>(PagedResultList<TSource> data, int start, int pageSize,
            string search)
        {
            return new ListViewModel<TTarget>
            {
                TotalCount = data.TotalCount,
                List = Mapper.Map<List<TTarget>>(data.List),
                PageSize = pageSize,
                Start = start,
                SearchQuery = search
            };
        }
    }
}