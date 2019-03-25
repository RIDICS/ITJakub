namespace Vokabular.ProjectImport.Model.Exceptions
{
    public class ImportFailedException : System.Exception
    {
        public ImportFailedException(string message) : base(message)
        {
        }

        public ImportFailedException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}