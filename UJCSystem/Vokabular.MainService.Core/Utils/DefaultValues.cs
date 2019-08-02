using System;
using System.Collections.Generic;
using Vokabular.Shared.Const;

namespace Vokabular.MainService.Core.Utils
{
    public static class DefaultValues
    {
        public const int AutocompleteCount = 5;
        public const int Start = 0;
        public const int Count = 20;
        public const int MaxCount = 200;

        public const int ProjectCount = 10;
        public const int MaxProjectCount = 100;

        public const string RoleForNewPermissions = "Admin";
        public static readonly IReadOnlyList<Tuple<string, string>> RequiredPermissionsWithDescription = new List<Tuple<string, string>>
        {
            // All non-default permission can be specified here to automatic creating them on Auth service
            new Tuple<string, string>(VokabularPermissionNames.ManageBibliographyImport, "Permission to manage configuration of external bibliography for import and allow to start import"),
            new Tuple<string, string>(VokabularPermissionNames.ManageCodeList, "Permission to manage categories, literary genres, authors, etc."),
        };
    }
}
