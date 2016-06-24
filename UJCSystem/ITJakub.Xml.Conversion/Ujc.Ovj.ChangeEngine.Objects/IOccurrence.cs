namespace Ujc.Ovj.ChangeEngine.Objects
{
    /// <summary>
    /// Interface for the occurrences of the string within another string
    /// </summary>
    public interface IOccurrence
    {
        /// <summary>
        /// Start index; index (in the string) wehre occurence starts.
        /// </summary>
        int StartIndex { get; set; }

        /// <summary>
        /// End index; index (in the string) where occurrence ends.
        /// </summary>
        int EndIndex { get; set; }
 
    }
}