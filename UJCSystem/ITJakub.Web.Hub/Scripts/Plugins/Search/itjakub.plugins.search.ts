
class SearchModule {
    bibliographyModule: BibliographyModule;

    constructor(bibModule : BibliographyModule) {
        this.bibliographyModule = bibModule;
    }

    //TODO just for test, books dtos should be returned in call filter method (not ids)
    public getBookWithIds(bookIds: string[]) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl()+"Search/Books",
            data: { bookIds: bookIds },
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                this.bibliographyModule.showBooks(response.books);
            }
        });
    }

    public getBookWithType(type: string) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl()+"Search/BooksWithType",
            data: { type: type },
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
                this.bibliographyModule.showBooks(response.books);
            }
        });
    }
}