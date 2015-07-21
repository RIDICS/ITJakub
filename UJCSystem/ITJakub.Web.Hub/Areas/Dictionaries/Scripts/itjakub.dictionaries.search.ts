$(document).ready(() => {
    var textSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaText";
    var jsonSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria";
    var search = new Search($("#dictionarySearchDiv"), jsonSearchUrl, textSearchUrl, processSearchResults);
    var disabledOptions = new Array<SearchTypeEnum>();
    disabledOptions.push(SearchTypeEnum.Headwords);
    disabledOptions.push(SearchTypeEnum.TokenDistanceHeadwords);
    search.makeSearch(disabledOptions);
});

function processSearchResults(result: any) {
    //alert("processed: " + result);
}