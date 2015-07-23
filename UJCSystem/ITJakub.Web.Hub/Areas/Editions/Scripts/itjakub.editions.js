//window.onload = () => { alert("hello from editions!"); }
function initReader(bookXmlId, bookTitle, pageList) {
    var readerPlugin = new ReaderModule($("#ReaderDiv")[0]);
    readerPlugin.makeReader(bookXmlId, bookTitle, pageList);
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
            }
        });
    }
    var search = new Search($("#SearchDiv")[0], advancedSearch, basicSearch);
    var disabledOptions = new Array();
    disabledOptions.push(0 /* Author */);
    disabledOptions.push(3 /* Dating */);
    disabledOptions.push(2 /* Editor */);
    disabledOptions.push(10 /* Headword */);
    disabledOptions.push(11 /* HeadwordDescription */);
    disabledOptions.push(12 /* HeadwordDescriptionTokenDistance */);
    disabledOptions.push(1 /* Title */);
    search.makeSearch(disabledOptions);
}
//# sourceMappingURL=itjakub.editions.js.map