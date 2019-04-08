using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Vokabular.ProjectParsing.Model.Entities;

namespace Vokabular.ProjectImport.Model
{
    public interface IProjectImportManager
    {
        string ExternalRepositoryTypeName { get; }

        Task ImportFromResource(string repository, ITargetBlock<object> buffer, RepositoryImportProgressInfo progressInfo,
            DateTime? lastImport = null, CancellationToken cancellationToken = default(CancellationToken));

        ImportedRecord ParseResponse(object response);
    }
}