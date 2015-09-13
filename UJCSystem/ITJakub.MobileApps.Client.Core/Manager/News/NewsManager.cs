using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.MobileApps.Client.Core.Manager.News
{
    public class NewsManager
    {
        public Task<ObservableCollection<string>> GetAllNews()
        {
            return Task.Factory.StartNew(() => new ObservableCollection<string>());
        }
    }
}
