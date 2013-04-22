using IT_Jakub.Classes.Exceptions;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.DatabaseModels {
    class SessionTable {

        private static MobileService ms = MobileService.getInstance();
        private static MobileServiceClient msc = MobileService.getMobileServiceClient();

        public SessionTable() {
        }


        internal MobileServiceCollectionView<Session> getAllSessions() {
            IMobileServiceTable<Session> sessionTable = msc.GetTable<Session>();
            MobileServiceCollectionView<Session> items = sessionTable.ToCollectionView();
            return items;
        }

        internal async Task<bool> createSession(Session s) {
            try {
                IMobileServiceTable<Session> table = msc.GetTable<Session>();
                await table.InsertAsync(s);
                return true;
            } catch (Exception e) {
            }
            return false;
        }

        internal async void removeSession(Session s) {
            try {
                IMobileServiceTable<Session> table = msc.GetTable<Session>();
                await table.DeleteAsync(s);
            } catch (Exception e) {
            }
        }


        internal async Task<Session> getSessionByName(string sessionName) {
            IMobileServiceTable<Session> table = msc.GetTable<Session>();
            List<Session> items;
            try {
                items = await table.Where(Item => Item.Name == sessionName.Trim()).ToListAsync();
            } catch (Exception e) {
                throw new ServerErrorException();
            }
            if (items.Count > 0) {
                return (Session)items[0];
            }
            return null;
        }
    }
}
