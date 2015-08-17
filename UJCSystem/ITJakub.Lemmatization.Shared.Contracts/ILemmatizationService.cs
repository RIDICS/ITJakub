using System.Collections.Generic;
using System.ServiceModel;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    [ServiceContract]
    public interface ILemmatizationService
    {
        [OperationContract]
        string GetLemma(string word);

        [OperationContract]
        string GetStemma(string word);

        [OperationContract]
        IList<LemmatizationTypeaheadContract> GetTypeaheadToken(string query);
    }
}
