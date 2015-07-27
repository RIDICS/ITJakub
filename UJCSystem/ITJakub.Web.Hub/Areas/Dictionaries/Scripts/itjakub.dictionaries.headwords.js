$(document).ready(function () {
    var pageSize = 50;
    var dictionaryViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription", true);
    var dictionaryViewerWrapper = new DictionaryViewerListWrapper(dictionaryViewer, pageSize);
    var searchBox = new SearchBox("#searchbox", "Dictionaries/Dictionaries");
    searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla");
    searchBox.create();
    var loadHeadwordsFunction = function (state) {
        dictionaryViewerWrapper.loadCount(state);
    };
    var updateSearchBox = function (state) {
        var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
        searchBox.clearAndDestroy();
        searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla", parametersUrl);
        searchBox.create();
    };
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = function (state) {
        loadHeadwordsFunction(state);
        updateSearchBox(state);
    };
    var dictionarySelector = new DropDownSelect2("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
    $("#printDescription").click(function () {
        dictionaryViewer.print();
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
    var favoriteHeadwords = new DictionaryFavoriteHeadwords("#saved-word-area", "#saved-word-area .saved-words-body", "#saved-word-area .saved-word-area-more");
    favoriteHeadwords.create();
    loadHeadwordsFunction(dictionarySelector.getState());
});
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
    }
    DictionaryViewerListWrapper.prototype.loadCount = function (state) {
        var _this = this;
        this.selectedBookIds = DropDownSelect.getBookIdsFromState(state);
        this.selectedCategoryIds = DropDownSelect.getCategoryIdsFromState(state);
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
                _this.dictionaryViewer.createViewer(resultCount, _this.loadHeadwords.bind(_this), _this.pageSize);
            }
        });
    };
    DictionaryViewerListWrapper.prototype.loadHeadwords = function (pageNumber) {
        var _this = this;
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
    return DictionaryViewerListWrapper;
})();
//# sourceMappingURL=itjakub.dictionaries.headwords.js.map