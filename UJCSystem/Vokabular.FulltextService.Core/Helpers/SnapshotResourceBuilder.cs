using System.Collections.Generic;
using System.Text;
using Vokabular.FulltextService.Core.Helpers.Converters;
using Vokabular.FulltextService.Core.Helpers.Markdown;
using Vokabular.FulltextService.Core.Managers;
using Vokabular.FulltextService.DataContracts.Contracts;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.FulltextService.Core.Helpers
{
    public class SnapshotResourceBuilder
    {
        private readonly TextResourceManager m_textResourceManager;
        private readonly ITextConverter m_textConverter;
        private readonly IMarkdownToPlainTextConverter m_markdownToPlainTextConverter;
        private const int MinSnippetSize = 15;

        public SnapshotResourceBuilder(TextResourceManager textResourceManager, ITextConverter textConverter, IMarkdownToPlainTextConverter markdownToPlainTextConverter)
        {
            m_textResourceManager = textResourceManager;
            m_textConverter = textConverter;
            m_markdownToPlainTextConverter = markdownToPlainTextConverter;
        }

        public SnapshotResourceContract GetSnapshotResourceFromPageIds(List<string> pageIds) {
            StringBuilder snapshotBuilder = new StringBuilder();
            
            var pages = new List<SnapshotPageResourceContract>();
            int pageIndex = 0;

            foreach (var pageId in pageIds)
            {
                var textResource = m_textResourceManager.GetTextResource(pageId);

                var pageTextWithoutMarkdown = m_markdownToPlainTextConverter.Convert(textResource.PageText);
                
                var pageTextWithIndex = InsertPageIndexIntoPageText(pageTextWithoutMarkdown, pageIndex);
                var page = new SnapshotPageResourceContract { Id = pageId, PageIndex = pageIndex};
                
                pages.Add(page);
                snapshotBuilder.Append(pageTextWithIndex);

                pageIndex++;
            }
            return new SnapshotResourceContract { SnapshotText = snapshotBuilder.ToString(), Pages = pages };
        }

        private string InsertPageIndexIntoPageText(string pageText, int pageIndex)
        {
            StringBuilder pageBuilder = new StringBuilder();
            
            int index = 0;
            while (index < pageText.Length)
            {
                int counter;
                for (counter = 0; index + counter < pageText.Length; counter++)
                {
                    if (counter + index == pageText.Length - 1)
                    {
                        pageBuilder.Append(pageText.Substring(index, counter));
                        pageBuilder.Append($" <{pageIndex}> ");
                        return pageBuilder.ToString();
                    }

                    if (counter >= MinSnippetSize) //To insert pageindex into whitespace add this condition:  && pageText[index + counter].Equals(' ')
                    {
                        pageBuilder.Append($" <{pageIndex}> ");
                        pageBuilder.Append(pageText.Substring(index, counter));
                        break;
                    }

                }
                index += counter;
            }

            return pageBuilder.ToString();
        }
    }
}