function initDictionaryViewer(categoryIdList, bookIdList, pageNumber) {
    var selectedCategoryIds = [];
    var selectedBookIds = [];
    var defaultPageNumber = Number(pageNumber);
    try {
        selectedCategoryIds = JSON.parse(categoryIdList);
    }
    catch (e) {
    }
    try {
        selectedBookIds = JSON.parse(bookIdList);
    }
    catch (e) {
    }
    var pageSize = 50;
    var dictionaryViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription", true);
    var dictionaryViewerWrapper = new DictionaryViewerListWrapper(dictionaryViewer, pageSize);
    var searchBox = new SearchBox("#searchbox", "Dictionaries/Dictionaries");
    searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla");
    searchBox.create();
    var updateSearchBox = function (state) {
        var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
        searchBox.clearAndDestroy();
        searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla", parametersUrl);
        searchBox.create();
    };
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = function (state) {
        dictionaryViewerWrapper.loadHeadwordList(state);
        updateSearchBox(state);
    };
    var dictionarySelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate, "#dropdownDescriptionDiv");
    dictionarySelector.makeAndRestore(selectedCategoryIds, selectedBookIds);
    $("#cancelFilter").click(function () {
        dictionaryViewer.cancelFilter();
        $("#cancelFilter").addClass("hidden");
    });
    $("#printDescription").click(function () {
        dictionaryViewer.print();
    });
    $("#printList").click(function () {
        dictionaryViewer.printList();
    });
    $("#searchButton").click(function () {
        var query = $("#searchbox").val();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordPageNumber",
            data: {
                selectedBookIds: DropDownSelect.getBookIdsFromState(dictionarySelector.getState()),
                selectedCategoryIds: DropDownSelect.getCategoryIdsFromState(dictionarySelector.getState()),
                query: query,
                pageSize: pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                var resultPageNumber = response;
                dictionaryViewer.goToPage(resultPageNumber);
            }
        });
    });
    dictionaryViewerWrapper.loadDefault(selectedCategoryIds, selectedBookIds, defaultPageNumber);
}
;
var DictionaryViewerListWrapper = (function () {
    function DictionaryViewerListWrapper(dictionaryViewer, pageSize) {
        var _this = this;
        this.pageSize = pageSize;
        this.dictionaryViewer = dictionaryViewer;
        window.matchMedia("print").addListener(function (mql) {
            if (mql.matches) {
                _this.dictionaryViewer.loadAllHeadwords();
            }
        });
        this.favoriteHeadwords = new DictionaryFavoriteHeadwords("#saved-word-area", "#saved-word-area .saved-words-body", "#saved-word-area .saved-word-area-more");
        this.favoriteHeadwords.create(this.goToPageWithHeadword.bind(this));
    }
    DictionaryViewerListWrapper.prototype.goToPageWithHeadword = function (bookId, entryXmlId) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordPageNumberById",
            data: {
                selectedBookIds: this.selectedBookIds,
                selectedCategoryIds: this.selectedCategoryIds,
                headwordBookId: bookId,
                headwordEntryXmlId: entryXmlId,
                pageSize: this.pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                var resultPageNumber = response;
                _this.dictionaryViewer.goToPage(resultPageNumber);
            }
        });
    };
    DictionaryViewerListWrapper.prototype.addNewFavoriteHeadword = function (bookId, entryXmlId) {
        this.favoriteHeadwords.addNewHeadword(bookId, entryXmlId);
    };
    DictionaryViewerListWrapper.prototype.loadDefault = function (categoryIds, bookIds, defaultPageNumber) {
        this.selectedBookIds = bookIds;
        this.selectedCategoryIds = categoryIds;
        this.dictionaryViewer.setDefaultPageNumber(defaultPageNumber);
        this.loadCount();
    };
    DictionaryViewerListWrapper.prototype.loadHeadwordList = function (state) {
        this.selectedBookIds = DropDownSelect.getBookIdsFromState(state);
        this.selectedCategoryIds = DropDownSelect.getCategoryIdsFromState(state);
        this.dictionaryViewer.setDefaultPageNumber(null);
        this.loadCount();
    };
    DictionaryViewerListWrapper.prototype.loadCount = function () {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordCount",
            data: {
                selectedBookIds: this.selectedBookIds,
                selectedCategoryIds: this.selectedCategoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                var resultCount = response;
                _this.dictionaryViewer.createViewer(resultCount, _this.loadHeadwords.bind(_this), _this.pageSize, null, null, _this.addNewFavoriteHeadword.bind(_this));
            }
        });
    };
    DictionaryViewerListWrapper.prototype.loadHeadwords = function (pageNumber) {
        var _this = this;
        this.currentPageNumber = pageNumber;
        this.updateUrl();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordList",
            data: {
                selectedBookIds: this.selectedBookIds,
                selectedCategoryIds: this.selectedCategoryIds,
                page: pageNumber,
                pageSize: this.pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.dictionaryViewer.showHeadwords(response);
            }
        });
    };
    DictionaryViewerListWrapper.prototype.updateUrl = function () {
        var url = "?categoryIdList=" + JSON.stringify(this.selectedCategoryIds) + "&bookIdList=" + JSON.stringify(this.selectedBookIds) + "&pageNumber=" + this.currentPageNumber;
        window.history.replaceState(null, null, url);
    };
    return DictionaryViewerListWrapper;
})();
//# sourceMappingURL=itjakub.dictionaries.headwords.js.map