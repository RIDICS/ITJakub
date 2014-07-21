using System;
using Windows.Storage;

namespace ITJakub.MobileApps.Client.Core.Configuration
{
    public class ApplicationConfigLoader
    {
        private ApplicationConfigLoader()
        {
            LoadConfigFromFileAsync();
        }

        public static readonly ApplicationConfigLoader Instance = new ApplicationConfigLoader();
        public ApplicationConfig CurrentConfig { get; private set; }

        private const string ConfigFileName = "ApplicationConfiguration.config";

        private async void LoadConfigFromFileAsync()
        {
            Windows.ApplicationModel.Package package = Windows.ApplicationModel.Package.Current;
            StorageFolder storageFolder = package.InstalledLocation;          
            var storageFile = await storageFolder.GetFileAsync(ConfigFileName);
            var fileString = await FileIO.ReadTextAsync(storageFile);

            CurrentConfig = ApplicationConfig.FromXml(fileString);
        }
    }
}
