using IT_Jakub.Classes.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IT_Jakub.Classes.Exceptions {
    
    class ServerErrorException : MyException {

        private static string myMessage = "Vyskytl se problém při kontaktování serveru, ověřte své připojení k internetu a zkuste to znovu.";

        public ServerErrorException(Exception e)
            : base(myMessage) {
            invoker = e;
            // MyDialogs.showDialogOK(myMessage + "\n\n" + e.Message);
        }
    }
}
