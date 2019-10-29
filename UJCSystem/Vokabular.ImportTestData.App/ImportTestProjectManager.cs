using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLipsum.Core;
using Vokabular.MainService.DataContracts.Clients;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.ImportTestData.App
{
    public class ImportTestProjectManager
    {
        private readonly MainServiceProjectClient m_projectClient;
        private readonly MainServiceSnapshotClient m_snapshotClient;
        private readonly Random m_random;
        private readonly string[] m_dataSets = {
            Lipsums.LoremIpsum,
            Lipsums.ChildHarold,
            Lipsums.Decameron,
            Lipsums.Faust,
            Lipsums.InDerFremde,
            Lipsums.LeBateauIvre,
            Lipsums.LeMasque,
            Lipsums.NagyonFaj,
            Lipsums.Omagyar,
            Lipsums.RobinsonoKruso,
            Lipsums.TheRaven,
            Lipsums.TierrayLuna,
        };
        
        public ImportTestProjectManager(MainServiceProjectClient projectClient, MainServiceSnapshotClient snapshotClient)
        {
            m_projectClient = projectClient;
            m_snapshotClient = snapshotClient;
            m_random = new Random();
        }

        public Task<ImportResult> ImportAsync(int index)
        {
            return Task.Run(() => Import(index));
        }

        public ImportResult Import(int index)
        {
            var startTime = DateTime.UtcNow;

            var projectId = m_projectClient.CreateProject(new CreateProjectContract
            {
                Name = $"Test project {index}",
                ProjectType = ProjectTypeContract.Community,
                BookTypes = new List<BookTypeEnumContract> {BookTypeEnumContract.Edition},
            });

            // Metadata
            m_projectClient.CreateNewProjectMetadataVersion(projectId, new ProjectMetadataContract
            {
                Authors = "Test Author",
                Title = $"Test project {index}",
                NotBefore = new DateTime(1947),
                NotAfter = new DateTime(1953),
                OriginDate = "okolo roku 1950",
                RelicAbbreviation = $"TP{index}",
                SourceAbbreviation = $"TP{index}",
            });

            // Pages
            var pageCount = m_random.Next(10, 120);
            var createPageList = new List<CreateOrUpdatePageContract>();
            for (int i = 1; i <= pageCount; i++)
            {
                createPageList.Add(new CreateOrUpdatePageContract
                {
                    Name = i.ToString(),
                    Position = i,
                });
            }

            m_projectClient.SetAllPageList(projectId, createPageList);

            // Texts
            var pages = m_projectClient.GetAllPageList(projectId);
            var rawDataSet = m_dataSets[index % m_dataSets.Length];
            var totalTextLength = 0;
            foreach (var pageContract in pages)
            {
                var text = LipsumGenerator.Generate(2, rawDataSet);
                totalTextLength += text.Length;

                m_projectClient.CreateTextResource(pageContract.Id, new CreateTextRequestContract
                {
                    Text = text,
                });
            }

            // Snapshot
            var texts = m_projectClient.GetAllTextResourceList(projectId, null);
            m_snapshotClient.CreateSnapshot(new CreateSnapshotContract
            {
                ProjectId = projectId,
                DefaultBookType = BookTypeEnumContract.TextBank,
                BookTypes = new List<BookTypeEnumContract> {BookTypeEnumContract.Edition, BookTypeEnumContract.TextBank},
                Comment = "Test data imported from random test data generator application",
                ResourceVersionIds = texts.Select(x => x.VersionId).ToList(),
            });

            return new ImportResult
            {
                ProjectId = projectId,
                PageCount = pageCount,
                TextLength = totalTextLength,
                Time = DateTime.UtcNow - startTime,
            };
        }
    }

    public class ImportResult
    {
        public long ProjectId { get; set; }
        public int PageCount { get; set; }
        public long TextLength { get; set; }
        public TimeSpan Time { get; set; }
    }
}