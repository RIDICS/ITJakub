using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;

namespace ITJakub.FileProcessing.Core.Sessions
{
    public class ResourceProcessorManager
    {
        private readonly IResourceProcessor m_docXConverter;
        

        public ResourceProcessorManager(IResourceProcessor docXConverter)
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