using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ITJakub.Core.Resources;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class TransformationsProcessor : IResourceProcessor {

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CategoryRepository m_categoryRepository;

        public TransformationsProcessor(CategoryRepository categoryRepository)
        {
            m_categoryRepository = categoryRepository;
        }


        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            var bookVersion = resourceSessionDirector.GetSessionInfoValue<BookVersion>(SessionInfo.BookVersionEntity);

            var trans = resourceSessionDirector.Resources.Where(x => x.ResourceType == ResourceType.Transformation);
            if (bookVersion.Transformations == null)
            {
                bookVersion.Transformations = new List<Transformation>();
            }

            foreach (var transResource in trans)
            {
                var transformation = GetTransformationObject(transResource);
                bookVersion.Transformations.Add(transformation);
            }

        }

        private Transformation GetTransformationObject(Resource resource)
        {
            const string logFormat = "{0} processing instruction in XSLT document not found.";
            const string logValidValueFormat =
                "{0} processing instruction in XSLT document is not valid. Current value: {1}";

            const string itjBookType = "itj-book-type";
            const string itjOutputFormat = "itj-output-format";

            var transformation = new Transformation
            {
                IsDefaultForBookType = false,
                Description = string.Empty,
                Name = resource.FileName,
                OutputFormat = OutputFormat.Html,
                ResourceLevel = ResourceLevel.Book //TODO add support for version?
            };

            var document = XDocument.Load(resource.FullPath);

            var bookType = (from node in document.Root.Nodes()
                where
                    node.NodeType == XmlNodeType.ProcessingInstruction &&
                    ((XProcessingInstruction)node).Target == itjBookType
                select (XProcessingInstruction)node).FirstOrDefault();

            var transformationBookType = GetTransformationBookType(bookType, logFormat, logValidValueFormat);

            var outputFormat = (from node in document.Root.Nodes()
                where
                    node.NodeType == XmlNodeType.ProcessingInstruction &&
                    ((XProcessingInstruction)node).Target == itjOutputFormat
                select (XProcessingInstruction)node).FirstOrDefault();


            var transformationOutputFormat = GetTransformationOutputFormat(outputFormat, logFormat, logValidValueFormat);


            transformation.BookType = transformationBookType;
            transformation.OutputFormat = transformationOutputFormat;
            return transformation;
        }

        private static OutputFormat GetTransformationOutputFormat(XProcessingInstruction outputFormat, string logFormat,
            string logValidValueFormat)
        {
            var transformationOutputFormat = OutputFormat.Unknown;
            const string outputformatName = "OutputFormat";
            const string invalidXsltTransformation = "Xslt tranformation without output format processing instruction.";

            if (outputFormat == null)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(logFormat, outputformatName);
                throw new InvalidXsltTransformationException(invalidXsltTransformation);
            }
            OutputFormat value;
            if (Enum.TryParse(outputFormat.Data.Trim(), true, out value))
            {
                transformationOutputFormat = value;
            }
            else
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(logValidValueFormat, outputformatName, outputFormat.Data);
                throw new InvalidEnumArgumentException();
            }
            return transformationOutputFormat;
        }

        private BookType GetTransformationBookType(XProcessingInstruction bookType, string logFormat,
            string logValidValueFormat)
        {
            BookType transformationBookType = null;
            const string bookTypeName = "BookType";
            const string invalidXsltTransformation = "Xslt tranformation without book type processing instruction.";
            if (bookType == null)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(logFormat, bookTypeName);

                throw new InvalidXsltTransformationException(invalidXsltTransformation);
            }
            BookTypeEnum value;
            if (Enum.TryParse(bookType.Data.Trim(), true, out value))
            {
                transformationBookType = m_categoryRepository.FindBookTypeByType(value);
            }
            else
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(logValidValueFormat, bookTypeName, bookType.Data);
                throw new InvalidEnumArgumentException();
            }
            return transformationBookType;
        }
    }
}