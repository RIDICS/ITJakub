namespace ITJakub.Contracts.Categories
{
    public class Book:SelectionBase
    {
        public Book(string name, bool isRoot)
        {
            Name = name;
            IsRoot = isRoot;
        }

     
        public bool IsRoot { get; set; }
    }
}