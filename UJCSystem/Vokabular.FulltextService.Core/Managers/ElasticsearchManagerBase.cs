using Microsoft.Extensions.Options;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.FulltextService.Core.Options;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public abstract class ElasticsearchManagerBase
    {
        protected readonly CommunicationProvider CommunicationProvider;
        private readonly IOptions<IndicesOption> m_indicesOptions;

        protected const string PageType = "page";
        protected const string SnapshotType = "snapshot";
        protected const string SnapshotIdField = "snapshotId";
        protected const string ProjectIdField = "projectId";
        protected const string PageTextField = "pageText";
        protected const string SnapshotTextField = "snapshotText";
        protected const string IdField = "_id";

        private const string TitleField = "title";
        private const string AuthorField = "authorsLabel";
        private const string DatingField = "originDate";
        
        protected ElasticsearchManagerBase(CommunicationProvider communicationProvider, IOptions<IndicesOption> indicesOptions)
        {
            CommunicationProvider = communicationProvider;
            m_indicesOptions = indicesOptions;
        }

        protected string SnapshotIndex => m_indicesOptions.Value.SnapshotIndex;

        protected string PageIndex => m_indicesOptions.Value.PageIndex;

        protected string GetElasticFieldName(SortTypeEnumContract sortValue)
        {
            switch (sortValue)
            {
                case SortTypeEnumContract.Title:
                    return TitleField;
                case SortTypeEnumContract.Author:
                    return AuthorField;
                case SortTypeEnumContract.Dating:
                    return DatingField;
                default:
                    return TitleField; 
            }
        }
    }
}