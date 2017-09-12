using ITJakub.Web.Hub.Core.Communication;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class AreaController : BaseController
    {
        protected AreaController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public abstract BookTypeEnumContract AreaBookType { get; }
        public abstract Shared.Contracts.BookTypeEnumContract OldAreaBookType { get; }
    }
}