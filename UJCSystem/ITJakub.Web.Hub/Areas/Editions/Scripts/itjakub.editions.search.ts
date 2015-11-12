
var search: Search;
 
$(document).ready(() => {

    const urlSearchKey = "search";
    const urlPageKey = "page";
    const urlSelectionKey = "selected";
    const urlSortAscKey = "sortAsc";
    const urlSortCriteriaKey = "sortCriteria";

    var booksCountOnPage = 5;

    var bookIdsInQuery = new Array();
    var categoryIdsInQuery = new Array();

    var selectedBookIds = new Array();
    var selectedCategoryIds = new Array();

    var bibliographyModule: BibliographyModule;

    function actualizeSelectedBooksAndCategoriesInQuery() {
        bookIdsInQuery = selectedBookIds;
        categoryIdsInQuery = selectedCategoryIds;
    }

    var editionsSelector: DropDownSelect2;
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = (state: State) => {
        selectedBookIds = new Array();

        for (var i = 0; i < state.SelectedItems.length; i++) {
            selectedBookIds.push(state.SelectedItems[i].Id);
        }

        selectedCategoryIds = new Array();

        for (var i = 0; i < state.SelectedCategories.length; i++) {
            selectedCategoryIds.push(state.SelectedCategories[i].Id);
        }

        var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
    };
    callbackDelegate.dataLoadedCallback = () => {
        var selectedIds = editionsSelector.getSelectedIds();
        selectedBookIds = selectedIds.selectedBookIds;
        selectedCategoryIds = selectedIds.selectedCategoryIds;
    };

    editionsSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Editions/Editions/GetEditionsWithCategories", true, callbackDelegate);
    editionsSelector.makeDropdown();

    function editionAdvancedSearchPaged(json: string, pageNumber: number) {

        if (typeof json === "undefined" || json === null || json === "") return;

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
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);
                updateQueryStringParameter(urlSearchKey, json);
                updateQueryStringParameter(urlPageKey, pageNumber);
            }
        });
    }

    function editionBasicSearchPaged(text: string, pageNumber: number) {

        //if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber-1) * bibliographyModule.getBooksCountOnPage();
        var count = bibliographyModule.getBooksCountOnPage();
        var sortAsc = bibliographyModule.isSortedAsc();
        var sortingEnum = bibliographyModule.getSortCriteria();

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchFulltextPaged",
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);
                updateQueryStringParameter(urlSearchKey, text);
                updateQueryStringParameter(urlPageKey, pageNumber);
            }
        });
    }

    function pageClickCallbackForBiblModule(pageNumber: number) {

        if (search.isLastQueryJson()) {
            editionAdvancedSearchPaged(search.getLastQuery(), pageNumber);
        } else {
            editionBasicSearchPaged(search.getLastQuery(), pageNumber);
        }
    }

    function sortOrderChanged() {
        bibliographyModule.showPage(1);
        updateQueryStringParameter(urlSortAscKey, bibliographyModule.isSortedAsc());
        updateQueryStringParameter(urlSortCriteriaKey, bibliographyModule.getSortCriteria());
    }

    bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", sortOrderChanged, BookTypeEnum.Edition, "Editions/Editions/GetSearchConfiguration");

    function editionBasicSearch(text: string) {
        actualizeSelectedBooksAndCategoriesInQuery();
        //if (typeof text === "undefined" || text === null || text === "") return;

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearchFulltextCount",
            data: { text: text, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.createPagination(booksCountOnPage, pageClickCallbackForBiblModule, response["count"]); //enable pagination
                updateQueryStringParameter(urlSearchKey, text);
                updateQueryStringParameter(urlSelectionKey, DropDownSelect2.getUrlStringFromState(editionsSelector.getState()));
            }
        });
    }

    function editionAdvancedSearch(json: string) {

        if (typeof json === "undefined" || json === null || json === "") return;
        actualizeSelectedBooksAndCategoriesInQuery();

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();
        
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery},
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.createPagination(booksCountOnPage, pageClickCallbackForBiblModule, response["count"]); //enable pagination
                updateQueryStringParameter(urlSearchKey, json);
                updateQueryStringParameter(urlSelectionKey, DropDownSelect2.getUrlStringFromState(editionsSelector.getState()));
            }
        });
    }

    var enabledOptions = new Array<SearchTypeEnum>();
    enabledOptions.push(SearchTypeEnum.Title);
    enabledOptions.push(SearchTypeEnum.Author);
    enabledOptions.push(SearchTypeEnum.Editor);
    enabledOptions.push(SearchTypeEnum.Dating);
    enabledOptions.push(SearchTypeEnum.Fulltext);
    enabledOptions.push(SearchTypeEnum.TokenDistance);
    enabledOptions.push(SearchTypeEnum.Heading);
    enabledOptions.push(SearchTypeEnum.Sentence);

    search = new Search(<any>$("#listSearchDiv")[0], editionAdvancedSearch, editionBasicSearch);
    search.makeSearch(enabledOptions);

});

