using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities;

namespace ITJakub.FileProcessing.Core.Sessions.Works.Helpers
{
    public class ProjectResponsibleNameEqualityComparer : IEqualityComparer<ProjectResponsiblePerson>
    {
        public bool Equals(ProjectResponsiblePerson obj1, ProjectResponsiblePerson obj2)
        {
            return string.Equals(obj1.ResponsiblePerson.FirstName, obj2.ResponsiblePerson.FirstName, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(obj1.ResponsiblePerson.LastName, obj2.ResponsiblePerson.LastName, StringComparison.InvariantCultureIgnoreCase) &&
                   string.Equals(obj1.ResponsibleType.Text, obj2.ResponsibleType.Text, StringComparison.InvariantCultureIgnoreCase);
        }

        public int GetHashCode(ProjectResponsiblePerson obj)
        {
            var hashCode = (obj.ResponsiblePerson.FirstName != null ? obj.ResponsiblePerson.FirstName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.ResponsiblePerson.LastName != null ? obj.ResponsiblePerson.LastName.GetHashCode() : 0);
            hashCode = (hashCode * 397) ^ (obj.ResponsibleType.Text != null ? obj.ResponsibleType.Text.GetHashCode() : 0);
            return hashCode;
        }
    }
}