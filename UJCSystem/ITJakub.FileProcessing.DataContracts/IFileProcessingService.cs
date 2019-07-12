using System.Collections.Generic;
using System.ServiceModel;

namespace ITJakub.FileProcessing.DataContracts
{
    [ServiceContract]
    public interface IFileProcessingService
    {
        [OperationContract]
        void AddResource(UploadResourceContract resourceInfoSkeleton);

        [OperationContract]
        bool ProcessSession(string sessionId, long? projectId, int userId, string uploadMessage, IList<PermissionFromAuthContract> autoImportPermissions);
    }
}