using IT_Jakub.Classes.Models;
using IT_Jakub.Classes.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Networking {
    public static class CommandBuilder {

        private static LoggedUser lu = LoggedUser.getInstance();

        public static string getOpenCrosswordCommand(string xml) {
            string text = CrosswordsAppCommand.CROSSWORDS_APPLICATION
             + Command.SEPARATOR
             + CrosswordsAppCommand.CROSSWORD 
             + Command.SEPARATOR 
             + "Open("
             + xml
             + ")";
            return text;
        }

        internal static string getCrosswordSolutionCommand(string crosswordSolution) {
            string text = CrosswordsAppCommand.CROSSWORDS_APPLICATION
             + Command.SEPARATOR
             + CrosswordsAppCommand.CROSSWORD
             + Command.SEPARATOR
             + "Solution("
             + lu.getUserData().Id
             + ", "
             + crosswordSolution
             + ")";
            return text;
        }
        
        internal static string getCrosswordFinalSolutionCommand(string crosswordSolution) {
            string text = CrosswordsAppCommand.CROSSWORDS_APPLICATION
             + Command.SEPARATOR
             + CrosswordsAppCommand.CROSSWORD
             + Command.SEPARATOR
             + "SolutionFinal("
             + DateTime.Now.ToString()
             + ", "
             + lu.getUserData().Id
             + ", "
             + crosswordSolution
             + ")";
            return text;
        }

        internal static string getEndSolutionCrosswordCommand(string xml) {
            string text = CrosswordsAppCommand.CROSSWORDS_APPLICATION
             + Command.SEPARATOR
             + CrosswordsAppCommand.CROSSWORD
             + Command.SEPARATOR
             + "SolutionEnd("
             + xml
             + ")";
            return text;
        }
    }
}
