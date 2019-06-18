namespace Vokabular.Authentication.DataContracts
{
    public class NonceContract : ContractBase
    {
        public int UserId { get; set; }
        
        public NonceTypeEnum Type { get; set; }
        
        public string Nonce { get; set; }
    }
}