using System.Collections.Generic;
using System.ServiceModel;

namespace ITJakub.Lemmatization.Shared.Contracts
{
    [ServiceContract]
    public interface ILemmatizationService
    {
        [OperationContract]
        IList<TokenContract> GetTypeaheadToken(string query);

        [OperationContract]
        long CreateToken(string token, string description);

        [OperationContract]
        IList<TokenCharacteristicContract> GetTokenCharacteristic(long tokenId);

        [OperationContract]
        long AddTokenCharacteristic(long tokenId, string morphologicalCharacteristic, string description);

        [OperationContract]
        void AddCanonicalForm(long tokenCharacteristicId, long canonicalFormId);

        [OperationContract]
        long CreateCanonicalForm(CanonicalFormTypeContract type, string text, string description);
    }
}
