using System;
using System.Collections.Generic;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace Vokabular.DataEntities.Database.Entities
{
    public abstract class SpecialPermission : IEquatable<SpecialPermission>
    {
        public virtual int Id { get; set; }

        public virtual SpecialPermissionCategorization PermissionCategorization { get; protected set; }

        public virtual IList<UserGroup> UserGroups { get; set; }

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


    public class AutoImportBookTypePermission : SpecialPermission
    {
        public AutoImportBookTypePermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Autoimport;
        }

        public virtual bool AutoImportIsAllowed { get; set; }

        public virtual BookType BookType { get; set; }
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

    public class DerivateLemmatizationPermission : SpecialPermission
    {
        public DerivateLemmatizationPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Action;
        }

        public virtual bool CanDerivateLemmatization { get; set; }
    }

    public class EditionPrintTextPermission : SpecialPermission
    {
        public EditionPrintTextPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Action;
        }

        public virtual bool CanEditionPrintText { get; set; }
    }

    public class EditStaticTextPermission : SpecialPermission
    {
        public EditStaticTextPermission()
        {
            PermissionCategorization = SpecialPermissionCategorization.Action;
        }

        public virtual bool CanEditStaticText { get; set; }
    }
}