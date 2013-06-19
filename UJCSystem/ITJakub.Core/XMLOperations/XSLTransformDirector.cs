using System.Collections.Generic;
using System.Linq;

namespace ITJakub.Core.XMLOperations
{
    public class XslTransformDirector
    {
        private readonly List<XslTransformationBase> m_transformations = new List<XslTransformationBase>();

        public XslTransformDirector()
        {
            LoadTransformations();
        }

        private void LoadTransformations()
        {
            //todo loading by attribute
            m_transformations.Add(new DictionaryXslt());
        }

        public string TransformResult(string xmlContext)
        {
            //todo transformation selection
           return m_transformations.First().Transform(xmlContext);
        }
    }
}
