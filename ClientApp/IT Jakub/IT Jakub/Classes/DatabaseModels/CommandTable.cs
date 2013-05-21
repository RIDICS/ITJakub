using IT_Jakub.Classes.Exceptions;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Networking;
using IT_Jakub.Classes.Utils;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.DatabaseModels {
    class CommandTable {

        private static MobileService ms = MobileService.getInstance();
        private static MobileServiceClient msc = MobileService.getMobileServiceClient();
        private IMobileServiceTable<Command> table = msc.GetTable<Command>();

        internal async Task<long> createCommand(Session s, User u, string commandText) {
            try {
                Command c = new Command {
                    UserId = u.Id,
                    CommandText = commandText,
                    SessionId = s.Id
                };
                await table.InsertAsync(c);
                List<Command> command = await table
                    .Where(Item => Item.CommandText == c.CommandText)
                    .Where(Item => Item.UserId == c.UserId)
                    .Where(Item => Item.SessionId == c.SessionId).ToListAsync();
                long id = command[command.Count - 1].Id;
                return id;
            } catch (Exception e) {
                object o = e;
                return -1;
            }
        }

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
                object o = e;
                return -1;
            }
        }

        internal async Task<List<Command>> getAllSessionCommands(Session s) {
            List<Command> items = null;
            try {
                items = await table.Where(Item => Item.SessionId == s.Id).ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return items;
        }

        internal async Task<List<Command>> getAllCommands() {
            List<Command> items = null;
            try {
                items = await table.ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return items;
        }

        public async Task deleteCommand(Command c) {
            try {
                List<Command> test = await table.Where(Item => Item.Id == c.Id).ToListAsync();
                if (test.Count > 0) {
                    try {
                        await table.DeleteAsync(c);
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

        internal async Task<bool> removeSessionsCommand(Session s) {
            List<Command> items;
            try {
                items = await table.Where(Item => Item.SessionId == s.Id).ToListAsync();
                for (int i = 0; i < items.Count; i++) {
                    deleteCommand(items[i]);
                }
            } catch (Exception e) {
                object o = e;
                return false;
            }
            return true;
        }


        internal async Task<List<Command>> getAllNewSessionCommands(SignedSession s) {
            List<Command> items = null;
            try {
                items = await table.Where(Item => Item.SessionId == s.getSessionData().Id).Where(Item => Item.Id > s.getLatestCommandId()).ToListAsync();
            } catch (Exception e) {
                object o = e;
                return null;
            }
            return items;
        }


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
                object o = e;
                return;
            }
        }

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
                object o = e;
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
                object o = e;
                return;
            }
        }

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
                object o = e;
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
                object o = e;
                return;
            }
        }

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
                object o = e;
                return;
            }
        }

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
                object o = e;
                return;
            }
        }

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

            }
            return null;
        }

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

            }
            return null;
        }


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

            }
            return null;
        }

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

            }
            return null;
        }

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

            }
            return null;
        }
    }
}
