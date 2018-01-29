using Vokabular.DataEntities.Database.Entities;

namespace ITJakub.FileProcessing.Core.Sessions.Works.Helpers
{
    public static class PersonHelper
    {
        public static PersonData ConvertToEntity(string authorText)
        {
            var tokens = authorText.Split(',');
            if (tokens.Length == 2)
            {
                return new PersonData
                {
                    FirstName = tokens[1].Trim(),
                    LastName = tokens[0].Trim()
                };
            }

            tokens = authorText.Split(' ');
            if (tokens.Length == 2)
            {
                return new PersonData
                {
                    FirstName = tokens[0],
                    LastName = tokens[1]
                };
            }

            return new PersonData
            {
                FirstName = string.Empty,
                LastName = authorText
            };
        }

        public static OriginalAuthor ConvertToOriginalAuthor(string text)
        {
            var person = ConvertToEntity(text);
            return new OriginalAuthor
            {
                FirstName = person.FirstName,
                LastName = person.LastName
            };
        }
    }

    public class PersonData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
