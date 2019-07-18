namespace Vokabular.ProjectImport.Shared.Options
{
    public class OaiPmhClientOption
    {
        public int RetryCount { get; set; }
        public int Delay { get; set; }
        public bool DisableSslValidation { get; set; }
    }
}