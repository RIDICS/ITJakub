namespace Vokabular.MainService.DataContracts
{
    public interface IMainServiceClientLocalization
    {
        void LocalizeApiException(MainServiceException exception);
        bool TryLocalizeErrorCode(string code, out string localizedString, params object[] codeParams);
    }
}