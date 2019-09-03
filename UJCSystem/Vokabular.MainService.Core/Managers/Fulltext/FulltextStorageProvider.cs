using System;
using System.Collections.Generic;
using System.Linq;
using Vokabular.DataEntities.Database.Entities.Enums;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Core.Managers.Fulltext
{
    public class FulltextStorageProvider
    {
        private readonly Dictionary<FulltextStorageType, IFulltextStorage> m_fulltextStorages;

        public FulltextStorageProvider(IFulltextStorage[] fulltextStorages)
        {
            m_fulltextStorages = fulltextStorages.ToDictionary(x => x.StorageType);
        }

        public IFulltextStorage GetFulltextStorage(ProjectTypeContract projectType)
        {
            return GetFulltextStorage((ProjectTypeEnum) projectType);
        }

        public IFulltextStorage GetFulltextStorage(ProjectTypeEnum projectType)
        {
            var storageType = GetStorageType(projectType);
            return m_fulltextStorages[storageType];
        }

        public FulltextStorageType GetStorageType(ProjectTypeEnum projectType)
        {
            switch (projectType)
            {
                case ProjectTypeEnum.Research:
                    return FulltextStorageType.ExistDb;
                case ProjectTypeEnum.Community:
                    return FulltextStorageType.ElasticSearch;
                case ProjectTypeEnum.Bibliography:
                    throw new ArgumentException("Bibliography project doesn't have any text or fulltext storage");
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectType), projectType, null);
            }
        }
    }
}
