using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models.CrosswordsApp {
    /// <summary>
    /// Implementation of Abstract class CrosswordField. This type stands for hint in crossword.
    /// </summary>
    class CrosswordHint : CrosswordField {

        /// <summary>
        /// The hint_text hintText.
        /// </summary>
        private string hint_text;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordHint" /> class.
        /// </summary>
        /// <param name="hintText">The hint text.</param>
        public CrosswordHint(string hintText) {
            hint_text = hintText;
        }

        /// <summary>
        /// Gets the text in the crossword field.
        /// </summary>
        /// <returns></returns>
        internal override string getText() {
            return hint_text;
        }

    }
}
