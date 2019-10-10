using System.Collections.Generic;
using System.IO;

namespace Vokabular.TextConverter.Markdown.Extensions
{
    public class MarkdownHeadingAnalyzer
    {
        public IList<MarkdownHeadingData> FindAllHeadings(string text)
        {
            var resultList = new List<MarkdownHeadingData>();

            using (var stringReader = new StringReader(text))
            {
                string line;
                while ((line = stringReader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (!line.StartsWith("#"))
                    {
                        continue;
                    }

                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] == '#')
                        {
                            continue;
                        }
                        
                        if (char.IsWhiteSpace(line[i]))
                        {
                            var headerData = new MarkdownHeadingData
                            {
                                Level = i,
                                Heading = line.Substring(i).Trim(),
                            };
                            resultList.Add(headerData);
                        }

                        break;
                    }
                }
            }

            return resultList;
        }
    }

    public class MarkdownHeadingData
    {
        public int Level { get; set; }
        public string Heading { get; set; }
    }
}
