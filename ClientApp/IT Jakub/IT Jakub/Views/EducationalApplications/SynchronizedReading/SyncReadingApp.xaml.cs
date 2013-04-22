using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.SyncronizedReadingApp;
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

        public SyncReadingApp() {
            this.InitializeComponent();
            textBox = textRichEditBox;
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
            await ss.sendCommand(SyncReadingAppCommand.getHighlightCommand("RGBA(" + c.R + ", "+ c.G + ", " + c.B + ", " + c.A + ")", startPosition, endPosition));
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
                    await Task.Delay(1000);
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
        
    }
}
