using System;
using System.Collections.Generic;

namespace ITJakub.MobileApps.Client.Core.ViewModel.Comparer
{
    public class GroupMemberComparer : IComparer<GroupMemberViewModel>
    {
        public int Compare(GroupMemberViewModel x, GroupMemberViewModel y)
        {
            var compareValue = String.Compare(x.LastName, y.LastName, StringComparison.CurrentCulture);
            return compareValue != 0
                ? compareValue
                : String.Compare(x.FirstName, y.FirstName, StringComparison.CurrentCulture);
        }
    }
}
