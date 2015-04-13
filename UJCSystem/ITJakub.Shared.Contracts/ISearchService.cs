using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ITJakub.Shared.Contracts
{
    [ServiceContract]
    public interface ISearchService
    {
        [OperationContract]
        Task<string> GetBookPageByXmlIdAsync(string bookId, string versionId, string xmlId, string transformationName, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        Task<string> GetBookPageByPositionAsync(string bookId, string versionId, int pagePosition, string transformationName, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        Task<string> GetBookPageByNameAsync(string bookId, string versionId, string pageName, string transformationName, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        Task<string> GetBookPagesByNameAsync(string bookId, string versionId, string startPageName, string endPageName, string transformationName, ResourceLevelEnumContract transformationLevel);

        [OperationContract]
        Task<IList<BookPageContract>> GetBookPageListAsync(string bookId,string versionId);

        [OperationContract]
        Task UploadVersionFileAsync(VersionResourceUploadContract versionResourceUploadContract);

        [OperationContract]
        Task UploadBookFileAsync(BookResourceUploadContract contract);

        [OperationContract]
        Task UploadSharedFileAsync(ResourceUploadContract contract);
    }
}
