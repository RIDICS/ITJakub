var search: Search;

$(document).ready(() => {
    var booksCountOnPage = 5;

    var bookIds = new Array();
    var categoryIds = new Array();

    function sortOrderChanged() {
        search.processSearch();
    }
    
    function corpusAdvancedSearchPaged(json: string, pageNumber: number) {

        if (typeof json === "undefined" || json === null || json === "") return;
        
        var start = 0;
        var count = 10;
        var sortingEnum = SortEnum.Title;
        var sortAsc = true;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchPaged",
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                
            }
        });
    }

    function corpusBasicSearchPaged(text: string, pageNumber: number) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var start = 0;
        var count = 10;
        var sortingEnum = SortEnum.Title;
        var sortAsc = true;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchPaged",
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
            }
        });
    }

    function corpusBasicSearchCount(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var start = 0;
        var count = 10;
        var sortingEnum = SortEnum.Title;
        var sortAsc = true;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchCount",
            data: { text: text, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                corpusBasicSearchPaged(text, 0);
            }
        });
    }

    function corpusAdvancedSearchCount(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;

        var start = 0;
        var count = 10;
        var sortingEnum = SortEnum.Title;
        var sortAsc = true;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                corpusAdvancedSearchPaged(json, 0);
            }
        });
    }

    search = new Search(<any>$("#listSearchDiv")[0], corpusAdvancedSearchCount, corpusBasicSearchCount);
    search.makeSearch();

    var typeaheadSearchBox = new SearchBox(".searchbar-input", "Editions/Editions");
    typeaheadSearchBox.addDataSet("Title", "Název");
    typeaheadSearchBox.create();
    typeaheadSearchBox.value($(".searchbar-input.tt-input").val());


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

    var editionsSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Editions/Editions/GetEditionsWithCategories", true, callbackDelegate);
    editionsSelector.makeDropdown();


    $(".searchbar-input.tt-input").change(() => {        //prevent clearing input value on blur() 
        typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
    });

});