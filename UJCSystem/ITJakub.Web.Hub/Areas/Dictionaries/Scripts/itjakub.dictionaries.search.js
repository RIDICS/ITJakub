$(document).ready(function () {
    var search = new Search($("#dictionarySearchDiv"), processSearchJson, processSearchText);
    var disabledOptions = new Array();
    disabledOptions.push(SearchTypeEnum.Author);
    disabledOptions.push(SearchTypeEnum.Title);
    disabledOptions.push(SearchTypeEnum.Editor);
    disabledOptions.push(SearchTypeEnum.Dating);
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
function processSearchJson(json) {
    $.ajax({
        type: "POST",
        traditional: true,
        data: JSON.stringify({
            "json": json,
            "start": 0,
            "count": 50
        }),
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            processSearchResults(response);
        },
        error: function (response) {
        }
    });
}
function processSearchText(text) {
    $.ajax({
        type: "GET",
        traditional: true,
        data: JSON.stringify({
            "text": text,
            "start": 0,
            "count": 50
        }),
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaText",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            processSearchResults(response);
        },
        error: function (response) {
        }
    });
}
//# sourceMappingURL=itjakub.dictionaries.search.js.map