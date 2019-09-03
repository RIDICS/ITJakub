using System.Collections.Generic;
using System.Linq;
using ITJakub.FileProcessing.DataContracts;

namespace ITJakub.FileProcessing.Core.Sessions.Processors.Fulltext
{
    public class FulltextStoreProcessorProvider
    {
        private readonly Dictionary<FulltextStoreTypeContract, IFulltextResourceProcessor> m_fulltextResourceProcessors;

        public FulltextStoreProcessorProvider(IList<IFulltextResourceProcessor> fulltextResourceProcessors)
        {
            m_fulltextResourceProcessors = fulltextResourceProcessors.ToDictionary(x => x.StoreType);
        }

        public IFulltextResourceProcessor GetByStoreType(FulltextStoreTypeContract storeType)
        {
            return m_fulltextResourceProcessors[storeType];
        }
    }
}