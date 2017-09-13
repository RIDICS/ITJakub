using System.Collections.Generic;
using System.Linq;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.DataContracts;
using Vokabular.MainService.DataContracts.Contracts;
using Vokabular.MainService.DataContracts.Contracts.Type;

namespace ITJakub.Web.Hub.Controllers
{
    public abstract class AreaController : BaseController
    {
        protected AreaController(CommunicationProvider communicationProvider) : base(communicationProvider)
        {
        }

        public abstract BookTypeEnumContract AreaBookType { get; }
        public abstract Shared.Contracts.BookTypeEnumContract OldAreaBookType { get; }

        protected BooksAndCategoriesContract GetBooksAndCategories()
        {
            using (var client = GetRestClient())
            {
                var categories = client.GetCategoryList();
                var books = client.GetBooksByType(AreaBookType);

                // Modify data for DropDownSelect usage

                const int rootCategoryId = 0;
                var rootBookTypeCategory = new CategoryContract
                {
                    Id = rootCategoryId,
                    Description = GetCategoryName(),
                    ParentCategoryId = null,
                };

                foreach (var category in categories)
                {
                    if (category.ParentCategoryId == null)
                        category.ParentCategoryId = rootCategoryId;
                }
                categories.Add(rootBookTypeCategory);


                var booksResult = new List<BookWithCategoryIdsContract>();
                foreach (var book in books)
                {
                    var categoryIds = book.CategoryList.Select(x => x.Id).ToList();
                    if (categoryIds.Count == 0)
                        categoryIds.Add(rootCategoryId);

                    booksResult.Add(new BookWithCategoryIdsContract
                    {
                        Id = book.Id,
                        Title = book.Title,
                        SubTitle = book.SubTitle,
                        Guid = null,
                        CategoryIds = categoryIds
                    });
                }

                var result = new BooksAndCategoriesContract
                {
                    BookType = AreaBookType,
                    Categories = categories,
                    Books = booksResult
                };
                return result;
            }
        }

        private string GetCategoryName()
        {
            switch (AreaBookType)
            {
                case BookTypeEnumContract.Edition:
                    return "Edice";
                case BookTypeEnumContract.Dictionary:
                    return "Slovníky";
                case BookTypeEnumContract.Grammar:
                    return "Digitalizované mluvnice";
                case BookTypeEnumContract.ProfessionalLiterature:
                    return "Odborná literatura";
                case BookTypeEnumContract.TextBank:
                    return "Textová banka";
                case BookTypeEnumContract.BibliographicalItem:
                    return "Bibliografie";
                case BookTypeEnumContract.CardFile:
                    return "Kartotéky";
                case BookTypeEnumContract.AudioBook:
                    return "Audio knihy";
                default:
                    return null;
            }
        }
    }
}