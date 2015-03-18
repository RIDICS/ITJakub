namespace ITJakub.DataEntities.Database.Exceptions
{
    public class BookDoesNotExistException : System.Exception
    {
        public BookDoesNotExistException(string message) : base(message)
        {
        }
    }
}