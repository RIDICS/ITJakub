using System.Collections.Generic;
using Newtonsoft.Json;
using Vokabular.MainService.DataContracts.Contracts;

namespace ITJakub.Web.Hub.Models
{
    public class BookListingModel
    {
        public long BookId { get; set; }
        public string SnapshotId { get; set; }
        public string BookTitle { get; set; }
        public IList<PageContract> BookPages { get; set; }
        public IList<TrackWithRecordingContract> AudioTracks { get; set; }
        public string SearchText { get; set; }
        public string InitPageId { get; set; }
        public bool CanPrintEdition { get; set; }
        public JsonSerializerSettings JsonSerializerSettingsForBiblModule { get; set; }
    }
}