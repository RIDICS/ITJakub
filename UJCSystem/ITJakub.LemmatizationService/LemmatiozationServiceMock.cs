using ITJakub.Core;
using ITJakub.Shared.Contracts;

namespace ITJakub.LemmatizationService
{
    public class LemmatiozationServiceMock : ILemmatizationService
    {
        public string GetLemma(string word)
        {
            return word;
        }

        public string GetStemma(string word)
        {
            return word;
        }
    }
}