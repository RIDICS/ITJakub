using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IT_Jakub.Classes.Models.CrosswordsApp {
    /// <summary>
    /// Implementation of Abstract class CrosswordField. This type stands for question hints in crossword.
    /// </summary>
    class CrosswordQuestion : CrosswordField {

        /// <summary>
        /// The hint_x hint for horizontal word.
        /// </summary>
        private string hint_x;
        /// <summary>
        /// The hint_y hint for vertical word.
        /// </summary>
        private string hint_y;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrosswordQuestion"/> class.
        /// </summary>
        /// <param name="hint_v">The hint_v is vertical question hint.</param>
        /// <param name="hint_h">The hint_h is horizontal question hint.</param>
        public CrosswordQuestion(string hint_v, string hint_h) {
            hint_x = hint_h;
            hint_y = hint_v;
        }

        /// <summary>
        /// Gets the text in the crossword field.
        /// </summary>
        /// <example>
        /// [horizontal hint]:[vertical hint]
        /// </example>
        /// <returns></returns>
        internal override string getText() {
            return '[' + hint_x + "]:[" + hint_y + ']';
        }
        
        /// <summary>
        /// Gets the horizontal hint.
        /// </summary>
        /// <returns></returns>
        internal string getHorizontalHint() {
            return hint_x;
        }

        /// <summary>
        /// Gets the vertical hint.
        /// </summary>
        /// <returns></returns>
        internal string getVerticalHint() {
            return hint_y;
        }

    }
}
