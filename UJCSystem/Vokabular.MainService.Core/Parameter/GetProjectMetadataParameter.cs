namespace Vokabular.MainService.Core.Parameter
{
    public class GetProjectMetadataParameter
    {
        public bool IncludeGenre { get; set; }
        public bool IncludeKind { get; set; }
        public bool IncludeOriginal { get; set; }
        public bool IncludeAuthor { get; set; }
        public bool IncludeResponsiblePerson { get; set; }
        public bool IncludeKeyword { get; set; }
        public bool IncludeCategory { get; set; }

        public bool IsAnyAdditionalParameter()
        {
            return IncludeGenre || IncludeKind || IncludeOriginal || IncludeAuthor || IncludeResponsiblePerson || IncludeKeyword || IncludeCategory;
        }
    }
}
