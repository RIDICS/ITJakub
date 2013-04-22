using Ujc.Naki.DataLayer;

namespace ServicesLayer
{
    /// <summary>
    ///     Data transfer object for search criteria.
    /// </summary>
    public class SearchCriteria
    {
        /// <summary>
        /// </summary>
        /// <param name="qry">(search) query to the document</param>
        /// <param name="title">title of the document</param>
        /// <param name="author">author of the document</param>
        /// <param name="datationFrom">datation boundary from for the document</param>
        /// <param name="datationTo">datation boundary to for the document</param>
        /// <param name="kind">kind of the document</param>
        /// <param name="genre">genre of the document</param>
        /// <param name="original">original of the document</param>
        public SearchCriteria(string qry, string title, string author, int datationFrom, int datationTo,
                              DocumentKind? kind, DocumentGenre? genre, DocumentOriginal? original)
        {
            Qry = qry;
            Title = title;
            Author = author;
            DatationFrom = datationFrom;
            DatationTo = datationTo;
            Kind = kind;
            Genre = genre;
            Original = original;
        }

        public string Qry { get; private set; }
        public string Title { get; private set; }
        public string Author { get; private set; }
        public int DatationFrom { get; private set; }
        public int DatationTo { get; private set; }
        public DocumentKind? Kind { get; private set; }
        public DocumentGenre? Genre { get; private set; }
        public DocumentOriginal? Original { get; private set; }
    }
}