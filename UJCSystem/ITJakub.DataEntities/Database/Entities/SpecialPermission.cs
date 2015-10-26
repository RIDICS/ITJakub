using System;
using System.Collections.Generic;
using ITJakub.DataEntities.Database.Entities.Enums;

namespace ITJakub.DataEntities.Database.Entities
{
    public abstract class SpecialPermission : IEquatable<SpecialPermission>
    {
        public virtual int Id { get; set; }

        public virtual SpecialPermissionCategorization PermissionCategorization { get; protected set; }

        public virtual IList<Group> Groups { get; set; }

        public virtual bool Equals(SpecialPermission other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SpecialPermission) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    public class UploadBookPermission : SpecialPermission
    {
        public UploadBookPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Action;
        }

        public virtual bool CanUploadBook { get; set; }
    }

    public class ManagePermissionsPermission : SpecialPermission
    {
        public ManagePermissionsPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Action;
        }

        public virtual bool CanManagePermissions { get; set; }
    }

    public class NewsPermission : SpecialPermission
    {
        public NewsPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Action;
        }

        public virtual bool CanAddNews { get; set; }
    }

    public class FeedbackPermission : SpecialPermission
    {
        public FeedbackPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Action;
        }

        public virtual bool CanManageFeedbacks { get; set; }
    }


    public class CardFilePermission : SpecialPermission
    {
        public CardFilePermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.CardFile;
        }

        public virtual bool CanReadCardFile { get; set; }

        public virtual string CardFileId { get; set; }

        public virtual string CardFileName { get; set; }
    }


    public class AutoImportCategoryPermission : SpecialPermission
    {
        public AutoImportCategoryPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Autoimport;
        }

        public virtual bool AutoImportIsAllowed { get; set; }

        public virtual Category Category { get; set; }
    }

    public class ReadLemmatizationPermission : SpecialPermission
    {
        public ReadLemmatizationPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.CardFile;
        }

        public virtual bool CanReadLemmatization { get; set; }
    }

    public class EditLemmatizationPermission : SpecialPermission
    {
        public EditLemmatizationPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Action;
        }

        public virtual bool CanEditLemmatization { get; set; }
    }
}