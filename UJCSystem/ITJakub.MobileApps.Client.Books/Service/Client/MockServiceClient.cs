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
                        Authors = new[] {new AuthorContract{Name = "Jméno Příjmení"}},
                        Guid = "1",
                        Title = "Název knihy",
                        PublishDate = "1800"
                    },
                    new BookContract
                    {
                        Authors = new[] {new AuthorContract{Name = "Nějaký Autor"}},
                        Guid = "1",
                        Title = "Velmi dlouhý název knihy",
                        PublishDate = "1870"
                    },
                    new BookContract
                    {
                        Authors = new[] {new AuthorContract{Name = "Jméno Příjmení"}, new AuthorContract{Name = "Další Autor"}},
                        Guid = "1",
                        Title = "Název knihy",
                        PublishDate = "1800"
                    },
                    new BookContract
                    {
                        Authors = new[] {new AuthorContract{Name = "Jméno Příjmení"}},
                        Guid = "1",
                        Title = "Název knihy",
                        PublishDate = "1800"
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
                        Authors = new[] {new AuthorContract{Name = "Jméno Příjmení"}},
                        Guid = "1",
                        Title = "Název knihy " + query,
                        PublishDate = "1800"
                    },
                    new BookContract
                    {
                        Authors = new[] {new AuthorContract{Name = "Jméno Příjmení"}},
                        Guid = "1",
                        Title = "Název "+ query +" knihy",
                        PublishDate = "1800"
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

        public Task<string> GetPageAsRtfAsync(string bookGuid, string pageId)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(2000);
                return "{\\rtf1\\fbidis\\ansi\\ansicpg1252\\deff0\\nouicompat\\deflang1033{\\fonttbl{\\f0\\fnil\\fcharset0 Segoe UI;}{\\f1\\fnil\\fcharset238 Segoe UI;}}\r\n{\\colortbl ;\\red0\\green0\\blue0;}\r\n{\\*\\generator Riched20 6.3.9600}\\viewkind4\\uc1 \r\n\\pard\\ltrpar\\tx720\\cf1\\f0\\fs22 Tuto se po\\f1\\'e8\\'edn\\'e1 p\\'f8edmluva dosp\\'ecl\\'e9ho mu\\'9ee Gvidona z Kolumny mez\\'e1nsk\\'e9 na Kroniku troj\\'e1nsk\\'fa\\par\r\n\\par\r\nKterak\\'9ekoli d\\'e1vn\\'e9 v\\'ecci nov\\'fdmi druhdy zapadaj\\'ed, v\\'9aak n\\'ecker\\'e9 v\\'ecci d\\'e1vn\\'e9 d\\'e1vno pominuly js\\'fa, kter\\'e9\\'9eto sv\\'fa velikost\\'ed duostojny js\\'fa \\'9eiv\\'facie pam\\'ecti, tak\\'9ee ani jich vetchost slep\\'fdm uhryz\\'e1n\\'edm muo\\'9e zahladiti, ani minul\\'e9ho \\'e8asu vetch\\'e1 l\\'e9ta usnul\\'fa ml\\'e8edlivost\\'ed zav\\'f8ieti. Neb trvaj\\'ed v nich d\\'f8ieve d\\'e1l\\'fdch v\\'ecc\\'ed velebnosti \\'fastavn\\'e1 vzpom\\'ednanie, kdy\\'9eto od p\\'f8edkuov ku potomn\\'edm \\'f8e\\'e8 b\\'fdv\\'e1 v\\'ecrn\\'fdm p\\'edsmem pops\\'e1na, a d\\'e1vn\\'fdch sklada\\'e8uov v\\'ecrn\\'e1 pops\\'e1nie, kter\\'e1\\'9eto sv\\'fdm zachov\\'e1n\\'edm minul\\'e9 jako\\'9eto p\\'f8\\'edtomn\\'e9 miern\\'ecjie okazuj\\'ed, a mu\\'9euov state\\'e8n\\'fdch, kter\\'e9\\'9eto dl\\'fah\\'fd v\\'eck sv\\'ecta d\\'e1vno skrze smrt pohltil, bedliv\\'fdm \\'e8\\'edtan\\'edm knih \\'9eiv\\'e9 obrazy na\\'9aim duch\\'f3m oznamuje.\\par\r\n\\par\r\nProto\\'9e troj\\'e1nsk\\'e9ho m\\'ecsta zru\\'9aenie nenie hodn\\'e9, by kter\\'e9ho dl\\'fah\\'e9ho \\'e8asu vetchost\\'ed bylo zahlazeno, jedno aby ustavi\\'e8n\\'fdm zpom\\'ednan\\'edm kvetlo na mysli lidsk\\'e9, mnoh\\'fdch p\\'edsa\\'f8uov ruka v\\'ecrn\\'fdm p\\'edsmem popsala jest. Mnoz\\'ed tak\\'e9 sklada\\'e8i t\\'e9to p\\'f8\\'edhody st\\'e1l\\'fa pravdu oby\\'e8ejem hercov\\'fdm v rozli\\'e8n\\'e1 podobenstvie s\\'fa p\\'f8etva\\'f8ovali, tak\\'9ee s\\'fa shled\\'e1ni v nepravd\\'ec, ale vymy\\'9alen\\'e9 b\\'e1sn\\'ec sepsav\\'9ae. Mezi nimi\\'9eto za sv\\'fdch dn\\'f3v p\\'f8evelik\\'e9 vz\\'e1cnosti Om\\'e9rus, skladatel \\'f8eck\\'fd, ty d\\'e1l\\'e9 v\\'ecci, p\\'fah\\'fa a prost\\'fa pravdu, v chytr\\'e9 rozpr\\'e1vky prom\\'ecnil jest, zam\\'fd\\'9aleje mnoh\\'e9 v\\'ecci, kter\\'e9\\'9e js\\'fa se ned\\'e1ly, a kter\\'e9\\'9e js\\'fa se d\\'e1ly, jinak p\\'f8etva\\'f8uje. Neb jest pravil, \\'9ee by bohov\\'e9, jim\\'9e se jest klan\\'ecla d\\'e1vn\\'e1 zpohanilost \\'f8eck\\'e1, oni troj\\'e1nsk\\'e9ho m\\'ecsta dob\\'fdvali, a v lidsk\\'e9 tv\\'e1rnosti bojuj\\'edce, mnoz\\'ed z nich se\\'9ali. Jeho\\'9eto bludu mnoz\\'ed chytrci v\\'9aete\\'e8n\\'ec n\\'e1sleduj\\'edce, dali s\\'fa vtipn\\'fdm srozum\\'ecti, \\'9ee Om\\'e9rus byl sklada\\'e8 b\\'e1sniv\\'fdch z\\'e1mysluov; a oni v tom jeho n\\'e1sledovn\\'edci byli, p\\'ed\\'9a\\'edce knihy rozli\\'e8n\\'e9. Proto\\'9e Ovidius sulmonensk\\'fd rukotr\\'9en\\'fdm skl\\'e1dan\\'edm ve mnoh\\'fdch knih\\'e1ch ob\\'e9 jest zuosnoval a mnoho z\\'e1mysluov k z\\'e1mysl\\'f3m p\\'f8i\\'e8inil, a mezi tiem n\\'eckde druhdy pravdy neop\\'fa\\'9at\\'ecje. Tak\\'e9[b] Virgilius v knih\\'e1ch sv\\'fdch Eneidorum[c] p\\'ed\\'9ae, jak\\'9ekoli v\\'9edy, kdy\\'9e se skutkuov troj\\'e1nsk\\'fdch dotekl, sv\\'ectle pravdu. Av\\'9aak od druh\\'fdch z\\'e1mysluov Om\\'e9rov\\'fdch necht\\'ecl sv\\'e9 ruky zdr\\'9eeti.\\par\r\n\\par\r\nNe\\'9e aby v\\'ecrn\\'fdch p\\'edsa\\'f8uov prav\\'e1 p\\'edsma o t\\'ecch d\\'e1l\\'fdch v\\'eccech mezi lidmi na z\\'e1pad slunce p\\'f8eb\\'fdvaj\\'edc\\'edm v bud\\'fac\\'edch \\'e8asiech v\\'ec\\'e8n\\'ec zuostala k \\'fa\\'9eitku, zvl\\'e1\\'9at\\'ec t\\'ecm, je\\'9ato kroniky rozli\\'e8n\\'e9 \\'e8\\'edtaj\\'ed, aby um\\'ecli rozeznati pravdu od k\\'f8ivdy, a ku prosp\\'ecchu udatenstvie lidu rytie\\'f8sk\\'e9ho, ty v\\'ecci, kter\\'e9\\'9e skrze Dita \\'f8e\\'e8sk\\'e9ho a Dana troj\\'e1nsk\\'e9ho js\\'fa pops\\'e1ny, kte\\'f8\\'ed\\'9eto v ty \\'e8asy byli p\\'f8\\'edtomni u vojsk\\'e1ch, a co js\\'fa o\\'e8it\\'ec spat\\'f8ili, to js\\'fa v\\'ecrn\\'ec popsali, a skrze m\\'ec, s\\'fadci Gvidona z Kolumny mez\\'e1nsk\\'e9, v tyto knihy js\\'fa seps\\'e1ny, jako\\'9e to v jich\\f0\\par\r\n\r\n\\pard\\ltrpar\\tx720\\par\r\n}\r\n\u0000";
            });
        }

        public Task<Stream> GetPagePhotoAsync(string bookGuid, string pageId)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(2000);
                
                var assemblyName = GetType().GetTypeInfo().Assembly.GetName().Name;
                var uri = new Uri(string.Format("ms-appx:///{0}/", assemblyName));

                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri, "Icon/pagePhotoMock.jpg"));
                return await file.OpenStreamForReadAsync();
            });
        }
    }
}
