using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Exceptions {
    class UserNotFoundException : MyException {
        public UserNotFoundException()
            : base("User was not found in database") {
        }
    }
}
