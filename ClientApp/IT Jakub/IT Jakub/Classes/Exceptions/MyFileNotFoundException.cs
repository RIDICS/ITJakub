using IT_Jakub.Classes.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IT_Jakub.Classes.Exceptions {
    
    class MyFileNotFoundException : MyException {

        private static string myMessage = "Soubor nemohl být nalezen.";

        public MyFileNotFoundException(Exception e) : base(myMessage){
            invoker = e;
            MyDialogs.showDialogOK(myMessage + "\n\n" + e.Message);
        }
    }
}
