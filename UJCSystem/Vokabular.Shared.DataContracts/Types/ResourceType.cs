namespace Vokabular.Shared.DataContracts.Types
{
    public enum ResourceType
    {
        UnknownResourceFile = 0,
        SourceDocument = 1,
        Book = 2,
        UploadedMetadata = 3,
        ConvertedMetadata = 4,
        Page = 5,
        Transformation = 6,
        Image = 7,
        Audio = 8,
        ExtractableArchive = 9,
        UnknownXmlFile = 10,
        BibliographyDocument = 11,
        ThumbDbFile = 12,
    }
}