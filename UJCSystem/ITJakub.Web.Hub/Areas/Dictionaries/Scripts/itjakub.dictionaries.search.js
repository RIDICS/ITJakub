$(document).ready(function () {
    var textSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaText";
    var jsonSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria";
    var search = new Search($("#dictionarySearchDiv"), jsonSearchUrl, textSearchUrl, processSearchResults);
    var disabledOptions = new Array();
    disabledOptions.push(10 /* Headwords */);
    disabledOptions.push(11 /* TokenDistanceHeadwords */);
    search.makeSearch(disabledOptions);
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = function (state) {
    };
    var dictionarySelector = new DropDownSelect2("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
});
function processSearchResults(result) {
    //alert("processed: " + result);
}
//# sourceMappingURL=itjakub.dictionaries.search.js.map