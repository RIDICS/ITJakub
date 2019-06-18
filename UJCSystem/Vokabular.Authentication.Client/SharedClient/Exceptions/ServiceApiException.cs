namespace Vokabular.Authentication.Client.SharedClient.Exceptions
{
    public class ServiceApiException : ServiceException
    {
        public ServiceApiException(string message) : base(message, null)
        {
        }

        public string Code { get; set; }

        public string Description { get; set; }
    }
}