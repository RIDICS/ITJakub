using Ujc.Naki.MVCWebLayer.Services.Enums;

namespace Ujc.Naki.MVCWebLayer.Services.DTOs
{
    public class Book
    {
        public Book() { }
        public Book(string name, BookCategory category, bool isRoot)
        {
            Name = name;
            Category = category;
            IsRoot = isRoot;
        }

        public string Name { get; set; }
        public BookCategory Category { get; set; }
        public bool IsRoot { get; set; }
    }
}