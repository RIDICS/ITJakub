using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.Commands;
using IT_Jakub.Classes.Models.Utils;
using IT_Jakub.Classes.Utils;
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
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        private bool autoUpdate = true;
        
        private ITextRange oldRange;
        private Color oldColor;
        private static double horizontalOffset;
        private static double verticalOffset;
        private ScrollViewer scrollViewer;
        private static Image staticPointer;
        private ITextRange rangeComp;
        private static ListView staticUserList;

        public SyncReadingApp() {
            this.InitializeComponent();
            textBox = textRichEditBox;
            staticPointer = pointer;
            oldRange = textBox.Document.GetRange(0, 0);
            scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();
            rangeComp = textBox.Document.GetRange(0, 0);
            staticUserList = userList;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e) {
        }

        private void openFileButton_Click(object sender, RoutedEventArgs e) {
            openPopup.IsOpen = true;
        }

        private async void highligtTextButton_Click(object sender, RoutedEventArgs e) {
            ITextCharacterFormat charFormatting = textRichEditBox.Document.Selection.CharacterFormat;
            textRichEditBox.IsReadOnly = false;
            Color c = Colors.Yellow;
            charFormatting.BackgroundColor = c;
            textRichEditBox.Document.Selection.CharacterFormat = charFormatting;
            textRichEditBox.IsReadOnly = true;
            int startPosition = textRichEditBox.Document.Selection.StartPosition;
            int endPosition = textRichEditBox.Document.Selection.EndPosition;
            await ss.sendCommand(SyncReadingAppCommand.getHighlightCommand("ARGB(" + c.A + ", "+ c.R + ", " + c.G + ", " + c.B + ")", startPosition, endPosition));
        }

        public static RichEditBox getTextRichEditBox() {
            return textBox;
        }

        private async void openButton_Click(object sender, RoutedEventArgs e) {
            RtfFileOpener opener = new RtfFileOpener();
            string source = await opener.openDocumentFromUri(uriTextBox.Text.Trim());

            if (source != null) {
                
                await ss.sendCommand(SyncReadingAppCommand.getOpenCommand(source));
            }
            openPopup.IsOpen = false;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e) {
            openPopup.IsOpen = false;
        }

        private void mainGrid_Loaded(object sender, RoutedEventArgs e) {
            startAutoUpdate();
        }

        private async void startAutoUpdate() {
            while (autoUpdate) {
                await Task.Delay(40);
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

        private void autoUpdateButton_Click(object sender, RoutedEventArgs e) {
            switch (autoUpdate) {
                case true:
                    autoUpdateButton.BorderBrush = new SolidColorBrush(Color.FromArgb(255,122,26,26));
                    autoUpdateButton.Background = new SolidColorBrush(Color.FromArgb(255, 122, 0, 0));
                    autoUpdateButton.Foreground = new SolidColorBrush(Colors.White);
                    autoUpdateButton.Content = "OFF";
                    autoUpdate = false;
                    break;
                case false:
                    autoUpdateButton.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 26, 122, 26));
                    autoUpdateButton.Background = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    autoUpdateButton.Foreground = new SolidColorBrush(Colors.Black);
                    autoUpdateButton.Content = "ON";
                    autoUpdate = true;
                    startAutoUpdate();
                    break;
            }
            
        }
        

        private void pointer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
                scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();

                verticalOffset = scrollViewer.VerticalOffset;
                horizontalOffset = scrollViewer.HorizontalOffset;

                double top = double.Parse(pointer.GetValue(Canvas.TopProperty).ToString()) + e.Delta.Translation.Y;
                double left = double.Parse(pointer.GetValue(Canvas.LeftProperty).ToString()) + e.Delta.Translation.X;

                pointer.SetValue(Canvas.LeftProperty, left);
                pointer.SetValue(Canvas.TopProperty, top);

                unhighlightPointerWord();

                ITextRange range = textBox.Document.GetRangeFromPoint(new Point(left + (pointer.Width / 2), top), PointOptions.Start);
                range.Expand(TextRangeUnit.Word);
                highlightPointerWord(range);

                setPointerScrollbarOffset();
                if (rangeComp.StartPosition != range.StartPosition) {
                    ss.sendPointerMoveCommand(range.StartPosition);
                    rangeComp = range;
                }
        }


        private void highlightPointerWord(ITextRange range) {
            textRichEditBox.Document.Selection.SetRange(range.StartPosition, range.EndPosition);

            ITextCharacterFormat charFormatting = textBox.Document.Selection.CharacterFormat;
            oldColor = charFormatting.BackgroundColor;

            textRichEditBox.IsReadOnly = false;
            Color c = Colors.Aqua;
            charFormatting.BackgroundColor = c;
            textRichEditBox.Document.Selection.CharacterFormat = charFormatting;
            textRichEditBox.IsReadOnly = true;
            oldRange = range;
        }

        private void unhighlightPointerWord() {
            textBox.Document.Selection.SetRange(oldRange.StartPosition, oldRange.EndPosition);
            ITextCharacterFormat charFormatting = textBox.Document.Selection.CharacterFormat;
            
            textRichEditBox.IsReadOnly = false;
            Color c = oldColor;
            charFormatting.BackgroundColor = c;
            textRichEditBox.Document.Selection.CharacterFormat = charFormatting;
            textRichEditBox.IsReadOnly = true;
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
            range.GetPoint(HorizontalCharacterAlignment.Center, VerticalCharacterAlignment.Baseline, PointOptions.None, out focusCharacterPoint);

            double docHeight = docEndPoint.Y - docStartPoint.Y;
            double charOffsetPercent = (focusCharacterPoint.Y - docStartPoint.Y) / docHeight;

            ScrollViewer scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();
            verticalOffset = charOffsetPercent * scrollViewer.ScrollableHeight;

            scrollViewer.ScrollToVerticalOffset(verticalOffset);

            range.GetPoint(HorizontalCharacterAlignment.Left, VerticalCharacterAlignment.Baseline, PointOptions.None, out focusCharacterPoint);

            staticPointer.SetValue(Canvas.LeftProperty, focusCharacterPoint.X - (staticPointer.ActualWidth / 2));
            staticPointer.SetValue(Canvas.TopProperty, focusCharacterPoint.Y);
        }

        private void pointer_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e) {
            autoUpdate = false;
        }

        private void pointer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            autoUpdate = true;
            startAutoUpdate();
        }

        public static async void updateUserList() {
            SessionUserTable sut = new SessionUserTable();
            UserTable ut = new UserTable();
            List<SessionUser> items = await sut.getAllUsersInSession(ss.getSessionData());
            List<User> userList = new List<User>();
            for (int i = 0; i < items.Count; i++) {
                User u = await ut.getUserById(items[i].UserId);
                userList.Add(u);
            }
            staticUserList.ItemsSource = userList;
        }

        private void updateUserListButton_Click(object sender, RoutedEventArgs e) {
            updateUserList();
        }

    }
}
