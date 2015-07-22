$(document).ready(function () {
    var pageSize = 25;
    var dictionaryViewerAdvanced = new DictionaryViewer("#advanced", "#pagination-advanced", "#headword-description", true);
    var dictionaryWrapperAdvanced = new DictionaryViewerJsonWrapper(dictionaryViewerAdvanced, pageSize);
    var processSearchJson = function (json) {
        $("#headword-description").empty();
        dictionaryWrapperAdvanced.loadCount(json);
    };
    var processSearchText = function (text) {
        $("#headword-description").empty();
        //TODO
    };
    var search = new Search($("#dictionarySearchDiv"), processSearchJson, processSearchText);
    var disabledOptions = new Array();
    disabledOptions.push(SearchTypeEnum.Fulltext);
    disabledOptions.push(SearchTypeEnum.TokenDistance);
    disabledOptions.push(SearchTypeEnum.Sentence);
    disabledOptions.push(SearchTypeEnum.Heading);
    search.makeSearch(disabledOptions);
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = function (state) {
    };
    var dictionarySelector = new DropDownSelect2("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
});
function processSearchResults(result) {
    alert("processed: " + result);
}
function processSearchText(text) {
    $.ajax({
        type: "GET",
        traditional: true,
        data: {
            text: text
        },
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicResultsCount",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            processSearchResults(response);
        },
        error: function (response) {
        }
    });
}
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
    function DictionaryViewerTextWrapper() {
    }
    return DictionaryViewerTextWrapper;
})();
//# sourceMappingURL=itjakub.dictionaries.search.js.map