namespace Vokabular.MainService.DataContracts.Contracts
{
    public class EditionNoteContract
    {
        public long Id { get; set; }
        public long VersionId { get; set; }
        public int VersionNumber { get; set; }

        public string Text { get; set; }
    }

    public class CreateEditionNoteContract
    {
        public string Text { get; set; }
        public long OriginalVersionId { get; set; }
        public string Comment { get; set; }
    }
}
