using IT_Jakub.Classes.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace IT_Jakub.Classes.Models {
    /// <summary>
    /// Representation of row in database table User.
    /// </summary>
    class User {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        /// <value>
        /// The nickname.
        /// </value>
        public string Nickname { get; set; }
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>
        /// The username.
        /// </value>
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>
        /// The date of birth.
        /// </value>
        public string DateOfBirth { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the open id.
        /// </summary>
        /// <value>
        /// The open id.
        /// </value>
        public string OpenId { get; set; }
        /// <summary>
        /// Gets or sets the name of the service for openId.
        /// </summary>
        /// <value>
        /// The name of the service.
        /// </value>
        public string ServiceName { get; set; }
        /// <summary>
        /// Gets or sets the name of the class.
        /// </summary>
        /// <value>
        /// The name of the class.
        /// </value>
        public string ClassName { get; set; }
        /// <summary>
        /// Gets or sets the assumed year of graduation.
        /// </summary>
        /// <value>
        /// The year of graduation.
        /// </value>
        public int YearOfGraduation { get; set; }
        /// <summary>
        /// Gets or sets the name of class if user is a classTeacher.
        /// </summary>
        /// <value>
        /// The class teacher.
        /// </value>
        public string ClassTeacher { get; set; }
        /// <summary>
        /// Gets or sets the users role.
        /// </summary>
        /// <value>
        /// The role.
        /// </value>
        public UserRole Role { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="User"/> class.
        /// </summary>
        public User() {
        }

    }
}
