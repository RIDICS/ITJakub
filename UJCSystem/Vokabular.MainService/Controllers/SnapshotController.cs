﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Vokabular.MainService.Core.Managers;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace Vokabular.MainService.Controllers
{
    [Route("api")]
    public class SnapshotController : Controller
    {
        private readonly SnapshotManager m_snapshotManager;

        public SnapshotController(SnapshotManager snapshotManager)
        {
            m_snapshotManager = snapshotManager;
        }

        [HttpPost("project/{projectId}/snapshot")]
        public long CreateSnapshot(long projectId, [FromBody] CreateSnapshotContract data)
        {
            return m_snapshotManager.CreateSnapshot(projectId, data);
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
                    GetSnapshotResourcesInfo(ResourceTypeEnumContract.Text, 3),
                    GetSnapshotResourcesInfo(ResourceTypeEnumContract.Image, 30),
                    GetSnapshotResourcesInfo(ResourceTypeEnumContract.Audio, 1)
                }
            };
        }

        private static SnapshotResourcesInfoContract GetSnapshotResourcesInfo(ResourceTypeEnumContract resourceType, int publishedCount)
        {
            return new SnapshotResourcesInfoContract
            {
                ResourceType = resourceType,
                PublishedCount = publishedCount,
            };
        }
    }
}
