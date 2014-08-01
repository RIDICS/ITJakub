using System;
using Windows.ApplicationModel;
using Windows.Storage;

namespace ITJakub.MobileApps.Client.Core.Configuration
{
    public class ApplicationConfigLoader
    {
        private const string ConfigFileName = "ApplicationConfiguration.config";

        private static readonly Lazy<ApplicationConfigLoader> m_instance = new Lazy<ApplicationConfigLoader>(() => new ApplicationConfigLoader());

        private ApplicationConfigLoader()
        {
            LoadConfigFromFile();
        }

        public static ApplicationConfigLoader Instance
        {
            get { return m_instance.Value; }
        }

        public ApplicationConfig CurrentConfig { get; private set; }

        private void LoadConfigFromFile()
        {
            Package package = Package.Current;
            StorageFolder storageFolder = package.InstalledLocation;
            StorageFile storageFile = storageFolder.GetFileAsync(ConfigFileName).AsTask().Result;
            string fileString = FileIO.ReadTextAsync(storageFile).AsTask().Result;

            CurrentConfig = ApplicationConfig.FromXml(fileString);
        }
    }
}