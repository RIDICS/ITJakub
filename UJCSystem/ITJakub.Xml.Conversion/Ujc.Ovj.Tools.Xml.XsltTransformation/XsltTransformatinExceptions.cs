using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ujc.Ovj.Tools.Xml.XsltTransformation
{
    public class XsltTransformatinException : Exception
    {
        public XsltTransformatinException(string message) : base(message)
        {
            
        }

        public XsltTransformatinException(string message, Exception innerException) : base(message, innerException)
        {
            
        }
    }

    public class XsltTransformatinNotFoundSectionException : XsltTransformatinException
    {
        public XsltTransformatinNotFoundSectionException(string message) : base(message)
        {

        }

        public XsltTransformatinNotFoundSectionException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
