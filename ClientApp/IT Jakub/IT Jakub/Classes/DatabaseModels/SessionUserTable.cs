using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.DatabaseModels {
    class SessionUserTable {

        private static MobileService ms = MobileService.getInstance();
        private static MobileServiceClient msc = MobileService.getMobileServiceClient();

        public async Task<List<SessionUser>> getAllSessionUsers() {
            IMobileServiceTable<SessionUser> sessionUserTable = msc.GetTable<SessionUser>();
            List<SessionUser> items = await sessionUserTable.ToListAsync();
            return items;
        }

        public async Task<List<SessionUser>> getAllUsersInSession(Session s) {
            IMobileServiceTable<SessionUser> sessionUserTable = msc.GetTable<SessionUser>();
            List<SessionUser> items = await sessionUserTable.Where(Item => Item.SessionId == s.Id).ToListAsync();
            return items;
        }

        public async Task<bool> signOutUserFromAllSessions(User u) {
            IMobileServiceTable<SessionUser> sessionUserTable = msc.GetTable<SessionUser>();
            List<SessionUser> list = await sessionUserTable.Where(Item => Item.UserId == u.Id).ToListAsync();
            for (int i = 0; i < list.Count; i++) {
                deleteSessionUser(list[i]);
            }
            return true;
        }

        internal async void loginUserInSession(User user, Session session) {
            IMobileServiceTable<SessionUser> sessionUserTable = msc.GetTable<SessionUser>();
            SessionUser su = new SessionUser { 
                SessionId = session.Id,
                UserId = user.Id
            };
            await sessionUserTable.InsertAsync(su);
        }

        private async void deleteSessionUser(SessionUser su) {
            IMobileServiceTable<SessionUser> sessionUserTable = msc.GetTable<SessionUser>();
            try {
                await sessionUserTable.DeleteAsync(su);
            } catch (Exception e) {
                return;
            }
        }

        internal async Task<bool> removeUserFromSession(Session s, User u) {
            IMobileServiceTable<SessionUser> sessionUserTable = msc.GetTable<SessionUser>();
            List<SessionUser> list = await sessionUserTable.Where(Item => Item.SessionId == s.Id).Where(Item => Item.UserId == u.Id).ToListAsync();
            for (int i = 0; i < list.Count; i++) {
                deleteSessionUser(list[i]);
            }
            return true;
        }

        internal async Task<bool> removeAllUsersFromSession(Session s) {
            IMobileServiceTable<SessionUser> sessionUserTable = msc.GetTable<SessionUser>();
            List<SessionUser> list = await sessionUserTable.Where(Item => Item.SessionId == s.Id).ToListAsync();
            for (int i = 0; i < list.Count; i++) {
                deleteSessionUser(list[i]);
            }
            return true;
        }
    }
}
