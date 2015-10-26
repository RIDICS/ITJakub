function initProfLitReader(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList: any, searchedText?: string, initPageXmlId?: string) {


    function readerPageChangedCallback(pageXmlId: string) {
        updateQueryStringParameter("page", pageXmlId);
    }

    var readerPanels = [ReaderPanelEnum.TextPanel, ReaderPanelEnum.ImagePanel, ReaderPanelEnum.ContentPanel, ReaderPanelEnum.SearchPanel, ReaderPanelEnum.SettingsPanel];
    var panelButtons = [PanelButtonEnum.Close, PanelButtonEnum.Pin, PanelButtonEnum.ToNewWindow];

    var readerPlugin = new ReaderModule(<any>$("#ReaderDiv")[0], readerPageChangedCallback, readerPanels, panelButtons, panelButtons);
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

    function professionalLiteratureAdvancedSearchPaged(json: string, pageNumber: number) {

        if (typeof json === "undefined" || json === null || json === "") return;

        var start = (pageNumber - 1) * readerPlugin.getSearchResultsCountOnPage();
        var count = readerPlugin.getSearchResultsCountOnPage();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchInBookPaged",
            data: { json: json, start: start, count: count, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                var convertedResults = convertSearchResults(response["results"]);
                readerPlugin.searchPanelRemoveLoading();
                readerPlugin.showSearchInPanel(convertedResults);
            }
        });
    }

    function professionalLiteratureBasicSearchPaged(text: string, pageNumber: number) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * readerPlugin.getSearchResultsCountOnPage();
        var count = readerPlugin.getSearchResultsCountOnPage();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchInBookPaged",
            data: { text: text, start: start, count: count, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                var convertedResults = convertSearchResults(response["results"]);
                readerPlugin.searchPanelRemoveLoading();
                readerPlugin.showSearchInPanel(convertedResults);
            }
        });
    }

    function paginatorPageClickCallback(pageNumber: number) {

        readerPlugin.searchPanelClearResults();
        readerPlugin.searchPanelShowLoading();

        if (search.isLastQueryJson()) {
            professionalLiteratureAdvancedSearchPaged(search.getLastQuery(), pageNumber);
        } else {
            professionalLiteratureBasicSearchPaged(search.getLastQuery(), pageNumber);
        }
    }

    function basicSearch(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchInBookCount",
            data: { text: text, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                updateQueryStringParameter("searchText", text);
                readerPlugin.setResultsPaging(response["count"], paginatorPageClickCallback);
            }
        });

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchInBookPagesWithMatchHit",
            data: { text: text, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                readerPlugin.showSearchResultInPages(text, false, response["pages"]);
            }
        });
    }

    function advancedSearch(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchInBookCount",
            data: { json: json, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                updateQueryStringParameter("searchText", json);
                readerPlugin.setResultsPaging(response["count"], paginatorPageClickCallback);
            }
        });


        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchInBookPagesWithMatchHit",
            data: { json: json, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                readerPlugin.showSearchResultInPages(json, true, response["pages"]);
            }
        });
    }



    search = new Search(<any>$("#SearchDiv")[0], advancedSearch, basicSearch);
    var enabledOptions = new Array<SearchTypeEnum>();
    enabledOptions.push(SearchTypeEnum.Fulltext);
    enabledOptions.push(SearchTypeEnum.TokenDistance);
    enabledOptions.push(SearchTypeEnum.Sentence);
    enabledOptions.push(SearchTypeEnum.Heading);

    search.makeSearch(enabledOptions);

    if (typeof searchedText !== "undefined" && searchedText !== null) {
        var decodedText = decodeURIComponent(searchedText);
        decodedText = replaceSpecialChars(decodedText);
        search.processSearchQuery(decodedText);
    }

    if (typeof initPageXmlId !== "undefined" && initPageXmlId !== null) {
        var decodedText = decodeURIComponent(initPageXmlId);
        decodedText = replaceSpecialChars(decodedText);
        readerPlugin.moveToPage(decodedText, true);
    }


    //label item in main menu
    $('#main-plugins-menu').find('li').removeClass('active');
    var mainMenuLi = $('#professional-menu');
    $(mainMenuLi).addClass('active');
}

function listProfLitBookReadClicked(target) {
    var bookId = $(target).parents("li.list-item").attr("data-bookid");
    if (search.isLastQueryJson()) {     //only text seach criteria we should propagate
        window.location.href = getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/Listing?bookId=" + bookId + "&searchText=" + search.getLastQuery();
    } else {
        window.location.href = getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/Listing?bookId=" + bookId;
    }

}

function searchProfLitBookReadClicked(target) {
    var bookId = $(target).parents("li.list-item").attr("data-bookid");
    window.location.href = getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/Listing?bookId=" + bookId + "&searchText=" + search.getLastQuery();
}