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
        ImportResultContract ProcessSession(string sessionId, long? projectId, int userId, string uploadMessage,
            ProjectTypeContract projectType, FulltextStoreTypeContract storeType, IList<PermissionFromAuthContract> autoImportPermissions);
    }
}