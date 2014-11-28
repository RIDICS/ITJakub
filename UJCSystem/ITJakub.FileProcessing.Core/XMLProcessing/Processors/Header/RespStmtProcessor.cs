using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class RespStmtProcessor : ListProcessorBase
    {
        private readonly BookVersionRepository m_bookVersionRepository;

        public RespStmtProcessor(BookVersionRepository bookVersionRepository,
            XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_bookVersionRepository = bookVersionRepository;
        }

        protected override string NodeName
        {
            get { return "respStmt"; }
        }

        protected override void PreprocessSetup(BookVersion bookVersion)
        {
            if (bookVersion.Responsibles == null) bookVersion.Responsibles = new List<Responsible>();
        }

        protected override void ProcessElement(BookVersion bookVersion, XmlReader xmlReader)
        {
            var responsible = new Responsible();

            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.IsStartElement() &&
                    xmlReader.LocalName.Equals("resp"))
                {
                    xmlReader.Read(); //read text value
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

            responsible = m_bookVersionRepository.FindResponsible(responsible.Text, responsible.ResponsibleType) ?? responsible;
            bookVersion.Responsibles.Add(responsible);
        }
    }
}