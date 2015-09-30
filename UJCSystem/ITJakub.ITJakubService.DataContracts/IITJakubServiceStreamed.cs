using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts.Contracts.AudioBooks;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.ITJakubService.DataContracts
{
    [ServiceContract]
    public interface IItJakubServiceStreamed
    {
        #region Resource Import

        [OperationContract]
        void AddResource(UploadResourceContract uploadFileInfoSkeleton);

        #endregion

        #region AudioBooks

        [OperationContract]
        FileDataContract DownloadWholeAudiobook(DownloadWholeBookContract requestContract);

        [OperationContract]
        AudioTrackContract DownloadAudioBookTrack(DownloadAudioBookTrackContract requestContract);

        #endregion

    }
}