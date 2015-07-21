$(document).ready(() => {
    var textSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaText";
    var jsonSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria";
    var search = new Search($("#dictionarySearchDiv"), jsonSearchUrl, textSearchUrl, processSearchResults);
    var disabledOptions = new Array<SearchTypeEnum>();
    disabledOptions.push(SearchTypeEnum.Headwords);
    disabledOptions.push(SearchTypeEnum.TokenDistanceHeadwords);
    search.makeSearch(disabledOptions);

    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = (state) => {
        
    };

    var dictionarySelector = new DropDownSelect2("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
});
function processSearchResults(result: any) {
    //alert("processed: " + result);
}