using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    class Session {
        
        public long Id { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public long SessionUpdateId { get; set; }
        public long PrefferedUserId { get; set; }
        public long OwnerUserId { get; set; }

        public Session() {
        }

    }
}
