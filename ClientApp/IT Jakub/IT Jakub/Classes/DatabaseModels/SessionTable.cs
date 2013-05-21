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

        private static MobileServiceClient msc = MobileService.getMobileServiceClient();
        private IMobileServiceTable<Session> table = msc.GetTable<Session>();

        public SessionTable() {
        }


        internal async Task<List<Session>> getAllSessions() {
            List<Session> items = null;
            try {
                items = await table.ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return items;
        }

        internal async Task<bool> createSession(Session s) {
            try {
                await table.InsertAsync(s);
                return true;
            } catch (Exception e) {
                object o = e;
                return false;
            }
        }

        internal async void removeSession(Session s) {
            try {
                List<Session> items = await table.Where(Item => Item.Id == s.Id).ToListAsync();
                if (items.Count > 0) {
                    try {
                        await table.DeleteAsync(s);
                    } catch (Exception e) {
                        object o = e;
                        return;
                    }
                }
            } catch (Exception e) {
                object o = e;
                return;
            }
        }


        internal async Task<Session> getSessionByName(string sessionName) {
            List<Session> items = null;
            try {
                items = await table.Where(Item => Item.Name == sessionName.Trim()).ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            if (items.Count > 0) {
                return (Session)items[0];
            }
            return null;
        }
    }
}
