namespace Vokabular.ProjectImport.Model.Exceptions
{
    public class ImportRunningException : System.Exception
    {
        public ImportRunningException()
        {
        }

        public ImportRunningException(string message) : base(message)
        {
        }
    }
}