using IT_Jakub.Classes.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;

namespace IT_Jakub.Classes.Authentication {

    /// <summary>
    /// Class that offers methods for user authentication.
    /// </summary>
    class Authenticator {

        /// <summary>
        /// Initializes a new instance of the <see cref="Authenticator"/> class.
        /// </summary>
        public Authenticator() {

        }


        /// <summary>
        /// Authenticates the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        internal bool authenticateUser(User user, string password) {
            if (user != null && password != null) {
                if (user.Password == Generator.generateHashForPassword(password)) {
                    return true;
                }
            }
            return false;
        }

    }
}
