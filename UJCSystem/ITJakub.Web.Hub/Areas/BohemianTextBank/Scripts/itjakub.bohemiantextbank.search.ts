var search: Search;
var actualPage: number;

function initSearch() {

    var resultsCountOnPage = 30;
    var paginationMaxVisibleElements = 5;

    var defaultErrorMessage = "Vyhledávání se nezdařilo. Ujistěte se, zda máte zadáno alespoň jedno kritérium na vyhledávání v textu.";

    const urlSearchKey = "search";
    const urlPageKey = "page";
    const urlSelectionKey = "selected";
    const urlSortAscKey = "sortAsc";
    const urlSortCriteriaKey = "sortCriteria";
    const urlContextSizeKey = "contextSize";

    var readyForInit = false;
    var notInitialized = true;

    var bookIdsInQuery = new Array();
    var categoryIdsInQuery = new Array();

    var selectedBookIds = new Array();
    var selectedCategoryIds = new Array();
    
    var booksSelector: DropDownSelect2;
    var sortBar: SortBar;
    var paginator: Pagination;

    var initPage: number = null;

    $("#commentCheckbox").change(() => {
        var checkbox = $("#commentCheckbox");
        var mainDiv = $("#corpus-search-div");

        if ($(checkbox).is(":checked")) {
            $(mainDiv).addClass("show-notes");
        } else {
            $(mainDiv).removeClass("show-notes");
        }
    });

    function initializeFromUrlParams() {
        if (readyForInit && notInitialized) {

            notInitialized = false;

            var page = getQueryStringParameterByName(urlPageKey);

            if (page) {
                initPage = parseInt(page);
            }

            var contextSize = getQueryStringParameterByName(urlContextSizeKey);
            if (contextSize) {
                $("#contextPositionsSelect").val(contextSize);
            }

            var sortedAsc = getQueryStringParameterByName(urlSortAscKey);
            var sortCriteria = getQueryStringParameterByName(urlSortCriteriaKey);

            if (sortedAsc && sortCriteria) {
                sortBar.setSortedAsc(sortedAsc === "true");
                sortBar.setSortCriteria(<SortEnum>(<any>(sortCriteria)));
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

    function sortOrderChanged() {
        if (paginator) {
            paginator.goToPage(1);    
        }
    }

    var sortBarContainer = "#listResultsHeader";

    $(sortBarContainer).empty();
    sortBar = new SortBar(sortOrderChanged);
    var sortBarHtml = sortBar.makeSortBar(sortBarContainer);
    $(sortBarContainer).append(sortBarHtml);

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

    function printErrorMessage(message: string) {
        hideLoading();
        var corpusErrorDiv = $("#corpus-search-results-table-div-loader");
        $(corpusErrorDiv).empty();
        $(corpusErrorDiv).html(message);
        $(corpusErrorDiv).show();
    }

    function fillResultsIntoTable(results: Array<any>) {
        var tableBody = document.getElementById("resultsTableBody");
        var abbrevTableBody = document.getElementById("resultsAbbrevTableBody");
        $(tableBody).empty();
        $(abbrevTableBody).empty();
        for (var i = 0; i < results.length; i++) {
            var result = results[i];
            var pageContext = result["PageResultContext"];
            var verseContext = result["VerseResultContext"];
            var bibleVerseContext = result["BibleVerseResultContext"];
            var contextStructure = pageContext["ContextStructure"];
            var bookXmlId = result["BookXmlId"];
            var pageXmlId = pageContext["PageXmlId"];
            var acronym = result["Acronym"];
            var notes = result["Notes"];

            var tr = document.createElement("tr");
            $(tr).addClass("search-result");
            $(tr).data("bookXmlId", bookXmlId);
            $(tr).data("versionXmlId", result["VersionXmlId"]);
            $(tr).data("author", result["Author"]);
            $(tr).data("title", result["Title"]);
            $(tr).data("dating", result["OriginDate"]);
            $(tr).data("pageXmlId", pageXmlId);
            $(tr).data("pageName", pageContext["PageName"]);
            $(tr).data("acronym", acronym);


            if (verseContext !== null && typeof verseContext !== "undefined") {
                $(tr).data("verseXmlId", verseContext["VerseXmlId"]);
                $(tr).data("verseName", verseContext["VerseName"]);
            }

            if (bibleVerseContext !== null && typeof bibleVerseContext !== "undefined") {
                $(tr).data("bibleBook", bibleVerseContext["BibleBook"]);
                $(tr).data("bibleChapter", bibleVerseContext["BibleChapter"]);
                $(tr).data("bibleVerse", bibleVerseContext["BibleVerse"]);
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

            if (notes !== null && typeof notes !== "undefined") {

                var notesTr = document.createElement("tr");
                $(notesTr).addClass("notes");

                var tdNotes = document.createElement("td");
                tdNotes.colSpan = 3;


                for (var j = 0; j < notes.length; j++) {
                    var noteSpan = document.createElement("span");
                    noteSpan.innerHTML = notes[j];
                    $(noteSpan).addClass("note");
                    tdNotes.appendChild(noteSpan);
                }


                notesTr.appendChild(tdNotes);

                var beforeNotesTr = document.createElement("tr");
                $(beforeNotesTr).addClass("notes spacer");

                var afterNotesTr = document.createElement("tr");
                $(afterNotesTr).addClass("notes spacer");

                tableBody.appendChild(beforeNotesTr);
                tableBody.appendChild(notesTr);
                tableBody.appendChild(afterNotesTr);

            }

            //fill left table with abbrev of corpus name
            var abbrevTr = document.createElement("tr");
            //$(abbrevTr).data("bookXmlId", bookXmlId);
            //$(abbrevTr).data("pageXmlId", pageXmlId);

            var abbrevTd = document.createElement("td");

            var abbrevHref = document.createElement("a");
            abbrevHref.href = getBaseUrl() + "Editions/Editions/Listing?bookId=" + bookXmlId + "&searchText=" + search.getLastQuery() + "&page=" + pageXmlId;
            abbrevHref.innerHTML = acronym;

            abbrevTd.appendChild(abbrevHref);

            abbrevTr.appendChild(abbrevTd);
            abbrevTableBody.appendChild(abbrevTr);

            if (notes !== null && typeof notes !== "undefined") {

                var abbRevNotesTr = document.createElement("tr");
                $(abbRevNotesTr).addClass("notes");

                var abbrevTdNotes = document.createElement("td");

                abbRevNotesTr.appendChild(abbrevTdNotes);

                var beforeAbbrevNotesTr = document.createElement("tr");
                $(beforeAbbrevNotesTr).addClass("notes spacer");

                var afterAbbrevNotesTr = document.createElement("tr");
                $(afterAbbrevNotesTr).addClass("notes spacer");

                abbrevTableBody.appendChild(beforeAbbrevNotesTr);
                abbrevTableBody.appendChild(abbRevNotesTr);
                abbrevTableBody.appendChild(afterAbbrevNotesTr);

            }
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
        const start = (pageNumber - 1) * resultsCountOnPage;
        var sortingEnum = sortBar.getSortCriteria();
        var sortAsc = sortBar.isSortedAsc();

        showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusPaged",
            data: { json: json, start: start, count: resultsCountOnPage, contextLength: contextLength, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                hideLoading();
                fillResultsIntoTable(response["results"]);
                updateQueryStringParameter(urlSearchKey, json);
                updateQueryStringParameter(urlPageKey, pageNumber);
                updateQueryStringParameter(urlSortAscKey, sortBar.isSortedAsc());
                updateQueryStringParameter(urlSortCriteriaKey, sortBar.getSortCriteria());
                updateQueryStringParameter(urlContextSizeKey, contextLength);
            },error: response => {
                printErrorMessage(defaultErrorMessage);
            }
        });
    }

    function corpusBasicSearchPaged(text: string, pageNumber: number, contextLength: number) {

        if (typeof text === "undefined" || text === null || text === "") return;
        const start = (pageNumber - 1) * resultsCountOnPage;
        var sortingEnum = sortBar.getSortCriteria();
        var sortAsc = sortBar.isSortedAsc();

        showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchFulltextPaged",
            data: { text: text, start: start, count: resultsCountOnPage, contextLength: contextLength, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                hideLoading();
                fillResultsIntoTable(response["results"]);
                updateQueryStringParameter(urlSearchKey, text);
                updateQueryStringParameter(urlPageKey, pageNumber);
                updateQueryStringParameter(urlSortAscKey, sortBar.isSortedAsc());
                updateQueryStringParameter(urlSortCriteriaKey, sortBar.getSortCriteria());
                updateQueryStringParameter(urlContextSizeKey, contextLength);
            }, error: response => {
                printErrorMessage(defaultErrorMessage);
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
        paginator = new Pagination(<any>paginatorContainer, paginationMaxVisibleElements);

        var pages = Math.ceil(resultsCount / resultsCountOnPage);

        if (initPage && initPage <= pages) {
            paginator.createPagination(resultsCount, resultsCountOnPage, searchForPageNumber, initPage);
        } else {
            paginator.createPagination(resultsCount, resultsCountOnPage, searchForPageNumber);
        }

        const totalResultsDiv = document.getElementById("totalResultCountDiv");
        totalResultsDiv.innerHTML = resultsCount.toString();
    }

    function corpusBasicSearchCount(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;
        actualizeSelectedBooksAndCategoriesInQuery();

        showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchFulltextCount",
            data: { text: text, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                var count = response["count"];
                createPagination(count);
                updateQueryStringParameter(urlSearchKey, text);
                updateQueryStringParameter(urlSelectionKey, DropDownSelect2.getUrlStringFromState(booksSelector.getState()));
                updateQueryStringParameter(urlSortAscKey, sortBar.isSortedAsc());
                updateQueryStringParameter(urlSortCriteriaKey, sortBar.getSortCriteria());
            }, error: response => {
                printErrorMessage(defaultErrorMessage);
            }
        });
    }

    function corpusAdvancedSearchCount(json: string) {

        if (typeof json === "undefined" || json === null || json === "") return;
        actualizeSelectedBooksAndCategoriesInQuery();
        showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusResultsCount",
            data: { json: json, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                var count = response["count"];
                createPagination(count);
                updateQueryStringParameter(urlSearchKey, json);
                updateQueryStringParameter(urlSelectionKey, DropDownSelect2.getUrlStringFromState(booksSelector.getState()));
                updateQueryStringParameter(urlSortAscKey, sortBar.isSortedAsc());
                updateQueryStringParameter(urlSortCriteriaKey, sortBar.getSortCriteria());
            }, error: response => {
                printErrorMessage(defaultErrorMessage);
            }
        });
    }

    const enabledOptions = new Array<SearchTypeEnum>();
    enabledOptions.push(SearchTypeEnum.Title);
    enabledOptions.push(SearchTypeEnum.Author);
    enabledOptions.push(SearchTypeEnum.Editor);
    enabledOptions.push(SearchTypeEnum.Dating);
    enabledOptions.push(SearchTypeEnum.Fulltext);
    enabledOptions.push(SearchTypeEnum.Heading);
    enabledOptions.push(SearchTypeEnum.Sentence);
    enabledOptions.push(SearchTypeEnum.Term);
    enabledOptions.push(SearchTypeEnum.TokenDistance);

    search = new Search(<any>$("#listSearchDiv")[0], corpusAdvancedSearchCount, corpusBasicSearchCount);
    search.makeSearch(enabledOptions);
    
    const callbackDelegate = new DropDownSelectCallbackDelegate();
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

    booksSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "BohemianTextBank/BohemianTextBank/GetCorpusWithCategories", true, callbackDelegate);
    booksSelector.makeDropdown();

    function printDetailInfo(tableRow: HTMLElement) {
        var undefinedReplaceString = "&lt;Nezadáno&gt;";

        document.getElementById("detail-author").innerHTML = typeof $(tableRow).data("author") !== "undefined" && $(tableRow).data("author") !== null ? $(tableRow).data("author") : undefinedReplaceString;
        document.getElementById("detail-title").innerHTML = typeof $(tableRow).data("title") !== "undefined" && $(tableRow).data("title") !== null ? $(tableRow).data("title") : undefinedReplaceString;
        document.getElementById("detail-dating").innerHTML = typeof $(tableRow).data("dating") !== "undefined" && $(tableRow).data("dating") !== null ? $(tableRow).data("dating") : undefinedReplaceString;
        document.getElementById("detail-dating-century").innerHTML = undefinedReplaceString; //TODO ask where is this info stored
        document.getElementById("detail-abbrev").innerHTML = typeof $(tableRow).data("acronym") !== "undefined" && $(tableRow).data("acronym") !== null ? $(tableRow).data("acronym") : undefinedReplaceString;

        var folioHref = document.createElement("a");
        folioHref.href = getBaseUrl() + "Editions/Editions/Listing?bookId=" + $(tableRow).data("bookXmlId") + "&searchText=" + search.getLastQuery() + "&page=" + $(tableRow).data("pageXmlId");
        folioHref.innerHTML = typeof $(tableRow).data("pageName") !== "undefined" && $(tableRow).data("pageName") !== null ? $(tableRow).data("pageName") : undefinedReplaceString;

        $("#detail-folio").empty();
        $("#detail-folio").append(folioHref);


        document.getElementById("detail-vers").innerHTML = typeof $(tableRow).data("verseName") !== "undefined" && $(tableRow).data("verseName") !== null ? $(tableRow).data("verseName") : undefinedReplaceString;

        document.getElementById("detail-bible-vers-book").innerHTML = typeof $(tableRow).data("bibleBook") !== "undefined" && $(tableRow).data("bibleBook") !== null ? $(tableRow).data("bibleBook") : undefinedReplaceString;
        document.getElementById("detail-bible-vers-chapter").innerHTML = typeof $(tableRow).data("bibleChapter") !== "undefined" && $(tableRow).data("bibleChapter") !== null ? $(tableRow).data("bibleChapter") : undefinedReplaceString;
        document.getElementById("detail-bible-vers-vers").innerHTML = typeof $(tableRow).data("bibleVerse") !== "undefined" && $(tableRow).data("bibleVerse") !== null ? $(tableRow).data("bibleVerse") : undefinedReplaceString;
    }

    $("#resultsTableBody").click((event: Event) => {
        var clickedRow = $(event.target).parents("tr");

        if ($(clickedRow).hasClass("notes")) {
            return;
        }

        $("#resultsTableBody").find("tr").removeClass("clicked");
        $(clickedRow).addClass("clicked");

        printDetailInfo(clickedRow[0]);
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

    initializeFromUrlParams();
}