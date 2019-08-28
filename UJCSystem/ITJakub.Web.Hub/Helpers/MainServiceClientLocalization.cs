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
        private const string Scope = "MainServiceErrorCode";

        public MainServiceClientLocalization(ILocalizationService localizationService)
        {
            m_localization = localizationService;
        }

        public void LocalizeApiException(MainServiceException ex)
        {
            if (TryLocalizeErrorCode(ex.Code, out var localizedDescription, ex.DescriptionParams))
            {
                ex.Description = localizedDescription;
            }
            //if translation is not defined, propagate original description
        }

        private string LocalizeErrorCode(string text)
        {
            try
            {
                return m_localization.Translate(text, Scope);
            }
            catch (Exception e) when (e is LocalizationLibraryException || e is TranslateException)
            {
                //if translation is not defined, propagate original text
                return text;
            }
        }

        public bool TryLocalizeErrorCode(string code, out string localizedString, params object[] codeParams)
        {
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    if (codeParams == null)
                    {
                        localizedString = m_localization.Translate(code, Scope);
                        return true;
                    }
                    
                    var localizationParams = new object[codeParams.Length];
                    for (var i = 0; i < codeParams.Length; i++)
                    {
                        localizationParams[i] = LocalizeErrorCode(codeParams[i].ToString());
                    }

                    localizedString = m_localization.TranslateFormat(code, Scope, localizationParams);
                    return true;
                }
            }
            catch (Exception e) when (e is LocalizationLibraryException || e is TranslateException)
            {
                if (m_logger.IsEnabled(LogLevel.Warning))
                {
                    m_logger.LogWarning("Translation for main service code '{0}' not found", code);
                }
            }

            localizedString = string.Empty;
            return false;
        }
    }
}