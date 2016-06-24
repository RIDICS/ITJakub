using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    /// <summary>
    /// Format of change pattern and replace string. Can be <value>String</value> or <value>RegularExpression</value>.
    /// Is prepared for custom change pattern.
    /// </summary>
    public enum ChangeFormat
    {
        [XmlEnum("string")]
        String,
        [XmlEnum("regularExpression")]
        RegularExpression
    }
}
