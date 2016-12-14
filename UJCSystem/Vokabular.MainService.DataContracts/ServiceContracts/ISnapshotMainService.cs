using System.Collections.Generic;
using Vokabular.MainService.DataContracts.Contracts;

namespace Vokabular.MainService.DataContracts.ServiceContracts
{
    public interface ISnapshotMainService
    {
        long CreateSnapshot(long projectId);

        List<SnapshotContract> GetSnapshotList(long projectId);
    }
}