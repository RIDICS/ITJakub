using System.ServiceModel;

namespace ITJakub.SearchService.Core.Exist
{
    public class ExistDaoClient : ClientBase<IExistDao>, IExistDao
    {

        public string GetPageList(string projectName, string documentId)
        {
            ClientCredentials.UserName.UserName = "admin";
            ClientCredentials.UserName.Password = "admin";
            return Channel.GetPageList(projectName, documentId);
        }

        public string GetPageListWithTransformation(string projectName, string documentId, string xslPath)
        {
            return Channel.GetPageListWithTransformation(projectName, documentId, xslPath);
        }
    }
}