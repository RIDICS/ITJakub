var DictionaryViewer = (function () {
    function DictionaryViewer(headwordListContainer, paginationContainer, headwordDescriptionContainer) {
        this.pageSize = 20;
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.paginationContainer = paginationContainer;
        this.headwordListContainer = headwordListContainer;
        this.pagination = new Pagination(paginationContainer);
    }
    DictionaryViewer.prototype.createViewer = function (recordCount, searchUrl, categories, query) {
        if (query === void 0) { query = null; }
        this.categoriesSelect = categories;
        this.currentQuery = query;
        this.recordCount = recordCount;
        this.searchUrl = searchUrl;
        var pageCount = Math.ceil(this.recordCount / this.pageSize);
        this.pagination.createPagination(pageCount, this.searchAndDisplay.bind(this));
    };
    DictionaryViewer.prototype.searchAndDisplay = function (pageNumber) {
        var _this = this;
        $.ajax({
            type: "POST",
            traditional: true,
            url: this.searchUrl,
            data: JSON.stringify({
                query: this.currentQuery,
                page: pageNumber,
                pageSize: this.pageSize,
                selectedBookIds: [4] //TODO get from categories DropDownSelect
            }),
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
                aLink.innerHTML = dictionary.BookAcronym;
                $(aLink).addClass("dictionary-result-headword-book");
                headwordLi.appendChild(aLink);
            }
            listUl.appendChild(headwordLi);
        }
        $(this.headwordListContainer).append(listUl);
    };
    DictionaryViewer.prototype.showHeadwordDescription = function (headword) {
        //TODO
    };
    return DictionaryViewer;
})();
var Pagination = (function () {
    function Pagination(paginationContainer, maxVisiblePageElements) {
        if (maxVisiblePageElements === void 0) { maxVisiblePageElements = 7; }
        this.usePaginationDots = false;
        this.maxPageElements = maxVisiblePageElements;
        this.paginationContainer = paginationContainer;
    }
    Pagination.prototype.createPagination = function (pageCount, pageClickCallback) {
        this.pageCount = pageCount;
        this.pageClickCallback = pageClickCallback;
        $(this.paginationContainer).empty();
        var paginationUl = document.createElement("ul");
        $(paginationUl).addClass("pagination").addClass("pagination-sm");
        var previousPageLi = this.createPageElement("&laquo;", "previous");
        paginationUl.appendChild(previousPageLi);
        for (var i = 1; i <= pageCount; i++) {
            var pageLi = this.createPageElement(String(i), i);
            paginationUl.appendChild(pageLi);
        }
        var nextPageLi = this.createPageElement("&raquo;", "next");
        paginationUl.appendChild(nextPageLi);
        this.usePaginationDots = pageCount > this.maxPageElements;
        if (this.usePaginationDots) {
            $(paginationUl.children[1]).after(this.createThreeDots());
            $(paginationUl.children[this.pageCount]).after(this.createThreeDots());
        }
        $(this.paginationContainer).append(paginationUl);
        this.updateCurrentPage(1);
    };
    Pagination.prototype.updateCurrentPage = function (newPageNumber) {
        this.getCurrentPageElement().removeClass("active");
        this.currentPage = newPageNumber;
        this.getCurrentPageElement().addClass("active");
        this.updateVisiblePageElements();
        this.pageClickCallback(newPageNumber);
    };
    Pagination.prototype.createPageElement = function (label, pageNumber) {
        var _this = this;
        var pageLi = document.createElement("li");
        var pageLink = document.createElement("a");
        pageLink.innerHTML = label;
        pageLink.href = "#";
        pageLink.setAttribute("data-page-number", pageNumber);
        $(pageLink).click(function (event) {
            event.preventDefault();
            var pageValue = $(event.target).data("page-number");
            var pageNumber;
            switch (pageValue) {
                case "previous":
                    pageNumber = _this.currentPage - 1;
                    break;
                case "next":
                    pageNumber = _this.currentPage + 1;
                    break;
                default:
                    pageNumber = Number(pageValue);
                    break;
            }
            if (pageNumber > 0 && pageNumber <= _this.pageCount) {
                _this.updateCurrentPage(pageNumber);
            }
        });
        pageLi.appendChild(pageLink);
        return pageLi;
    };
    Pagination.prototype.createThreeDots = function () {
        var element = document.createElement("li");
        $(element).addClass("disabled").addClass("three-dots");
        var contentElement = document.createElement("span");
        contentElement.innerText = "...";
        element.appendChild(contentElement);
        return element;
    };
    Pagination.prototype.getCurrentPageElement = function () {
        var selector = "li:has(*[data-page-number=\"" + this.currentPage + "\"])";
        return $(selector);
    };
    Pagination.prototype.updateVisiblePageElements = function () {
        if (!this.usePaginationDots)
            return;
        var pageNumber = this.currentPage;
        var centerVisibleIndex = (this.maxPageElements - 1) / 2;
        var paginationListUl = $(this.paginationContainer).children().children();
        for (var i = 2; i < paginationListUl.length - 2; i++) {
            $(paginationListUl[i]).addClass("hidden");
        }
        var visibleInCenter = this.maxPageElements - 4; //two buttons on each side always visible
        var leftDotsHidden = false;
        var rightDotsHidden = false;
        var threeDotsElements = $(".three-dots");
        threeDotsElements.addClass("hidden");
        if (pageNumber > centerVisibleIndex) {
            threeDotsElements.first().removeClass("hidden");
            leftDotsHidden = true;
        }
        if (pageNumber < this.pageCount - centerVisibleIndex) {
            threeDotsElements.last().removeClass("hidden");
            rightDotsHidden = true;
        }
        if (!leftDotsHidden) {
            for (var j = 0; j < visibleInCenter + 1; j++) {
                $(paginationListUl[j + 3]).removeClass("hidden");
            }
        }
        else if (!rightDotsHidden) {
            for (var l = 0; l < visibleInCenter + 1; l++) {
                $(paginationListUl[this.pageCount - l]).removeClass("hidden");
            }
        }
        else {
            var centerIndex = this.currentPage + 1;
            $(paginationListUl[centerIndex]).removeClass("hidden");
            var iterations = (visibleInCenter - 1) / 2;
            for (var k = 1; k <= iterations; k++) {
                $(paginationListUl[centerIndex - k]).removeClass("hidden");
                $(paginationListUl[centerIndex + k]).removeClass("hidden");
            }
        }
    };
    return Pagination;
})();
//# sourceMappingURL=itjakub.dictionariesViewer.js.map