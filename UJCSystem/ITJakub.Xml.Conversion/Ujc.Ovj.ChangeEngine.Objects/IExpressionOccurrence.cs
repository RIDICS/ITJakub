namespace Ujc.Ovj.ChangeEngine.Objects
{
    /// <summary>
    /// Interface r the occurence of the expression in some text.
    /// </summary>
    public interface IExpressionOccurrence : IOccurrence
    {
        /// <summary>
        /// Pattern which is searched in input text.
        /// Interpretation of pattern is based on its format.
        /// </summary>
        string Pattern { get; set; }

        /// <summary>
        /// Format of <see cref="Pattern"/>. How is it interpreted during pattern recognition.
        /// </summary>
        ChangeFormat Format { get; set; }
        
        /// <summary>
        /// Text which match the patern in the input string.
        /// </summary>
        string Text { get; set; }
    }
}