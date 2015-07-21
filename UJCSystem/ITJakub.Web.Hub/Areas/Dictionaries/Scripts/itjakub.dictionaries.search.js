$(document).ready(function () {
    var textSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaText";
    var jsonSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria";
    var search = new Search($("#dictionarySearchDiv"), jsonSearchUrl, textSearchUrl, processSearchResults);
    var disabledOptions = new Array();
    disabledOptions.push(0 /* Author */);
    disabledOptions.push(1 /* Title */);
    disabledOptions.push(2 /* Editor */);
    disabledOptions.push(3 /* Dating */);
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
//# sourceMappingURL=itjakub.dictionaries.search.js.map