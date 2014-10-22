var SearchModule = (function () {
    function SearchModule() {
    }
    //TODO just for test, books dtos should be returned in call filter method (not ids)
    SearchModule.prototype.getBookWithIds = function (bookIds, container) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: "/Search/Books",
            data: { bookIds: bookIds },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                BibliographyModule.getInstance().showBooks(response.books, container);
            }
        });
    };

    SearchModule.prototype.getBookWithType = function (type, container) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: "/Search/BooksWithType",
            data: { type: type },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                BibliographyModule.getInstance().showBooks(response.books, container);
            }
        });
    };
    return SearchModule;
})();
//# sourceMappingURL=itjakub.plugins.search.js.map
