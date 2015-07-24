$(document).ready(function () {
    var booksCountOnPage = 3;
    var bookIds = new Array();
    var categoryIds = new Array();
    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", 0 /* Edition */);
    function editionAdvancedSearchPaged(json, pageNumber) {
        if (typeof json === "undefined" || json === null || json === "")
            return;
        var start = (pageNumber - 1) * bibliographyModule.getBooksCountOnPage();
        var count = bibliographyModule.getBooksCountOnPage();
        var sortAsc = bibliographyModule.isSortedAsc();
        var sortingEnum = bibliographyModule.getSortCriteria();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchPaged",
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                bibliographyModule.showBooks(response.books);
            }
        });
    }
    function editionBasicSearchPaged(text, pageNumber) {
        if (typeof text === "undefined" || text === null || text === "")
            return;
        var start = (pageNumber - 1) * bibliographyModule.getBooksCountOnPage();
        var count = bibliographyModule.getBooksCountOnPage();
        var sortAsc = bibliographyModule.isSortedAsc();
        var sortingEnum = bibliographyModule.getSortCriteria();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchCount",
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                bibliographyModule.showBooks(response.books);
            }
        });
    }
    var search;
    function pageClickCallbackForBiblModule(pageNumber) {
        if (search.isLastQueryJson()) {
            editionAdvancedSearchPaged(search.getLastQuery(), pageNumber);
        }
        else {
            editionBasicSearchPaged(search.getLastQuery(), pageNumber);
        }
    }
    function editionBasicSearch(text) {
        if (typeof text === "undefined" || text === null || text === "")
            return;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchCount",
            data: { text: text, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                bibliographyModule.createPagination(booksCountOnPage, pageClickCallbackForBiblModule, response["count"]); //enable pagination
            }
        });
    }
    function editionAdvancedSearch(json) {
        if (typeof json === "undefined" || json === null || json === "")
            return;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                bibliographyModule.createPagination(booksCountOnPage, pageClickCallbackForBiblModule, response["count"]); //enable pagination
            }
        });
    }
    search = new Search($("#listSearchDiv")[0], editionAdvancedSearch, editionBasicSearch);
    search.makeSearch();
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = function (state) {
        bookIds = new Array();
        bookIds.push(1); //TODO remove
        for (var i = 0; i < state.SelectedItems.length; i++) {
            bookIds.push(state.SelectedItems[i].Id);
        }
        categoryIds = new Array();
        for (var i = 0; i < state.SelectedCategories.length; i++) {
            categoryIds.push(state.SelectedCategories[i].Id);
        }
    };
    var editionsSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Editions/Editions/GetEditionsWithCategories", true, callbackDelegate);
    editionsSelector.makeDropdown();
});
//# sourceMappingURL=itjakub.editions.search.js.map