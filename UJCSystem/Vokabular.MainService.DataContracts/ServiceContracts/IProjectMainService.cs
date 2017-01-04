using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.DataContracts.ServiceContracts
{
    public interface IProjectMainService
    {
        List<ProjectContract> GetProjectList(int? start, int? count);

        ProjectContract GetProject(long projectId);

        long CreateProject(ProjectContract project);

        void DeleteProject(long projectId);

        ProjectMetadataContract GetProjectMetadata(long projectId);
    }
}
