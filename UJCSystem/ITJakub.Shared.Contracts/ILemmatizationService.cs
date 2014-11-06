using System.ServiceModel;

namespace ITJakub.Shared.Contracts
{
    [ServiceContract]
    public interface ILemmatizationService
    {
        [OperationContract]
        string GetLemma(string word);

        [OperationContract]
        string GetStemma(string word);
    }
}
