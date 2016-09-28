using ITJakub.Shared.Contracts;
using ITJakub.Web.Hub.Managers;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class AreaController : BaseController
    {
        protected AreaController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public abstract BookTypeEnumContract AreaBookType { get; }
    }
}