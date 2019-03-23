namespace Vokabular.ProjectImport.Model.Exception
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