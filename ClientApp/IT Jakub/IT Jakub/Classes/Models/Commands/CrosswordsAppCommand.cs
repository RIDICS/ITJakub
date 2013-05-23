using IT_Jakub.Classes.DatabaseModels;
using IT_Jakub.Classes.Enumerations;
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
    /// <summary>
    /// Class represents Command which is supposed to be proceded as Crosswords App Command.
    /// </summary>
    class CrosswordsAppCommand : Command {

        
        /// <summary>
        /// Command which says to the Application to Start.
        /// </summary>
        public const string START_APPLICATION = "Start()";
        
        /// <summary>
        /// The lu is singleton instance of LoggedUser.
        /// </summary>
        public static LoggedUser lu = LoggedUser.getInstance();

        /// <summary>
        /// The c is representation of the particular Command.
        /// </summary>
        private Command c;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordsAppCommand"/> class.
        /// </summary>
        /// <param name="c">The Command which created this instance.</param>
        public CrosswordsAppCommand(Command c) {
            this.c = c;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <returns></returns>
        internal async override Task<bool> procedeCommand() {
            switch (c.commandObject) {
                case CrosswordAppObject.WHOLE_APPLICATION:
                    procedeAppCommand();
                    break;
                case CrosswordAppObject.CROSSWORD:
                    procedeCrosswordCommand();
                    break;
            }
            return true;
        }

        /// <summary>
        /// Procedes the command targeted on the Crossword itself.
        /// </summary>
        private void procedeCrosswordCommand() {
            if (c.command.StartsWith("Open(")) {
                procedeOpenCommand();
            }
            if (c.command.StartsWith("Solution(") && c.UserId == lu.getUserData().Id) {
                procedeFillUserSolutionCommand();
            }
        }

        /// <summary>
        /// <para>
        /// Procedes the fill user solution command.
        /// </para>
        /// <para>
        /// Open the partial solution of crossword.
        /// </para>
        /// </summary>
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

        /// <summary>
        /// <para>
        /// Procedes the open command.
        /// </para>
        /// <para>
        /// Opens new crossword.
        /// </para>
        /// </summary>
        private void procedeOpenCommand() {
            string xml;
            Regex r = new Regex("^Open" + Regex.Escape("("));
            xml = r.Replace(c.command,"");
            xml.TrimEnd();
            r = new Regex(Regex.Escape(")") + "$");
            xml = r.Replace(xml, "");
            Views.EducationalApplications.Crosswords.CrosswordsApp.openCrossword(xml);
        }

        /// <summary>
        /// Procedes the command targeted at whole application.
        /// </summary>
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
