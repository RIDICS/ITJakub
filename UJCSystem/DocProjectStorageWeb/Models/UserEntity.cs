//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DocProjectStorageWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserEntity
    {
        public UserEntity()
        {
            this.Roles = new HashSet<UserRoleEntity>();
        }
    
        public int Id { get; set; }
        public string Email { get; set; }
    
        public virtual ICollection<UserRoleEntity> Roles { get; set; }
    }
}
