function initGrammarReader(bookId: number, bookXmlId: string, versionXmlId: string, bookTitle: string, pageList: any, searchedText?: string, initPageXmlId?: string) {


    function readerPageChangedCallback(pageId: number) {
        updateQueryStringParameter("page", pageId);
    }

    function hideTypeahead() {
        $(".twitter-typeahead").find(".tt-menu").hide();
    };

    const readerPanels = [ReaderPanelEnum.ImagePanel, ReaderPanelEnum.TermsPanel];
    const leftPanelButtons = [PanelButtonEnum.Pin, PanelButtonEnum.Close];
    const mainPanelButtons = [PanelButtonEnum.Pin];
    
    var readerPlugin = new ReaderModule(<HTMLDivElement>$("#ReaderDiv")[0], readerPageChangedCallback, readerPanels, leftPanelButtons, mainPanelButtons);
    readerPlugin.makeReader(bookXmlId, versionXmlId, bookTitle, pageList);
    readerPlugin.setTermPanelCallback((termId: number, text: string) => {
        window.location.href = getBaseUrl() + "OldGrammar/OldGrammar/Search?search=" + text;
    });
    
    readerPlugin.changeSidePanelVisibility(readerPlugin.termsPanelIdentificator, 'left');

    var search: Search;
    
    function convertSearchResults(responseResults: Array<IPage>): PageDescription[]{

        var searchResults = new Array<PageDescription>();
        for (var i = 0; i < responseResults.length; i++) {
            var result = responseResults[i];
            var searchResult = new PageDescription();
            searchResult.pageId = result.id;
            searchResult.pageName = result.name;
            searchResults.push(searchResult);
        }

        return searchResults;
    }

    function basicSearch(text: string) {
        hideTypeahead();
        if (typeof text === "undefined" || text === null || text === "") return;

        readerPlugin.termsPanelClearResults();
        readerPlugin.termsPanelShowLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "OldGrammar/OldGrammar/TextSearchInBook",
            data: { text: text, projectId: readerPlugin.getBookXmlId(), snapshotId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                updateQueryStringParameter("searchText", text);

                readerPlugin.termsPanelRemoveLoading();
                readerPlugin.showSearchInTermsPanel(convertSearchResults(response["results"]));
            }
        });
    }

    function advancedSearch(json: string) {
        hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        readerPlugin.termsPanelClearResults();
        readerPlugin.termsPanelShowLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "OldGrammar/OldGrammar/AdvancedSearchInBook",
            data: { json: json, projectId: readerPlugin.getBookXmlId(), snapshotId: readerPlugin.getVersionXmlId() },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                updateQueryStringParameter("searchText", json);

                readerPlugin.termsPanelRemoveLoading();
                readerPlugin.showSearchInTermsPanel(convertSearchResults(response["results"]));
            }
        });
    }

    var enabledOptions = new Array<SearchTypeEnum>();
    enabledOptions.push(SearchTypeEnum.Term);

    var favoriteQueriesConfig: IModulInicializatorConfigurationSearchFavorites = {
        bookType: BookTypeEnum.Grammar,
        queryType: QueryTypeEnum.Reader
    };
    search = new Search(<any>$("#SearchDiv")[0], advancedSearch, basicSearch, favoriteQueriesConfig);
    search.makeSearch(enabledOptions);

    var typeaheadSearchBox = new SearchBox(".searchbar-input", "OldGrammar/OldGrammar");
    typeaheadSearchBox.addDataSet("Term", "Téma", `selectedBookIds=${bookId}`);
    typeaheadSearchBox.create();
    typeaheadSearchBox.value($(".searchbar-input.tt-input").val());

    $(".searchbar-input.tt-input").change(() => {        //prevent clearing input value on blur() 
        typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
    });

    if (typeof searchedText !== "undefined" && searchedText !== null) {
        var decodedText = decodeURIComponent(searchedText);
        decodedText = replaceSpecialChars(decodedText);
        search.processSearchQuery(decodedText);
    }

    if (typeof initPageXmlId !== "undefined" && initPageXmlId !== null) {
        var decodedText = decodeURIComponent(initPageXmlId);
        decodedText = replaceSpecialChars(decodedText);
        var pageId = Number(decodedText);
        readerPlugin.moveToPage(pageId, true);
    }

    //label item in main menu
    $('#main-plugins-menu').find('li').removeClass('active');
    var mainMenuLi = $('#grammars-menu');
    $(mainMenuLi).addClass('active');
}

function listGrammarBookReadClicked(target) {
    return context => {
        var bookId = $(target).parents("li.list-item").attr("data-id");
        if (context.search.isLastQueryJson()) { //only text seach criteria we should propagate
            onClickHref(context.event, getBaseUrl() + "BookReader/BookReader/Listing?bookId=" + bookId + "&searchText=" + context.search.getLastQuery());
        } else {
            onClickHref(context.event, getBaseUrl() + "BookReader/BookReader/Listing?bookId=" + bookId);
        }
    }
}

function searchGrammarBookReadClicked(target) {
    return context => {
        var bookId = $(target).parents("li.list-item").attr("data-id");
        onClickHref(context.event, getBaseUrl() + "BookReader/BookReader/Listing?bookId=" + bookId + "&searchText=" + context.search.getLastQuery());
    }
}