﻿//window.onload = () => { alert("hello from editions!"); }

function initReader(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList: any, searchedText?: string) {
    var readerPlugin = new ReaderModule(<any>$("#ReaderDiv")[0]);
    readerPlugin.makeReader(bookXmlId, versionXmlId, bookTitle, pageList);
    var search: Search;

    function convertSearchResults(responseResults: Array<Object>): SearchResult[] {
        var searchResults = new Array<SearchResult>();
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

    function editionAdvancedSearchPaged(json: string, pageNumber: number) {

        if (typeof json === "undefined" || json === null || json === "") return;

        var start = (pageNumber - 1) * readerPlugin.getSearchResultsCountOnPage();
        var count = readerPlugin.getSearchResultsCountOnPage();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchInBookPaged",
            data: { json: json, start: start, count: count, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                var convertedResults = convertSearchResults(response["results"]);
                readerPlugin.showSearch(convertedResults);
            }
        });
    }

    function editionBasicSearchPaged(text: string, pageNumber: number) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * readerPlugin.getSearchResultsCountOnPage();
        var count = readerPlugin.getSearchResultsCountOnPage();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchInBookPaged",
            data: { text: text, start: start, count: count, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                var convertedResults = convertSearchResults(response["results"]);
                readerPlugin.showSearch(convertedResults);
            }
        });
    }

    function pageClickCallback(pageNumber: number) {

        if (search.isLastQueryJson()) {
            editionAdvancedSearchPaged(search.getLastQuery(), pageNumber);
        } else {
            editionBasicSearchPaged(search.getLastQuery(), pageNumber);
        }
    }

    function basicSearch(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var bookIds = new Array(); //TODO
        var categoryIds = new Array();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchCount",
            data: { text: text, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                updateQueryStringParameter("searchText", text);
                readerPlugin.setResultsPaging(response, pageClickCallback);
            }
        });
    }

    function advancedSearch(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;

        var bookIds = new Array();  //TODO
        var categoryIds = new Array();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                updateQueryStringParameter("searchText", json);
                readerPlugin.setResultsPaging(response, pageClickCallback);
            }
        });
    }



    search = new Search(<any>$("#SearchDiv")[0], advancedSearch, basicSearch);
    var disabledOptions = new Array<SearchTypeEnum>();
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