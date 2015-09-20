using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ITJakub.MobileApps.Client.Core.ViewModel.News;

namespace ITJakub.MobileApps.Client.Core.Manager.News
{
    public class NewsManager
    {
             
        public Task<List<SyndicationItemViewModel>> GetAllNews()
        {
            var client = new MobileAppsNewsClient();            

            //return Task.Factory.StartNew(() =>   new List<SyndicationItemViewModel>
            //{
            //    new SyndicationItemViewModel
            //    {
            //        CreateDate = DateTime.Now,
            //        Title = "Byla zveřejněna nová kniha",
            //        Text =
            //            "Byla zveřejněna nová kniha Jungmanův slovník pro uživatele mobilních aplikací. Jungmanův slovník zveřejněn i ve formě obrazové formy jednotlivých stránek",
            //        Url = "http://censeo2.felk.cvut.cz",
            //        UserEmail = "t@t.t",
            //        UserFirstName = "test",
            //        UserLastName = "testovaci"
            //    },
            //    new SyndicationItemViewModel
            //    {
            //        CreateDate = DateTime.Now,
            //        Title = "Nové ikony pro hangmana",
            //        Text = "Nové ikony vytvořeny speciálně pro mobilní aplikaci staročeská šibenice.",
            //        Url = "http://censeo2.felk.cvut.cz",
            //        UserEmail = "t@t.t",
            //        UserFirstName = "test",
            //        UserLastName = "testovaci"
            //    },        new SyndicationItemViewModel
            //    {
            //        CreateDate = DateTime.Now,
            //        Title = "Nové ikony pro hangmana",
            //        Text = "Nové ikony vytvořeny speciálně pro mobilní aplikaci staročeská šibenice.",
            //        Url = "http://censeo2.felk.cvut.cz",
            //        UserEmail = "t@t.t",
            //        UserFirstName = "test",
            //        UserLastName = "testovaci"
            //    },        new SyndicationItemViewModel
            //    {
            //        CreateDate = DateTime.Now,
            //        Title = "Nové ikony pro hangmana",
            //        Text = "Nové ikony vytvořeny speciálně pro mobilní aplikaci staročeská šibenice.",
            //        Url = "http://censeo2.felk.cvut.cz",
            //        UserEmail = "t@t.t",
            //        UserFirstName = "test",
            //        UserLastName = "testovaci"
            //    },        new SyndicationItemViewModel
            //    {
            //        CreateDate = DateTime.Now,
            //        Title = "Nové ikony pro hangmana",
            //        Text = "Nové ikony vytvořeny speciálně pro mobilní aplikaci staročeská šibenice.",
            //        Url = "http://censeo2.felk.cvut.cz",
            //        UserEmail = "t@t.t",
            //        UserFirstName = "test",
            //        UserLastName = "testovaci"
            //    },        new SyndicationItemViewModel
            //    {
            //        CreateDate = DateTime.Now,
            //        Title = "Nové ikony pro hangmana",
            //        Text = "Nové ikony vytvořeny speciálně pro mobilní aplikaci staročeská šibenice.",
            //        Url = "http://censeo2.felk.cvut.cz",
            //        UserEmail = "t@t.t",
            //        UserFirstName = "test",
            //        UserLastName = "testovaci"
            //    },        new SyndicationItemViewModel
            //    {
            //        CreateDate = DateTime.Now,
            //        Title = "Nové ikony pro hangmana",
            //        Text = "Nové ikony vytvořeny speciálně pro mobilní aplikaci staročeská šibenice.",
            //        Url = "http://censeo2.felk.cvut.cz",
            //        UserEmail = "t@t.t",
            //        UserFirstName = "test",
            //        UserLastName = "testovaci"
            //    },
            //});

            return Task.Factory.StartNew(() =>
            {
                var news = client.GetNewsForMobileApps(0, 10);
                var result = new List<SyndicationItemViewModel>();

                foreach (var newItem in news)
                {
                    result.Add(new SyndicationItemViewModel
                    {
                        Text = newItem.Text,
                        Title = newItem.Title,
                        CreateDate = newItem.CreateDate,
                        UserLastName = newItem.UserLastName,
                        UserFirstName = newItem.UserFirstName,
                        UserEmail = newItem.UserEmail,
                        Url = newItem.Url,
                    });
                }

                return result;

            });
        }
    }


   

}