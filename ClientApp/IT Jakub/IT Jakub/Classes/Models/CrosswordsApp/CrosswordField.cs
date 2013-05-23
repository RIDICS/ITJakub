using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models.CrosswordsApp {
    /// <summary>
    /// Abstract class of one field in crossword.
    /// </summary>
    abstract class CrosswordField {
        public CrosswordField() {

        }

        /// <summary>
        /// Gets the text in crossword field.
        /// </summary>
        /// <returns></returns>
        internal abstract string getText();
    }
}
