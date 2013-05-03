using IT_Jakub.Classes.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace IT_Jakub.Classes.Models {
    class User {

        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Nickname { get; set; }
        public string Username { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string OpenId { get; set; }
        public string ServiceName { get; set; }
        public string ClassName { get; set; }
        public int YearOfGraduation { get; set; }
        public string ClassTeacher { get; set; }
        public UserRole Role { get; set; }
        
        public User() {
        }

    }
}
