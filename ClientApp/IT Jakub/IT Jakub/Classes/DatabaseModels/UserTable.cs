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
    class UserTable {
        
        private static MobileService ms = MobileService.getInstance();
        private static MobileServiceClient msc = MobileService.getMobileServiceClient();
        private IMobileServiceTable<User> userTable = msc.GetTable<User>();

        public UserTable() {
        }

        internal async Task<User> createUser(User user) {
            try {
                await userTable.InsertAsync(user);
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return user;
        }

        internal MobileServiceCollectionView<User> getAllUsers() {
            MobileServiceCollectionView<User> items = null;
            try {
                items = userTable.ToCollectionView();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return items;
        }

        internal async Task<User> getUserByUsername(string username) {
            List<User> items = null;
            try {
                items = await userTable.Take(1).Where(userItem => userItem.Username == username).ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            if (items.Count > 0) {
                return items[0];
            }
            return null;
        }

        internal async Task<User> getUserById(long id) {
            List<User> items = null;
            try {
                items = await userTable.Take(1).Where(userItem => userItem.Id == id).ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            if (items.Count > 0) {
                return items[0];
            }
            return null;
        }

        internal async Task<User> getUserByEmail(string email) {
            List<User> items = null;
            try {
                items = await userTable.Take(1).Where(userItem => userItem.Email == email).ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            if (items.Count > 0) {
                return items[0];
            }
            return null;
        }

        internal async Task deleteUser(User u) {
            try {
                List<User> items = await userTable.Where(Item => Item.Id == u.Id).ToListAsync();
                if (items.Count > 0) {
                    try {
                        await userTable.DeleteAsync(u);
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
        

        internal async Task updateUser(User u) {
            try {
                await userTable.UpdateAsync(u);
            } catch (Exception e) {
                object o = e;
                return;
            }
        }
    }
}
