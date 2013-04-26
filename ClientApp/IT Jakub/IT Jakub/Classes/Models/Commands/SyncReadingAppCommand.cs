using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinRTXamlToolkit.Controls.Extensions;

namespace IT_Jakub.Classes.Models.Commands {
    class SyncReadingAppCommand : Command {

        public const string WHOLE_APPLICATION = "App";
        public const string TEXT = "Text";
        public const string POINTER = "Pointer";
        public const string START_APPLICATION = "Start()";

        RichEditBox textBox = Views.EducationalApplications.SynchronizedReading.SyncReadingApp.getTextRichEditBox();

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
                case POINTER:
                    procedePointerCommand();
                    break;
            }
            return true;
        }

        private void procedePointerCommand() {
            if (c.command.StartsWith("Move(")) {
                string commandAtribute = c.command.Replace("Move(", "");
                commandAtribute = commandAtribute.Replace(")", "");
                Views.EducationalApplications.SynchronizedReading.SyncReadingApp.movePointerToCharIndex(int.Parse(commandAtribute));
            }
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
            RichEditBox textRichEditBox = Views.EducationalApplications.SynchronizedReading.SyncReadingApp.getTextRichEditBox();
            ScrollViewer scrollViewer = textRichEditBox.GetFirstDescendantOfType<ScrollViewer>();
            double verticalOffset = scrollViewer.VerticalOffset;
            double horizontalOffset = scrollViewer.HorizontalOffset;

            string[] splitedCommand = c.command.Split(';');

            string colorCommand = splitedCommand[0];
            string startingRangeText = splitedCommand[1];
            string endingRangeText = splitedCommand[2];
            endingRangeText = endingRangeText.Replace(")", "");
            colorCommand = colorCommand.Replace("Highlight(ARGB(", "");
            colorCommand = colorCommand.Replace(")", "");

            string[] argb = colorCommand.Split(',');
            byte[] argbInt = new byte[4];
            for (int i = 0; i < argbInt.Length; i++) {
                argbInt[i] = byte.Parse(argb[i].Trim());
            }

            int startingRange = int.Parse(startingRangeText);
            int endingRange = int.Parse(endingRangeText);

            textRichEditBox.Document.Selection.SetRange(startingRange, endingRange);
            ITextCharacterFormat charFormatting = textRichEditBox.Document.Selection.CharacterFormat;
            textRichEditBox.IsReadOnly = false;
            Color color = Color.FromArgb(argbInt[0], argbInt[1], argbInt[2], argbInt[3]);
            charFormatting.BackgroundColor = color;
            textRichEditBox.Document.Selection.CharacterFormat = charFormatting;
            textRichEditBox.IsReadOnly = true;
            textRichEditBox.Document.Selection.SetRange(0, 0);

            scrollViewer.ScrollToVerticalOffset(verticalOffset);
            scrollViewer.ScrollToHorizontalOffset(horizontalOffset);
        }

        private void procedeAppCommand() {
            switch (c.command) {
                case START_APPLICATION:
                    Frame rootFrame = Window.Current.Content as Frame;
                    rootFrame.Navigate(typeof(Views.EducationalApplications.SynchronizedReading.SyncReadingApp));
                    break;
            }
        }

        internal static string getPointerMoveCommand(int charIndex) {
            string text = SYNCHRONIZED_READING_APPLICATION + SEPARATOR + POINTER + SEPARATOR + "Move("+charIndex+")";
            return text;
        }


        
    }
}
