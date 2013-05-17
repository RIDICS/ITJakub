using IT_Jakub.Classes.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Exceptions {
    class UserNotFoundException : MyException {

        private static string myMessage = "Uživatel nebyl nalezen v databázi.";

        public UserNotFoundException(Exception e)
            : base(myMessage) {
            invoker = e;
            MyDialogs.showDialogOK(myMessage + "\n\n" + e.Message);
        }
    }
}
