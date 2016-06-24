using System.Collections.Generic;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    [XmlRoot("spellchecking")]
    public class Spellchecking : ISpellchecking
    {
        public Spellchecking()
        {
        }

        public Spellchecking(SpellcheckStatus status)
        {
            Status = status;
        }

        public Spellchecking(SpellcheckStatus status, List<string> suggestions)
        {
            Status = status;
            Suggestions = suggestions;
        }

        #region Implementation of ISpellchecking

        [XmlAttribute("status")]
        public SpellcheckStatus Status { get; set; }

        [XmlArray("suggestions")]
        [XmlArrayItem("suggestion")]
        public List<string> Suggestions { get; set; }

        #endregion
    }
}
