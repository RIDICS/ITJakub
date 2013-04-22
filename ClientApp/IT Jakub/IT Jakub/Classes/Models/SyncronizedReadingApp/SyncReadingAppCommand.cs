using IT_Jakub.Classes.DatabaseModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace IT_Jakub.Classes.Models.SyncronizedReadingApp {
    class SyncReadingAppCommand : Command {

        public const string WHOLE_APPLICATION = "App";
        public const string TEXT = "Text";
        public const string START_APPLICATION = "Start()";


        private Command c;

        public SyncReadingAppCommand(Command c) {
            this.c = c;
        }

        internal static string getHighlightCommand(string color, int startingRange, int endingRange) {
            string text = SYNCHRONIZED_READING_APPLICATION + SEPARATOR + TEXT + SEPARATOR + "Highlight(" + color + ';' + startingRange.ToString() + ';' + endingRange.ToString() + ")";
            return text;
        }

        internal static string getOpenCommand(string path) {
            string text = SYNCHRONIZED_READING_APPLICATION + SEPARATOR + TEXT + SEPARATOR + "Open(" + path + ")";
            return text;
        }

        internal async override Task<bool> procedeCommand() {
            switch (c.commandObject) {
                case WHOLE_APPLICATION:
                    procedeAppCommand();
                    break;
                case TEXT:
                    await procedeTextCommand();
                    break;
            }
            return true;
        }

        private async Task<bool> procedeTextCommand() {
            if (c.command.StartsWith("Highlight(")) {
                procedeTextHighlightCommand();
                return true;
            }
            if (c.command.StartsWith("Open(")) {
                await procedeOpenCommand();
                return true;
            }
            return false;
        }

        private async Task<bool> procedeOpenCommand() {
            int index = c.CommandText.IndexOf("Open(");
            string path = c.CommandText.Substring(index);
            path = path.Replace("Open(", "");
            path = path.Replace(")", "");
            
            RtfFileOpener opener = new RtfFileOpener();
            string source = await opener.openDocumentFromUri(path.Trim());
            
            if (source != null) {
                return true;
            }

            return false;
        }

        private void procedeTextHighlightCommand() {
            string[] splitedCommand = c.command.Split(';');

            string colorCommand = splitedCommand[0];
            string startingRangeText = splitedCommand[1];
            string endingRangeText = splitedCommand[2];
            endingRangeText = endingRangeText.Replace(")", "");
            colorCommand = colorCommand.Replace("Highlight(RGBA(", "");
            colorCommand = colorCommand.Replace(")", "");

            int startingRange = int.Parse(startingRangeText);
            int endingRange = int.Parse(endingRangeText);

            RichEditBox textRichEditBox = Views.EducationalApplications.SynchronizedReading.SyncReadingApp.getTextRichEditBox();
            textRichEditBox.Document.Selection.SetRange(startingRange, endingRange);
            ITextCharacterFormat charFormatting = textRichEditBox.Document.Selection.CharacterFormat;
            textRichEditBox.IsReadOnly = false;
            Color color = Colors.Violet;
            charFormatting.BackgroundColor = color;
            textRichEditBox.Document.Selection.CharacterFormat = charFormatting;
            textRichEditBox.IsReadOnly = true;
            textRichEditBox.Document.Selection.SetRange(0, 0);
        }

        private void procedeAppCommand() {
            switch (c.command) {
                case START_APPLICATION:
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(Views.EducationalApplications.SynchronizedReading.SyncReadingApp));
                    break;
            }
        }
    }
}
