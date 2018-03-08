using System.Collections.Generic;
using System.Linq;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public class FulltextStorageProvider
    {
        private readonly Dictionary<ProjectType, IFulltextStorage> m_fulltextStorages;

        public FulltextStorageProvider(IFulltextStorage[] fulltextStorages)
        {
            m_fulltextStorages = fulltextStorages.ToDictionary(x => x.ProjectType);
        }

        public IFulltextStorage GetFulltextStorage(ProjectType projectType = ProjectType.Community)
        {
            return m_fulltextStorages[projectType];
        }
    }
}
