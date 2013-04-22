﻿using IT_Jakub.Classes.DatabaseModels;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Networking {
    class MobileService {

        internal static MobileServiceClient mobileServiceClient;
        internal static MobileService instance;

        private MobileService() {
            try {
                mobileServiceClient = new MobileServiceClient(
                    "https://itjakub.azure-mobile.net/",
                    "IKzmwpfkbiryIglFPmMRlsmAqwnLdY61");
            } catch (Exception e) {
                throw new Exception("Srat");
            }
        }

        public static MobileService getInstance() {
                if (instance == null) {
                    instance = new MobileService();
                }
                return instance;
        }

        internal static MobileServiceClient getMobileServiceClient() {
            return mobileServiceClient;
        }
    }
}
