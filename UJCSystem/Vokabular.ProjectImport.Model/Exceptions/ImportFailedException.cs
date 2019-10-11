namespace Vokabular.ProjectImport.Model.Exceptions
{
    public class ImportFailedException : System.Exception
    {
        public string Code { get; }
        public object[] CodeParams { get; }
        
        public ImportFailedException(string code, string message) : base(message)
        {
            Code = code;
        }

        public ImportFailedException(string code, string message, params object[] codeParams) : base(message)
        {
            Code = code;
            CodeParams = codeParams;
        }

        public ImportFailedException(string code, string message, System.Exception innerException) : base(message, innerException)
        {
            Code = code;
        }

        public ImportFailedException(string code, string message, System.Exception innerException, params object[] codeParams) : base(message, innerException)
        {
            Code = code;
            CodeParams = codeParams;
        }
    }
}