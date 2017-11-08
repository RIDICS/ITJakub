using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using ITJakub.FileProcessing.Core.Data;
using log4net;
using Vokabular.Core.Storage.Resources;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.Shared.DataContracts.Types;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class TransformationsProcessor : IResourceProcessor {

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void Process(ResourceSessionDirector resourceSessionDirector)
        {
            var bookData = resourceSessionDirector.GetSessionInfoValue<BookData>(SessionInfo.BookData);

            var trans = resourceSessionDirector.Resources.Where(x => x.ResourceType == ResourceType.Transformation);
            if (bookData.Transformations == null)
            {
                bookData.Transformations = new List<TransformationData>();
            }

            foreach (var transResource in trans)
            {
                var transformation = GetTransformationObject(transResource);
                bookData.Transformations.Add(transformation);
            }

        }

        private TransformationData GetTransformationObject(FileResource resource)
        {
            const string logFormat = "{0} processing instruction in XSLT document not found.";
            const string logValidValueFormat =
                "{0} processing instruction in XSLT document is not valid. Current value: {1}";

            const string itjBookType = "itj-book-type";
            const string itjOutputFormat = "itj-output-format";

            var transformation = new TransformationData
            {
                IsDefaultForBookType = false,
                Description = string.Empty,
                Name = resource.FileName,
                OutputFormat = OutputFormatEnum.Html,
                ResourceLevel = ResourceLevelEnum.Book //TODO add support for version?
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

        private static OutputFormatEnum GetTransformationOutputFormat(XProcessingInstruction outputFormat, string logFormat,
            string logValidValueFormat)
        {
            var transformationOutputFormat = OutputFormatEnum.Unknown;
            const string outputformatName = "OutputFormat";
            const string invalidXsltTransformation = "Xslt tranformation without output format processing instruction.";

            if (outputFormat == null)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(logFormat, outputformatName);
                throw new InvalidXsltTransformationException(invalidXsltTransformation);
            }
            OutputFormatEnum value;
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

        private BookTypeEnum GetTransformationBookType(XProcessingInstruction bookType, string logFormat,
            string logValidValueFormat)
        {
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
                return value;
            }
            else
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat(logValidValueFormat, bookTypeName, bookType.Data);
                throw new InvalidEnumArgumentException();
            }
        }
    }
}