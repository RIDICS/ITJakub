using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models.CrosswordsApp {
    /// <summary>
    /// Implementation of Abstract class CrosswordField. This type stands for actual answer field in crossword.
    /// </summary>
    class CrosswordChar : CrosswordField {

        /// <summary>
        /// The content of field.
        /// </summary>
        private string content;

        /// <summary>
        /// Determines if field is part of puzzle
        /// </summary>
        private bool isPuzzle = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordChar"/> class.
        /// </summary>
        /// <param name="c">The text that should be in the field.</param>
        public CrosswordChar(string c) {
            content = c;
        }

        /// <summary>
        /// Gets the text in crossword field.
        /// </summary>
        /// <returns></returns>
        internal override string getText() {
            return content;
        }

        internal void makePuzzle() {
            isPuzzle = true;
        }

        internal bool isPuzzleChar() {
            return isPuzzle;
        }

    }
}
