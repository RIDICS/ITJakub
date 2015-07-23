/// <reference path="../../../Scripts/Plugins/itjakub.plugins.pagination.ts" />
var DictionaryViewer = (function () {
    function DictionaryViewer(headwordListContainer, paginationContainer, headwordDescriptionContainer, lazyLoad) {
        var _this = this;
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
    DictionaryViewer.prototype.createViewer = function (recordCount, showPageCallback, pageSize, searchCriteria, isCriteriaJson) {
        if (searchCriteria === void 0) { searchCriteria = null; }
        if (isCriteriaJson === void 0) { isCriteriaJson = false; }
        this.recordCount = recordCount;
        this.showPageCallback = showPageCallback;
        this.pageSize = pageSize;
        this.searchCriteria = searchCriteria;
        this.isCriteriaJson = isCriteriaJson;
        var pageCount = Math.ceil(this.recordCount / this.pageSize);
        this.pagination.createPagination(pageCount, this.searchAndDisplay.bind(this));
    };
    DictionaryViewer.prototype.goToPage = function (pageNumber) {
        this.pagination.goToPage(pageNumber);
    };
    DictionaryViewer.prototype.searchAndDisplay = function (pageNumber) {
        this.isRequestToPrint = false;
        this.showPageCallback(pageNumber);
    };
    DictionaryViewer.prototype.showHeadwords = function (headwords) {
        $(this.headwordListContainer).empty();
        $(this.headwordDescriptionContainer).empty();
        this.headwordDescriptionDivs = [];
        this.dictionariesInfo = [];
        this.headwordList = [];
        this.dictionariesMetadataList = headwords.BookList;
        var listUl = document.createElement("ul");
        var descriptionsDiv = document.createElement("div");
        for (var i = 0; i < headwords.HeadwordList.length; i++) {
            var headwordLi = document.createElement("li");
            var record = headwords.HeadwordList[i];
            var headwordSpan = document.createElement("span");
            $(headwordSpan).text(record.Headword);
            $(headwordSpan).addClass("dictionary-result-headword");
            var favoriteGlyphSpan = document.createElement("span");
            $(favoriteGlyphSpan).addClass("glyphicon").addClass("glyphicon-star-empty").addClass("dictionary-result-headword-favorite");
            headwordLi.appendChild(headwordSpan);
            headwordLi.appendChild(favoriteGlyphSpan);
            var dictionaryListDiv = document.createElement("div");
            $(dictionaryListDiv).addClass("dictionary-result-book-list");
            for (var j = 0; j < record.Dictionaries.length; j++) {
                var dictionary = record.Dictionaries[j];
                var dictionaryMetadata = this.dictionariesMetadataList[dictionary.BookXmlId];
                // create description
                var mainHeadwordDiv = document.createElement("div");
                var descriptionDiv = document.createElement("div");
                $(mainHeadwordDiv).addClass("loading-background");
                $(descriptionDiv).addClass("dictionary-entry-description-container");
                if (this.isLazyLoad) {
                    this.prepareLazyLoad(mainHeadwordDiv);
                }
                else {
                    this.getAndShowHeadwordDescription(record.Headword, dictionary.BookXmlId, dictionary.EntryXmlId, descriptionDiv);
                }
                var commentsDiv = document.createElement("div");
                var commentsLink = document.createElement("a");
                $(commentsLink).text("Připomínky");
                commentsLink.href = "#";
                $(commentsDiv).addClass("dictionary-entry-comments");
                commentsDiv.appendChild(commentsLink);
                var dictionaryDiv = document.createElement("div");
                var dictionaryLink = document.createElement("a");
                $(dictionaryLink).text(dictionaryMetadata.BookTitle);
                dictionaryLink.href = "?guid=" + dictionary.BookXmlId;
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
                if (j > 0) {
                    var delimiterSpan = document.createElement("span");
                    $(delimiterSpan).text(" | ");
                    dictionaryListDiv.appendChild(delimiterSpan);
                }
                var aLink = document.createElement("a");
                aLink.href = "#";
                aLink.innerHTML = dictionaryMetadata.BookAcronym;
                aLink.setAttribute("data-entry-index", String(this.headwordDescriptionDivs.length - 1));
                $(aLink).addClass("dictionary-result-headword-book");
                this.createLinkListener(aLink, record.Headword, dictionary, descriptionDiv);
                dictionaryListDiv.appendChild(aLink);
            }
            headwordLi.appendChild(dictionaryListDiv);
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
    DictionaryViewer.prototype.showLoadHeadword = function (response, container) {
        $(container).empty();
        $(container).parent().removeClass("loading-background");
        container.innerHTML = response;
        if (this.isRequestToPrint)
            this.print();
    };
    DictionaryViewer.prototype.showLoadError = function (headword, container) {
        $(container).empty();
        $(container).parent().removeClass("loading-background");
        $(container).text("Chyba při náčítání hesla '" + headword + "'.");
        if (this.isRequestToPrint)
            this.print();
    };
    DictionaryViewer.prototype.getAndShowHeadwordDescription = function (headword, bookGuid, xmlEntryId, container) {
        if (this.searchCriteria == null)
            this.getAndShowHeadwordDescriptionBasic(headword, bookGuid, xmlEntryId, container);
        else
            this.getAndShowHeadwordDescriptionFromSearch(headword, bookGuid, xmlEntryId, container);
    };
    DictionaryViewer.prototype.getAndShowHeadwordDescriptionBasic = function (headword, bookGuid, xmlEntryId, container) {
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
                _this.showLoadHeadword(response, container);
            },
            error: function () {
                _this.showLoadError(headword, container);
            }
        });
    };
    DictionaryViewer.prototype.getAndShowHeadwordDescriptionFromSearch = function (headword, bookGuid, xmlEntryId, container) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordDescriptionFromSearch",
            data: {
                criteria: this.searchCriteria,
                isCriteriaJson: this.isCriteriaJson,
                bookGuid: bookGuid,
                xmlEntryId: xmlEntryId
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.showLoadHeadword(response, container);
            },
            error: function () {
                _this.showLoadError(headword, container);
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
        this.getAndShowHeadwordDescription(headword, dictionaryInfo.BookXmlId, dictionaryInfo.EntryXmlId, descriptionContainer);
    };
    DictionaryViewer.prototype.isAllLoaded = function () {
        var descriptions = $(this.headwordDescriptionContainer);
        var notLoaded = $(".loading-background", descriptions);
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
//# sourceMappingURL=itjakub.dictionariesViewer.js.map