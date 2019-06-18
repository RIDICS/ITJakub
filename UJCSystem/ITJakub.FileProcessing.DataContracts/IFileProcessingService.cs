using System.Collections.Generic;
using System.ServiceModel;
using Vokabular.Shared;

namespace ITJakub.FileProcessing.DataContracts
{
    [ServiceContract]
    public interface IFileProcessingService
    {
        [OperationContract]
        void AddResource(UploadResourceContract resourceInfoSkeleton);

        [OperationContract]
        ImportResult ProcessSession(string sessionId, long? projectId, int userId, string uploadMessage, IList<PermissionFromAuthContract> autoImportPermissions);
    }
}