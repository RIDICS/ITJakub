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
    /// <summary>
    /// Class represents table User from database and allows some operations with this table.
    /// </summary>
    class UserTable {

        /// <summary>
        /// The ms stands for MobileService singleton class
        /// </summary>
        private static MobileService ms = MobileService.getInstance();
        /// <summary>
        /// The msc stands for MobileServiceClient which allows Azure SQL database connections.
        /// </summary>
        private static MobileServiceClient msc = ms.getMobileServiceClient();
        /// <summary>
        /// The table represents connection to database table User.
        /// </summary>
        private IMobileServiceTable<User> userTable = msc.GetTable<User>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserTable"/> class.
        /// </summary>
        public UserTable() {
        }

        /// <summary>
        /// Creates the user and insert it in database.
        /// </summary>
        /// <param name="user">The user which is created and inserted in database.</param>
        /// <returns></returns>
        internal async Task<User> createUser(User user) {
            try {
                await userTable.InsertAsync(user);
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return user;
        }

        /// <summary>
        /// Gets all users from database.
        /// </summary>
        /// <returns></returns>
        internal async Task<List<User>> getAllUsers() {
            List<User> items = null;
            try {
                items = await userTable.ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return items;
        }

        /// <summary>
        /// Gets the user by his username.
        /// </summary>
        /// <param name="username">The Users username</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the user by his id.
        /// </summary>
        /// <param name="id">The Users id</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the user by email.
        /// </summary>
        /// <param name="email">The Users email</param>
        /// <returns></returns>
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

        /// <summary>
        /// Deletes the user from database.
        /// </summary>
        /// <param name="u">The User which needs to be deleted.</param>
        /// <returns></returns>
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


        /// <summary>
        /// Updates the user.
        /// </summary>
        /// <param name="u">The User which needs to be updated.</param>
        /// <returns></returns>
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
