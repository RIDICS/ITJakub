using System.Collections.Generic;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.Web.Hub.Areas.Admin.Controllers.Constants
{
    public class ProjectConstants
    {
        public const string CurrentUserPermissions = "CurrentUserPermissions";

        public static readonly IList<BookTypeEnumContract> AvailableBookTypes = new List<BookTypeEnumContract>
        {
            BookTypeEnumContract.Edition,
            BookTypeEnumContract.TextBank,
            BookTypeEnumContract.Grammar,
            //BookTypeEnumContract.AudioBook,
        };
    }
}