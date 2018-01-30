using System.Collections.Generic;

namespace Vokabular.RestClient.Contracts
{
    public class ValidationResultContract
    {
        public string Message { get; set; }

        public List<ValidationErrorContract> Errors { get; set; }
    }
}
