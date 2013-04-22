using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Exceptions {
    class MyException : Exception {
        protected MyException(string message)
            : base(message) {
        }
    }
}
