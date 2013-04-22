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

        public UserTable() {
        }

        internal async Task<User> createUser(User user) {
            IMobileServiceTable<User> userTable = msc.GetTable<User>();
            try {
                await userTable.InsertAsync(user);
            } catch (Exception e) {
                throw new ServerErrorException();
            }
            return user;
        }

        internal MobileServiceCollectionView<User> getAllUsers() {
            IMobileServiceTable<User> userTable = msc.GetTable<User>();
            MobileServiceCollectionView<User> items;
            try {
                items = userTable.ToCollectionView();
            } catch (Exception e) {
                throw new ServerErrorException();
            }
            return items;
        }

        internal async Task<User> getUserByUsername(string username) {
            IMobileServiceTable<User> userTable = msc.GetTable<User>();
            List<User> items;
            try {
                items = await userTable.Where(userItem => userItem.Username == username).ToListAsync();
            } catch (Exception e) {
                throw new ServerErrorException();
            }
            if (items.Count > 0) {
                return (User)items[0];
            }
            return null;
        }

        internal async Task<User> getUserByEmail(string email) {
            IMobileServiceTable<User> userTable = msc.GetTable<User>();
            List<User> items;
            try {
                items = await userTable.Where(userItem => userItem.Email == email).ToListAsync();
            } catch (Exception e) {
                throw new ServerErrorException();
            }
            if (items.Count > 0) {
                return (User)items[0];
            }
            return null;
        }

        internal async Task<User> deleteUser(User u) {
            IMobileServiceTable<User> userTable = msc.GetTable<User>();
            await userTable.DeleteAsync(u);
            return null;
        }

        internal async Task updateUser(User u) {
            IMobileServiceTable<User> userTable = msc.GetTable<User>();
            await userTable.UpdateAsync(u);
        }
    }
}
