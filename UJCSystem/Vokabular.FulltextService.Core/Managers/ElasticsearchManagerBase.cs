using System;
using Elasticsearch.Net;
using Vokabular.FulltextService.Core.Communication;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Managers
{
    public abstract class ElasticsearchManagerBase
    {
        protected readonly CommunicationProvider CommunicationProvider;
        protected const string SnapshotIndex = "snapshotindex"; 
        protected const string PageIndex = "pageindex"; 
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

        protected ElasticsearchManagerBase(CommunicationProvider communicationProvider)
        {
            CommunicationProvider = communicationProvider;
        }

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