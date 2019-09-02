using System;
using Microsoft.Extensions.Logging;
using Scalesoft.Localization.AspNetCore;
using Scalesoft.Localization.Core.Exception;
using Scalesoft.Localization.Core.Manager;
using Scalesoft.Localization.Core.Util;
using Vokabular.MainService.DataContracts;
using Vokabular.MainService.DataContracts.Clients;
using Vokabular.Shared;

namespace ITJakub.Web.Hub.Helpers
{
    public class MainServiceClientLocalization : IMainServiceClientLocalization
    {
        private static readonly ILogger m_logger = ApplicationLogging.CreateLogger<MainServiceRestClient>();
        private readonly ILocalizationService m_localization;
        private readonly IAutoLocalizationManager m_localizationManager;
        private const string Scope = "MainServiceErrorCode";

        public MainServiceClientLocalization(ILocalizationService localization, IAutoLocalizationManager localizationService)
        {
            m_localization = localization;
            m_localizationManager = localizationService;
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
                        var localizeResult = m_localizationManager.Translate(LocTranslationSource.File, m_localization.GetRequestCulture(), Scope, code);
                        localizedString = localizeResult.Value;
                        return !localizeResult.ResourceNotFound;
                    }
                    else
                    {
                        var localizationParams = new object[codeParams.Length];
                        for (var i = 0; i < codeParams.Length; i++)
                        {
                            localizationParams[i] = LocalizeErrorCode(codeParams[i].ToString());
                        }

                        var localizeResult = m_localizationManager.TranslateFormat(LocTranslationSource.File, m_localization.GetRequestCulture(), Scope, code, localizationParams);
                        localizedString = localizeResult.Value;
                        return !localizeResult.ResourceNotFound;
                    }
                }
            }
            catch (Exception e) when (e is LocalizationLibraryException || e is TranslateException)
            {
                if (m_logger.IsEnabled(LogLevel.Warning))
                {
                    m_logger.LogWarning("Translation for main service code '{0}' not found", code);
                }
            }

            localizedString = code;
            return false;
        }
    }
}