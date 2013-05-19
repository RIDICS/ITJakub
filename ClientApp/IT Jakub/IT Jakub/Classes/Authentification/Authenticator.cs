using IT_Jakub.Classes.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Utils;

namespace IT_Jakub.Classes.Authentification {
    class Authenticator {

        public Authenticator() {

        }

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
