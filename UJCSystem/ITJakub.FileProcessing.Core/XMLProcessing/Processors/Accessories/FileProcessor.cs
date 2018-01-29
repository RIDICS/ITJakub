using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;
using Vokabular.DataEntities.Database.Entities.Enums;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Accessories
{
    public class FileProcessor : ProcessorBase
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public FileProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "file"; }
        }

        protected override IEnumerable<ProcessorBase> SubProcessors { get {return new List<ProcessorBase>();} }

        protected override void ProcessAttributes(BookData bookData, XmlReader xmlReader)
        {
            var position = bookData.Accessories.Count + 1;

            var typeString = xmlReader.GetAttribute("type");
            var fileName = xmlReader.GetAttribute("name");

            if (string.IsNullOrEmpty(typeString) && m_log.IsFatalEnabled)
                m_log.ErrorFormat("Metadata_processor : Accessory in position {0} does not have type attribute", position);

            var type = GetTypeByTypeString(typeString);


            var accessory = new BookAccessoryData
            {
                FileName = fileName,
                Type = type
            };

            bookData.Accessories.Add(accessory);            
        }

        private AccessoryType GetTypeByTypeString(string typeString)
        {
            switch (typeString)
            {
                case "content":
                    return AccessoryType.Content;
                case "cover":
                    return AccessoryType.Cover;
                case "bibliography":
                    return AccessoryType.Bibliography;

                default:
                    return AccessoryType.Unknown;
            }
        }        
    }
}