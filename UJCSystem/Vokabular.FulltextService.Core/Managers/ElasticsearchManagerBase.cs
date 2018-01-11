using System;
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
        protected const string TitleField = "title";
        protected const string AuthorField = "author";
        protected const string DatingField = "dating";

        private const string BadSortValueErrorMessage = "Bad sorting value";

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
                    return TitleField; //TODO throw exception or set default sorting
                    throw new ArgumentException(BadSortValueErrorMessage);
            }
        }
    }
}