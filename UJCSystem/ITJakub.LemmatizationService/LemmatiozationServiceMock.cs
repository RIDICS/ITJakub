using ITJakub.Contracts;
using ITJakub.Core;

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