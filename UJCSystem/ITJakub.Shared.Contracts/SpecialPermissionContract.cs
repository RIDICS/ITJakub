using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    [KnownType(typeof(AddNewsPermissionContract))]
    [KnownType(typeof(UploadBookPermissionContract))]
    [KnownType(typeof(ManagePermissionsPermissionContract))]
    [KnownType(typeof(FeedbackPermissionContract))]
    public class SpecialPermissionContract
    {
        [DataMember]
        public DateTime CreateTime { get; set; }

        [DataMember]
        public UserContract CreatedBy { get; set; }

        [DataMember]
        public IList<UserContract> Members { get; set; }
    }

    [DataContract]
    public class AddNewsPermissionContract: SpecialPermissionContract
    {
        [DataMember]
        public bool CanAddNews { get; set; }
    }

    [DataContract]
    public class UploadBookPermissionContract : SpecialPermissionContract
    {
        [DataMember]
        public bool CanUploadBook { get; set; }
    }

    [DataContract]
    public class ManagePermissionsPermissionContract : SpecialPermissionContract
    {
        [DataMember]
        public bool CanManagePermissions { get; set; }
    }

    [DataContract]
    public class FeedbackPermissionContract : SpecialPermissionContract
    {
        [DataMember]
        public bool CanManageFeedbacks { get; set; }
    }
}