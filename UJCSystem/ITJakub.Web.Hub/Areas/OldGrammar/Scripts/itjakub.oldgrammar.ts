function initGrammarReader(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList: any, searchedText?: string, initPageXmlId?: string) {


    function readerPageChangedCallback(pageXmlId: string) {
        updateQueryStringParameter("page", pageXmlId);
    }

    var readerPanels = [ReaderPanelEnum.ImagePanel, ReaderPanelEnum.TermsPanel];
    var leftPanelButtons = [PanelButtonEnum.Pin, PanelButtonEnum.Close];
    var mainPanelButtons = [PanelButtonEnum.Pin];

    var readerPlugin = new ReaderModule(<any>$("#ReaderDiv")[0], readerPageChangedCallback, readerPanels, leftPanelButtons, mainPanelButtons);
    readerPlugin.makeReader(bookXmlId, versionXmlId, bookTitle, pageList);
    var search: Search;
    
    function convertSearchResults(responseResults: Array<Object>): PageDescription[]{

        var searchResults = new Array<PageDescription>();
        for (var i = 0; i < responseResults.length; i++) {
            var result = responseResults[i];
            var searchResult = new PageDescription();
            searchResult.PageXmlId = result["PageXmlId"];
            searchResult.PageName = result["PageName"];
            searchResults.push(searchResult);
        }

        return searchResults;
    }

    function basicSearch(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;

        readerPlugin.termsPanelClearResults();
        readerPlugin.termsPanelShowLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchInBook",
            data: { text: text, bookXmlId: readerPlugin.getBookXmlId(), versionXmlId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                updateQueryStringParameter("searchText", text);

                readerPlugin.termsPanelRemoveLoading();
                readerPlugin.showSearchInTermsPanel(convertSearchResults(response["results"]));
            }
        });

    }

    search = new Search(<any>$("#SearchDiv")[0], null, basicSearch);
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

    if (typeof initPageXmlId !== "undefined" && initPageXmlId !== null) {
        var decodedText = decodeURIComponent(initPageXmlId);
        decodedText = replaceSpecialChars(decodedText);
        readerPlugin.moveToPage(decodedText, true);
    }
}

function listGrammarBook(target) {
    var bookId = $(target).parents("li.list-item").attr("data-bookid");
    if (search.isLastQueryJson()) {     //only text seach criteria we should propagate
        window.location.href = getBaseUrl() + "OldGrammar/OldGrammar/Listing?bookId=" + bookId + "&searchText=" + search.getLastQuery();
    } else {
        window.location.href = getBaseUrl() + "OldGrammar/OldGrammar/Listing?bookId=" + bookId;
    }

}