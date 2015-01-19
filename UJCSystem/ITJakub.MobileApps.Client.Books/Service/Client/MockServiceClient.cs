using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Storage;
using ITJakub.MobileApps.MobileContracts;

namespace ITJakub.MobileApps.Client.Books.Service.Client
{
    public class MockServiceClient : IServiceClient
    {
        public Task<IList<BookContract>> GetBookListAsync(CategoryContract category)
        {
            return Task.Run(async () =>
            {
                
                await Task.Delay(2000);
                var list = new List<BookContract>
                {
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = "1",
                        Title = "Název knihy",
                        Year = 1800
                    },
                    new BookContract
                    {
                        Author = "Nějaký Autor",
                        Guid = "1",
                        Title = "Velmi dlouhý název knihy",
                        Year = 1870
                    },
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = "1",
                        Title = "Název knihy",
                        Year = 1800
                    },
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = "1",
                        Title = "Název knihy",
                        Year = 1800
                    }
                };

                return (IList<BookContract>) list;
            });
        }

        public Task<IList<BookContract>> SearchForBookAsync(CategoryContract category, SearchDestinationContract searchBy, string query)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(2000);
                IList<BookContract> list = new List<BookContract>
                {
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = "1",
                        Title = "Název knihy " + query,
                        Year = 1800
                    },
                    new BookContract
                    {
                        Author = "Jméno Příjmení",
                        Guid = "1",
                        Title = "Název "+ query +" knihy",
                        Year = 1800
                    }
                };
                return list;
            });
        }

        public Task<IList<string>> GetPageListAsync(string bookGuid)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(2000);
                IList<string> list = new List<string>
                {
                    "1L", "1R", "2L", "2R", "3L", "3R", "4L", "4R", "5L", "5R", "6L", "6R", "7L", "7R", "8L", "8R", "9L", "9R",
                    "10L", "10R", "11L", "12R"
                };
                return list;
            });
        }

        public Task<Stream> GetPageAsRtfAsync(string bookGuid, string pageId)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(2000);
                Stream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(@"{\rtf1\ansi{\fonttbl\f0\fswiss Helvetica;}\f0\pard Toto je {\b tucny} text.\par}");
                writer.Flush();
                stream.Position = 0;
                return stream;
            });
        }

        public Task<Stream> GetPagePhotoAsync(string bookGuid, string pageId)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(2000);
                
                var assemblyName = GetType().GetTypeInfo().Assembly.GetName().Name;
                var uri = new Uri(string.Format("ms-appx:///{0}/", assemblyName));

                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri, "Icon/book-48.png"));
                return await file.OpenStreamForReadAsync();
            });
        }
    }
}
