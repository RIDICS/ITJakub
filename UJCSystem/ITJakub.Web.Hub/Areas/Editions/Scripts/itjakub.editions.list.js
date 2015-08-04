var search;
$(document).ready(function () {
    var booksCountOnPage = 5;
    var bookIds = new Array();
    var categoryIds = new Array();
    function sortOrderChanged() {
        search.processSearch();
    }
    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", sortOrderChanged, 0 /* Edition */);
    function editionAdvancedSearchPaged(json, pageNumber) {
        if (typeof json === "undefined" || json === null || json === "")
            return;
        var start = (pageNumber - 1) * bibliographyModule.getBooksCountOnPage();
        var count = bibliographyModule.getBooksCountOnPage();
        var sortAsc = bibliographyModule.isSortedAsc();
        var sortingEnum = bibliographyModule.getSortCriteria();
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();
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
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchPaged",
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
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();
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
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();
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
    var disabledOptions = new Array();
    disabledOptions.push(4 /* Fulltext */);
    disabledOptions.push(5 /* Heading */);
    disabledOptions.push(10 /* Headword */);
    disabledOptions.push(11 /* HeadwordDescription */);
    disabledOptions.push(12 /* HeadwordDescriptionTokenDistance */);
    disabledOptions.push(6 /* Sentence */);
    disabledOptions.push(9 /* TokenDistance */);
    search = new Search($("#listSearchDiv")[0], editionAdvancedSearch, editionBasicSearch);
    search.makeSearch(disabledOptions);
    var typeaheadSearchBox = new SearchBox(".searchbar-input", "Editions/Editions");
    typeaheadSearchBox.addDataSet("Title", "Název");
    typeaheadSearchBox.create();
    typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
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
        typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
    };
    var editionsSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Editions/Editions/GetEditionsWithCategories", true, callbackDelegate);
    editionsSelector.makeDropdown();
    $(".searchbar-input.tt-input").change(function () {
        typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
    });
    search.processSearchQuery("%"); //search for all by default criteria (title)
    search.writeTextToTextField("");
});
//# sourceMappingURL=itjakub.editions.list.js.map