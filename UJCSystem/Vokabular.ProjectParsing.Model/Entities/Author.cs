namespace Vokabular.ProjectParsing.Model.Entities
{
    public class Author
    {
        public string FirstName;
        public string LastName;

        public Author(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
