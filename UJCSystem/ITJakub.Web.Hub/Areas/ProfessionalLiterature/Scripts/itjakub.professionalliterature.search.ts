
var search: Search;

$(document).ready(() => {

    const urlSearchKey = "search";
    const urlPageKey = "page";
    const urlSelectionKey = "selected";
    const urlSortAscKey = "sortAsc";
    const urlSortCriteriaKey = "sortCriteria";

    var readyForInit = false;
    var notInitialized = true;

    var booksCountOnPage = 5;

    var bookIdsInQuery = new Array();
    var categoryIdsInQuery = new Array();

    var selectedBookIds = new Array();
    var selectedCategoryIds = new Array();

    var bibliographyModule: BibliographyModule;
    var booksSelector: DropDownSelect2;

    var initPage: number = null;

    function initializeFromUrlParams() {
        if (readyForInit && notInitialized) {

            notInitialized = false;

            var page = getQueryStringParameterByName(urlPageKey);

            if (page) {
                initPage = parseInt(page);
            }

            var sortedAsc = getQueryStringParameterByName(urlSortAscKey);
            var sortCriteria = getQueryStringParameterByName(urlSortCriteriaKey);

            if (sortedAsc && sortCriteria) {
                bibliographyModule.setSortedAsc(sortedAsc === "true");
                bibliographyModule.setSortCriteria(<SortEnum>(<any>(sortCriteria)));
            }

            var selected = getQueryStringParameterByName(urlSelectionKey);

            var searched = getQueryStringParameterByName(urlSearchKey);
            search.writeTextToTextField(searched);

            if (selected) {
                booksSelector.setStateFromUrlString(selected);
            }

        } else if (!notInitialized) {
            search.processSearch();
        } else {
            readyForInit = true;
        }

    }

    function actualizeSelectedBooksAndCategoriesInQuery() {
        bookIdsInQuery = selectedBookIds;
        categoryIdsInQuery = selectedCategoryIds;
    }

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
        var selectedIds = booksSelector.getSelectedIds();
        selectedBookIds = selectedIds.selectedBookIds;
        selectedCategoryIds = selectedIds.selectedCategoryIds;
        initializeFromUrlParams();
    };
    
    booksSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/GetProfessionalLiteratureWithCategories", true, callbackDelegate);
    booksSelector.makeDropdown();

    function professionalLiteratureAdvancedSearchPaged(json: string, pageNumber: number) {

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
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchPaged",
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);
                updateQueryStringParameter(urlSearchKey, json);
                updateQueryStringParameter(urlPageKey, pageNumber);
                updateQueryStringParameter(urlSortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(urlSortCriteriaKey, bibliographyModule.getSortCriteria());
            }
        });
    }

    function professionalLiteratureBasicSearchPaged(text: string, pageNumber: number) {

        //if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * bibliographyModule.getBooksCountOnPage();
        var count = bibliographyModule.getBooksCountOnPage();
        var sortAsc = bibliographyModule.isSortedAsc();
        var sortingEnum = bibliographyModule.getSortCriteria();

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchFulltextPaged",
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);
                updateQueryStringParameter(urlSearchKey, text);
                updateQueryStringParameter(urlPageKey, pageNumber);
                updateQueryStringParameter(urlSortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(urlSortCriteriaKey, bibliographyModule.getSortCriteria());
            }
        });
    }

    function pageClickCallbackForBiblModule(pageNumber: number) {

        if (search.isLastQueryJson()) {
            professionalLiteratureAdvancedSearchPaged(search.getLastQuery(), pageNumber);
        } else {
            professionalLiteratureBasicSearchPaged(search.getLastQuery(), pageNumber);
        }
    }

    function sortOrderChanged() {
        bibliographyModule.showPage(1);
    }

    bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", sortOrderChanged, BookTypeEnum.ProfessionalLiterature, "ProfessionalLiterature/ProfessionalLiterature/GetSearchConfiguration");


    function createPagination(booksCount: number) {
        var pages = Math.ceil(booksCount / booksCountOnPage);
        if (initPage && initPage <= pages) {
            bibliographyModule.createPagination(booksCountOnPage, pageClickCallbackForBiblModule, booksCount, initPage);
        } else {
            bibliographyModule.createPagination(booksCountOnPage, pageClickCallbackForBiblModule, booksCount);
        }
    }


    function professionalLiteratureBasicSearch(text: string) {
        actualizeSelectedBooksAndCategoriesInQuery();
        //if (typeof text === "undefined" || text === null || text === "") return;

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchFulltextCount",
            data: { text: text, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(urlSearchKey, text);
                updateQueryStringParameter(urlSelectionKey, DropDownSelect2.getUrlStringFromState(booksSelector.getState()));
                updateQueryStringParameter(urlSortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(urlSortCriteriaKey, bibliographyModule.getSortCriteria());
            }
        });
    }

    function professionalLiteratureAdvancedSearch(json: string) {

        if (typeof json === "undefined" || json === null || json === "") return;
        actualizeSelectedBooksAndCategoriesInQuery();

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(urlSearchKey, json);
                updateQueryStringParameter(urlSelectionKey, DropDownSelect2.getUrlStringFromState(booksSelector.getState()));
                updateQueryStringParameter(urlSortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(urlSortCriteriaKey, bibliographyModule.getSortCriteria());
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

    search = new Search(<any>$("#listSearchDiv")[0], professionalLiteratureAdvancedSearch, professionalLiteratureBasicSearch);
    search.makeSearch(enabledOptions);

    initializeFromUrlParams();
});

