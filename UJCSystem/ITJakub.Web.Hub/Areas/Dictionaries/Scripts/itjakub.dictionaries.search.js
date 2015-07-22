$(document).ready(function () {
    var search = new Search($("#dictionarySearchDiv")[0], processSearchJson, processSearchText);
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
function processSearchResults(result) {
    alert("processed: " + result);
}
function processSearchJson(json) {
    $.ajax({
        type: "POST",
        traditional: true,
        data: JSON.stringify({
            "json": json
        }),
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaResultsCount",
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
//# sourceMappingURL=itjakub.dictionaries.search.js.map