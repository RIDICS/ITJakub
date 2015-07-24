//window.onload = () => { alert("hello from editions!"); }
function initReader(bookXmlId, versionXmlId, bookTitle, pageList, searchedText) {
    var readerPlugin = new ReaderModule($("#ReaderDiv")[0]);
    readerPlugin.makeReader(bookXmlId, versionXmlId, bookTitle, pageList);
    var search;
    function convertSearchResults(responseResults) {
        var searchResults = new Array();
        for (var i = 0; i < responseResults.length; i++) {
            var book = responseResults[i];
            var searchResult = new SearchResult();
            //searchResult.after = book.    //TODO
            searchResults.push(searchResult);
        }
        return searchResults;
    }
    function editionAdvancedSearchPaged(json, pageNumber) {
        if (typeof json === "undefined" || json === null || json === "")
            return;
        var start = (pageNumber - 1) * readerPlugin.getSearchResultsCountOnPage();
        var count = readerPlugin.getSearchResultsCountOnPage();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookPaged",
            data: { json: json, start: start, count: count, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                var convertedResults = convertSearchResults(response);
                readerPlugin.showSearch(convertedResults);
            }
        });
    }
    function editionBasicSearchPaged(text, pageNumber) {
        if (typeof text === "undefined" || text === null || text === "")
            return;
        var start = (pageNumber - 1) * readerPlugin.getSearchResultsCountOnPage();
        var count = readerPlugin.getSearchResultsCountOnPage();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookPaged",
            data: { text: text, start: start, count: count, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                var convertedResults = convertSearchResults(response);
                readerPlugin.showSearch(convertedResults);
            }
        });
    }
    function pageClickCallback(pageNumber) {
        if (search.isLastQueryJson()) {
            editionAdvancedSearchPaged(search.getLastQuery(), pageNumber);
        }
        else {
            editionBasicSearchPaged(search.getLastQuery(), pageNumber);
        }
    }
    function basicSearch(text) {
        if (typeof text === "undefined" || text === null || text === "")
            return;
        var bookIds = new Array(); //TODO
        var categoryIds = new Array();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchCount",
            data: { text: text, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                updateQueryStringParameter("searchText", text);
                readerPlugin.setResultsPaging(response, pageClickCallback);
            }
        });
    }
    function advancedSearch(json) {
        if (typeof json === "undefined" || json === null || json === "")
            return;
        var bookIds = new Array(); //TODO
        var categoryIds = new Array();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                updateQueryStringParameter("searchText", json);
                readerPlugin.setResultsPaging(response, pageClickCallback);
            }
        });
    }
    search = new Search($("#SearchDiv")[0], advancedSearch, basicSearch);
    var disabledOptions = new Array();
    disabledOptions.push(SearchTypeEnum.Author);
    disabledOptions.push(SearchTypeEnum.Dating);
    disabledOptions.push(SearchTypeEnum.Editor);
    disabledOptions.push(SearchTypeEnum.Headword);
    disabledOptions.push(SearchTypeEnum.HeadwordDescription);
    disabledOptions.push(SearchTypeEnum.HeadwordDescriptionTokenDistance);
    disabledOptions.push(SearchTypeEnum.Title);
    search.makeSearch(disabledOptions);
    if (typeof searchedText !== "undefined" && searchedText !== null) {
        var decodedText = decodeURIComponent(searchedText);
        decodedText = replaceSpecialChars(decodedText);
        search.processSearchQuery(decodedText);
    }
}
//# sourceMappingURL=itjakub.editions.js.map