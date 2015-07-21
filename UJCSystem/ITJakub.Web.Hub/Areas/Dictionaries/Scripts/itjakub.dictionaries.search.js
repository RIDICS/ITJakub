$(document).ready(function () {
    var textSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaText";
    var jsonSearchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria";
    var search = new Search($("#dictionarySearchDiv"), jsonSearchUrl, textSearchUrl, processSearchResults);
    var disabledOptions = new Array();
    disabledOptions.push(2 /* Editor */);
    disabledOptions.push(0 /* Author */);
    search.makeSearch(disabledOptions);
});
function processSearchResults(result) {
    //alert("processed: " + result);
}
//# sourceMappingURL=itjakub.dictionaries.search.js.map