using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Ujc.Ovj.ChangeEngine.Objects
{
    [XmlRoot("expressions")]
    public class Expressions : Collection<Expression>
    {
         
    }
}