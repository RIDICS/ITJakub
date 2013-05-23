using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Enumerations;
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
    /// <summary>
    /// Class represents Command which is supposed to be proceded as Synchronized readnig App Command.
    /// </summary>
    class SyncReadingAppCommand : Command {
        
        /// <summary>
        /// Command start this application.
        /// </summary>
        public const string START_APPLICATION = "Start()";

        /// <summary>
        /// The text box in Synchronized Reading App.
        /// </summary>
        RichEditBox textBox = Views.EducationalApplications.SynchronizedReading.SyncReadingApp.getTextRichEditBox();

        /// <summary>
        /// The c is representation of the particular Command.
        /// </summary>
        private Command c;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyncReadingAppCommand"/> class.
        /// </summary>
        /// <param name="c">The Command which created this instance.</param>
        public SyncReadingAppCommand(Command c) {
            this.c = c;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns></returns>
        internal async override Task<bool> procedeCommand() {
            switch (c.commandObject) {
                case SyncReadingAppObject.WHOLE_APPLICATION:
                    procedeAppCommand();
                    break;
                case SyncReadingAppObject.TEXT:
                    await procedeTextCommand();
                    break;
                case SyncReadingAppObject.POINTER:
                    procedePointerCommand();
                    break;
            }
            return true;
        }

        /// <summary>
        /// Procedes the text pointer command.
        /// </summary>
        private void procedePointerCommand() {
            if (c.command.StartsWith("Move(")) {
                string commandAtribute = c.command.Replace("Move(", "");
                commandAtribute = commandAtribute.Replace(")", "");
                Views.EducationalApplications.SynchronizedReading.SyncReadingApp.movePointerToCharIndex(int.Parse(commandAtribute));
            }
        }

        /// <summary>
        /// Procedes the actual text command.
        /// </summary>
        /// <returns></returns>
        private async Task<bool> procedeTextCommand() {
            if (c.command.StartsWith("Highlight(")) {
                await procedeTextHighlightCommand();
                return true;
            }
            if (c.command.StartsWith("Open(")) {
                await procedeOpenCommand();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Procedes the open file command.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Procedes the text highlight command.
        /// </summary>
        /// <returns></returns>
        private async Task procedeTextHighlightCommand() {
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

        /// <summary>
        /// Procedes the whole application command.
        /// </summary>
        private void procedeAppCommand() {
            switch (c.command) {
                case START_APPLICATION:
                    Frame mainFrame = MainPage.getMainFrame();
                    mainFrame.Navigate(typeof(Views.EducationalApplications.SynchronizedReading.SyncReadingApp));
                    break;
            }
        }
    }
}
