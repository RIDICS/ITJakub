using Callisto.Controls;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
using IT_Jakub.Classes.Utils;
using IT_Jakub.Views.Controls.FlyoutControls;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;

namespace IT_Jakub.Classes.DatabaseModels {

    /// <summary>
    ///   <para>
    /// Class represents table "Command" in SQL database.
    ///   </para>
    ///   <para>
    /// Methods in this class offer operations in this table.
    ///   </para>
    /// </summary>
    class CommandTable {


        /// <summary>
        /// The ms stands for MobileService singleton class
        /// </summary>
        private static MobileService ms = MobileService.getInstance();
        /// <summary>
        /// The msc stands for MobileServiceClient which allows Azure SQL database connections.
        /// </summary>
        private static MobileServiceClient msc = ms.getMobileServiceClient();
        /// <summary>
        /// The table represents connection to database table Command.
        /// </summary>
        private IMobileServiceTable<Command> table = msc.GetTable<Command>();

        /// <summary>
        /// Creates the command and insert it in database.
        /// </summary>
        /// <param name="s">The <see cref="Session"/> for command, each command is assigned to Session.</param>
        /// <param name="u">The <see cref="User"/> who created this command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>Task</returns>
        internal async Task<long> createCommand(Session s, User u, string commandText) {
            try {
                Command c = new Command {
                    UserId = u.Id,
                    CommandText = commandText,
                    SessionId = s.Id
                };
                await table.InsertAsync(c);
                int length = commandText.Length;
                if (length > 45) { length = 45; }
                string substr = commandText.Substring(0, length);
                List<Command> command = await table
                    .Where(Item => Item.CommandText.StartsWith(substr))
                    .Where(Item => Item.UserId == c.UserId)
                    .Where(Item => Item.SessionId == c.SessionId).ToListAsync();
                long id = command[command.Count - 1].Id;
                return id;
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return -1;
            }
        }

        /// <summary>
        /// Creates the cross word solution command.
        /// </summary>
        /// <param name="s">The Session for which is command assigned</param>
        /// <param name="u">The User who created this command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns>Task</returns>
        internal async Task<long> createCrossWordSolutionCommand(Session s, User u, string commandText) {
            try {
                Command c = new Command {
                    UserId = u.Id,
                    CommandText = commandText,
                    SessionId = s.Id
                };
                await table.InsertAsync(c);
                List<Command> command = await table
                    .Where(Item => Item.CommandText.Contains("Solution(" + c.UserId + ", "))
                    .Where(Item => Item.UserId == c.UserId)
                    .Where(Item => Item.SessionId == c.SessionId).ToListAsync();
                long id = command[command.Count - 1].Id;
                return id;
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return -1;
            }
        }

        /// <summary>
        /// Gets all session commands.
        /// </summary>
        /// <param name="s">The Session for which is Commands requested.</param>
        /// <returns>Task</returns>
        internal async Task<List<Command>> getAllSessionCommands(Session s) {
            List<Command> items = null;
            try {
                items = await table.Where(Item => Item.SessionId == s.Id).ToListAsync();
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return null;
            }
            return items;
        }

        /// <summary>
        /// Gets all commands.
        /// </summary>
        /// <returns>Task</returns>
        internal async Task<List<Command>> getAllCommands() {
            List<Command> items = null;
            try {
                items = await table.ToListAsync();
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return null;
            }
            return items;
        }

        /// <summary>
        /// Deletes the command.
        /// </summary>
        /// <param name="c">The Command to delete</param>
        /// <returns>Task</returns>
        public async Task deleteCommand(Command c) {
            try {
                List<Command> test = await table.Where(Item => Item.Id == c.Id).ToListAsync();
                if (test.Count > 0) {
                    try {
                        await table.DeleteAsync(c);
                    } catch (Exception e) {
                        Flyout f = new Flyout();
                        f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                        f.PlacementTarget = MainPage.getMainFrame();
                        f.Placement = PlacementMode.Top;
                        f.IsOpen = true;
                        return;
                    }
                }
            } catch (Exception e) {
                object o = e;
                return;
            }
        }

        /// <summary>
        /// Removes all sessions command.
        /// </summary>
        /// <param name="s">The Session where commands need to be deleted.</param>
        /// <returns>Task</returns>
        internal async Task<bool> removeSessionsCommand(Session s) {
            List<Command> items;
            try {
                items = await table.Where(Item => Item.SessionId == s.Id).ToListAsync();
                for (int i = 0; i < items.Count; i++) {
                    deleteCommand(items[i]);
                }
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return false;
            }
            return true;
        }


        /// <summary>
        /// Gets all new session commands. Which is newer then latest executed command in SignedSession singleton class.
        /// </summary>
        /// <param name="s">The SignedSession, where is User currently logged in.</param>
        /// <returns>Task</returns>
        internal async Task<List<Command>> getAllNewSessionCommands(SignedSession s) {
            List<Command> items = null;
            try {
                items = await table.Where(Item => Item.SessionId == s.getSessionData().Id).Where(Item => Item.Id > s.getLatestCommandId()).ToListAsync();
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return null;
            }
            return items;
        }


        /// <summary>
        /// Deletes the previous pointer move commands for Syncronized Reading App.
        /// </summary>
        /// <param name="s">The Session where move commands need to be deleted.</param>
        /// <returns></returns>
        internal async Task deletePrevMoveCommands(Session s) {
            List<Command> items = null;
            try {
                items = await table.Take(150).Where(Item => Item.SessionId == s.Id).Where(Item => Item.CommandText.Contains("Move(")).ToListAsync();
                if (items.Count > 0) {
                    for (int i = 0; i < items.Count-1; i++) {
                        await deleteCommand(items[i]);
                    }
                }
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return;
            }
        }


        /// <summary>
        /// Removes the user login logout commands in specified Session for particular User.
        /// </summary>
        /// <param name="s">The Session where commands need to be deleted.</param>
        /// <param name="u">The User who is logged in or logged out in particular Session.</param>
        /// <returns></returns>
        internal async Task removeUserLoginLogoutCommands(Session s, User u) {
            List<Command> loginItems;
            try {
                loginItems = await table.Where(Item => Item.SessionId == s.Id).Where(Item => Item.CommandText.Contains("Login(" + u.Id + ")")).ToListAsync();
                if (loginItems.Count > 0) {
                    for (int i = 0; i < loginItems.Count; i++) {
                        deleteCommand(loginItems[i]);
                    }
                }
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return;
            }

            List<Command> logoutItems;
            try {
                logoutItems = await table.Where(Item => Item.SessionId == s.Id).Where(Item => Item.CommandText.Contains("Logout(" + u.Id + ")")).ToListAsync();

                if (logoutItems.Count > 0) {
                    for (int i = 0; i < logoutItems.Count; i++) {
                        deleteCommand(logoutItems[i]);
                    }
                }
            } catch (Exception e) {
                Flyout f = new Flyout();
                f.Content = new ErrorFlyout("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n" + e.Message, f);
                f.PlacementTarget = MainPage.getMainFrame();
                f.Placement = PlacementMode.Top;
                f.IsOpen = true;
                return;
            }
        }

        /// <summary>
        /// Deletes the previous promote demote commands.
        /// </summary>
        /// <param name="s">The Session where promote and demote commands need to be deleted.</param>
        /// <returns></returns>
        internal async Task deletePrevPromoteDemoteCommands(Session s) {
            List<Command> promoteItems;
            try {
                promoteItems = await table.Where(Item => Item.SessionId == s.Id).Where(Item => Item.CommandText.Contains("Promote(")).ToListAsync();

                if (promoteItems.Count > 1) {
                    for (int i = 0; i < promoteItems.Count - 1; i++) {
                        deleteCommand(promoteItems[i]);
                    }
                }
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                return;
            }

            List<Command> demoteItems;
            try {
                demoteItems = await table.Where(Item => Item.SessionId == s.Id).Where(Item => Item.CommandText.Contains("Demote(")).ToListAsync();
                if (demoteItems.Count > 1) {
                    for (int i = 0; i < demoteItems.Count - 1; i++) {
                        deleteCommand(demoteItems[i]);
                    }
                }
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                return;
            }
        }

        /// <summary>
        /// Removes the old open file commands.
        /// </summary>
        /// <param name="s">The Session where commands need to be deleted.</param>
        /// <returns></returns>
        internal async Task removeOldOpenCommands(Session s) {
            List<Command> items;
            try {
                items = await table.Where(Item => Item.SessionId == s.Id).Where(Item => Item.CommandText.Contains("Open(")).ToListAsync();
                if (items.Count > 1) {
                    for (int i = 0; i < items.Count - 1; i++) {
                        deleteCommand(items[i]);
                    }
                    long idsToDelete = items[items.Count - 1].Id;
                    items = await table.Where(Item => Item.SessionId == s.Id).Where(Item => Item.Id < idsToDelete).ToListAsync();
                    if (items.Count > 0) {
                        for (int i = 1; i < items.Count; i++) {
                            deleteCommand(items[i]);
                        }
                    }
                }
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                return;
            }
        }

        /// <summary>
        /// Updates the command with specified Command.Id.
        /// </summary>
        /// <param name="updateId">The Id of Command</param>
        /// <param name="s">The Session for whis command.</param>
        /// <param name="u">The User who created this command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        internal async Task updateCommandById(long updateId, Session s, User u, string commandText) {
            try {
                Command c = new Command {
                    Id = updateId,
                    UserId = u.Id,
                    CommandText = commandText,
                    SessionId = s.Id
                };
                await table.UpdateAsync(c);
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
                return;
            }
        }

        /// <summary>
        /// Gets the users solution command for Crosswords Application.
        /// </summary>
        /// <param name="s">The Session where is this commands assigned.</param>
        /// <param name="u">The User whose is this solution.</param>
        /// <returns></returns>
        internal async Task<Command> getUsersSolutionCommand(Session s, User u) {
            try {
                List<Command> items = await table
                    .Where(Item => Item.SessionId == s.Id)
                    .Where(Item => Item.UserId == u.Id)
                    .Where(Item => Item.CommandText.Contains("Solution(" + u.Id))
                    .ToListAsync();
                if (items.Count > 0) {
                    return items[items.Count - 1];
                }
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
            }
            return null;
        }

        /// <summary>
        /// Gets all users final solution commands for Crosswords App.
        /// </summary>
        /// <param name="s">The Session where is this commands assigned.</param>
        /// <returns></returns>
        internal async Task<List<Command>> getAllUsersFinalSolutionCommands(Session s) {
            try {
                List<Command> items = await table
                    .Where(Item => Item.SessionId == s.Id)
                    .Where(Item => Item.CommandText.Contains("SolutionFinal("))
                    .ToListAsync();
                if (items.Count > 0) {
                    return items;
                }
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
            }
            return null;
        }


        /// <summary>
        /// Gets all final solution made by Session Owner.
        /// </summary>
        /// <param name="s">The Session for which is this commands assigned.</param>
        /// <returns></returns>
        internal async Task<List<Command>> getAllFinalSolutionMadeByOwner(Session s) {
            try {
                List<Command> items = await table
                    .Where(Item => Item.SessionId == s.Id)
                    .Where(Item => Item.UserId == s.OwnerUserId)
                    .Where(Item => Item.CommandText.Contains("SolutionFinal("))
                    .ToListAsync();
                if (items.Count > 0) {
                    return items;
                }
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
            }
            return null;
        }

        /// <summary>
        /// Gets all session solution commands made by all users.
        /// </summary>
        /// <param name="s">The Session where is commands assigned.</param>
        /// <returns></returns>
        internal async Task<List<Command>> getAllSessionSolutionCommands(Session s) {
            try {
                List<Command> items = await table
                    .Where(Item => Item.SessionId == s.Id)
                    .Where(Item => Item.CommandText.Contains("Solution("))
                    .ToListAsync();
                if (items.Count > 0) {
                    return items;
                }
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
            }
            return null;
        }

        /// <summary>
        /// Gets the end solution command. This is original and right solution of Crossword.
        /// </summary>
        /// <param name="s">The Session where is command assigned.</param>
        /// <returns></returns>
        internal async Task<Command> getEndSolutionCommand(Session s) {
            try {
                List<Command> items = await table
                    .Where(Item => Item.SessionId == s.Id)
                    .Where(Item => Item.UserId == s.OwnerUserId)
                    .Where(Item => Item.CommandText.Contains("SolutionEnd("))
                    .ToListAsync();
                if (items.Count > 0) {
                    return items[items.Count - 1];
                }
            } catch (Exception e) {
                MainPage.showError("Chyba v komunikaci se serverem !", "Nepodařilo se kontaktovat server.\r\nZkontrolujte prosím připojení k internetu a akci opakujte.\r\n", e.Message);
            }
            return null;
        }
    }
}
