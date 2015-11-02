using System;
using System.Collections.Generic;
using ITJakub.MobileApps.Client.Shared.Data;

namespace ITJakub.MobileApps.Client.Fillwords2.ViewModel.Comparer
{
    public class UserInfoNameComparer : IComparer<UserInfo>
    {
        public int Compare(UserInfo x, UserInfo y)
        {
            var lastNameCompare = string.Compare(x.LastName, y.LastName, StringComparison.CurrentCultureIgnoreCase);

            if (lastNameCompare != 0)
                return lastNameCompare;

            return string.Compare(x.FirstName, y.FirstName, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
