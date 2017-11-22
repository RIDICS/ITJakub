using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared;

namespace Vokabular.FulltextService.Controllers
{
    [Route("api/[controller]")]
    public class SnapshotController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<TextController>();

        private readonly SnapshotResourceManager m_snapshotResourceManager;
        private readonly TextResourceManager m_textResourceManager;

        public SnapshotController(SnapshotResourceManager snapshotResourceManager, TextResourceManager textResourceManager)
        {
            m_snapshotResourceManager = snapshotResourceManager;
            m_textResourceManager = textResourceManager;
        }

        [HttpPost]
        public ResultContract CreateSnapshot([FromBody] SnapshotPageIdsResourceContract snapshotPageIdsResourceContract)
        {
            StringBuilder snapshotBuilder = new StringBuilder();
            StringBuilder pageBuilder = new StringBuilder();

            var pages = new List<SnapshotPageResourceContract>();
            int indexInSnapshot = 0;

            foreach (var pageId in snapshotPageIdsResourceContract.PageIds)
            {
                var textResource = m_textResourceManager.GetTextResource(pageId);

                var pageText = textResource.PageText;
                pageBuilder.Append($"<{indexInSnapshot}>");

                var page = new SnapshotPageResourceContract {Id = pageId, indexFrom = indexInSnapshot};

                int indexInPage = 0;
                while (indexInPage < pageText.Length - 20)
                {
                    var indexOf = pageText.IndexOf(" ", indexInPage + 20, StringComparison.Ordinal);
                    if (indexOf == -1)
                    {
                        break;
                    }
                    pageBuilder.Append(pageText.Substring(indexInPage, indexOf - indexInPage));
                    indexInPage = indexOf;
                    pageBuilder.Append($"<{indexInSnapshot + indexInPage}>");
                }

                int size = pageText.Length - indexInPage - 1;
                if (size > 0)
                {
                    pageBuilder.Append(pageText.Substring(indexInPage, size));
                    indexInPage += size;
                }
                indexInSnapshot += indexInPage;
                pageBuilder.Append($"<{indexInSnapshot}>");
                page.indexTo = indexInSnapshot++;
                pages.Add(page);
                snapshotBuilder.Append(pageBuilder);
                pageBuilder.Clear();
            }
            var snapShotResource = new SnapshotResourceContract{SnapshotId = snapshotPageIdsResourceContract.SnapshotId, ProjectId = snapshotPageIdsResourceContract.ProjectId, SnapshotText = snapshotBuilder.ToString(), Pages = pages};
            var result = m_snapshotResourceManager.CreateSnapshotResource(snapShotResource);
            return result;

        }
    }
}