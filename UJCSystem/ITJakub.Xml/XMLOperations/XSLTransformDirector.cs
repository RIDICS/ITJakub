﻿using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ITJakub.Xml.XSLTransformations.Dictionaries;

namespace ITJakub.Xml.XMLOperations
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
            m_transformations.Add(new UniversalXslt());
            //m_transformations.Add(new DictionaryXslt());
        }

        public string TransformResult(string xmlContext, string searchedTerm)
        {
            //todo transformation selection
           return m_transformations.First().Transform(xmlContext, searchedTerm);
        }

        public string TransformResult(XmlNode selectSingleNode, string searchedTerm)
        {
            throw new System.NotImplementedException();
        }
    }
}
