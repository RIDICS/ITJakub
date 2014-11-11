using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class RespStmtProcessor : ListProcessorBase
    {
        public RespStmtProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "respStmt"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            var responsible = new Responsible();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                    xmlReader.LocalName.Equals("resp"))
                {
                    xmlReader.Read();                           //read text value
                    string value = xmlReader.Value;
                    responsible.ResponsibleType = new ResponsibleType
                    {
                        Text = value,
                        Type = ParseEnum<ResponsibleTypeEnum>(value)
                    };
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                    xmlReader.LocalName.Equals("name"))
                {
                    responsible.Text = GetInnerContentAsString(xmlReader);
                }
            }

            if (bookVersion.Responsibles == null) bookVersion.Responsibles = new List<Responsible>();
            bookVersion.Responsibles.Add(responsible);
        }
    }
}