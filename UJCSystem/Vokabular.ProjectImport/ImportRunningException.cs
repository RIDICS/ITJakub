using System;

namespace Vokabular.ProjectImport
{
    public class ImportRunningException : Exception
    {
        public ImportRunningException()
        {
        }

        public ImportRunningException(string message) : base(message)
        {
        }
    }
}