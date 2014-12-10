using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceProcessorManger
    {
        private readonly DocxProcessor m_docXConverter;
        

        public ResourceProcessorManger(DocxProcessor docXConverter)
        {
            m_docXConverter = docXConverter;
            //TODO add other processors
        }        

        public bool ProcessSessionResources(List<Resource> resources)
        {
            //TODO call processor process method for processing resources (process images, files, xmlMetadataFile)
            return true;
        }
    }
}