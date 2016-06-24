using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    public class TokenBase
    {
        [XmlAttribute("text")]
        public string Text { get; set; }
        [XmlAttribute("length")]
        public int Length { get { return Text != null ? Text.Length : 0; } }
        [XmlAttribute("correction")]
        public string Correction { get; set; }
    }
}
