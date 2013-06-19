using System.Collections.Generic;

namespace ITJakub.Core.XMLOperations
{
    public class XslTransformDirector
    {
        private List<XslTransformationBase> m_transformations = new List<XslTransformationBase>();

        public XslTransformDirector()
        {
            LoadTransformations();
        }

        private void LoadTransformations()
        {
            m_transformations.Add(new DictionaryXslt());
        }
    }
}
