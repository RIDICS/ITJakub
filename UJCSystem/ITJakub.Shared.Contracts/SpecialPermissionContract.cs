using System.Runtime.Serialization;

namespace ITJakub.Shared.Contracts
{
    [DataContract]
    [KnownType(typeof (NewsPermissionContract))]
    [KnownType(typeof (UploadBookPermissionContract))]
    [KnownType(typeof (ManagePermissionsPermissionContract))]
    [KnownType(typeof (FeedbackPermissionContract))]
    public class SpecialPermissionContract
    {
        [DataMember]
        public int Id { get; set; }
    }

    [DataContract]
    public class NewsPermissionContract : SpecialPermissionContract
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