using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.DatabaseModels {
    /// <summary>
    /// Class represents SessionUser table in database, this table is for assigning users in particular Session.
    /// </summary>
    class SessionUserTable {

        /// <summary>
        /// The ms stands for MobileService singleton class
        /// </summary>
        private static MobileService ms = MobileService.getInstance();
        /// <summary>
        /// The msc stands for MobileServiceClient which allows Azure SQL database connections.
        /// </summary>
        private static MobileServiceClient msc = ms.getMobileServiceClient();
        /// <summary>
        /// The table represents connection to database table SessionUser.
        /// </summary>
        private IMobileServiceTable<SessionUser> sessionUserTable = msc.GetTable<SessionUser>();

        /// <summary>
        /// Gets all data from this table. That means all Users which are assigned in some Session.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SessionUser>> getAllSessionUsers() {
            List<SessionUser> items = null;
            try {
                items = await sessionUserTable.ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return items;
        }

        /// <summary>
        /// Gets all users in particular session.
        /// </summary>
        /// <param name="s">The Session where users are signed in</param>
        /// <returns></returns>
        public async Task<List<SessionUser>> getAllUsersInSession(Session s) {
            List<SessionUser> items = null;
            try {
                items = await sessionUserTable.Where(Item => Item.SessionId == s.Id).ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return items;
        }

        /// <summary>
        /// Signs out particular user from all sessions.
        /// </summary>
        /// <param name="u">The User which needs to be signed out from all sessions.</param>
        /// <returns></returns>
        public async Task<bool> signOutUserFromAllSessions(User u) {
            List<SessionUser> items = null;
            try {
                items = await sessionUserTable.Where(Item => Item.UserId == u.Id).ToListAsync();
                for (int i = 0; i < items.Count; i++) {
                    deleteSessionUser(items[i]);
                }
            } catch (Exception e) {
                object o = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Login the user in session.
        /// </summary>
        /// <param name="user">The User which needs to be signed in the particular Session</param>
        /// <param name="session">The Session in which User needs to be signed in.</param>
        internal async void loginUserInSession(User user, Session session) {
            SessionUser su = new SessionUser { 
                SessionId = session.Id,
                UserId = user.Id
            };
            try {
                await sessionUserTable.InsertAsync(su);
            } catch (Exception e) {
                object o = e;
                return;
            }
        }

        /// <summary>
        /// Deletes the SessionUser row from database.
        /// </summary>
        /// <param name="su">The SessionUser row in database.</param>
        private async void deleteSessionUser(SessionUser su) {
            try {
                List<SessionUser> items = await sessionUserTable.Where(Item => Item.UserId == su.UserId).ToListAsync();
                if (items.Count > 0) {
                    try {
                        await sessionUserTable.DeleteAsync(su);
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

        /// <summary>
        /// Removes the particular user from particular session.
        /// </summary>
        /// <param name="s">The Session where User need to be signed out.</param>
        /// <param name="u">The User who needs to be signed out.</param>
        /// <returns></returns>
        internal async Task<bool> removeUserFromSession(Session s, User u) {
            List<SessionUser> items;
            try {
                items = await sessionUserTable.Where(Item => Item.SessionId == s.Id).Where(Item => Item.UserId == u.Id).ToListAsync();
                for (int i = 0; i < items.Count; i++) {
                    deleteSessionUser(items[i]);
                }
            } catch (Exception e) {
                object o = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Removes all users from particular Session.
        /// </summary>
        /// <param name="s">The Session in which Users need to be signed out.</param>
        /// <returns></returns>
        internal async Task<bool> removeAllUsersFromSession(Session s) {
            List<SessionUser> items;
            try {
                items = await sessionUserTable.Where(Item => Item.SessionId == s.Id).ToListAsync();
                for (int i = 0; i < items.Count; i++) {
                    deleteSessionUser(items[i]);
                }
            } catch (Exception e) {
                object o = e;
                return false;
            }
            return true;
        }
    }
}
