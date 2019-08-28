using System;
using Vokabular.MainService.DataContracts;

namespace ITJakub.BatchImport.Client.ServiceClient
{
    public class MainServiceClientLocalization : IMainServiceClientLocalization
    {
        public void LocalizeApiException(MainServiceException exception)
        {
            // Localization is not supported in this application, so do nothing
        }

        public bool TryLocalizeErrorCode(string code, out string localizedString, params object[] codeParams)
        {
            throw new NotSupportedException();
            // Localization is not supported in this application, so do nothing
        }
    }
}