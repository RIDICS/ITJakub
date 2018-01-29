using ITJakub.FileProcessing.Service.Properties;
using Microsoft.Extensions.Options;
using Vokabular.Core;

namespace ITJakub.FileProcessing.Service
{
    public class PathConfigImplementation : IOptions<PathConfiguration>
    {
        public PathConfiguration Value => new PathConfiguration
        {
            FileStorageRootFolder = Settings.Default.FileStorageRootFolder,
        };
    }
}