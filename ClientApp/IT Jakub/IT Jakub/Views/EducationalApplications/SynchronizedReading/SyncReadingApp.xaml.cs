using Callisto.Controls;
using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Enumerations;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.Commands;
using IT_Jakub.Classes.Models.Utils;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 

    public sealed partial class SyncReadingApp : Page {

        static RichEditBox textBox;
        private static SignedSession ss = SignedSession.getInstance();
        private static bool autoUpdate = true;
        private static LoggedUser lu = LoggedUser.getInstance();

        private static ITextRange oldRange;
        private static Color oldColor;
        private static double horizontalOffset;
        private static double verticalOffset;
        private ScrollViewer scrollViewer;
        private static Image staticPointer;
        private ITextRange rangeCompare;
        private static ListView staticUserList;
        private static bool autoUpdateAllowed;
        private static LinkedList<string> commandList = new LinkedList<string>();
        private static Task commandListSender;
        private static bool sendingMoveCommandsAllowed = false;
        private double fluctY = 0;

        public SyncReadingApp() {
            this.InitializeComponent();
            textBox = textRichEditBox;
            staticPointer = pointer;
            oldRange = textBox.Document.GetRange(0, 0);
            scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();
            rangeCompare = textBox.Document.GetRange(0, 0);
            staticUserList = userList;
            setUsersRights();
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
            staticPointer.ManipulationMode = ManipulationModes.None;
        }

        private static void setPreferedUserRights() {
            Controls.BottomAppBar.setUserRights(UserRights.Prefered);
            staticPointer.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
        }

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

        public static async void highligtTextButton_Click() {
            ITextCharacterFormat charFormatting = textBox.Document.Selection.CharacterFormat;
            textBox.IsReadOnly = false;
            Color c = Colors.Yellow;
            charFormatting.BackgroundColor = c;
            textBox.Document.Selection.CharacterFormat = charFormatting;
            textBox.IsReadOnly = true;
            int startPosition = textBox.Document.Selection.StartPosition;
            int endPosition = textBox.Document.Selection.EndPosition;
            await ss.sendCommand(SyncReadingAppCommand.getHighlightCommand("ARGB(" + c.A + ", " + c.R + ", " + c.G + ", " + c.B + ")", startPosition, endPosition));
        }

        public static RichEditBox getTextRichEditBox() {
            return textBox;
        }

        internal static async void openFile(string uri) {
            RtfFileOpener opener = new RtfFileOpener();

            string source = await opener.openDocumentFromUri(uri.Trim());

            if (source != null) {
                await ss.sendCommand(SyncReadingAppCommand.getOpenCommand(source));
                CommandTable ct = new CommandTable();
                await ct.removeOldOpenCommands(ss.getSessionData());
            }
        }

        private void mainGrid_Loaded(object sender, RoutedEventArgs e) {
            autoUpdateAllowed = true;
            autoUpdate = true;
            startSendingMoveCommands();
            startAutoUpdate();
        }

        public static void killAutoUpdateTask() {
            autoUpdateAllowed = false;
            stopSendingMoveCommands();
        }

        private async void startAutoUpdate() {
            while (autoUpdateAllowed) {
                await Task.Delay(250);
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
            }
        }
        
        public static bool isAutoUpdateEnabled() {
            return autoUpdate;
        }

        private void pointer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();

            verticalOffset = scrollViewer.VerticalOffset;
            horizontalOffset = scrollViewer.HorizontalOffset;

            fluctY += e.Delta.Translation.Y;

            double top = double.Parse(pointer.GetValue(Canvas.TopProperty).ToString()) + e.Delta.Translation.Y;
            
            if (fluctY > 25 || fluctY < -25) {
                pointer.SetValue(Canvas.TopProperty, top);                
            }
            double left = double.Parse(pointer.GetValue(Canvas.LeftProperty).ToString()) + e.Delta.Translation.X;

            pointer.SetValue(Canvas.LeftProperty, left);

            unhighlightPointerWord();

            double testX = double.Parse(pointer.GetValue(Canvas.LeftProperty).ToString());
            double testY = double.Parse(pointer.GetValue(Canvas.TopProperty).ToString());
            double testW = (pointer.ActualWidth / 3);

            double leftPoint = double.Parse(pointer.GetValue(Canvas.LeftProperty).ToString()) + (pointer.ActualWidth / 2.7);
            double topPoint = double.Parse(pointer.GetValue(Canvas.TopProperty).ToString()) - 10;

            ITextRange range = textRichEditBox.Document.GetRangeFromPoint(new Point(leftPoint, topPoint), PointOptions.None);
            range.Expand(TextRangeUnit.Character);
            highlightPointerWord(range);

            setPointerScrollbarOffset();
            if (rangeCompare.StartPosition != range.StartPosition) {
                sendPointerMoveCommand(range.StartPosition);
                rangeCompare = range;
            }
        }


        private static void highlightPointerWord(ITextRange range) {
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

        private static void unhighlightPointerWord() {
            textBox.Document.Selection.SetRange(oldRange.StartPosition, oldRange.EndPosition);
            ITextCharacterFormat charFormatting = textBox.Document.Selection.CharacterFormat;

            textBox.IsReadOnly = false;
            Color c = oldColor;
            charFormatting.BackgroundColor = c;
            textBox.Document.Selection.CharacterFormat = charFormatting;
            textBox.IsReadOnly = true;
        }

        private void setPointerScrollbarOffset() {
            scrollViewer.ScrollToVerticalOffset(verticalOffset);
            scrollViewer.ScrollToHorizontalOffset(horizontalOffset);
        }

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

            unhighlightPointerWord();
            highlightPointerWord(range);

            range.GetPoint(HorizontalCharacterAlignment.Left, VerticalCharacterAlignment.Baseline, PointOptions.None, out focusCharacterPoint);
            staticPointer.SetValue(Canvas.LeftProperty, focusCharacterPoint.X - (staticPointer.ActualWidth / 2.5));
            staticPointer.SetValue(Canvas.TopProperty, focusCharacterPoint.Y);
        }

        private void pointer_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e) {
            autoUpdate = false;
            fluctY = 0;
        }

        private void pointer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            autoUpdate = true;
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

        private void updateUserListButton_Click(object sender, RoutedEventArgs e) {
            updateUserList();
        }

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

        internal static async void promoteUser(User u) {
            await ss.sendCommand(GeneralCommand.getPromoteCommand(u));
            ss.promoteUser(u.Id);
        }

        internal static async void demoteUser(User u) {
            await ss.sendCommand(GeneralCommand.getDemoteCommand(u));
            ss.demoteUser(u.Id);
        }


        internal static void stopSendingMoveCommands() {
            Task.Delay(1000);
            sendingMoveCommandsAllowed = false;
        }

        internal void startSendingMoveCommands() {
            sendingMoveCommandsAllowed = true;
            commandListSender = new Task(sendCommandList);
            commandListSender.Start();
        }

        internal void sendPointerMoveCommand(int pointerIndex) {
            CommandTable ct = new CommandTable();
            string commandText = Classes.Models.Commands.SyncReadingAppCommand.getPointerMoveCommand(pointerIndex);
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
            }
        }

        internal static void setAutoUpdate(bool allowed) {
            autoUpdate = allowed;
        }
    }
}
