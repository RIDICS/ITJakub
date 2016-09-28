using ITJakub.Shared.Contracts;
using ITJakub.Web.Hub.Managers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class AreaController : BaseController
    {
        protected AreaController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public abstract BookTypeEnumContract AreaBookType { get; }

        protected JsonSerializerSettings GetJsonSerializerSettingsForBiblModule()
        {
            return new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
            };
        }
    }
}