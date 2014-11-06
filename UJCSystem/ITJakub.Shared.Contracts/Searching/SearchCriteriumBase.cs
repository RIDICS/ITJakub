using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts.Searching
{
    [DataContract]
    [KnownType(typeof(AuthorSearchCriterium))]
    [KnownType(typeof(TitleSearchCriterium))]
    public class SearchCriteriumBase
    {
    }

    [DataContract]
    public class AuthorSearchCriterium:SearchCriteriumBase
    {
        public CriteriumTextElement Author { get; private set; }

        public AuthorSearchCriterium(string author)
        {
            Author = new CriteriumTextElement(author);
        }
    }


    [DataContract]
    public class TitleSearchCriterium:SearchCriteriumBase
    {
        public CriteriumTextElement Title { get; private set; }

        public TitleSearchCriterium(string title)
        {
            Title = new CriteriumTextElement(title);
        }
    }
}