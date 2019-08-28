namespace Vokabular.RestClient.Contracts
{
    public class ErrorContract
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public object[] DescriptionParams { get; set; }
    }
}