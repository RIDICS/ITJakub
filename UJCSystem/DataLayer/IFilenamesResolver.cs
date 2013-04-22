namespace Ujc.Naki.DataLayer
{
    /// <summary>
    /// Interface for classes that are used for resolving names of files within DB.
    /// </summary>
    public interface IFilenamesResolver
    {
        /// <summary>
        /// Generates new unique ID.
        /// </summary>
        /// <returns>unique ID</returns>
        int GenerateUniqueID();

        /// <summary>
        /// Gets all IDs wihthin the DB.
        /// </summary>
        /// <returns>all IDs in DB</returns>
        int[] GetAllIDs();

        /// <summary>
        /// Gets all names (ID + view) within the DB.
        /// </summary>
        /// <returns>all names within the DB</returns>
        string[] GetAllNames();

        /// <summary>
        /// Gets all names (ID + view) for document with specified ID.
        /// </summary>
        /// <param name="id">ID of the document</param>
        /// <returns>all names for document with specified ID</returns>
        string[] GetNamesFromID(int id);


        /// <summary>
        /// Gets views for document with specified ID.
        /// </summary>
        /// <param name="id">views of the document</param>
        /// <returns>views for document with specified ID</returns>
        string[] GetViewsFromID(int id);

        /// <summary>
        /// Gets ID from specified name.
        /// </summary>
        /// <param name="name">name of the document</param>
        /// <returns>ID of the document</returns>
        int GetIDFromName(string name);

        /// <summary>
        /// Gets view from specified name.
        /// </summary>
        /// <param name="name">name of the document</param>
        /// <returns>view of the document</returns>
        string GetViewFromName(string name);
        /// <summary>
        /// Gets ID separator (within name).
        /// </summary>
        /// <returns>ID separator</returns>
        string GetSeparator();

        /// <summary>
        /// Constructs XML name from specified ID and view.
        /// </summary>
        /// <param name="id">ID of the document</param>
        /// <param name="view">view type</param>
        /// <returns>constructed XML name</returns>
        string ConstructXmlName(int id, DocumentView view);


        /// <summary>
        /// Constructs name for image (typically cover).
        /// </summary>
        /// <param name="id">ID of the document</param>
        /// <param name="view">view type</param>
        /// <returns>constructed image name</returns>
        string ConstructImgName(int id, DocumentView view);

        /// <summary>
        /// Constructs name for page image.
        /// </summary>
        /// <param name="id">ID of the document</param>
        /// <param name="view">view type</param>
        /// <param name="pageNr">number of page</param>
        /// <returns>constructed image name</returns>
        string ConstructImgName(int id, DocumentView view, int pageNr);

        /// <summary>
        /// Constructs template for page image where page number is the predefined string for future replace.
        /// </summary>
        /// <param name="id">ID of the document</param>
        /// <param name="view">view type</param>
        /// <param name="pageNumberTempl">any string for future replace</param>
        /// <returns></returns>
        string ConstructImgPageNameTemplate(int id, DocumentView view, string pageNumberTempl);
    }
}
