using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models {
    /// <summary>
    /// Representation of row in database table Session.
    /// </summary>
    class Session {

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        public string Password { get; set; }
        /// <summary>
        /// Gets or sets the session update id.
        /// </summary>
        /// <value>
        /// The session update id.
        /// </value>
        public long SessionUpdateId { get; set; }
        /// <summary>
        /// Gets or sets the preffered user id.
        /// </summary>
        /// <value>
        /// The preffered user id.
        /// </value>
        public long PrefferedUserId { get; set; }
        /// <summary>
        /// Gets or sets the owner user id.
        /// </summary>
        /// <value>
        /// The owner user id.
        /// </value>
        public long OwnerUserId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        public Session() {
        }

    }
}
