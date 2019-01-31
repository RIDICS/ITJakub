namespace Vokabular.RestClient.Contracts
{
    //TODO get from Vokabular.Authentication.DataContracts (after upgrading to .NET Standard)
    public class ContractException
    {
        public string Code { get; set; }

        public string Description { get; set; }
    }
}
