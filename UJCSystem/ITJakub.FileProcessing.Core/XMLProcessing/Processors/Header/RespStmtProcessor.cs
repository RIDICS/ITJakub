using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
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

        protected override void PreprocessSetup(BookData bookData)
        {
            if (bookData.Responsibles == null) bookData.Responsibles = new List<ResponsibleData>();
        }

        protected override void ProcessElement(BookData bookData, XmlReader xmlReader)
        {
            var responsible = new ResponsibleData();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                    xmlReader.LocalName.Equals("resp"))
                {
                    xmlReader.Read(); //read text value
                    var responsibleTypeText = xmlReader.Value;
                    var responsibleTypeType = ParseEnum<ResponsibleTypeEnum>(responsibleTypeText);
                    var tmpResponsibleType = new ResponsibleTypeData
                    {
                        Text = responsibleTypeText,
                        Type = responsibleTypeType
                    };

                    responsible.ResponsibleType = tmpResponsibleType;
                }

                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                    xmlReader.LocalName.Equals("name"))
                {
                    responsible.Text = GetInnerContentAsString(xmlReader);
                }
            }

            bookData.Responsibles.Add(responsible);
        }
    }
}