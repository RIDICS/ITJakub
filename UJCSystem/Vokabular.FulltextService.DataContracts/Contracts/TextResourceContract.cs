
using System;

namespace Vokabular.FulltextService.DataContracts.Contracts
{
    public class TextResourceContract
    {
        public string Id { get; set; }
        public string PageText { get; set; }
        public string Name { get; set; }
        public int VersionNumber { get; set; }
    }
}