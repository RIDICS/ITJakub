namespace Vokabular.ForumSite.Core.Helpers
{
    static class BookTypeHelper
    {
        public static UrlBookTypeEnum[] GetBookTypeEnumsWithCategories()
        {
            return new UrlBookTypeEnum[]
            {
                UrlBookTypeEnum.Dictionaries,
                UrlBookTypeEnum.Editions,
                UrlBookTypeEnum.BohemianTextBank,
                UrlBookTypeEnum.OldGrammar,
                UrlBookTypeEnum.ProfessionalLiterature,
                UrlBookTypeEnum.AudioBooks
            };
        }
    }
}
