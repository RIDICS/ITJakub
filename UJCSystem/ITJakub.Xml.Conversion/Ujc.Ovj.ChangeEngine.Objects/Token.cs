using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    [XmlRoot("token")]
    [XmlType("token")]
    public class Token : TokenBase
    {
        [XmlAttribute("lang")]
        public string Language { get; set; }

        [XmlAttribute("frequency")]
        public int Frequency { get; set; }

        public Token()
        {
            
        }

        public Token(string text, string language)
        {
            Text = text;
            Language = language;
        }

        public Token(string text, string language, int frequency) : this(text, language)
        {
            Frequency = frequency;
        }
    }
}
