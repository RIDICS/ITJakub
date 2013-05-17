using Callisto.Controls;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Enumerations;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.CrosswordsApp;
using IT_Jakub.Classes.Networking;

using IT_Jakub.Views.Controls.CrossWord;
using IT_Jakub.Views.Controls.FlyoutControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
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
    public sealed partial class CrosswordsApp : Page {

        private static Grid _mainGrid;
        private static ScrollViewer _s;
        private static SignedSession ss = SignedSession.getInstance();
        private static LoggedUser lu = LoggedUser.getInstance();
        private static ListView staticUserList;
        private static bool autoUpdateAllowed = true;
        private static long updateId;
        private static TextBlock _userSolution;

        public CrosswordsApp() {
            this.InitializeComponent();
            _s = null;
            updateId = -1;
            setUsersRights();
            _mainGrid = mainGrid;
            staticUserList = userList;
            _userSolution = userSolution;
        }
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }

        public static void setUpdateId(long commandId) {
            updateId = commandId;
        }

        public static void setUsersRights() {

            if (ss.getSessionData().OwnerUserId == lu.getUserData().Id) {
                setOwnersRights();
                return;
            }
            if (ss.getSessionData().PrefferedUserId == lu.getUserData().Id) {
                setPreferedUserRights();
                return;
            }
            setDefaultUserRights();
        }

        private static void setDefaultUserRights() {
            Controls.BottomAppBar.setUserRights(UserRights.Default);
        }

        private static void setPreferedUserRights() {
            Controls.BottomAppBar.setUserRights(UserRights.Prefered);
        }

        private static void setOwnersRights() {
            Controls.BottomAppBar.setUserRights(UserRights.Owner);
        }

        private async void mainGrid_Loaded(object sender, RoutedEventArgs e) {
            autoUpdateAllowed = true;
            if (updateId == -1) {
                string xml = getFilledCrossword();
                if (xml != null) {
                    updateId = await sendSolutionCommand(CommandBuilder.getCrosswordSolutionCommand(xml));
                }
            }
            CommandTable ct = new CommandTable();
            startAutoUpdate();
        }

        private static async Task<long> sendSolutionCommand(string commandText) {
            CommandTable ct = new CommandTable();
            long sentCommandId = await ct.createCrossWordSolutionCommand(ss.getSessionData(), lu.getUserData(), commandText);
            return sentCommandId;
        }

        public static void killAutoUpdateTask() {
            autoUpdateAllowed = false;
        }

        private static async void startAutoUpdate() {
            while (autoUpdateAllowed) {
                try {
                    CommandTable ct = new CommandTable();
                    List<Command> newCommands = await ct.getAllNewSessionCommands(ss);
                    for (int i = 0; i < newCommands.Count; i++) {
                        await newCommands[i].procedeCommand();
                    }
                    sendUpdateCrosswordCommand();
                } catch {
                }
                await Task.Delay(5000);
            }
        }

        private static async void sendUpdateCrosswordCommand() {
            CommandTable ct = new CommandTable();
            if (getFilledCrossword() != null) {
                await ct.updateCommandById(updateId, ss.getSessionData(), lu.getUserData(), CommandBuilder.getCrosswordSolutionCommand(getFilledCrossword()));
            }
        }

        internal static void openCrossword(string XmlString, User user = null) {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XmlString);
            Crossword c = new Crossword(xmlDoc);

            _s = c.getUI();
            _s.Margin = new Thickness(300,300,0,0);
            _mainGrid.Children.Clear();
            _mainGrid.Children.Add(_s);

            if (user == null || user.Id == lu.getUserData().Id) {
                _userSolution.Text = "Prohlížíte valstní řešení. ";
            } else {
                _userSolution.Text = "Prohlížíte řešení uživatele: " + user.LastName + " " + user.FirstName + " \"" + user.Nickname + "\"";
            }
        }

        public static ScrollViewer getCrossWord() {
            return _s;
        }

        public static void changeWord(string word, int start_x, int start_y, Direction direction) {
            word = word.ToUpper();
            var charArray = word.ToCharArray();
            var sv = _s.Content as StackPanel;
            if (direction == Direction.Horizontal) {
                for (int i = 1; i <= charArray.Length; i++) {
                    
                    var col = sv.Children[start_x + i] as StackPanel;
                    if (col != null) {
                        if (col.Children[start_y].GetType() == typeof(IT_Jakub.Views.Controls.CrossWord.Char)) {
                            var cell = col.Children[start_y] as IT_Jakub.Views.Controls.CrossWord.Char;
                            if (cell != null) {
                                cell.changeText(charArray[i - 1]);
                            }
                        }
                    }
                }
            }
            if (direction == Direction.Vertical) {
                for (int i = 1; i <= charArray.Length; i++) {
                    var col = sv.Children[start_x] as StackPanel;
                    if (col != null) {
                        if (col.Children[start_y + i].GetType() == typeof(IT_Jakub.Views.Controls.CrossWord.Char)) {
                            var cell = col.Children[start_y + i] as IT_Jakub.Views.Controls.CrossWord.Char;
                            if (cell != null) {
                                cell.changeText(charArray[i - 1]);
                            }
                        }
                    }
                }
            }
        }

        private static string getFilledCrossword() {
            if (_s == null) {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            var sv = _s.Content as StackPanel;
            StackPanel col = sv.Children[0] as StackPanel;
            sb.AppendLine("<crossword>");

            sb.AppendLine("<size x=\"" + sv.Children.Count + "\" y=\"" + col.Children.Count + "\" />");

            for (int i = 0; i < sv.Children.Count; i++) {
                col = sv.Children[i] as StackPanel;
                for (int j = 0; j < col.Children.Count; j++) {
                    if (col.Children[j].GetType() == typeof(Hint)) {
                        var cell = col.Children[j] as Hint;
                        string horizontalHint = cell.getHorizontalHint();
                        string verticalHint = cell.getVerticalHint();
                        sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"hint\">");
                        if (horizontalHint != "") {
                            sb.AppendLine("<horizontal>" + horizontalHint + "</horizontal>");
                        }
                        if (verticalHint != "") {
                            sb.AppendLine("<vertical>" + verticalHint + "</vertical>");
                        }
                        sb.AppendLine("</field>");
                    }
                    if (col.Children[j].GetType() == typeof(IT_Jakub.Views.Controls.CrossWord.Char)) {
                        var cell = col.Children[j] as IT_Jakub.Views.Controls.CrossWord.Char;
                        string cellText = cell.getText();
                        sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"char\">");
                        sb.AppendLine(cellText.Trim());
                        sb.AppendLine("</field>");
                    }
                }
            }
            sb.AppendLine("</crossword>");
            return sb.ToString();
        }

        private static string getEmptyCrossword() {

            StringBuilder sb = new StringBuilder();
            var sv = _s.Content as StackPanel;
            StackPanel col = sv.Children[0] as StackPanel;
            sb.AppendLine("<crossword>");

            sb.AppendLine("<size x=\"" + sv.Children.Count + "\" y=\"" + col.Children.Count + "\" />");

            for (int i = 0; i < sv.Children.Count; i++) {
                col = sv.Children[i] as StackPanel;
                for (int j = 0; j < col.Children.Count; j++) {
                    if (col.Children[j].GetType() == typeof(Hint)) {
                        var cell = col.Children[j] as Hint;
                        string horizontalHint = cell.getHorizontalHint();
                        string verticalHint = cell.getVerticalHint();
                        sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"hint\">");
                        if (horizontalHint != "") {
                            sb.AppendLine("<horizontal>" + horizontalHint + "</horizontal>");
                        }
                        if (verticalHint != "") {
                            sb.AppendLine("<vertical>" + verticalHint + "</vertical>");
                        }
                        sb.AppendLine("</field>");
                    }
                    if (col.Children[j].GetType() == typeof(IT_Jakub.Views.Controls.CrossWord.Char)) {
                        var cell = col.Children[j] as IT_Jakub.Views.Controls.CrossWord.Char;
                        string cellText = cell.getText();
                        sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"char\">");
                        sb.AppendLine("");
                        sb.AppendLine("</field>");
                    }
                }
            }
            sb.AppendLine("</crossword>");
            return sb.ToString();
        }

        internal async static void openFileFromUri(string uri) {
            Uri u = new Uri(uri, UriKind.Absolute);
            try {
                StorageFile file = await FileDownloader.downloadFileFromUri(u);

                XmlLoadSettings settings = new XmlLoadSettings();
                settings.ElementContentWhiteSpace = false;
                settings.ValidateOnParse = false;

                XmlDocument xmlDoc = await XmlDocument.LoadFromFileAsync(file, settings);
                string xmlString = xmlDoc.GetXml();
                openCrossword(xmlString);
                await ss.sendCommand(CommandBuilder.getOpenCrosswordCommand(getEmptyCrossword()));
                await ss.sendCommand(CommandBuilder.getEndSolutionCrosswordCommand(getFilledCrossword()));
                updateId = await sendSolutionCommand(CommandBuilder.getCrosswordSolutionCommand(xmlString));
            } catch (Exception e) {
                object o = e;
            }
        }

        public static async void updateUserList() {
            SessionUserTable sut = new SessionUserTable();
            UserTable ut = new UserTable();
            try {
                List<SessionUser> items = await sut.getAllUsersInSession(ss.getSessionData());
                List<User> userList = new List<User>();
                for (int i = 0; i < items.Count; i++) {
                    User u = await ut.getUserById(items[i].UserId);
                    userList.Add(u);
                }
                userList.OrderBy(Item => Item.LastName);
                staticUserList.ItemsSource = userList;
            } catch (Exception e) {
                object o = e;
            }
        }
        
        private void userList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (ss.getSessionData().OwnerUserId == lu.getUserData().Id) {
                if (e.AddedItems.Count == 1) {
                    User u = e.AddedItems[0] as User;

                    Flyout flyOut = new Flyout();
                    flyOut.Content = new CrosswordUserListChangedFlyout(u, flyOut);
                    flyOut.PlacementTarget = sender as UIElement;
                    flyOut.Placement = PlacementMode.Top;
                    flyOut.IsOpen = true;
                }
            }
            userList.SelectedValue = null;
        }

        private void topUsersListAppBar_Opened(object sender, object e) {
            updateUserList();
        }


        internal static void stopSendingUpdate() {
            autoUpdateAllowed = false;
        }

        internal static void startSendinAutoUpdate() {
            autoUpdateAllowed = true;
            startAutoUpdate();
        }

        internal async static void sendFinalSolution() {
            await ss.sendCommand(CommandBuilder.getCrosswordFinalSolutionCommand(getFilledCrossword()));
        }

        internal async static void requestFinalSolutions() {
            CommandTable ct = new CommandTable();
            
            List<Command> itemsToDelete = await ct.getAllFinalSolutionMadeByOwner(ss.getSessionData());
            for (int i = 0; i < itemsToDelete.Count; i++) {
                await ct.deleteCommand(itemsToDelete[i]);
            }

            List<Command> items = await ct.getAllSessionSolutionCommands(ss.getSessionData());
            for (int i = 0; i < items.Count; i++) {
                if (items[i].CommandText.Contains("Solution(")) {
                    items[i].CommandText = items[i].CommandText.Replace("Solution(", "SolutionFinal(" + DateTime.Now.ToString() + ", ");
                    await ct.createCommand(ss.getSessionData(), lu.getUserData(), items[i].CommandText);
                }
            }
            List<Command> AllItems = await ct.getAllSessionCommands(ss.getSessionData());
        }

        internal static void evaluateSolutions() {
            Frame mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(EvaluateSolutions));
        }
    }
}
