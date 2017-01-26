using Vokabular.DataEntities.Database.Entities;

namespace ITJakub.FileProcessing.Core.Sessions.Works.Helpers
{
    public static class AuthorHelper
    {
        public static OriginalAuthor ConvertToEntity(string authorText)
        {
            var tokens = authorText.Split(',');
            if (tokens.Length == 2)
            {
                return new OriginalAuthor
                {
                    FirstName = tokens[1],
                    LastName = tokens[0]
                };
            }

            tokens = authorText.Split(' ');
            if (tokens.Length == 2)
            {
                return new OriginalAuthor
                {
                    FirstName = tokens[1],
                    LastName = tokens[0]
                };
            }

            return new OriginalAuthor
            {
                FirstName = string.Empty,
                LastName = authorText
            };
        }
    }
}
