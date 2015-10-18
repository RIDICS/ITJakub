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
        IList<TokenCharacteristicDetailContract> GetTokenCharacteristic(long tokenId);

        [OperationContract]
        long AddTokenCharacteristic(long tokenId, string morphologicalCharacteristic, string description);

        [OperationContract]
        void AddCanonicalForm(long tokenCharacteristicId, long canonicalFormId);

        [OperationContract]
        void SetHyperCanonicalForm(long canonicalFormId, long hyperCanonicalFormId);

        [OperationContract]
        long CreateCanonicalForm(long tokenCharacteristicId, CanonicalFormTypeContract type, string text, string description);

        [OperationContract]
        long CreateHyperCanonicalForm(long canonicalFormId, HyperCanonicalFormTypeContract type, string text, string description);

        [OperationContract]
        IList<CanonicalFormTypeaheadContract> GetTypeaheadCanonicalForm(CanonicalFormTypeContract type, string query);

        [OperationContract]
        IList<HyperCanonicalFormContract> GetTypeaheadHyperCanonicalForm(HyperCanonicalFormTypeContract type, string query);

        [OperationContract]
        void EditToken(long tokenId, string description);

        [OperationContract]
        void EditTokenCharacteristic(long tokenCharacteristicId, string morphologicalCharacteristic, string description);

        [OperationContract]
        void EditCanonicalForm(long canonicalFormId, string text, CanonicalFormTypeContract type, string description);

        [OperationContract]
        void EditHyperCanonicalForm(long hyperCanonicalFormId, string text, HyperCanonicalFormTypeContract type, string description);

        [OperationContract]
        int GetTokenCount();

        [OperationContract]
        IList<TokenContract> GetTokenList(int start, int count);

        [OperationContract]
        TokenContract GetToken(long tokenId);

        [OperationContract]
        IList<long> GetCanonicalFormIdList(long hyperCanonicalFormId);

        [OperationContract]
        IList<InverseCanonicalFormContract> GetCanonicalFormDetail(long canonicalFormId);
    }
}
