using System.Collections.Generic;
using System.Linq;
using ITJakub.FileStorage.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class ExistDbStoreProcessor
    {
        public void Process(string bookId, string versionId, IList<Resource> resources)
        {
            List<Resource> transformations = resources.Where(resource => resource.ResourceType == ResourceTypeEnum.Transformation).ToList();
            foreach (Resource transformation in transformations)
            {
                //TODO save transformation to exist here
            }

            Resource book = resources.FirstOrDefault(resource => resource.ResourceType == ResourceTypeEnum.Book);
            //TODO save book to exist here

            List<Resource> pages = resources.Where(resource => resource.ResourceType == ResourceTypeEnum.Page).ToList();
            foreach (Resource page in pages)
            {
                //TODO save page to exist here
            }
        }
    }
}