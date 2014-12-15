namespace ITJakub.FileStorage.Resources
{
    public class Resource
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public ResourceTypeEnum ResourceType { get; set; }
    }

    public enum ResourceTypeEnum
    {
        SourceDocument = 0,
        Book = 1,
        Metadata = 2,
        Page = 3,
        Transformation = 4,
        Image = 5,
    }
}