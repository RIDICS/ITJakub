$(document).ready(function () {
    var search = new Search($("#dictionarySearchDiv"));
    var disabledOptions = new Array();
    disabledOptions.push(SearchTypeEnum.Editor);
    disabledOptions.push(SearchTypeEnum.Author);
    search.makeSearch(disabledOptions);
});
//# sourceMappingURL=itjakub.dictionaries.search.js.map