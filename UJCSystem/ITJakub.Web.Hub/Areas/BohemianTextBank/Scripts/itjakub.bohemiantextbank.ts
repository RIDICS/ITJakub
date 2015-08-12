var search: Search;
var actualPage: number;

$(document).ready(() => {
    var resultsCountOnPage = 30;
    var paginationMaxVisibleElements = 5;

    var bookIds = new Array();
    var categoryIds = new Array();

    function sortOrderChanged() {
        search.processSearch();
    }

    function fillResultsIntoTable(results: Array<any>) {
        var tableBody = document.getElementById("resultsTableBody");
        $(tableBody).empty();
        for (var i = 0; i < results.length; i++) {
            var result = results[i];
            var pageContext = result["PageResultContext"];
            var contextStructure = pageContext["ContextStructure"];

            var tr = document.createElement("tr");

            var tdBefore = document.createElement("td");
            tdBefore.innerHTML = contextStructure["Before"];

            var tdMatch = document.createElement("td");
            $(tdMatch).addClass("match");
            tdMatch.innerHTML = contextStructure["Match"];

            var tdAfter = document.createElement("td");
            tdAfter.innerHTML = contextStructure["After"];

            tr.appendChild(tdBefore);
            tr.appendChild(tdMatch);
            tr.appendChild(tdAfter);

            tableBody.appendChild(tr);
        }
    }
    
    function corpusAdvancedSearchPaged(json: string, pageNumber: number, contextLength: number) {

        if (typeof json === "undefined" || json === null || json === "") return;
        
        var start = (pageNumber - 1) * resultsCountOnPage;
        var sortingEnum = SortEnum.Title; //TODO
        var sortAsc = true; //TODO

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchPaged",
            data: { json: json, start: start, count: resultsCountOnPage, contextLength: contextLength, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                fillResultsIntoTable(response["results"]);
            }
        });
    }

    function corpusBasicSearchPaged(text: string, pageNumber: number, contextLength: number) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * resultsCountOnPage;
        var sortingEnum = SortEnum.Title; //TODO
        var sortAsc = true; //TODO

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchPaged",
            data: { text: text, start: start, count: resultsCountOnPage, contextLength: contextLength, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                fillResultsIntoTable(response["results"]);
            }
        });
    }

    function searchForPageNumber(pageNumber: number) {
        actualPage = pageNumber;
        var contextLength = $("#contextPositionsSelect").val();

        if (search.isLastQueryJson()) {
            corpusAdvancedSearchPaged(search.getLastQuery(), pageNumber, contextLength);
        } else {
            corpusBasicSearchPaged(search.getLastQuery(), pageNumber, contextLength);
        }
    }

    function createPagination(resultsCount: number) {
        var paginatorContainer = document.getElementById("paginationContainer");
        var paginator = new Pagination(<any>paginatorContainer, paginationMaxVisibleElements);
        paginator.createPagination(resultsCount, resultsCountOnPage, searchForPageNumber);

        var totalResultsDiv = document.getElementById("totalResultCountDiv");
        totalResultsDiv.innerHTML = resultsCount.toString();
    }

    function corpusBasicSearchCount(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchCount",
            data: { text: text, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                var count = response["count"];
                createPagination(count);
            }
        });
    }

    function corpusAdvancedSearchCount(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                var count = response["count"];
                createPagination(count);
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

    $("#resultsTableBody").click((event: Event) => {
        $("#resultsTableBody").find("tr").removeClass("clicked");
        var row = $(event.target).parents("tr");
        $(row).addClass("clicked");
    });

    $("#contextPositionsSelect").change((evnet: Event) => {
        searchForPageNumber(actualPage);
    });
});