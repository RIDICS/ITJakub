using Microsoft.WindowsAzure;

namespace ITJakub.MobileApps.Configuration
{
    public interface IConnectionStringProvider
    {
        string GetAzureTableConnectionString();
        
    }

    public class AzureConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string m_azureTableConnectionString;

        public AzureConnectionStringProvider(string azureConnectionStringName)
        {
            m_azureTableConnectionString = CloudConfigurationManager.GetSetting(azureConnectionStringName);
        }

        public string GetAzureTableConnectionString()
        {
            return m_azureTableConnectionString;
        }
    }

    public class CastleConnectionStringProvider : IConnectionStringProvider
    {
        private readonly string m_azureTableConnectionString;

        public CastleConnectionStringProvider(string azureTableConnectionString)
        {
            m_azureTableConnectionString = azureTableConnectionString;
        }

        public string GetAzureTableConnectionString()
        {
            return m_azureTableConnectionString;
        }
    }

    public class DebugConnectionStringProvider : IConnectionStringProvider
    {
        public string GetAzureTableConnectionString()
        {
            return "UseDevelopmentStorage=true";
        }
    }
}