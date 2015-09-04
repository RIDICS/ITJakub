using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;
using log4net;

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

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            var position = bookVersion.Accessories.Count + 1;

            var typeString = xmlReader.GetAttribute("type");
            var fileName = xmlReader.GetAttribute("name");

            if (string.IsNullOrEmpty(typeString) && m_log.IsFatalEnabled)
                m_log.ErrorFormat("Metadata_processor : Accessory in position {0} does not have type attribute", position);

            var type = GetTypeByTypeString(typeString);


            var accessory = new BookAccessory
            {
                FileName = fileName,
                BookVersion = bookVersion,
                Type = type
            };

            bookVersion.Accessories.Add(accessory);            
        }

        private AccessoryType GetTypeByTypeString(string typeString)
        {
            switch (typeString)
            {
                case "cover":
                    return AccessoryType.Cover;
                case "editorial-comment":
                    return AccessoryType.EditoralComment;
                case "bibliography":
                    return AccessoryType.Bibliography;

                default:
                    return AccessoryType.Unknown;
            }
        }        
    }
}