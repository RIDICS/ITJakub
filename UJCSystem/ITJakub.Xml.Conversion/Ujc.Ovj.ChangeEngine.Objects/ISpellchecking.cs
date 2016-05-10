using System.Collections.Generic;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    interface ISpellchecking
    {
        /// <summary>
        /// Status of the spellchecked word.
        /// </summary>
        SpellcheckStatus Status { get; set; }

        /// <summary>
        /// Suggestions of the spellchecker in the cas the word is misspelled.
        /// </summary>
        List<string> Suggestions { get; set; }
    }
}
