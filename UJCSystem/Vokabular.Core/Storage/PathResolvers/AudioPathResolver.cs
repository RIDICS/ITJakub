using System.IO;
using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.PathResolvers
{
    public class AudioPathResolver : IResourceTypePathResolver
    {
        public ResourceType ResolvingResourceType()
        {
            return ResourceType.Audio;
        }

        public string ResolvePath(string bookId, string bookVersionId, string fileName)
        {
            return Path.Combine(bookId, "Audio", fileName);
        }
    }
}