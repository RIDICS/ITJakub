using IT_Jakub.Classes.Enumerations;
using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Networking {
    /// <summary>
    /// Class that creates the commandText for Command.
    /// </summary>
    public static class CommandBuilder {

        /// <summary>
        /// The lu is singleton instance of LoggedUser. LoggedUser is user which is currently logged in.
        /// </summary>
        private static LoggedUser lu = LoggedUser.getInstance();

        /// <summary>
        /// The SEPARATOR between commands parts.
        /// </summary>
        internal const char SEPARATOR = ':';

        /// <summary>
        /// Gets the open crossword command.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static string getOpenCrosswordCommand(string xml) {
            string text = ApplicationType.CROSSWORDS_APPLICATION.ToString()
             + SEPARATOR
             + CrosswordAppObject.CROSSWORD.ToString()
             + SEPARATOR 
             + "Open("
             + xml
             + ")";
            return text;
        }

        /// <summary>
        /// Gets the crossword solution command.
        /// </summary>
        /// <param name="crosswordSolution">The crossword solution XML</param>
        /// <returns></returns>
        internal static string getCrosswordSolutionCommand(string crosswordSolution) {
            string text = ApplicationType.CROSSWORDS_APPLICATION.ToString()
             + SEPARATOR
             + CrosswordAppObject.CROSSWORD.ToString()
             + SEPARATOR
             + "Solution("
             + lu.getUserData().Id
             + ", "
             + crosswordSolution
             + ")";
            return text;
        }

        /// <summary>
        /// Gets the crossword final solution command.
        /// </summary>
        /// <param name="crosswordSolution">The crossword solution XML</param>
        /// <returns></returns>
        internal static string getCrosswordFinalSolutionCommand(string crosswordSolution) {
            string text = ApplicationType.CROSSWORDS_APPLICATION.ToString()
             + SEPARATOR
             + CrosswordAppObject.CROSSWORD.ToString()
             + SEPARATOR
             + "SolutionFinal("
             + DateTime.Now.ToString()
             + ", "
             + lu.getUserData().Id
             + ", "
             + crosswordSolution
             + ")";
            return text;
        }

        /// <summary>
        /// Gets the correct solution crossword command.
        /// </summary>
        /// <param name="xml">The crossword solution XML</param>
        /// <returns></returns>
        internal static string getCorrectSolutionCrosswordCommand(string xml) {
            string text = ApplicationType.CROSSWORDS_APPLICATION.ToString()
             + SEPARATOR
             + CrosswordAppObject.CROSSWORD.ToString()
             + SEPARATOR
             + "SolutionEnd("
             + xml
             + ")";
            return text;
        }

        /// <summary>
        /// Gets the promote command.
        /// </summary>
        /// <param name="u">The User which should be promoted.</param>
        /// <returns></returns>
        internal static string getPromoteCommand(User u) {
            string text = ApplicationType.GENERAL
                + SEPARATOR
                + GeneralApplicationObject.USER.ToString()
                + SEPARATOR
                + "Promote(" + u.Id + ")";
            return text;
        }

        /// <summary>
        /// Gets the demote command.
        /// </summary>
        /// <param name="u">The User which should be demoted</param>
        /// <returns></returns>
        internal static string getDemoteCommand(User u) {
            string text = ApplicationType.GENERAL
                + SEPARATOR
                + GeneralApplicationObject.USER.ToString()
                + SEPARATOR 
                + "Demote(" + u.Id + ")";
            return text;
        }

        /// <summary>
        /// Gets the highlight command.
        /// </summary>
        /// <param name="color">The color of highlight</param>
        /// <param name="startingRange">The starting index of range.</param>
        /// <param name="endingRange">The ending index of range.</param>
        /// <returns></returns>
        internal static string getHighlightCommand(string color, int startingRange, int endingRange) {
            string text = ApplicationType.SYNCHRONIZED_READING_APPLICATION
                + SEPARATOR
                + SyncReadingAppObject.TEXT.ToString()
                + SEPARATOR
                + "Highlight(" + color + ';' + startingRange.ToString() + ';' + endingRange.ToString() + ")";
            return text;
        }

        /// <summary>
        /// Gets the open file command.
        /// </summary>
        /// <param name="path">The URI to the book</param>
        /// <returns></returns>
        internal static string getOpenBookCommand(string path) {
            string text = ApplicationType.SYNCHRONIZED_READING_APPLICATION
                + SEPARATOR
                + SyncReadingAppObject.TEXT.ToString()
                + SEPARATOR 
                + "Open(" + path + ")";
            return text;
        }

        /// <summary>
        /// Gets the pointer move command.
        /// </summary>
        /// <param name="charIndex">Index of the char.</param>
        /// <returns></returns>
        internal static string getPointerMoveCommand(int charIndex) {
            string text = ApplicationType.SYNCHRONIZED_READING_APPLICATION
                + SEPARATOR
                + SyncReadingAppObject.POINTER.ToString()
                + SEPARATOR
                + "Move(" + charIndex + ")";
            return text;
        }

        /// <summary>
        /// Gets the user logged in command.
        /// </summary>
        /// <param name="user">The user who logged in</param>
        /// <returns></returns>
        internal static string getUserLoggedInCommand(User user) {
            string text = ApplicationType.GENERAL
                + SEPARATOR
                + GeneralApplicationObject.USER.ToString()
                + SEPARATOR 
                + "Login(" + user.Id + ")";
            return text;
        }

        /// <summary>
        /// Gets the user logged out command.
        /// </summary>
        /// <param name="user">The user who logged out.</param>
        /// <returns></returns>
        internal static string getUserLoggedOutCommand(User user) {
            string text = ApplicationType.GENERAL
                + SEPARATOR
                + GeneralApplicationObject.USER.ToString()
                + SEPARATOR
                + "Logout(" + user.Id + ")";
            return text;
        }

        internal static string getCrosswordAppStartCommand() {
            string text = ApplicationType.CROSSWORDS_APPLICATION
                + SEPARATOR
                + CrosswordAppObject.WHOLE_APPLICATION.ToString()
                + SEPARATOR
                + CrosswordsAppCommand.START_APPLICATION;
            return text;
        }

        internal static string getSyncReadingAppStartCommand() {
                string text = ApplicationType.SYNCHRONIZED_READING_APPLICATION
                + SEPARATOR
                + SyncReadingAppObject.WHOLE_APPLICATION.ToString()
                + SEPARATOR
                + SyncReadingAppCommand.START_APPLICATION;
            return text;
        }
    }
}
