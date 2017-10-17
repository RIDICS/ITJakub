using Vokabular.Shared.DataContracts.Types;

namespace Vokabular.Core.Storage.Resources
{
    public class Resource
    {
        public string FullPath { get; set; }

        public string FileName { get; set; }

        public ResourceType ResourceType { get; set; }
    }
}