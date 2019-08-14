using System;
using Microsoft.Extensions.Logging;
using Scalesoft.Localization.AspNetCore;
using Scalesoft.Localization.Core.Exception;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Clients;
using Vokabular.Shared;

namespace ITJakub.Web.Hub.Helpers
{
    public class MainServiceClientLocalization : IMainServiceClientLocalization
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly ILocalizationService m_localization;

        public MainServiceClientLocalization(ILocalizationService localizationService)
        {
            m_localization = localizationService;
        }

        public void LocalizeApiException(MainServiceException ex)
        {
            try
            {
                if (!string.IsNullOrEmpty(ex.Code))
                {
                    if (ex.DescriptionParams == null)
                    {
                        ex.Description = m_localization.Translate(ex.Code, "MainServiceErrorCode");
                    }
                    else
                    {
                        ex.Description = m_localization.TranslateFormat(ex.Code, "MainServiceErrorCode", ex.DescriptionParams);
                    }
                }
            }
            catch (Exception e) when (e is LocalizationLibraryException || e is TranslateException)
            {
                if (m_logger.IsEnabled(LogLevel.Warning))
                {
                    m_logger.LogWarning("Translation for main service code '{0}' not found", ex.Code);
                }

                //if translation is not defined, propagate original description
            }
        }
    }
}
