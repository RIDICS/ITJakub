var SearchModule = (function () {
    function SearchModule(bibModule) {
        this.bibliographyModule = bibModule;
    }
    //TODO just for test, books dtos should be returned in call filter method (not ids)
    SearchModule.prototype.getBookWithIds = function (bookIds) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: "/Search/Books",
            data: { bookIds: bookIds },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                _this.bibliographyModule.showBooks(response.books);
            }
        });
    };
    SearchModule.prototype.getBookWithType = function (type) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: "/Search/BooksWithType",
            data: { type: type },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                _this.bibliographyModule.showBooks(response.books);
            }
        });
    };
    return SearchModule;
})();
//# sourceMappingURL=itjakub.plugins.search.js.map