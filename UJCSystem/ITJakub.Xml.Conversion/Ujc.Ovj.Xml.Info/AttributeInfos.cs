using System.Collections.Generic;
using System.Linq;

namespace Ujc.Ovj.Xml.Info
{
	public class AttributeInfos : List<AttributeInfo>
	{
		public AttributeInfos(AttributeInfos attributes) : base(attributes)
		{
		}

		public AttributeInfos(int capacity) : base(capacity)
		{
		}

		public AttributeInfos() : base()
		{
			
		}

	    public bool AttributeExists(string prefix, string name)
	    {
            var attribute = (from a in this
                             where a.Prefix == prefix && a.LocalName == name
                             select a).FirstOrDefault();
	        return (attribute != null);
	    }
        public AttributeInfo GetAttributeByLocalName(string prefix, string name)
		{
			var attribute = (from a in this where a.Prefix == prefix && a.LocalName == name
			select a).First();

			return attribute;
		}
	}
}