using Callisto.Controls;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
using IT_Jakub.Views.Controls.FlyoutControls;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace IT_Jakub.Classes.DatabaseModels {
    
    /// <summary>
    /// Class represents database Table Session and allows through some metods operations with this table.
    /// </summary>
    class SessionTable {

        /// <summary>
        /// The ms stands for MobileService singleton class
        /// </summary>
        private static MobileService ms = MobileService.getInstance();
        /// <summary>
        /// The msc stands for MobileServiceClient which allows Azure SQL database connections.
        /// </summary>
        private static MobileServiceClient msc = ms.getMobileServiceClient();
        /// <summary>
        /// The table represents connection to database table Session.
        /// </summary>
        private IMobileServiceTable<Session> table = msc.GetTable<Session>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionTable"/> class.
        /// </summary>
        public SessionTable() {
        }


        /// <summary>
        /// Gets all sessions.
        /// </summary>
        /// <returns></returns>
        internal async Task<List<Session>> getAllSessions() {
            List<Session> items = null;
            try {
                items = await table.ToListAsync();
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                return null;
            }
            return items;
        }

        /// <summary>
        /// Creates the session and insert it in database.
        /// </summary>
        /// <param name="s">The Session which needs to be created and inserted in database.</param>
        /// <returns></returns>
        internal async Task<bool> createSession(Session s) {
            try {
                await table.InsertAsync(s);
                return true;
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Removes the particular session from database.
        /// </summary>
        /// <param name="s">The Session which needs to be deleted.</param>
        internal async void removeSession(Session s) {
            try {
                List<Session> items = await table.Where(Item => Item.Id == s.Id).ToListAsync();
                if (items.Count > 0) {
                    try {
                        await table.DeleteAsync(s);
                    } catch (Exception e) {
                        MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                        return;
                    }
                }
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                return;
            }
        }


        /// <summary>
        /// Gets the Session by its name.
        /// </summary>
        /// <param name="sessionName">Name of the session.</param>
        /// <returns></returns>
        internal async Task<Session> getSessionByName(string sessionName) {
            List<Session> items = null;
            try {
                items = await table.Where(Item => Item.Name == sessionName.Trim()).ToListAsync();
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                return null;
            }
            if (items.Count > 0) {
                return (Session)items[0];
            }
            return null;
        }

        /// <summary>
        /// Finds sessions that contains the name string.
        /// </summary>
        /// <param name="name">The string for sessions to contain.</param>
        /// <returns></returns>
        internal async Task<List<Session>> findSessionsByName(string name) {
            List<Session> items = null;
            try {
                items = await table.Where(Item => Item.Name.Contains(name)).ToListAsync();
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                return null;
            }
            if (items.Count > 0) {
                return items;
            }
            return null;
        }
    }
}
