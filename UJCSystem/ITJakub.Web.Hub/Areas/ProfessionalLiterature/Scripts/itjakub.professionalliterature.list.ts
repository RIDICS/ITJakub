

var search: Search;

$(document).ready(() => {
    var booksCountOnPage = 5;

    var bookIds = new Array();
    var categoryIds = new Array();

    function sortOrderChanged() {
        var textInTextField = search.getTextFromTextField();
        search.processSearchQuery(search.getLastQuery());
        search.writeTextToTextField(textInTextField);
    }

    function hideTypeahead() {
        $(".twitter-typeahead").find(".tt-menu").hide();
    };

    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", sortOrderChanged, BookTypeEnum.ProfessionalLiterature, "ProfessionalLiterature/ProfessionalLiterature/GetListConfiguration");

    function advancedSearchPaged(json: string, pageNumber: number) {
        hideTypeahead();
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
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);
            }
        });
    }

    function basicSearchPaged(text: string, pageNumber: number) {
        hideTypeahead();
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
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchPaged",
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);
            }
        });
    }

    function pageClickCallbackForBiblModule(pageNumber: number) {

        if (search.isLastQueryJson()) {
            advancedSearchPaged(search.getLastQuery(), pageNumber);
        } else {
            basicSearchPaged(search.getLastQuery(), pageNumber);
        }
    }

    function proffesionalLiteratureBasicSearch(text: string) {
        hideTypeahead();
        //if (typeof text === "undefined" || text === null || text === "") return;

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/TextSearchCount",
            data: { text: text, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.createPagination(booksCountOnPage, pageClickCallbackForBiblModule, response["count"]); //enable pagination
            }
        });
    }

    function professionalLiteratureAdvancedSearch(json: string) {
        hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {

                bibliographyModule.createPagination(booksCountOnPage, pageClickCallbackForBiblModule, response["count"]); //enable pagination

            }
        });
    }


    var enabledOptions = new Array<SearchTypeEnum>();
    enabledOptions.push(SearchTypeEnum.Title);
    enabledOptions.push(SearchTypeEnum.Author);
    enabledOptions.push(SearchTypeEnum.Editor);
    enabledOptions.push(SearchTypeEnum.Dating);

    search = new Search(<any>$("#listSearchDiv")[0], professionalLiteratureAdvancedSearch, proffesionalLiteratureBasicSearch);
    search.makeSearch(enabledOptions);

    var typeaheadSearchBox = new SearchBox(".searchbar-input", "ProfessionalLiterature/ProfessionalLiterature");
    typeaheadSearchBox.addDataSet("Title", "Název");
    typeaheadSearchBox.create();
    typeaheadSearchBox.value($(".searchbar-input.tt-input").val());

    var professionalLiteratureSelector: DropDownSelect2;
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = (state: State) => {
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
    callbackDelegate.dataLoadedCallback = () => {
        var selectedIds = professionalLiteratureSelector.getSelectedIds();
        bookIds = selectedIds.selectedBookIds;
        categoryIds = selectedIds.selectedCategoryIds;
        search.processSearch();
        //search.processSearchQuery("%"); //search for all by default criteria (title)
        //search.writeTextToTextField("");
    };

    professionalLiteratureSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "ProfessionalLiterature/ProfessionalLiterature/GetProfessionalLiteratureWithCategories", true, callbackDelegate);
    professionalLiteratureSelector.makeDropdown();


    $(".searchbar-input.tt-input").change(() => {        //prevent clearing input value on blur() 
        typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
    });

});



