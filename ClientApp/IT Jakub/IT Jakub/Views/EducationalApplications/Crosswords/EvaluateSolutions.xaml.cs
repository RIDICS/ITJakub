using Callisto.Controls;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.CrosswordsApp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IT_Jakub.Views.EducationalApplications.Crosswords {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EvaluateSolutions : Page {

        private static SignedSession ss = SignedSession.getInstance();
        private static LoggedUser lu = LoggedUser.getInstance();
        private static ListView _userList;
        private static ListView _userSolutions;
        private static List<CrossWordSolution> solutions;

        public EvaluateSolutions() {
            this.InitializeComponent();
            _userList = userList;
            _userSolutions = userSolutionsList;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
            updateUserList();
        }

        public static async void updateUserList() {
            CommandTable ct = new CommandTable();
            UserTable ut = new UserTable();

            List<Command> items = await ct.getAllUsersFinalSolutionCommands(ss.getSessionData());

            solutions = new List<CrossWordSolution>();


            Dictionary<long, long> userIds = new Dictionary<long, long>(); ;

            for (int i = 0; i < items.Count; i++) {
                User creator = await ut.getUserById(items[i].UserId);
                Command endSolution = await ct.getEndSolutionCommand(ss.getSessionData());
                CrossWordSolution sol = new CrossWordSolution(items[i], creator, endSolution);
                solutions.Add(sol);
                userIds[sol.userId] = sol.userId;
            }

            

            List<User> userList = new List<User>();

            foreach (KeyValuePair<long,long> item in userIds) {
                userList.Add(await ut.getUserById(item.Key));
            }

            _userList.ItemsSource = userList;
        }

        private void userList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ss.getSessionData().OwnerUserId == lu.getUserData().Id) {
                if (e.AddedItems.Count == 1) {
                    User u = e.AddedItems[0] as User;
                    List<CrossWordSolution> usersSolutions = solutions.FindAll(
                delegate(CrossWordSolution cs) {

                    return cs.userId == u.Id;
                }
            );
                    name.Text = u.LastName + " " + u.FirstName;
                    userSolutionsList.ItemsSource = usersSolutions;
                }
            }

        }

        private void userSolutionsList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ss.getSessionData().OwnerUserId == lu.getUserData().Id) {
                if (e.AddedItems.Count == 1) {
                    CrossWordSolution cs = e.AddedItems[0] as CrossWordSolution;
                    percentage.Text = "Dosáhl výsledku: " + cs.percentage + " %";
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(cs.text);
                    Crossword c = new Crossword(xmlDoc);
                    solutionGrid.Children.Clear();
                    solutionGrid.Children.Add(c.getUI());
                }
            }
        }
    }
}
