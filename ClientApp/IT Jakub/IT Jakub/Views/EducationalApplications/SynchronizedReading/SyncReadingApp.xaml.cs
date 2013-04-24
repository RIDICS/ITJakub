using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.SyncronizedReadingApp;
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
        static TranslateTransform tt;
        private static SignedSession ss = SignedSession.getInstance();
        private bool autoUpdate = true;
        
        private ITextRange oldRange;
        private Color oldColor;
        private static double horizontalOffset;
        private static double verticalOffset;
        private ScrollViewer scrollViewer;
        private static Image staticPointer;

        public SyncReadingApp() {
            this.InitializeComponent();
            textBox = textRichEditBox;
            staticPointer = pointer;
            oldRange = textBox.Document.GetRange(0, 0);
            scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();
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
                try {
                    await Task.Delay(40);
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

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            
        }
        

        private async void pointer_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e) {
            scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();

            verticalOffset = scrollViewer.VerticalOffset;
            horizontalOffset = scrollViewer.HorizontalOffset;
            
            double top = double.Parse(pointer.GetValue(Canvas.TopProperty).ToString()) + e.Delta.Translation.Y;
            double left = double.Parse(pointer.GetValue(Canvas.LeftProperty).ToString()) + e.Delta.Translation.X;

            pointer.SetValue(Canvas.LeftProperty, left);
            pointer.SetValue(Canvas.TopProperty, top);

            unhighlightPointerWord();

            ITextRange range = textBox.Document.GetRangeFromPoint(new Point(left + (pointer.Width/2), top), PointOptions.Start);
            highlightPointerWord(range);

            setPointerScrollbarOffset();
            await ss.sendCommand(Classes.Models.SyncronizedReadingApp.SyncReadingAppCommand.getPointerMoveCommand(range.StartPosition));
        }


        private void highlightPointerWord(ITextRange range) {
            range.Expand(TextRangeUnit.Character);
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
            ITextRange range = textBox.Document.GetRange(index, index+1);
            Point p;
            range.GetPoint(HorizontalCharacterAlignment.Left, VerticalCharacterAlignment.Baseline, PointOptions.ClientCoordinates, out p);

            verticalOffset = (p.Y - textBox.ActualHeight) + 30;

            if (verticalOffset <= 0) {
                verticalOffset = 0;
            }

            ScrollViewer scrollViewer = textBox.GetFirstDescendantOfType<ScrollViewer>();
            scrollViewer.ScrollToVerticalOffset(verticalOffset);
            
            staticPointer.SetValue(Canvas.LeftProperty, p.X - (staticPointer.Width/2));
            staticPointer.SetValue(Canvas.TopProperty, p.Y - verticalOffset + 30);
        }

        private void pointer_ManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e) {
            autoUpdate = false;
        }

        private void pointer_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) {
            autoUpdate = true;
            startAutoUpdate();
        }
    }
}
