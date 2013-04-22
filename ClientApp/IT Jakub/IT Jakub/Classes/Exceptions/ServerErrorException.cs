using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IT_Jakub.Classes.Exceptions {
    
    class ServerErrorException : MyException {

        public ServerErrorException() : base("Server error ocured, check your internet connection"){

        }
    }
}
