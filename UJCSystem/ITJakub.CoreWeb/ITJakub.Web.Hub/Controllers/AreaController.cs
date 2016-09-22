using ITJakub.Shared.Contracts;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class AreaController : BaseController
    {
        public abstract BookTypeEnumContract AreaBookType { get; }
    }
}