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
using System.Text.RegularExpressions;
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
    /// Page for Crossword Application
    /// </summary>
    public sealed partial class CrosswordsApp : Page {

        /// <summary>
        /// The _main grid of whole frame
        /// </summary>
        private static Grid _mainGrid;
        /// <summary>
        /// ScrollViewer Where the crossword GUI should be.
        /// </summary>
        private static ScrollViewer _s;
        /// <summary>
        /// The ss is singleton instance of SignedSession where user is signed in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();
        /// <summary>
        /// The lu is singleton instance of LoggedUser. LoggedUser is user which is currently logged in.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();
        /// <summary>
        /// The UserList
        /// </summary>
        private static ListView staticUserList;
        /// <summary>
        /// if AutoUpdate commands is 
        /// </summary>
        private static bool autoUpdateAllowed = true;
        /// <summary>
        /// The update id is Id of parial solution command which is automatically saved.
        /// </summary>
        private static long updateId;
        /// <summary>
        /// The TextBlock which says whose solution is user watching.
        /// </summary>
        private static TextBlock _userSolution;
        /// <summary>
        /// The grid for Crossword GUI
        /// </summary>
        private static Grid _crosswordGrid;
        /// <summary>
        /// The Update button
        /// </summary>
        private static Button _updateButton;
        /// <summary>
        /// The spectated user
        /// </summary>
        private static User spectatedUser;

        /// <summary>
        /// The current crossword
        /// </summary>
        private static Crossword currentCrossword;

        public CrosswordsApp() {
            this.InitializeComponent();
            _s = null;
            updateId = -1;
            setUsersRights();
            _mainGrid = mainGrid;
            staticUserList = userList;
            _userSolution = userSolution;
            _crosswordGrid = crosswordGrid;
            _updateButton = updateSolution;
        }
        
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }

        /// <summary>
        /// Sets the update id.
        /// </summary>
        /// <param name="commandId">The command id.</param>
        public static void setUpdateId(long commandId) {
            updateId = commandId;
        }

        /// <summary>
        /// Sets the users rights.
        /// </summary>
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

        /// <summary>
        /// Sets the default user rights.
        /// </summary>
        private static void setDefaultUserRights() {
            Controls.BottomAppBar.setUserRights(UserRights.Default);
        }

        /// <summary>
        /// Sets the prefered user rights.
        /// </summary>
        private static void setPreferedUserRights() {
            Controls.BottomAppBar.setUserRights(UserRights.Prefered);
        }

        /// <summary>
        /// Sets the owners rights.
        /// </summary>
        private static void setOwnersRights() {
            Controls.BottomAppBar.setUserRights(UserRights.Owner);
        }

        /// <summary>
        /// Handles the Loaded event of the mainGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private async void mainGrid_Loaded(object sender, RoutedEventArgs e) {
            autoUpdateAllowed = true;
            try {
                CommandTable ct = new CommandTable();
                Command c = await ct.getUsersSolutionCommand(ss.getSessionData(), lu.getUserData());
                if (c != null) {
                    string xml;
                    Regex r = new Regex("^.*Solution" + Regex.Escape("(") + ".+" + Regex.Escape(", "));
                    xml = r.Replace(c.CommandText, "");
                    xml.TrimEnd();
                    r = new Regex(Regex.Escape(")") + "$");
                    xml = r.Replace(xml, "");
                    updateId = c.Id;
                    Views.EducationalApplications.Crosswords.CrosswordsApp.openCrossword(xml, spectatedUser);
                }
            } catch (Exception ex) {
                object o = ex;
            }
            if (updateId == -1) {
                string xml = getFilledCrossword();
                if (xml != null) {
                    updateId = await sendSolutionCommand(CommandBuilder.getCrosswordSolutionCommand(xml));
                } else {
                    updateId = await sendSolutionCommand(CommandBuilder.getCrosswordSolutionCommand("noSolution"));
                }
            }
            startAutoUpdate();
        }

        /// <summary>
        /// Sends the solution command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        private static async Task<long> sendSolutionCommand(string commandText) {
            CommandTable ct = new CommandTable();
            long sentCommandId = await ct.createCrossWordSolutionCommand(ss.getSessionData(), lu.getUserData(), commandText);
            return sentCommandId;
        }

        /// <summary>
        /// Ends the auto update task.
        /// </summary>
        public static void killAutoUpdateTask() {
            autoUpdateAllowed = false;
        }

        /// <summary>
        /// Starts the auto update.
        /// </summary>
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
                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// Sends the update crossword command.
        /// </summary>
        private static async void sendUpdateCrosswordCommand() {
            CommandTable ct = new CommandTable();
            if (getFilledCrossword() != null) {
                await ct.updateCommandById(updateId, ss.getSessionData(), lu.getUserData(), CommandBuilder.getCrosswordSolutionCommand(getFilledCrossword()));
            }
        }

        /// <summary>
        /// Opens the crossword.
        /// </summary>
        /// <param name="XmlString">The XML string.</param>
        /// <param name="user">The user whose is opened crossword.</param>
        internal static void openCrossword(string XmlString, User user = null) {
            if (XmlString == "noSolution") {
                return;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XmlString);
            Crossword c = new Crossword(xmlDoc);
            currentCrossword = c;
            spectatedUser = user;
            _s = c.getGUI();
            _crosswordGrid.Children.Clear();
            _crosswordGrid.Children.Add(_s);

            if (user == null || user.Id == lu.getUserData().Id) {
                _userSolution.Text = "Prohlížíte valstní řešení. ";
                _updateButton.Visibility = Visibility.Collapsed;
            } else {
                _userSolution.Text = "Prohlížíte řešení uživatele: " + user.LastName + " " + user.FirstName + " \"" + user.Nickname + "\"";
                _updateButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Gets the cross word GUI.
        /// </summary>
        /// <returns></returns>
        public static ScrollViewer getCrossWord() {
            return _s;
        }

        /// <summary>
        /// Changes the word.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="start_x">The start_x position of hint</param>
        /// <param name="start_y">The start_y position of hint</param>
        /// <param name="direction">The direction of filled word</param>
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

        /// <summary>
        /// Gets the filled crossword XML.
        /// </summary>
        /// <returns></returns>
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
                    if (col.Children[j].GetType() == typeof(Question)) {
                        var cell = col.Children[j] as Question;
                        string horizontalHint = cell.getHorizontalHint();
                        string verticalHint = cell.getVerticalHint();
                        sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"question\">");
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
                        bool isPuzzle = cell.isPuzzleChar();
                        if (isPuzzle) {
                            sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"char\" puzzle=\"puzzle\">");
                        } else {
                            sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"char\">");
                        }
                        sb.AppendLine(cellText.Trim());
                        sb.AppendLine("</field>");
                    }
                    if (col.Children[j].GetType() == typeof(IT_Jakub.Views.Controls.CrossWord.Hint)) {
                        sb.AppendLine("<hint>");
                        var cell = col.Children[j] as IT_Jakub.Views.Controls.CrossWord.Hint;
                        string cellText = cell.getText();
                        Regex r = new Regex("\\r\\n");
                        string[] hints = r.Split(cellText);
                        for (int k = 0; k < hints.Length; k++) {
                            if (hints.ToString().Trim() != "") {
                                sb.AppendLine("<item>");
                                sb.Append(hints[k].ToString());
                                sb.Append("</item>");
                            }
                        }
                        sb.AppendLine("</hint>");
                    }
                }
            }
            sb.AppendLine("</crossword>");
            return sb.ToString();
        }

        /// <summary>
        /// Gets the empty crossword XML.
        /// </summary>
        /// <returns></returns>
        private static string getEmptyCrossword() {

            StringBuilder sb = new StringBuilder();
            var sv = _s.Content as StackPanel;
            StackPanel col = sv.Children[0] as StackPanel;
            sb.AppendLine("<crossword>");
            
            sb.AppendLine("<size x=\"" + sv.Children.Count + "\" y=\"" + col.Children.Count + "\" />");

            for (int i = 0; i < sv.Children.Count; i++) {
                col = sv.Children[i] as StackPanel;
                for (int j = 0; j < col.Children.Count; j++) {
                    if (col.Children[j].GetType() == typeof(Question)) {
                        var cell = col.Children[j] as Question;
                        string horizontalHint = cell.getHorizontalHint();
                        string verticalHint = cell.getVerticalHint();
                        sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"question\">");
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
                        bool isPuzzle = cell.isPuzzleChar();
                        if (isPuzzle) {
                            sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"char\" puzzle=\"puzzle\">");
                        } else {
                            sb.AppendLine("<field pos=\"" + i + '.' + j + "\" type=\"char\">");
                        }
                        sb.AppendLine("");
                        sb.AppendLine("</field>");
                    }
                    if (col.Children[j].GetType() == typeof(IT_Jakub.Views.Controls.CrossWord.Hint)) {
                        sb.AppendLine("<hint>");
                        var cell = col.Children[j] as IT_Jakub.Views.Controls.CrossWord.Hint;
                        string cellText = cell.getText();
                        Regex r = new Regex("\\r\\n");
                        string[] hints = r.Split(cellText);
                        for (int k = 0; k < hints.Length; k++) {
                            if (hints.ToString().Trim() != "") {
                                sb.AppendLine("<item>");
                                sb.Append(hints[k].ToString());
                                sb.Append("</item>");
                            }
                        }
                        sb.AppendLine("</hint>");
                    }
                }
            }
            sb.AppendLine("</crossword>");
            return sb.ToString();
        }

        /// <summary>
        /// Opens the file from URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
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
                await ss.sendCommand(CommandBuilder.getCorrectSolutionCrosswordCommand(getFilledCrossword()));
                updateId = await sendSolutionCommand(CommandBuilder.getCrosswordSolutionCommand(xmlString));
            } catch (Exception e) {
                object o = e;
                return;
            }
        }

        /// <summary>
        /// Updates the user list.
        /// </summary>
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
                return;
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the userList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs"/> instance containing the event data.</param>
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

        /// <summary>
        /// Handles the Open event of the top app bar.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void topUsersListAppBar_Opened(object sender, object e) {
            updateUserList();
        }


        /// <summary>
        /// Stops sending the update.
        /// </summary>
        internal static void stopSendingUpdate() {
            autoUpdateAllowed = false;
        }

        /// <summary>
        /// Starts sending the updates.
        /// </summary>
        internal static void startSendinAutoUpdate() {
            autoUpdateAllowed = true;
            startAutoUpdate();
        }

        /// <summary>
        /// Sends the final solution.
        /// </summary>
        internal async static void sendFinalSolution() {
            await ss.sendCommand(CommandBuilder.getCrosswordFinalSolutionCommand(getFilledCrossword()));
        }

        /// <summary>
        /// Requests the final solutions from all users signed in the session.
        /// </summary>
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

        /// <summary>
        /// Evaluates the solutions.
        /// </summary>
        internal static void evaluateSolutions() {
            Frame mainFrame = MainPage.getMainFrame();
            mainFrame.Navigate(typeof(EvaluateSolutions));
        }

        internal async static void openFileFromLocalStorage(StorageFile f) {
            try {
                XmlLoadSettings settings = new XmlLoadSettings();
                settings.ElementContentWhiteSpace = false;
                settings.ValidateOnParse = false;

                XmlDocument xmlDoc = await XmlDocument.LoadFromFileAsync(f, settings);
                string xmlString = xmlDoc.GetXml();
                openCrossword(xmlString);
                await ss.sendCommand(CommandBuilder.getOpenCrosswordCommand(getEmptyCrossword()));
                await ss.sendCommand(CommandBuilder.getCorrectSolutionCrosswordCommand(getFilledCrossword()));
                updateId = await sendSolutionCommand(CommandBuilder.getCrosswordSolutionCommand(xmlString));
            } catch (Exception e) {
                object o = e;
                return;
            }
        }

        private async void update_Click(object sender, RoutedEventArgs e) {
            if (spectatedUser != null) {
                CommandTable ct = new CommandTable();
                Command c = await ct.getUsersSolutionCommand(ss.getSessionData(), spectatedUser);
                if (c != null) {
                    string xml;
                    Regex r = new Regex("^.*Solution" + Regex.Escape("(") + ".+" + Regex.Escape(", "));
                    xml = r.Replace(c.CommandText, "");
                    xml.TrimEnd();
                    r = new Regex(Regex.Escape(")") + "$");
                    xml = r.Replace(xml, "");
                    if (spectatedUser.Id != lu.getUserData().Id) {
                        Views.EducationalApplications.Crosswords.CrosswordsApp.stopSendingUpdate();
                    }
                    Views.EducationalApplications.Crosswords.CrosswordsApp.openCrossword(xml, spectatedUser);
                    if (spectatedUser.Id == lu.getUserData().Id) {
                        Views.EducationalApplications.Crosswords.CrosswordsApp.startSendinAutoUpdate();
                    }
                }
            }
        }
    }
}
