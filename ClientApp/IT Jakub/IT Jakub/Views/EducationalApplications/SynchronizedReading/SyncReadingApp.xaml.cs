using Callisto.Controls;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Enumerations;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.Commands;
using IT_Jakub.Classes.Models.Utils;
using IT_Jakub.Classes.Networking;
using IT_Jakub.Classes.Utils;
using IT_Jakub.Views.Controls.FlyoutControls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using WinRTXamlToolkit.Controls.Extensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace IT_Jakub.Views.EducationalApplications.SynchronizedReading {
    /// <summary>
    /// Page used to show synchronized reading application.
    /// </summary>
    /// 

    public sealed partial class SyncReadingApp : Page {

        /// <summary>
        /// The text box with readed document
        /// </summary>
        static RichEditBox textBox;
        /// <summary>
        /// The ss is singleton instance of SignedSession where user is signed in.
        /// </summary>
        private static SignedSession ss = SignedSession.getInstance();
        /// <summary>
        /// if should application autoupdate commands while dragging a pointer
        /// </summary>
        private static bool autoUpdate = true;
        /// <summary>
        /// The lu is singleton instance of LoggedUser. LoggedUser is user which is currently logged in.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();

        /// <summary>
        /// The old range of text which was highlighted by pointer before it moved
        /// </summary>
        private static ITextRange oldRange;
        /// <summary>
        /// The old color of text highlight which was highlighted by pointer before it moved
        /// </summary>
        private static Color oldColor;
        /// <summary>
        /// The horizontal offset of scrollbar
        /// </summary>
        private static double horizontalOffset;
        /// <summary>
        /// The vertical offset of scrollbar
        /// </summary>
        private static double verticalOffset;
        /// <summary>
        /// The scroll viewer which is assigned to textblock of readed text
        /// </summary>
        private ScrollViewer scrollViewer;
        /// <summary>
        /// The text pointer
        /// </summary>
        private static Image staticPointer;
        /// <summary>
        /// The range compare serves for comparing if text pointer moved, so it is old position of pointer.
        /// </summary>
        private ITextRange rangeCompare;
        /// <summary>
        /// The user list
        /// </summary>
        private static ListView staticUserList;
        /// <summary>
        /// If is autoupdate allowed
        /// </summary>
        private static bool autoUpdateAllowed;
        /// <summary>
        /// The list of commands
        /// </summary>
        private static LinkedList<string> commandList = new LinkedList<string>();
        /// <summary>
        /// The command list sender task
        /// </summary>
        private static Task commandListSender;
        /// <summary>
        /// The sending move commands allowed
        /// </summary>
        private static bool sendingMoveCommandsAllowed = false;
        /// <summary>
        /// The fluct Y serves for eliminating fluctation of pointer position while dragged
        /// </summary>
        private double fluctY = 0;
        /// <summary>
        /// The Canvas
        /// </summary>
        private static Canvas _canvas;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncReadingApp"/> class.
        /// </summary>
        public SyncReadingApp() {
            this.InitializeComponent();
            textBox = textRichEditBox;
            staticPointer = pointer;
            oldRange = textBox.Document.GetRange(0, 0);
            scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();
            rangeCompare = textBox.Document.GetRange(0, 0);
            staticUserList = userList;
            _canvas = canvas;
            setUsersRights();
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
            staticPointer.ManipulationMode = ManipulationModes.None;
        }

        /// <summary>
        /// Sets the prefered user rights.
        /// </summary>
        private static void setPreferedUserRights() {
            Controls.BottomAppBar.setUserRights(UserRights.Prefered);
            staticPointer.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        }

        /// <summary>
        /// Sets the owners rights.
        /// </summary>
        private static void setOwnersRights() {
            Controls.BottomAppBar.setUserRights(UserRights.Owner);
            staticPointer.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {

        }

        /// <summary>
        /// Highligts the text which user selected.
        /// </summary>
        public static async void highligtTextButton_Click() {
            ITextCharacterFormat charFormatting = textBox.Document.Selection.CharacterFormat;
            textBox.IsReadOnly = false;
            Color c = Colors.Yellow;
            charFormatting.BackgroundColor = c;
            textBox.Document.Selection.CharacterFormat = charFormatting;
            textBox.IsReadOnly = true;
            int startPosition = textBox.Document.Selection.StartPosition;
            int endPosition = textBox.Document.Selection.EndPosition;
            await ss.sendCommand(CommandBuilder.getHighlightCommand("ARGB(" + c.A + ", " + c.R + ", " + c.G + ", " + c.B + ")", startPosition, endPosition));
        }

        /// <summary>
        /// Gets the text rich edit box with document.
        /// </summary>
        /// <returns></returns>
        public static RichEditBox getTextRichEditBox() {
            return textBox;
        }

        /// <summary>
        /// Opens the file from URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        internal static async void openFile(string uri) {
            RtfFileOpener opener = new RtfFileOpener();

            string source = await opener.openDocumentFromUri(uri.Trim());

            if (source != null) {
                await ss.sendCommand(CommandBuilder.getOpenBookCommand(source));
                CommandTable ct = new CommandTable();
                await ct.removeOldOpenCommands(ss.getSessionData());
            }
        }

        /// <summary>
        /// Handles the Loaded event of the mainGrid control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void mainGrid_Loaded(object sender, RoutedEventArgs e) {
            this.sessionName.Text = ss.getSessionData().Name;
            autoUpdateAllowed = true;
            autoUpdate = true;
            startAutoUpdate();
        }

        /// <summary>
        /// Ends the auto update task.
        /// </summary>
        public static void killAutoUpdateTask() {
            autoUpdateAllowed = false;
            stopSendingMoveCommands();
        }

        /// <summary>
        /// Starts the auto update of commands.
        /// </summary>
        private async void startAutoUpdate() {
            while (autoUpdateAllowed) {
                if (autoUpdate) {
                    try {
                        CommandTable ct = new CommandTable();
                        List<Command> newCommands = await ct.getAllNewSessionCommands(ss);
                        for (int i = 0; i < newCommands.Count; i++) {
                            await newCommands[i].procedeCommand();
                        }
                    } catch {
                    }
                }
                await Task.Delay(250);
            }
        }

        /// <summary>
        /// Determines whether [is auto update enabled].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if [is auto update enabled]; otherwise, <c>false</c>.
        /// </returns>
        public static bool isAutoUpdateEnabled() {
            return autoUpdate;
        }

        /// <summary>
        /// Handles the ManipulationDelta event of the text pointer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationDeltaRoutedEventArgs"/> instance containing the event data.</param>
        private void pointer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();

            verticalOffset = scrollViewer.VerticalOffset;
            horizontalOffset = scrollViewer.HorizontalOffset;

            fluctY += e.Delta.Translation.Y;
            double top = (double.Parse(pointer.GetValue(Canvas.TopProperty).ToString()) + e.Delta.Translation.Y);
            
            if (fluctY > 25 || fluctY < -25) {
                pointer.SetValue(Canvas.TopProperty, top);                
            }
            double left = (double.Parse(pointer.GetValue(Canvas.LeftProperty).ToString()) + e.Delta.Translation.X);

            pointer.SetValue(Canvas.LeftProperty, left);

            unhighlightPointerChar();

            double leftPoint = (double.Parse(pointer.GetValue(Canvas.LeftProperty).ToString()) + (pointer.ActualWidth / 2.7));
            double topPoint = ((double.Parse(pointer.GetValue(Canvas.TopProperty).ToString()) + canvas.Margin.Top)) - 10;

            ITextRange range = textRichEditBox.Document.GetRangeFromPoint(new Point(leftPoint, topPoint), PointOptions.None);
            range.Expand(TextRangeUnit.Character);
            highlightPointerChar(range);

            setPointerScrollbarOffset();
            if (rangeCompare.StartPosition != range.StartPosition) {
                sendPointerMoveCommand(range.StartPosition);
                rangeCompare = range;
            }
        }
        
        /// <summary>
        /// Highlights the pointer char.
        /// </summary>
        /// <param name="range">The range.</param>
        private static void highlightPointerChar(ITextRange range) {
            textBox.Document.Selection.SetRange(range.StartPosition, range.EndPosition);

            ITextCharacterFormat charFormatting = textBox.Document.Selection.CharacterFormat;
            oldColor = charFormatting.BackgroundColor;

            textBox.IsReadOnly = false;
            Color c = Colors.Aqua;
            charFormatting.BackgroundColor = c;
            textBox.Document.Selection.CharacterFormat = charFormatting;
            textBox.IsReadOnly = true;
            oldRange = range;
        }

        /// <summary>
        /// Unhighlights the pointer char.
        /// </summary>
        private static void unhighlightPointerChar() {
            textBox.Document.Selection.SetRange(oldRange.StartPosition, oldRange.EndPosition);
            ITextCharacterFormat charFormatting = textBox.Document.Selection.CharacterFormat;

            textBox.IsReadOnly = false;
            Color c = oldColor;
            charFormatting.BackgroundColor = c;
            textBox.Document.Selection.CharacterFormat = charFormatting;
            textBox.IsReadOnly = true;
        }

        /// <summary>
        /// Sets the pointer scrollbar offset.
        /// </summary>
        private void setPointerScrollbarOffset() {
            scrollViewer.ScrollToVerticalOffset(verticalOffset);
            scrollViewer.ScrollToHorizontalOffset(horizontalOffset);
        }

        /// <summary>
        /// Moves the the pointer to char index in document.
        /// </summary>
        /// <param name="index">The index.</param>
        public static void movePointerToCharIndex(int index) {
            string documentText;
            textBox.Document.GetText(TextGetOptions.None, out documentText);
            int docLenght = documentText.Length;

            ITextRange range = textBox.Document.GetRange(docLenght, docLenght);
            range.Expand(TextRangeUnit.Character);
            Point docEndPoint;
            range.GetPoint(HorizontalCharacterAlignment.Center, VerticalCharacterAlignment.Baseline, PointOptions.None, out docEndPoint);

            range = textBox.Document.GetRange(0, 0);
            range.Expand(TextRangeUnit.Character);
            Point docStartPoint;
            range.GetPoint(HorizontalCharacterAlignment.Center, VerticalCharacterAlignment.Baseline, PointOptions.None, out docStartPoint);

            range = textBox.Document.GetRange(index, index);
            range.Expand(TextRangeUnit.Word);
            Point focusCharacterPoint;

            double docHeight = docEndPoint.Y - docStartPoint.Y;
            double charOffsetPercent = (focusCharacterPoint.Y - docStartPoint.Y) / docHeight;

            ScrollViewer scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();
            verticalOffset = charOffsetPercent * scrollViewer.ScrollableHeight;
            scrollViewer.ScrollToVerticalOffset(verticalOffset);

            range = textBox.Document.GetRange(index, index);
            range.Expand(TextRangeUnit.Character);

            unhighlightPointerChar();
            highlightPointerChar(range);

            range.GetPoint(HorizontalCharacterAlignment.Left, VerticalCharacterAlignment.Baseline, PointOptions.None, out focusCharacterPoint);
            staticPointer.SetValue(Canvas.LeftProperty, focusCharacterPoint.X - (staticPointer.ActualWidth / 2.5));
            staticPointer.SetValue(Canvas.TopProperty, focusCharacterPoint.Y - _canvas.Margin.Top);
        }

        /// <summary>
        /// Handles the ManipulationStarting event of the pointer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationStartingRoutedEventArgs"/> instance containing the event data.</param>
        private void pointer_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e) {
            autoUpdate = false;
            fluctY = 0;
            startSendingMoveCommands();
        }

        /// <summary>
        /// Handles the ManipulationCompleted event of the pointer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ManipulationCompletedRoutedEventArgs"/> instance containing the event data.</param>
        private void pointer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            autoUpdate = true;
            stopSendingMoveCommands();
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
        /// Handles the Click event of the updateUserListButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void updateUserListButton_Click(object sender, RoutedEventArgs e) {
            updateUserList();
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
                    flyOut.Content = new PromoteDemoteFlyout(u, flyOut);
                    flyOut.PlacementTarget = sender as UIElement;
                    flyOut.Placement = PlacementMode.Top;
                    flyOut.IsOpen = true;
                }
            }
            userList.SelectedValue = null;
        }

        /// <summary>
        /// Promotes the user.
        /// </summary>
        /// <param name="u">The u.</param>
        internal static async void promoteUser(User u) {
            await ss.sendCommand(CommandBuilder.getPromoteCommand(u));
            ss.promoteUser(u.Id);
        }

        /// <summary>
        /// Demotes the user.
        /// </summary>
        /// <param name="u">The u.</param>
        internal static async void demoteUser(User u) {
            await ss.sendCommand(CommandBuilder.getDemoteCommand(u));
            ss.demoteUser(u.Id);
        }


        /// <summary>
        /// Stops sending the move commands.
        /// </summary>
        internal static void stopSendingMoveCommands() {
            Task.Delay(1000);
            sendingMoveCommandsAllowed = false;
        }

        /// <summary>
        /// Starts sending the move commands.
        /// </summary>
        internal void startSendingMoveCommands() {
            sendingMoveCommandsAllowed = true;
            commandListSender = new Task(sendCommandList);
            commandListSender.Start();
        }

        /// <summary>
        /// Sends the pointer move command.
        /// </summary>
        /// <param name="pointerIndex">Index of the pointer.</param>
        internal void sendPointerMoveCommand(int pointerIndex) {
            CommandTable ct = new CommandTable();
            string commandText = CommandBuilder.getPointerMoveCommand(pointerIndex);
            commandList.AddLast(commandText);
        }

        private async void sendCommandList() {
            CommandTable ct = new CommandTable();
            while (sendingMoveCommandsAllowed) {
                if (commandList.Last != null) {
                    await ss.sendCommand(commandList.Last.Value);
                    commandList.Clear();
                    await ct.deletePrevMoveCommands(ss.getSessionData());
                }
                await Task.Delay(250);
            }
        }

        /// <summary>
        /// Sets the auto update.
        /// </summary>
        /// <param name="allowed">if set to <c>true</c> [allowed].</param>
        internal static void setAutoUpdate(bool allowed) {
            autoUpdate = allowed;
        }
    }
}
