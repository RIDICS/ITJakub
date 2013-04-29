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
    class SessionUserTable {

        private static MobileService ms = MobileService.getInstance();
        private static MobileServiceClient msc = MobileService.getMobileServiceClient();
        private IMobileServiceTable<SessionUser> sessionUserTable = msc.GetTable<SessionUser>();

        public async Task<List<SessionUser>> getAllSessionUsers() {
            List<SessionUser> items;
            try {
                items = await sessionUserTable.ToListAsync();
            } catch (Exception e) {
                throw new ServerErrorException(e);
            }
            return items;
        }

        public async Task<List<SessionUser>> getAllUsersInSession(Session s) {
            List<SessionUser> items;
            try {
                items = await sessionUserTable.Where(Item => Item.SessionId == s.Id).ToListAsync();
            } catch (Exception e) {
                throw new ServerErrorException(e);
            }
            return items;
        }

        public async Task<bool> signOutUserFromAllSessions(User u) {
            List<SessionUser> items = null;
            try {
                items = await sessionUserTable.Where(Item => Item.UserId == u.Id).ToListAsync();
                for (int i = 0; i < items.Count; i++) {
                    deleteSessionUser(items[i]);
                }
            } catch (Exception e) {
                throw new ServerErrorException(e);
            }
            return true;
        }

        internal async void loginUserInSession(User user, Session session) {
            SessionUser su = new SessionUser { 
                SessionId = session.Id,
                UserId = user.Id
            };
            try {
                await sessionUserTable.InsertAsync(su);
            } catch (Exception e) {
                throw new ServerErrorException(e);
            }
        }

        private async void deleteSessionUser(SessionUser su) {
            try {
                List<SessionUser> items = await sessionUserTable.Where(Item => Item.UserId == su.UserId).ToListAsync();
                if (items.Count > 0) {
                    try {
                        await sessionUserTable.DeleteAsync(su);
                    } catch (Exception e) {
                        object o = e;
                    }
                }
            } catch (Exception e) {
                throw new ServerErrorException(e);
            }
        }

        internal async Task<bool> removeUserFromSession(Session s, User u) {
            List<SessionUser> items;
            try {
                items = await sessionUserTable.Where(Item => Item.SessionId == s.Id).Where(Item => Item.UserId == u.Id).ToListAsync();
                for (int i = 0; i < items.Count; i++) {
                    deleteSessionUser(items[i]);
                }
            } catch (Exception e) {
                throw new ServerErrorException(e);
            }
            return true;
        }

        internal async Task<bool> removeAllUsersFromSession(Session s) {
            List<SessionUser> items;
            try {
                items = await sessionUserTable.Where(Item => Item.SessionId == s.Id).ToListAsync();
                for (int i = 0; i < items.Count; i++) {
                    deleteSessionUser(items[i]);
                }
            } catch (Exception e) {
                throw new ServerErrorException(e);
            }
            return true;
        }
    }
}
