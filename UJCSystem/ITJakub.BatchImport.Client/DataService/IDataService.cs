using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITJakub.BatchImport.Client.DataService
{
    public interface IDataService
    {
        void TestMethod(Action<string, Exception> callback);
    }

    public class DataService : IDataService
    {
        public void TestMethod(Action<string, Exception> callback)
        {
            callback("test", null);
        }
    }

    public class DesignDataService : IDataService
    {
        public void TestMethod(Action<string, Exception> callback)
        {
            callback("design test path", null);
        }
    }

}
