using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IT_Jakub.Classes.Exceptions {
    
    class MyFileNotFoundException : MyException {

        public MyFileNotFoundException(Exception e) : base("Soubor nemohl být nalezen."){
            invoker = e;
        }
    }
}
