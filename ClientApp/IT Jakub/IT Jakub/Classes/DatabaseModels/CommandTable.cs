﻿using IT_Jakub.Classes.Models;
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

        internal async Task<long> createCommand(Session s, User u, string commandText) {
            try {
                IMobileServiceTable<Command> table = msc.GetTable<Command>();
                Command c = new Command {
                    UserId = u.Id,
                    CommandText = commandText,
                    SessionId = s.Id
                };
                await table.InsertAsync(c);
                List<Command> command = await table.Take(1)
                    .Where(Item => Item.CommandText == c.CommandText)
                    .Where(Item => Item.UserId == c.UserId)
                    .Where(Item => Item.SessionId == c.SessionId).ToListAsync();
                long id = command[command.Count-1].Id;
                return id;
            } catch (Exception e) {
                // MyDialogs.showDialogOK(e.Message);
            }
            return -1;
        }

        internal async Task<List<Command>> getAllSessionCommands(Session s) {
            IMobileServiceTable<Command> table = msc.GetTable<Command>();
            List<Command> items = await table.Where(Item => Item.SessionId == s.Id).ToListAsync();
            return items;
        }

        internal async Task<List<Command>> getAllCommands() {
            IMobileServiceTable<Command> table = msc.GetTable<Command>();
            List<Command> items = await table.ToListAsync();
            return items;
        }


        private void deleteCommand(Command c) {
            IMobileServiceTable<Command> sessionUserTable = msc.GetTable<Command>();
            try {
                sessionUserTable.DeleteAsync(c);
            } catch (Exception e) {
            }
        }

        internal async Task<bool> removeSessionsCommand(Session s) {
            IMobileServiceTable<Command> sessionUserTable = msc.GetTable<Command>();
            List<Command> list = await sessionUserTable.Where(Item => Item.SessionId == s.Id).ToListAsync();
            for (int i = 0; i < list.Count; i++) {
                deleteCommand(list[i]);
            }
            return true;
        }


        internal async Task<List<Command>> getAllNewSessionCommands(SignedSession s) {
            IMobileServiceTable<Command> table = msc.GetTable<Command>();
            List<Command> items = await table.Where(Item => Item.SessionId == s.getSessionData().Id).Where(Item => Item.Id > s.getLatestCommandId()).ToListAsync();
            return items;
        }


        internal async void deletePrevMoveCommands(long latestCommandId, Session s) {
            IMobileServiceTable<Command> table = msc.GetTable<Command>();
            try {
                List<Command> items = await table.Take(150).Where(Item => Item.SessionId == s.Id).Where(Item => Item.Id < latestCommandId).Where(Item => Item.CommandText.Contains("Move(")).ToListAsync();
                if (items.Count > 0) {
                    LinkedList<Command> l = new LinkedList<Command>(items);
                    for (LinkedListNode<Command> ln = l.First; ln != l.Last.Next; ln = ln.Next) {
                        this.deleteCommand(ln.Value);
                    }
                }
            } catch (Exception e) {
            }
        }
    }
}
