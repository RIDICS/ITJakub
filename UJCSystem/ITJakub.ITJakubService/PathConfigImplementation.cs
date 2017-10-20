using ITJakub.ITJakubService.Properties;
using Microsoft.Extensions.Options;
using Vokabular.Core;

namespace ITJakub.ITJakubService
{
    public class PathConfigImplementation : IOptions<PathConfiguration>
    {
        public PathConfiguration Value => new PathConfiguration
        {
            FileStorageRootFolder = Settings.Default.FileStorageRootFolder,
        };
    }
}