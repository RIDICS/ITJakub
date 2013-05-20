using Ujc.Naki.MVCWebLayer.Services.Enums;
using Ujc.Naki.MVCWebLayer.Services.Mocks;

namespace Ujc.Naki.MVCWebLayer.Services.DTOs
{
    public class Book:SelectionBase
    {
        public Book(string name, BookCategory category, bool isRoot)
        {
            Name = name;
            Category = category;
            IsRoot = isRoot;
        }

        
        public BookCategory Category { get; set; }
        public bool IsRoot { get; set; }
    }
}