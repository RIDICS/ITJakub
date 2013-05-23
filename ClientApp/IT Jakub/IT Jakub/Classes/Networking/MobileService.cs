using IT_Jakub.Classes.DatabaseModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Networking {
    /// <summary>
    /// Singleton which creates an instance of MobileServiceClient which connects to the database.
    /// </summary>
    class MobileService {


        /// <summary>
        /// The address of database host.
        /// </summary>
        private string host = "https://itjakub.azure-mobile.net/";
        /// <summary>
        /// The password for database.
        /// </summary>
        private string passwd = "IKzmwpfkbiryIglFPmMRlsmAqwnLdY61";

        /// <summary>
        /// The mobile service client
        /// </summary>
        internal static MobileServiceClient mobileServiceClient;
        /// <summary>
        /// The instance
        /// </summary>
        internal static MobileService instance;

        /// <summary>
        /// <para>
        /// Prevents a default instance of the <see cref="MobileService"/> class from being created form outside.
        /// </para>
        /// <para>
        /// Also creates a connection with specified database mashine.
        /// </para>
        /// </summary>
        private MobileService() {
            try {
                mobileServiceClient = new MobileServiceClient(
                    host,
                    passwd);
            } catch (Exception e) {
                object o = e;
                return;
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static MobileService getInstance() {
                if (instance == null) {
                    instance = new MobileService();
                }
                return instance;
        }

        /// <summary>
        /// Gets the mobile service client.
        /// </summary>
        /// <returns></returns>
        internal MobileServiceClient getMobileServiceClient() {
            return mobileServiceClient;
        }
    }
}
