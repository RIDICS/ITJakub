using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    [XmlRoot("changedToken")]
    [XmlType("changedToken")]
    public class ChangedToken : TokenBase
    {
        public ChangedToken()
        {
            AppliedChanges = new Collection<AppliedChange>();
        }

        public ChangedToken(Token token)
            : this()
        {
            Source = token;
        }

        /// <summary>
        /// True if token is propper noun (to keep first letter capitalized).
        /// </summary>
        [XmlAttribute("isProperNoun")]
        public bool IsProperNoun { get; set; }

        /// <summary>
        /// Original text of token, before changes are applied.
        /// </summary>
        [XmlElement("source")]
        public Token Source { get; set; }

        /// <summary>
        /// List of applied changes.
        /// </summary>
        [XmlArray("appliedChanges")]
        [XmlArrayItem("change")]
        public Collection<AppliedChange> AppliedChanges { get; set; }

        /// <summary>
        /// Note for changed token.
        /// </summary>
        [XmlElement("note")]
        public string Note { get; set; }

        /// <summary>
        /// Result of the spellchecking process of the current changed token
        /// </summary>
        [XmlElement("spellchecking")]
        public Spellchecking Spellchecking { get; set; } 

        ///// <summary>
        ///// Result of spellchecking applied on changed token.
        ///// </summary>
        //[XmlAttribute("spellcheckStatus")]
        //public SpellcheckStatus SpellcheckStatus { get; set; }

		/// <summary>
		/// List of contexts in which token is found.
		/// </summary>
		[XmlArray("contexts")]
		[XmlArrayItem("context")]
		public List<string> Contexts { get; set; }

		/// <summary>
		/// True if change is not OK. False if change is OK or is not checked.
		/// </summary>
		[XmlAttribute(AttributeName = "isOk")]
	    public bool IsOk { get; set; }
    }
}
