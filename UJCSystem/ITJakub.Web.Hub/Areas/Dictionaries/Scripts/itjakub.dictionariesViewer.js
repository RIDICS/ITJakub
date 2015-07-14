var DictionaryViewer = (function () {
    function DictionaryViewer(headwordListContainer, paginationContainer, headwordDescriptionContainer, lazyLoad) {
        var _this = this;
        if (lazyLoad === void 0) { lazyLoad = false; }
        this.isRequestToPrint = false;
        this.headwordDescriptionContainer = headwordDescriptionContainer;
        this.paginationContainer = paginationContainer;
        this.headwordListContainer = headwordListContainer;
        this.isLazyLoad = lazyLoad;
        this.pagination = new Pagination(paginationContainer);
        window.matchMedia("print").addListener(function (mql) {
            if (mql.matches) {
                _this.loadAllHeadwords();
            }
        });
    }
    DictionaryViewer.prototype.createViewer = function (recordCount, searchUrl, state, query, pageSize) {
        if (query === void 0) { query = null; }
        if (pageSize === void 0) { pageSize = 50; }
        this.selectedBookIds = DropDownSelect.getBookIdsFromState(state);
        this.selectedCategoryIds = DropDownSelect.getCategoryIdsFromState(state);
        this.currentQuery = query;
        this.recordCount = recordCount;
        this.searchUrl = searchUrl;
        this.pageSize = pageSize;
        var pageCount = Math.ceil(this.recordCount / this.pageSize);
        this.pagination.createPagination(pageCount, this.searchAndDisplay.bind(this));
    };
    DictionaryViewer.prototype.goToPage = function (pageNumber) {
        this.pagination.goToPage(pageNumber);
    };
    DictionaryViewer.prototype.searchAndDisplay = function (pageNumber) {
        var _this = this;
        this.isRequestToPrint = false;
        $.ajax({
            type: "GET",
            traditional: true,
            url: this.searchUrl,
            data: {
                selectedBookIds: this.selectedBookIds,
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
        this.headwordDescriptionDivs = [];
        this.dictionariesInfo = [];
        this.headwordList = [];
        var listUl = document.createElement("ul");
        var descriptionsDiv = document.createElement("div");
        for (var i = 0; i < headwords.length; i++) {
            var headwordLi = document.createElement("li");
            var record = headwords[i];
            var headwordSpan = document.createElement("span");
            $(headwordSpan).text(record.Headword);
            $(headwordSpan).addClass("dictionary-result-headword");
            var favoriteGlyphSpan = document.createElement("span");
            $(favoriteGlyphSpan).addClass("glyphicon").addClass("glyphicon-star-empty").addClass("dictionary-result-headword-favorite");
            headwordLi.appendChild(headwordSpan);
            headwordLi.appendChild(favoriteGlyphSpan);
            for (var j = 0; j < record.Dictionaries.length; j++) {
                var dictionary = record.Dictionaries[j];
                // create description
                var mainHeadwordDiv = document.createElement("div");
                var descriptionDiv = document.createElement("div");
                $(descriptionDiv).addClass("loading").addClass("dictionary-entry-description-container");
                if (this.isLazyLoad) {
                    this.prepareLazyLoad(mainHeadwordDiv);
                }
                else {
                    this.getAndShowHeadwordDescription(record.Headword, dictionary.BookGuid, dictionary.XmlEntryId, descriptionDiv);
                }
                var commentsDiv = document.createElement("div");
                var commentsLink = document.createElement("a");
                $(commentsLink).text("Připomínky");
                commentsLink.href = "#";
                $(commentsDiv).addClass("dictionary-entry-comments");
                commentsDiv.appendChild(commentsLink);
                var dictionaryDiv = document.createElement("div");
                var dictionaryLink = document.createElement("a");
                $(dictionaryLink).text(dictionary.BookTitle);
                dictionaryLink.href = "?guid=" + dictionary.BookGuid;
                $(dictionaryDiv).addClass("dictionary-entry-name");
                dictionaryDiv.appendChild(dictionaryLink);
                mainHeadwordDiv.appendChild(descriptionDiv);
                mainHeadwordDiv.appendChild(commentsDiv);
                mainHeadwordDiv.appendChild(dictionaryDiv);
                mainHeadwordDiv.appendChild(document.createElement("hr"));
                mainHeadwordDiv.setAttribute("data-entry-index", String(this.headwordDescriptionDivs.length));
                this.headwordDescriptionDivs.push(mainHeadwordDiv);
                this.dictionariesInfo.push(dictionary);
                this.headwordList.push(record.Headword);
                descriptionsDiv.appendChild(mainHeadwordDiv);
                // create link
                var aLink = document.createElement("a");
                aLink.href = "#";
                aLink.innerHTML = dictionary.BookAcronym;
                aLink.setAttribute("data-entry-index", String(this.headwordDescriptionDivs.length - 1));
                $(aLink).addClass("dictionary-result-headword-book");
                this.createLinkListener(aLink, record.Headword, dictionary, descriptionDiv);
                headwordLi.appendChild(aLink);
            }
            listUl.appendChild(headwordLi);
        }
        $(this.headwordListContainer).append(listUl);
        $(this.headwordDescriptionContainer).append(descriptionsDiv);
    };
    DictionaryViewer.prototype.createLinkListener = function (aLink, headword, headwordInfo, container) {
        var _this = this;
        $(aLink).click(function (event) {
            event.preventDefault();
            var index = $(event.target).data("entry-index");
            var headwordDiv = _this.headwordDescriptionDivs[index];
            for (var k = 0; k < _this.headwordDescriptionDivs.length; k++) {
                $(_this.headwordDescriptionDivs[k]).addClass("hidden");
            }
            $(headwordDiv).removeClass("hidden");
            if ($(headwordDiv).hasClass("lazy-loading")) {
                _this.loadHeadwordDescription(index);
            }
        });
    };
    DictionaryViewer.prototype.prepareLazyLoad = function (mainDescriptionElement) {
        var _this = this;
        $(mainDescriptionElement).addClass("lazy-loading");
        $(mainDescriptionElement).bind("appearing", function (event) {
            var descriptionDiv = event.target;
            var index = $(descriptionDiv).data("entry-index");
            _this.loadHeadwordDescription(index);
        });
    };
    DictionaryViewer.prototype.getAndShowHeadwordDescription = function (headword, bookGuid, xmlEntryId, container) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordDescription",
            data: {
                bookGuid: bookGuid,
                xmlEntryId: xmlEntryId
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                $(container).empty();
                $(container).removeClass("loading");
                container.innerHTML = response;
                if (_this.isRequestToPrint)
                    _this.print();
            },
            error: function () {
                $(container).empty();
                $(container).removeClass("loading");
                $(container).text("Chyba při náčítání hesla '" + headword + "'.");
                if (_this.isRequestToPrint)
                    _this.print();
            }
        });
    };
    DictionaryViewer.prototype.loadHeadwordDescription = function (index) {
        var mainDescriptionDiv = this.headwordDescriptionDivs[index];
        var headword = this.headwordList[index];
        var dictionaryInfo = this.dictionariesInfo[index];
        var descriptionContainer = $(".dictionary-entry-description-container", mainDescriptionDiv).get(0);
        $(mainDescriptionDiv).unbind("appearing");
        $(mainDescriptionDiv).removeClass("lazy-loading");
        this.getAndShowHeadwordDescription(headword, dictionaryInfo.BookGuid, dictionaryInfo.XmlEntryId, descriptionContainer);
    };
    DictionaryViewer.prototype.isAllLoaded = function () {
        var descriptions = $(this.headwordDescriptionContainer);
        var notLoaded = $(".loading", descriptions);
        var notLoadedVisible = notLoaded.parent(":not(.hidden)");
        return notLoadedVisible.length === 0;
    };
    DictionaryViewer.prototype.loadAllHeadwords = function () {
        for (var i = 0; i < this.headwordDescriptionDivs.length; i++) {
            var descriptionDiv = this.headwordDescriptionDivs[i];
            if ($(descriptionDiv).hasClass("lazy-loading") && !$(descriptionDiv).hasClass("hidden")) {
                this.loadHeadwordDescription(i);
            }
        }
    };
    DictionaryViewer.prototype.showPrintModal = function () {
        if (this.isRequestToPrint)
            return;
        $("#print-modal").modal({
            backdrop: "static",
            show: true
        });
    };
    DictionaryViewer.prototype.hidePrintModal = function () {
        $("#print-modal").modal("hide");
    };
    DictionaryViewer.prototype.print = function () {
        // check if all entries are loaded
        if (!this.isAllLoaded()) {
            this.showPrintModal();
            this.isRequestToPrint = true;
            if (this.isLazyLoad) {
                this.loadAllHeadwords();
            }
            return;
        }
        this.isRequestToPrint = false;
        this.hidePrintModal();
        window.print();
    };
    return DictionaryViewer;
})();
var Pagination = (function () {
    function Pagination(paginationContainer, maxVisiblePageElements) {
        if (maxVisiblePageElements === void 0) { maxVisiblePageElements = 9; }
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
        contentElement.innerHTML = "&hellip;";
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
    Pagination.prototype.goToPage = function (pageNumber) {
        if (pageNumber > 0 && pageNumber <= this.pageCount) {
            this.updateCurrentPage(pageNumber);
        }
    };
    return Pagination;
})();
//# sourceMappingURL=itjakub.dictionariesViewer.js.map