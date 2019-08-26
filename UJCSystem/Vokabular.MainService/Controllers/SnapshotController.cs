using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    public class SnapshotController : Controller
    {
        [HttpPost("project/{projectId}/snapshot")]
        public long CreateSnapshot(long projectId)
        {
            return 73;
        }

        [HttpGet("project/{projectId}/snapshot")]
        public List<SnapshotAggregatedInfoContract> GetSnapshotList(long projectId)
        {
            return new List<SnapshotAggregatedInfoContract>
            {
                MockDataSnapshot.GetSnapshot(1),
                MockDataSnapshot.GetSnapshot(2),
                MockDataSnapshot.GetSnapshot(3),
            };
        }
    }

    public class MockDataSnapshot
    {
        public static SnapshotAggregatedInfoContract GetSnapshot(long id)
        {
            return new SnapshotAggregatedInfoContract
            {
                Id = 5,
                PublishDate = DateTime.Now,
                Author = "Jan Novák",
                ResourcesInfo = new List<SnapshotResourcesInfoContract>
                {
                    GetSnapshotResourcesInfo(ResourceTypeEnumContract.Text, 3, 3),
                    GetSnapshotResourcesInfo(ResourceTypeEnumContract.Image, 30, 30),
                    GetSnapshotResourcesInfo(ResourceTypeEnumContract.Audio, 1, 1),
                    GetSnapshotResourcesInfo(ResourceTypeEnumContract.Video, 0, 0)
                }
            };
        }

        private static SnapshotResourcesInfoContract GetSnapshotResourcesInfo(ResourceTypeEnumContract resourceType, int publishedCount, int totalCount)
        {
            return new SnapshotResourcesInfoContract
            {
                ResourceType = resourceType,
                PublishedCount = publishedCount,
                TotalCount = totalCount
            };
        }
    }
}
