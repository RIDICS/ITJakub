using System.ServiceModel;
using ITJakub.Shared.Contracts;

namespace ITJakub.LemmatizationService
{
    public class LemmatizationService : ILemmatizationServiceLocal
    {
        private readonly ILemmatizationService m_lemmatizationServiceImpl;

        public LemmatizationService()
        {
            m_lemmatizationServiceImpl = new LemmatiozationServiceMock();
        }

        public string GetLemma(string word)
        {
            return m_lemmatizationServiceImpl.GetLemma(word);
        }

        public string GetStemma(string word)
        {
            return m_lemmatizationServiceImpl.GetStemma(word);
        }
    }

    [ServiceContract]
    public interface ILemmatizationServiceLocal:ILemmatizationService
    {
    }
}
