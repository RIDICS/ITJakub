namespace Ujc.Ovj.ChangeEngine.Objects
{
    public class ChangeException : Expression, IChangeException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ChangeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public ChangeException(string pattern, ChangeFormat format)
        {
            Pattern = pattern;
            Format = format;
        }

        
    }
}