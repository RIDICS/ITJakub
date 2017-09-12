using System.Linq;
using ITJakub.Web.Hub.Core.Communication;
using ITJakub.Web.Hub.DataContracts;
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
                // TODO add root category

                var books = client.GetBooksByType(AreaBookType);
                var booksResult = books.Select(x => new BookWithCategoryIdsContract
                {
                    Id = x.Id,
                    Title = x.Title,
                    SubTitle = x.SubTitle,
                    Guid = null,
                    CategoryIds = x.CategoryList.Select(c => c.Id).ToList()
                }).ToList();
                // TODO assign books without category to root category

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