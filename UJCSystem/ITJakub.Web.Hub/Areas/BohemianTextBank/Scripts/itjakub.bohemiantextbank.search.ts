var search: Search;
var actualPage: number;

function initSearch() {

    var resultsCountOnPage = 30;
    var paginationMaxVisibleElements = 5;

    var bookIds = new Array();
    var categoryIds = new Array();

    function sortOrderChanged() {
        search.processSearch();
    }

    function showLoading() {
        $("#result-table").hide();
        $("#result-abbrev-table").hide();
        $("#corpus-search-results-table-div-loader").empty();
        $("#corpus-search-results-table-div-loader").show();
        $("#corpus-search-results-table-div-loader").addClass("loader");
    }

    function hideLoading() {
        $("#corpus-search-results-table-div-loader").removeClass("loader");
        $("#corpus-search-results-table-div-loader").hide();
        $("#result-abbrev-table").show();
        $("#result-table").show();
    }

    function fillResultsIntoTable(results: Array<any>) {
        var tableBody = document.getElementById("resultsTableBody");
        var abbrevTableBody = document.getElementById("resultsAbbrevTableBody");
        $(tableBody).empty();
        for (var i = 0; i < results.length; i++) {
            var result = results[i];
            var pageContext = result["PageResultContext"];
            var verseContext = result["VerseResultContext"];
            var contextStructure = pageContext["ContextStructure"];
            var bookXmlId = result["BookXmlId"];
            var pageXmlId = pageContext["PageXmlId"];

            var tr = document.createElement("tr");
            $(tr).data("bookXmlId", bookXmlId);
            $(tr).data("versionXmlId", result["VersionXmlId"]);
            $(tr).data("author", result["Author"]);
            $(tr).data("title", result["Title"]);
            $(tr).data("dating", result["OriginDate"]);
            $(tr).data("pageXmlId", pageXmlId );
            $(tr).data("pageName", pageContext["PageName"]);


            if (verseContext !== null && typeof verseContext !== "undefined") {
                $(tr).data("verseXmlId", verseContext["VerseXmlId"]);
                $(tr).data("verseName", verseContext["VerseName"]);   
            }

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

            //fill left table with abbrev of corpus name
            var abbrevTr = document.createElement("tr");
            //$(abbrevTr).data("bookXmlId", bookXmlId);
            //$(abbrevTr).data("pageXmlId", pageXmlId);

            var abbrevTd = document.createElement("td");

            var abbrevHref = document.createElement("a");
            abbrevHref.href = getBaseUrl() + "Editions/Editions/Listing?bookId=" + bookXmlId + "&searchText=" + search.getLastQuery() + "&page=" + pageXmlId;
            abbrevHref.innerHTML = result["Acronym"];

            abbrevTd.appendChild(abbrevHref);

            abbrevTr.appendChild(abbrevTd);
            abbrevTableBody.appendChild(abbrevTr);
        }
        

        //scroll from left to center match column in table
        var firstChildTdWidth = $(tableBody).children("tr").first().children("td").first().width();
        var tableContainer = $(tableBody).parents("#corpus-search-results-table-div");
        var tableContainerWidth = $(tableContainer).width();
        var scrollOffset = firstChildTdWidth - tableContainerWidth / 2;
        $(tableContainer).scrollLeft(scrollOffset);
    }
    
    function corpusAdvancedSearchPaged(json: string, pageNumber: number, contextLength: number) {

        if (typeof json === "undefined" || json === null || json === "") return;
        
        var start = (pageNumber - 1) * resultsCountOnPage;
        var sortingEnum = SortEnum.Title; //TODO
        var sortAsc = true; //TODO

        showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusPaged",
            data: { json: json, start: start, count: resultsCountOnPage, contextLength: contextLength, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                hideLoading();
                fillResultsIntoTable(response["results"]);
            }
        });
    }

    function corpusBasicSearchPaged(text: string, pageNumber: number, contextLength: number) {

        if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * resultsCountOnPage;
        var sortingEnum = SortEnum.Title; //TODO
        var sortAsc = true; //TODO

        showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchFulltextPaged",
            data: { text: text, start: start, count: resultsCountOnPage, contextLength: contextLength, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                hideLoading();
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

        showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchFulltextCount",
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

        showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusResultsCount",
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

    var editionsSelector: DropDownSelect2;
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
    };
    callbackDelegate.dataLoadedCallback = () => {
        var selectedIds = editionsSelector.getSelectedIds();
        bookIds = selectedIds.selectedBookIds;
        categoryIds = selectedIds.selectedCategoryIds;
    };

    editionsSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "BohemianTextBank/BohemianTextBank/GetCorpusWithCategories", true, callbackDelegate);
    editionsSelector.makeDropdown();

    function printDetailInfo(tableRow: HTMLElement) {
        var undefinedReplaceString = "&lt;Nezadáno&gt;";

        document.getElementById("detail-author").innerHTML = typeof $(tableRow).data("author") !== "undefined" && $(tableRow).data("author") !== null? $(tableRow).data("author") : undefinedReplaceString;
        document.getElementById("detail-title").innerHTML = typeof $(tableRow).data("title") !== "undefined" && $(tableRow).data("title") !== null? $(tableRow).data("title") : undefinedReplaceString;
        document.getElementById("detail-dating").innerHTML = typeof $(tableRow).data("dating") !== "undefined" && $(tableRow).data("dating") !== null ? $(tableRow).data("dating") : undefinedReplaceString;;
        document.getElementById("detail-dating-century").innerHTML = undefinedReplaceString; //TODO ask where is this info stored
        document.getElementById("detail-abbrev").innerHTML = undefinedReplaceString; //TODO ask where is this info stored

        var folioHref = document.createElement("a");
        folioHref.href = getBaseUrl() + "Editions/Editions/Listing?bookId=" + $(tableRow).data("bookXmlId") + "&searchText=" + search.getLastQuery() + "&page=" + $(tableRow).data("pageXmlId");
        folioHref.innerHTML = typeof $(tableRow).data("pageName") !== "undefined" && $(tableRow).data("pageName") !== null ? $(tableRow).data("pageName") : undefinedReplaceString;

        $("#detail-folio").empty();
        $("#detail-folio").append(folioHref);


        document.getElementById("detail-vers").innerHTML = typeof $(tableRow).data("verseName") !== "undefined" && $(tableRow).data("verseName") !== null ? $(tableRow).data("verseName") : undefinedReplaceString;
    }

    $("#resultsTableBody").click((event: Event) => {
        $("#resultsTableBody").find("tr").removeClass("clicked");
        var row = $(event.target).parents("tr");
        $(row).addClass("clicked");

        printDetailInfo(row[0]);
    });

    $("#contextPositionsSelect").change((evnet: Event) => {
        searchForPageNumber(actualPage);
    });

    $("#corpus-search-results-table-div").scroll((event: Event) => {
        $("#corpus-search-results-abbrev-table-div").scrollTop($(event.target).scrollTop());
    });

    $("#corpus-search-results-abbrev-table-div").scroll((event: Event) => {
        $("#corpus-search-results-table-div").scrollTop($(event.target).scrollTop());
    });
}