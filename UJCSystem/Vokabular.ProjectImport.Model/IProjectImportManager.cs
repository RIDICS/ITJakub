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

        /// <summary>
        /// Start importing from an external resource, which is specified in the configuration. Post data to buffer.
        /// </summary>
        /// <param name="configuration">Configuration of external repository</param>
        /// <param name="buffer"></param>
        /// <param name="progressInfo"></param>
        /// <param name="lastImport"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task ImportFromResource(string configuration, ITargetBlock<object> buffer, RepositoryImportProgressInfo progressInfo,
            DateTime? lastImport = null, CancellationToken cancellationToken = default(CancellationToken));
        
        /// <summary>
        /// Method parses the response and creates ImportedRecord, which contains RawData and record TimeStamp.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        ImportedRecord ParseResponse(object response);
    }
}