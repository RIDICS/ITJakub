using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Models.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    class CrosswordsAppCommand : Command {

        public const string WHOLE_APPLICATION = "App";
        public const string START_APPLICATION = "Start()";
        public const string CROSSWORD = "Crossword";
        public static LoggedUser lu = LoggedUser.getInstance();

        RichEditBox textBox = Views.EducationalApplications.SynchronizedReading.SyncReadingApp.getTextRichEditBox();

        private Command c;

        public CrosswordsAppCommand(Command c) {
            this.c = c;
        }

        internal async override Task<bool> procedeCommand() {
            switch (c.commandObject) {
                case WHOLE_APPLICATION:
                    procedeAppCommand();
                    break;
                case CROSSWORD:
                    procedeCrosswordCommand();
                    break;
            }
            return true;
        }

        private void procedeCrosswordCommand() {
            if (c.command.StartsWith("Open(")) {
                procedeOpenCommand();
            }
            if (c.command.StartsWith("Solution(") && c.UserId == lu.getUserData().Id) {
                procedeFillUserSolutionCommand();
            }
        }

        private void procedeFillUserSolutionCommand() {
            string xml;
            Regex r = new Regex("^Solution" + Regex.Escape("(") + ".+" + Regex.Escape(", "));
            xml = r.Replace(c.command, "");
            xml.TrimEnd();
            r = new Regex(Regex.Escape(")") + "$");
            xml = r.Replace(xml, "");
            Views.EducationalApplications.Crosswords.CrosswordsApp.openCrossword(xml);
            Views.EducationalApplications.Crosswords.CrosswordsApp.setUpdateId(c.Id);
        }

        private void procedeOpenCommand() {
            string xml;
            Regex r = new Regex("^Open" + Regex.Escape("("));
            xml = r.Replace(c.command,"");
            xml.TrimEnd();
            r = new Regex(Regex.Escape(")") + "$");
            xml = r.Replace(xml, "");
            Views.EducationalApplications.Crosswords.CrosswordsApp.openCrossword(xml);
        }

        private void procedeAppCommand() {
            switch (c.command) {
                case START_APPLICATION:
                    Frame mainFrame = MainPage.getMainFrame();
                    mainFrame.Navigate(typeof(Views.EducationalApplications.Crosswords.CrosswordsApp));
                    break;
            }
        }

    }
}
