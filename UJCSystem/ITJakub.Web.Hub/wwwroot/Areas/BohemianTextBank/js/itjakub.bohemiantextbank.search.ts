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
    
    var booksSelector: DropDownSelect2;
    var sortBar: SortBar;
    var paginator: Pagination;

    var initPage: number = null;

    $("#wordCheckbox").change(() => {
        var checkbox = $("#wordCheckbox");
        var mainDiv = $("#corpus-search-div");

        if ($(checkbox).is(":checked")) {
            $(mainDiv).addClass("show-word");
        } else {
            $(mainDiv).removeClass("show-word");
        }
    });

    $("#commentCheckbox").change(() => {
        var checkbox = $("#commentCheckbox");
        var mainDiv = $("#corpus-search-div");

        if ($(checkbox).is(":checked")) {
            $(mainDiv).addClass("show-notes");
        } else {
            $(mainDiv).removeClass("show-notes");
        }
    });


    $("#languageCheckbox").change(() => {
        var checkbox = $("#languageCheckbox");
        var mainDiv = $("#corpus-search-div");

        if ($(checkbox).is(":checked")) {
            $(mainDiv).addClass("show-language");
        } else {
            $(mainDiv).removeClass("show-language");
        }
    });


    $("#structureCheckbox").change(() => {
        var checkbox = $("#structureCheckbox");
        var mainDiv = $("#corpus-search-div");

        if ($(checkbox).is(":checked")) {
            $(mainDiv).addClass("show-structure");
        } else {
            $(mainDiv).removeClass("show-structure");
        }
    });


    $("#paragraphCheckbox").change(() => {
        var checkbox = $("#paragraphCheckbox");
        var mainDiv = $("#corpus-search-div");

        if ($(checkbox).is(":checked")) {
            $(mainDiv).addClass("show-paragraph");
        } else {
            $(mainDiv).removeClass("show-paragraph");
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
                booksSelector.restoreFromSerializedState(selected);
            }

        } else if (!notInitialized) {
            search.processSearch();
        } else {
            readyForInit = true;
        }

    }

    function actualizeSelectedBooksAndCategoriesInQuery() {
        var selectedIds = booksSelector.getSelectedIds();
        bookIdsInQuery = selectedIds.selectedBookIds;
        categoryIdsInQuery = selectedIds.selectedCategoryIds;
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

    function fillResultsIntoTable(results: Array<ICorpusSearchResult>) {
        var tableBody = document.getElementById("resultsTableBody");
        var abbrevTableBody = document.getElementById("resultsAbbrevTableBody");
        $(tableBody).empty();
        $(abbrevTableBody).empty();
        for (var i = 0; i < results.length; i++) {
            var result = results[i];
            var pageContext = result.pageResultContext;
            var verseContext = result.verseResultContext;
            var bibleVerseContext = result.bibleVerseResultContext;
            var contextStructure = pageContext.contextStructure;
            var bookId = result.bookId;
            var pageId = pageContext.id;
            var acronym = result.sourceAbbreviation;
            var notes = result.notes;

            var tr = document.createElement("tr");
            $(tr).addClass("search-result");
            $(tr).data("bookId", bookId);
            $(tr).data("author", result.author);
            $(tr).data("title", result.title);
            $(tr).data("dating", result.originDate);
            $(tr).data("pageId", pageId);
            $(tr).data("pageName", pageContext.name);
            $(tr).data("acronym", acronym);


            if (verseContext !== null && typeof verseContext !== "undefined") {
                $(tr).data("verseXmlId", verseContext.verseXmlId);
                $(tr).data("verseName", verseContext.verseName);
            }

            if (bibleVerseContext !== null && typeof bibleVerseContext !== "undefined") {
                $(tr).data("bibleBook", bibleVerseContext.bibleBook);
                $(tr).data("bibleChapter", bibleVerseContext.bibleChapter);
                $(tr).data("bibleVerse", bibleVerseContext.bibleVerse);
            }

            var tdBefore = document.createElement("td");
            tdBefore.innerHTML = contextStructure.before;

            var tdMatch = document.createElement("td");
            $(tdMatch).addClass("match");
            tdMatch.innerHTML = contextStructure.match;

            var tdAfter = document.createElement("td");
            tdAfter.innerHTML = contextStructure.after;

            tr.appendChild(tdBefore);
            tr.appendChild(tdMatch);
            tr.appendChild(tdAfter);

            tableBody.appendChild(tr);

            if (notes !== null && typeof notes !== "undefined") {

                var notesTr = document.createElement("tr");
                $(notesTr).addClass("notes");

                var tdNotes = document.createElement("td");
                tdNotes.colSpan = 2;


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
            abbrevHref.href = getBaseUrl() + "Editions/Editions/Listing?bookId=" + bookId + "&searchText=" + search.getLastQuery() + "&page=" + pageId;
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

        const payload: JQuery.PlainObject = {
            json: json,
            start: start,
            count: resultsCountOnPage,
            contextLength: contextLength,
            sortingEnum: sortingEnum,
            sortAsc: sortAsc,
            selectedBookIds: bookIdsInQuery,
            selectedCategoryIds: categoryIdsInQuery
        };
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusPaged",
            data: payload,
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

        const payload: JQuery.PlainObject = {
            text: text,
            start: start,
            count: resultsCountOnPage,
            contextLength: contextLength,
            sortingEnum: sortingEnum,
            sortAsc: sortAsc,
            selectedBookIds: bookIdsInQuery,
            selectedCategoryIds: categoryIdsInQuery
        };
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchFulltextPaged",
            data: payload,
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
        var contextLength = parseInt($("#contextPositionsSelect").val() as string);
        if (search.isLastQueryJson()) {
            corpusAdvancedSearchPaged(search.getLastQuery(), pageNumber, contextLength);
        } else {
            corpusBasicSearchPaged(search.getLastQuery(), pageNumber, contextLength);
        }
    }

    function createPagination(resultsCount: number) {
        var paginatorContainer = document.getElementById("paginationContainer");
        paginator = new Pagination({
            container: <HTMLDivElement>paginatorContainer,
            maxVisibleElements: paginationMaxVisibleElements,
            pageClickCallback: searchForPageNumber,
            callPageClickCallbackOnInit: true
        });

        var pages = Math.ceil(resultsCount / resultsCountOnPage);

        if (initPage && initPage <= pages) {
            paginator.make(resultsCount, resultsCountOnPage, initPage);
        } else {
            paginator.make(resultsCount, resultsCountOnPage);
        }

        const totalResultsDiv = document.getElementById("totalResultCountDiv");
        totalResultsDiv.innerHTML = resultsCount.toString();
    }

    function corpusBasicSearchCount(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;
        actualizeSelectedBooksAndCategoriesInQuery();

        showLoading();

        const payload: JQuery.PlainObject = {
            text: text,
            selectedBookIds: bookIdsInQuery,
            selectedCategoryIds: categoryIdsInQuery
        };
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchFulltextCount",
            data: payload,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                var count = response["count"];
                createPagination(count);
                updateQueryStringParameter(urlSearchKey, text);
                updateQueryStringParameter(urlSelectionKey, booksSelector.getSerializedState());
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

        const payload: JQuery.PlainObject =
            { json: json, selectedBookIds: bookIdsInQuery, selectedCategoryIds: categoryIdsInQuery };
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusResultsCount",
            data: payload,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                var count = response["count"];
                createPagination(count);
                updateQueryStringParameter(urlSearchKey, json);
                updateQueryStringParameter(urlSelectionKey, booksSelector.getSerializedState());
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

    var favoritesQueriesConfig: IModulInicializatorConfigurationSearchFavorites = {
        bookType: BookTypeEnum.TextBank,
        queryType: QueryTypeEnum.Search
    }
    var search = new Search($("#listSearchDiv")[0] as Node as HTMLDivElement, corpusAdvancedSearchCount, corpusBasicSearchCount, favoritesQueriesConfig);
    search.limitFullTextSearchToOne();
    search.makeSearch(enabledOptions);
    
    const callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = (state: State) => {
        
    };
    callbackDelegate.dataLoadedCallback = () => {
        initializeFromUrlParams();
    };

    booksSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "BohemianTextBank/BohemianTextBank/GetCorpusWithCategories", BookTypeEnum.TextBank, true, callbackDelegate);
    booksSelector.makeDropdown();

    function printDetailInfo(tableRow: HTMLElement) {
        const undefinedReplaceString = "<Nezadáno>";

        const tableRowEl = $(tableRow);

        $("#detail-author").text(tableRowEl.data("author") ? tableRowEl.data("author") : undefinedReplaceString);
        $("#detail-title").text(tableRowEl.data("title") ? tableRowEl.data("title") : undefinedReplaceString);
        $("#detail-dating").text(tableRowEl.data("dating") ? tableRowEl.data("dating") : undefinedReplaceString);
        $("#detail-dating-century").text(undefinedReplaceString); //TODO ask where is this info stored
        $("#detail-abbrev").text(tableRowEl.data("acronym") ? tableRowEl.data("acronym") : undefinedReplaceString);

        //Edition note
        var editionNoteAnchor = $("#detail-edition-note-href");
        editionNoteAnchor.prop("href", `/EditionNote/EditionNote?bookId=${tableRowEl.data("bookId")}`);

        var folioHref = $("<a></a>");
        folioHref.prop("href", `${getBaseUrl()}Editions/Editions/Listing?bookId=${tableRowEl.data("bookId")}&searchText=${search.getLastQuery()}&page=${tableRowEl.data("pageId")}`);
        folioHref.text(tableRowEl.data("pageName") ? tableRowEl.data("pageName") : undefinedReplaceString);

        $("#detail-folio").empty().append(folioHref);

        $("#detail-vers").text(tableRowEl.data("verseName") ? tableRowEl.data("verseName") : undefinedReplaceString);
        $("#detail-bible-vers-book").text(tableRowEl.data("bibleBook") ? tableRowEl.data("bibleBook") : undefinedReplaceString);
        $("#detail-bible-vers-chapter").text(tableRowEl.data("bibleChapter") ? tableRowEl.data("bibleChapter") : undefinedReplaceString);
        $("#detail-bible-vers-vers").text(tableRowEl.data("bibleVerse") ? tableRowEl.data("bibleVerse") : undefinedReplaceString);
    }

    $("#resultsTableBody").click((event) => {
        var clickedRow = $(event.target as Node as Element).parents("tr");

        if ($(clickedRow).hasClass("notes")) {
            return;
        }

        $("#resultsTableBody").find("tr").removeClass("clicked");
        $(clickedRow).addClass("clicked");

        printDetailInfo(clickedRow[0] as Node as HTMLElement);
    });

    $("#contextPositionsSelect").change(() => {
        searchForPageNumber(actualPage);
    });

    $("#corpus-search-results-table-div").scroll((event) => {
        $("#corpus-search-results-abbrev-table-div").scrollTop($(event.target as Node as Element).scrollTop());
    });

    $("#corpus-search-results-abbrev-table-div").scroll((event) => {
        $("#corpus-search-results-table-div").scrollTop($(event.target as Node as Element).scrollTop());
    });

    initializeFromUrlParams();
}