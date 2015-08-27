function initGrammarReader(bookXmlId: string, versionXmlId: string, bookTitle: string, pageList: any, searchedText?: string, initPageXmlId?: string) {


    function readerPageChangedCallback(pageXmlId: string) {
        updateQueryStringParameter("page", pageXmlId);
    }

    var readerPanels = [ReaderPanelEnum.ImagePanel, ReaderPanelEnum.TermsPanel];
    var leftPanelButtons = [PanelButtonEnum.Pin, PanelButtonEnum.Close, PanelButtonEnum.ToNewWindow];
    var mainPanelButtons = [PanelButtonEnum.Pin];

    var readerPlugin = new ReaderModule(<any>$("#ReaderDiv")[0], readerPageChangedCallback, readerPanels, leftPanelButtons, mainPanelButtons);
    readerPlugin.makeReader(bookXmlId, versionXmlId, bookTitle, pageList);
    var search: Search;
    

    function basicSearch(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;

    }

    function advancedSearch(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;

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