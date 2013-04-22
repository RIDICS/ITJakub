using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    class SessionUpdate {

        public long Id { get; set; }
        public long SessionId { get; set; }
        public long LastCommand { get; set; }

        public SessionUpdate() {
        }
    }
}
