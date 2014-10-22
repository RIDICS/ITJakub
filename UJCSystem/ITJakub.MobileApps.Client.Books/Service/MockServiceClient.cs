using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Service
{
    public class MockServiceClient : ServiceClient
    {
        public MockServiceClient()
        {
        }

        public new Task<IList<BookContract>> GetBookListAsync(CategoryContract category)
        {
            return Task.Run(async () =>
            {
                
                await Task.Delay(1000);
                var list = new List<BookContract>
                {
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = 1,
                        Title = "Název knihy",
                        Year = 1800
                    },
                    new BookContract
                    {
                        Author = "Nějaký Autor",
                        Guid = 1,
                        Title = "Velmi dlouhý název knihy",
                        Year = 1870
                    },
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = 1,
                        Title = "Název knihy",
                        Year = 1800
                    },
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = 1,
                        Title = "Název knihy",
                        Year = 1800
                    },
                };

                return (IList<BookContract>) list;
            });
        }

        public new Task<IList<BookContract>> SearchForBookAsync(CategoryContract category, SearchDestinationContract searchBy, string query)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(1000);
                IList<BookContract> list = new List<BookContract>
                {
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = 1,
                        Title = "Název knihy " + query,
                        Year = 1800
                    },
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = 1,
                        Title = "Název "+ query +" knihy",
                        Year = 1800
                    },
                };
                return list;
            });
        }

        public new Task<IList<string>> GetPageListAsync(string bookGuid)
        {
            return Task.Run(() =>
            {
                IList<string> list = new List<string>
                {
                    "1L", "1R", "2L", "2R", "3L", "3R", "4L", "4R", "5L", "5R", "6L", "6R", "7L", "7R", "8L", "8R", "9L", "9R",
                    "10L", "10R", "11L", "12R",
                };
                return list;
            });
        }

        public new Stream GetPageAsRtf(string bookGuid, string pageId)
        {
            Stream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(@"{\rtf1\ansi{\fonttbl\f0\fswiss Helvetica;}\f0\pard Toto je {\b tucny} text.\par}");
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public new Stream GetPagePhoto(string bookGuid, string pageId)
        {
            return null;
        }
    }
}
