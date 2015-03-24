namespace ITJakub.DataEntities.Database.Exceptions
{
    public class BookDoesNotExistException : System.Exception
    {
        public BookDoesNotExistException(string bookGuid)
            : base(string.Format("Kniha s id {0} neexistuje..", bookGuid))
        {
        }
    }
}