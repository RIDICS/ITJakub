using System.Net.Http;

namespace Vokabular.Authentication.Client.Contract
{
    public class StreamHydrator<TContract> where TContract : StreamContract
    {
        public virtual void Hydrate(
            TContract responseObject,
            HttpResponseMessage response
        )
        {
        }
    }
}