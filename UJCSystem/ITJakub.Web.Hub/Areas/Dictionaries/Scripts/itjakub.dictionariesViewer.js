var DictionaryViewer = (function () {
    function DictionaryViewer(headwordListContainer, paginationContainer, headwordDescriptionContainer) {
        this.pageSize = 20;
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.paginationContainer = paginationContainer;
        this.headwordListContainer = headwordListContainer;
    }
    DictionaryViewer.prototype.search = function (query) {
        var _this = this;
        this.currentQuery = query;
        this.searchAndDisplay(1);
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetResultCountOfSearch",
            data: {
                query: this.currentQuery
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                //TODO save and display result count (headwords and fulltext)
                _this.pageCount = 19;
                _this.createPagination(_this.pageCount);
            }
        });
    };
    DictionaryViewer.prototype.searchAndDisplay = function (pageNumber) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchHeadword",
            data: {
                query: this.currentQuery,
                page: pageNumber,
                pageSize: this.pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.showHeadwords(response);
            }
        });
    };
    DictionaryViewer.prototype.showHeadwords = function (headwords) {
        $(this.headwordListContainer).empty();
        $(this.headwordDescriptionContainer).empty();
        var listUl = document.createElement("ul");
        for (var i = 0; i < headwords.length; i++) {
            var headwordLi = document.createElement("li");
            var record = headwords[i];
            var headwordSpan = document.createElement("span");
            headwordSpan.innerText = record.Headword;
            $(headwordSpan).addClass("dictionary-result-headword");
            var favoriteGlyphSpan = document.createElement("span");
            $(favoriteGlyphSpan).addClass("glyphicon").addClass("glyphicon-star-empty").addClass("dictionary-result-headword-favorite");
            headwordLi.appendChild(headwordSpan);
            headwordLi.appendChild(favoriteGlyphSpan);
            for (var j = 0; j < record.Dictionaries.length; j++) {
                var dictionary = record.Dictionaries[j];
                var aLink = document.createElement("a");
                aLink.href = "?guid=" + dictionary.BookGuid + "&xmlEntryId=" + dictionary.XmlEntryId;
                aLink.innerText = dictionary.BookAcronym;
                $(aLink).addClass("dictionary-result-headword-book");
                headwordLi.appendChild(aLink);
            }
            listUl.appendChild(headwordLi);
        }
        $(this.headwordListContainer).append(listUl);
    };
    DictionaryViewer.prototype.createPageElement = function (label) {
        var _this = this;
        var pageLi = document.createElement("li");
        var pageA = document.createElement("a");
        pageA.innerText = label;
        $(pageA).click(function (event) {
            var clickedLink = (event.target);
            var pageNumber = Number(clickedLink.innerText);
            //TODO don't use innerHTML
            _this.searchAndDisplay(pageNumber);
        });
        pageLi.appendChild(pageA);
        return pageLi;
    };
    DictionaryViewer.prototype.createPagination = function (recordCount) {
        $(this.paginationContainer).empty();
        var paginationUl = document.createElement("ul");
        $(paginationUl).addClass("pagination").addClass("pagination-sm");
        var pageCount = Math.ceil(recordCount / this.pageSize);
        var previousPageLi = this.createPageElement("&laquo;");
        paginationUl.appendChild(previousPageLi);
        for (var i = 1; i <= pageCount; i++) {
            var pageLi = this.createPageElement(String(i));
            paginationUl.appendChild(pageLi);
        }
        var nextPageLi = this.createPageElement("&raquo;");
        paginationUl.appendChild(nextPageLi);
        $(this.paginationContainer).append(paginationUl);
    };
    DictionaryViewer.prototype.showHeadwordDescription = function (headword) {
        //TODO
    };
    return DictionaryViewer;
})();
//# sourceMappingURL=itjakub.dictionariesViewer.js.map