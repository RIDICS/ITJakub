using System;
using System.Text;
using System.Collections.Generic;


namespace Vokabular.ForumSite.DataEntities.Database.Entities {
    
    public class ProvMembership {
        public string UserID { get; set; }
        public System.Guid ApplicationID { get; set; }
        public string Username { get; set; }
        public string UsernameLwd { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }
        public string PasswordFormat { get; set; }
        public string Email { get; set; }
        public string EmailLwd { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool? IsApproved { get; set; }
        public bool? IsLockedOut { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? LastActivity { get; set; }
        public DateTime? LastPasswordChange { get; set; }
        public DateTime? LastLockOut { get; set; }
        public int? FailedPasswordAttempts { get; set; }
        public int? FailedAnswerAttempts { get; set; }
        public DateTime? FailedPasswordWindow { get; set; }
        public DateTime? FailedAnswerWindow { get; set; }
        public DateTime? Joined { get; set; }
        public string Comment { get; set; }
    }
}
