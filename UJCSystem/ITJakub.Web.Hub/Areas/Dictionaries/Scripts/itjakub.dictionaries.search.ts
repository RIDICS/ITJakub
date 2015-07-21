$(document).ready(() => {
    var search = new Search($("#dictionarySearchDiv"));
    var disabledOptions = new Array<SearchTypeEnum>();
    disabledOptions.push(SearchTypeEnum.Editor);
    disabledOptions.push(SearchTypeEnum.Author);
    search.makeSearch(disabledOptions);
});