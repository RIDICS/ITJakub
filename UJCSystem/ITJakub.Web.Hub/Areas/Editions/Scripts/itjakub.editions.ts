//window.onload = () => { alert("hello from editions!"); }

function initReader(bookXmlId: string, bookTitle: string, pageList: any, searchedText?: string) {
    var readerPlugin = new ReaderModule(<any>$("#ReaderDiv")[0]);
    readerPlugin.makeReader(bookXmlId, bookTitle, pageList);

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
            }
        });
    }

    var search = new Search(<any>$("#SearchDiv")[0], advancedSearch, basicSearch);
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