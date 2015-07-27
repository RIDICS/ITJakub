//window.onload = () => { alert("hello from editions!"); }
function initReader(bookXmlId, versionXmlId, bookTitle, pageList, searchedText) {
    var readerPlugin = new ReaderModule($("#ReaderDiv")[0]);
    readerPlugin.makeReader(bookXmlId, versionXmlId, bookTitle, pageList);
    var search;
    function convertSearchResults(responseResults) {
        var searchResults = new Array();
        for (var i = 0; i < responseResults.length; i++) {
            var result = responseResults[i];
            var resultContextStructure = result["ContextStructure"];
            var searchResult = new SearchResult();
            searchResult.pageXmlId = result["PageXmlId"];
            searchResult.pageName = result["PageName"];
            searchResult.before = resultContextStructure["Before"];
            searchResult.match = resultContextStructure["Match"];
            searchResult.after = resultContextStructure["After"];
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
                var convertedResults = convertSearchResults(response["results"]);
                readerPlugin.showSearchInPanel(convertedResults);
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
                var convertedResults = convertSearchResults(response["results"]);
                readerPlugin.showSearchInPanel(convertedResults);
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
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookCount",
            data: { text: text, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                updateQueryStringParameter("searchText", text);
                readerPlugin.setResultsPaging(response["count"], pageClickCallback);
            }
        });
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookPagesWithMatchHit",
            data: { text: text, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                readerPlugin.showSearchResultInPages(text, false, response["pages"]);
            }
        });
    }
    function advancedSearch(json) {
        if (typeof json === "undefined" || json === null || json === "")
            return;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookCount",
            data: { json: json, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                updateQueryStringParameter("searchText", json);
                readerPlugin.setResultsPaging(response["count"], pageClickCallback);
            }
        });
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookPagesWithMatchHit",
            data: { json: json, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                readerPlugin.showSearchResultInPages(json, true, response["pages"]);
            }
        });
    }
    search = new Search($("#SearchDiv")[0], advancedSearch, basicSearch);
    var disabledOptions = new Array();
    disabledOptions.push(0 /* Author */);
    disabledOptions.push(3 /* Dating */);
    disabledOptions.push(2 /* Editor */);
    disabledOptions.push(10 /* Headword */);
    disabledOptions.push(11 /* HeadwordDescription */);
    disabledOptions.push(12 /* HeadwordDescriptionTokenDistance */);
    disabledOptions.push(1 /* Title */);
    search.makeSearch(disabledOptions);
    if (typeof searchedText !== "undefined" && searchedText !== null) {
        var decodedText = decodeURIComponent(searchedText);
        decodedText = replaceSpecialChars(decodedText);
        search.processSearchQuery(decodedText);
    }
}
//# sourceMappingURL=itjakub.editions.js.map