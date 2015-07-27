var search;
$(document).ready(function () {
    var booksCountOnPage = 3;
    var bookIds = new Array();
    var categoryIds = new Array();
    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", BookTypeEnum.Edition);
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
    var typeaheadSearchBox = new SearchBox(".searchbar-input", "Editions/Editions");
    typeaheadSearchBox.addDataSet("Title", "Název");
    typeaheadSearchBox.create();
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = function (state) {
        bookIds = new Array();
        for (var i = 0; i < state.SelectedItems.length; i++) {
            bookIds.push(state.SelectedItems[i].Id);
        }
        categoryIds = new Array();
        for (var i = 0; i < state.SelectedCategories.length; i++) {
            categoryIds.push(state.SelectedCategories[i].Id);
        }
        var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
        typeaheadSearchBox.clearAndDestroy();
        typeaheadSearchBox.addDataSet("Title", "Název", parametersUrl);
        typeaheadSearchBox.create();
    };
    var editionsSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Editions/Editions/GetEditionsWithCategories", true, callbackDelegate);
    editionsSelector.makeDropdown();
});
function listBook(target) {
    var bookId = $(target).parents("li.list-item").attr("data-bookid");
    window.location.href = getBaseUrl() + "Editions/Editions/Listing?bookId=" + bookId + "&searchText=" + search.getLastQuery();
}
//# sourceMappingURL=itjakub.editions.search.js.map