
class SearchModule {

    constructor() {
        
    }

    //TODO just for test, books dtos should be returned in call filter method (not ids)
    public getBookWithIds(bookIds: string[], container: string) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: "/Search/Books",
            data: { bookIds: bookIds },
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                BibliographyModule.getInstance().showBooks(response.books, container);
            }
        });
    }

    public getBookWithType(type: string, container: string) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: "/Search/BooksWithType",
            data: { type: type },
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                BibliographyModule.getInstance().showBooks(response.books,container);
            }
        });
    }
}