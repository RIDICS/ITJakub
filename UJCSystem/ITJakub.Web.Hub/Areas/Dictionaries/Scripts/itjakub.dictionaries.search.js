$(document).ready(function () {
    var pageSize = 25;
    var dictionaryViewerHeadword = new DictionaryViewer("#headwords-list", "#headwords-pagination", "#headword-description", true);
    var dictionaryViewerFulltext = new DictionaryViewer("#headwords-list-fulltext", "#headwords-pagination-fulltext", "#headword-description", true);
    var dictionaryViewerAdvanced = new DictionaryViewer("#headwords-list-advanced", "#headwords-pagination-advanced", "#headword-description", true);
    var dictionaryWrapperBasic = new DictionaryViewerTextWrapper(dictionaryViewerHeadword, dictionaryViewerFulltext, pageSize);
    var dictionaryWrapperAdvanced = new DictionaryViewerJsonWrapper(dictionaryViewerAdvanced, pageSize);
    var processSearchJson = function (json) {
        $("#headword-description").empty();
        dictionaryWrapperAdvanced.loadCount(json);
    };
    var processSearchText = function (text) {
        $("#headword-description").empty();
        dictionaryWrapperBasic.loadCount(text);
    };
    var search = new Search($("#dictionarySearchDiv"), processSearchJson, processSearchText);
    var disabledOptions = new Array();
    disabledOptions.push(4 /* Fulltext */);
    disabledOptions.push(9 /* TokenDistance */);
    disabledOptions.push(6 /* Sentence */);
    disabledOptions.push(5 /* Heading */);
    search.makeSearch(disabledOptions);
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = function (state) {
    };
    var dictionarySelector = new DropDownSelect2("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
});
var DictionaryViewerJsonWrapper = (function () {
    function DictionaryViewerJsonWrapper(dictionaryViewer, pageSize) {
        this.pageSize = pageSize;
        this.dictionaryViewer = dictionaryViewer;
    }
    DictionaryViewerJsonWrapper.prototype.loadCount = function (json) {
        var _this = this;
        this.json = json;
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaResultsCount",
            data: JSON.stringify({
                "json": json
            }),
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                var resultCount = response;
                _this.dictionaryViewer.createViewer(resultCount, _this.loadHeadwords.bind(_this), _this.pageSize, json);
            }
        });
    };
    DictionaryViewerJsonWrapper.prototype.loadHeadwords = function (pageNumber) {
        var _this = this;
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria",
            data: JSON.stringify({
                "json": this.json,
                "start": this.pageSize * (pageNumber - 1),
                "count": this.pageSize
            }),
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.dictionaryViewer.showHeadwords(response);
            }
        });
    };
    return DictionaryViewerJsonWrapper;
})();
var DictionaryViewerTextWrapper = (function () {
    function DictionaryViewerTextWrapper(headwordViewer, fulltextViewer, pageSize) {
        this.pageSize = pageSize;
        this.headwordViewer = headwordViewer;
        this.fulltextViewer = fulltextViewer;
    }
    DictionaryViewerTextWrapper.prototype.loadCount = function (text) {
        var _this = this;
        this.text = text;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicResultsCount",
            data: {
                text: text
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.headwordViewer.createViewer(response.HeadwordCount, _this.loadHeadwords.bind(_this), _this.pageSize, text);
                _this.fulltextViewer.createViewer(response.FulltextCount, _this.loadFulltextHeadwords.bind(_this), _this.pageSize, text);
            }
        });
    };
    DictionaryViewerTextWrapper.prototype.loadHeadwords = function (pageNumber) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicHeadword",
            data: {
                text: this.text,
                start: this.pageSize * (pageNumber - 1),
                count: this.pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.headwordViewer.showHeadwords(response);
            }
        });
    };
    DictionaryViewerTextWrapper.prototype.loadFulltextHeadwords = function (pageNumber) {
        var _this = this;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicFulltext",
            data: {
                text: this.text,
                start: this.pageSize * (pageNumber - 1),
                count: this.pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                _this.fulltextViewer.showHeadwords(response);
            }
        });
    };
    return DictionaryViewerTextWrapper;
})();
//# sourceMappingURL=itjakub.dictionaries.search.js.map