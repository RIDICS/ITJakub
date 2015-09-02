namespace Ujc.Ovj.Xml.Tei.Splitting
{
    public class SourceDocumentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public SourceDocumentInfo(string xmlId, string versionId)
        {
            XmlId = xmlId;
            VersionId = versionId;
        }

        /// <summary>
        /// Identifier of main document
        /// </summary>
        public string XmlId { get; set; }
        /// <summary>
        /// Version identifier od main document
        /// </summary>
        public string VersionId { get; set; }
    }
}