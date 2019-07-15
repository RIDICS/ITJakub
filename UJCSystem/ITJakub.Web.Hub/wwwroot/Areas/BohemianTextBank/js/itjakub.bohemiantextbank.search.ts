function initSearch() {
    const searchClass = new BohemianTextbankSearch();
    searchClass.initSearch();
}

class BohemianTextbankSearch {
    private actualPage: number;

    private resultsCountOnPage = 30;
    private paginationMaxVisibleElements = 5;

    private localization: Localization;
    private defaultErrorMessage;

    private urlSearchKey = "search";
    private urlPageKey = "page";
    private urlSelectionKey = "selected";
    private urlSortAscKey = "sortAsc";
    private urlSortCriteriaKey = "sortCriteria";
    private urlContextSizeKey = "contextSize";

    private readyForInit = false;
    private notInitialized = true;

    private bookIdsInQuery = new Array();
    private categoryIdsInQuery = new Array();

    private booksSelector: DropDownSelect2;
    private sortBar: SortBar;
    private paginator: Pagination;

    private initPage: number = null;

    private enabledOptions = new Array<SearchTypeEnum>();

    private search: Search;

    constructor() {
        this.localization = localization;
        this.defaultErrorMessage = this.localization.translate("SearchDefaultError", "BohemianTextBank").value;
    }

    initSearch() {

        $("#wordCheckbox").change(() => {
            var checkbox = $("#wordCheckbox");
            var mainDiv = $("#corpus-search-div");

            if (checkbox.is(":checked")) {
                mainDiv.addClass("show-word");
            } else {
                mainDiv.removeClass("show-word");
            }
        });

        $("#commentCheckbox").change(() => {
            var checkbox = $("#commentCheckbox");
            var mainDiv = $("#corpus-search-div");

            if (checkbox.is(":checked")) {
                mainDiv.addClass("show-notes");
            } else {
                mainDiv.removeClass("show-notes");
            }
        });


        $("#languageCheckbox").change(() => {
            var checkbox = $("#languageCheckbox");
            var mainDiv = $("#corpus-search-div");

            if (checkbox.is(":checked")) {
                mainDiv.addClass("show-language");
            } else {
                mainDiv.removeClass("show-language");
            }
        });


        $("#structureCheckbox").change(() => {
            var checkbox = $("#structureCheckbox");
            var mainDiv = $("#corpus-search-div");

            if (checkbox.is(":checked")) {
                mainDiv.addClass("show-structure");
            } else {
                mainDiv.removeClass("show-structure");
            }
        });


        $("#paragraphCheckbox").change(() => {
            var checkbox = $("#paragraphCheckbox");
            var mainDiv = $("#corpus-search-div");

            if (checkbox.is(":checked")) {
                mainDiv.addClass("show-paragraph");
            } else {
                mainDiv.removeClass("show-paragraph");
            }
        });

        $("#resultsTableBody").click((event) => {
            var clickedRow = $(event.target as Node as Element).parents("tr");

            if (clickedRow.hasClass("notes")) {
                return;
            }

            $("#resultsTableBody").find("tr").removeClass("clicked");
            clickedRow.addClass("clicked");

            this.printDetailInfo(clickedRow);
        });

        $("#contextPositionsSelect").change(() => {
            this.searchForPageNumber(this.actualPage);
        });

        $("#corpus-search-results-table-div").scroll((event) => {
            $("#corpus-search-results-abbrev-table-div").scrollTop($(event.target as Node as Element).scrollTop());
        });

        $("#corpus-search-results-abbrev-table-div").scroll((event) => {
            $("#corpus-search-results-table-div").scrollTop($(event.target as Node as Element).scrollTop());
        });

        this.initializeFromUrlParams();

        const sortBarContainer = "#listResultsHeader";

        const sortBarContainerEl = $(sortBarContainer);
        sortBarContainerEl.empty();
        this.sortBar = new SortBar(this.sortOrderChanged);
        const sortBarHtml = this.sortBar.makeSortBar(sortBarContainer);
        sortBarContainerEl.append(sortBarHtml);

        this.enabledOptions.push(SearchTypeEnum.Title);
        this.enabledOptions.push(SearchTypeEnum.Author);
        this.enabledOptions.push(SearchTypeEnum.Editor);
        this.enabledOptions.push(SearchTypeEnum.Dating);
        this.enabledOptions.push(SearchTypeEnum.Fulltext);
       // this.enabledOptions.push(SearchTypeEnum.Heading);TODO restore once implemented
       // this.enabledOptions.push(SearchTypeEnum.Sentence);
        this.enabledOptions.push(SearchTypeEnum.Term);
       // this.enabledOptions.push(SearchTypeEnum.TokenDistance);

        const favoritesQueriesConfig: IModulInicializatorConfigurationSearchFavorites = {
            bookType: BookTypeEnum.TextBank,
            queryType: QueryTypeEnum.Search
        };
        this.search = new Search($("#listSearchDiv")[0] as Node as HTMLDivElement,
            this.corpusAdvancedSearchCount.bind(this),
            this.corpusBasicSearchCount.bind(this),
            favoritesQueriesConfig);
        this.search.limitFullTextSearchToOne();
        this.search.makeSearch(this.enabledOptions);

        const callbackDelegate = new DropDownSelectCallbackDelegate();
        callbackDelegate.selectedChangedCallback = (state: State) => {

        };
        callbackDelegate.dataLoadedCallback = () => {
            this.initializeFromUrlParams();
        };

        this.booksSelector = new DropDownSelect2("#dropdownSelectDiv",
            getBaseUrl() + "BohemianTextBank/BohemianTextBank/GetCorpusWithCategories",
            BookTypeEnum.TextBank,
            true,
            callbackDelegate);
        this.booksSelector.makeDropdown();
    }

    private initializeFromUrlParams() {
        if (this.readyForInit && this.notInitialized) {

            this.notInitialized = false;

            const page = getQueryStringParameterByName(this.urlPageKey);

            if (page) {
                this.initPage = parseInt(page);
            }

            const contextSize = getQueryStringParameterByName(this.urlContextSizeKey);
            if (contextSize) {
                $("#contextPositionsSelect").val(contextSize);
            }

            const sortedAsc = getQueryStringParameterByName(this.urlSortAscKey);
            const sortCriteria = getQueryStringParameterByName(this.urlSortCriteriaKey);

            if (sortedAsc && sortCriteria) {
                this.sortBar.setSortedAsc(sortedAsc === "true");
                this.sortBar.setSortCriteria(((sortCriteria) as any) as SortEnum);
            }

            const selected = getQueryStringParameterByName(this.urlSelectionKey);

            const searched = getQueryStringParameterByName(this.urlSearchKey);
            this.search.writeTextToTextField(searched);

            if (selected) {
                this.booksSelector.restoreFromSerializedState(selected);
            }

        } else if (!this.notInitialized) {
            this.search.processSearch();
        } else {
            this.readyForInit = true;
        }

    }

    private actualizeSelectedBooksAndCategoriesInQuery() {
        const selectedIds = this.booksSelector.getSelectedIds();
        this.bookIdsInQuery = selectedIds.selectedBookIds;
        this.categoryIdsInQuery = selectedIds.selectedCategoryIds;
    }

    private sortOrderChanged() {
        if (paginator) {
            paginator.goToPage(1);
        }
    }

    private showLoading() {
        $("#result-table").hide();
        $("#result-abbrev-table").hide();
        $("#corpus-search-results-table-div-loader").empty();
        $("#corpus-search-results-table-div-loader").show();
        $("#corpus-search-results-table-div-loader").addClass("loader");
    }


    private hideLoading() {
        $("#corpus-search-results-table-div-loader").removeClass("loader");
        $("#corpus-search-results-table-div-loader").hide();
        $("#result-abbrev-table").show();
        $("#result-table").show();
    }

    private printErrorMessage(message: string) {
        this.hideLoading();
        const corpusErrorDiv = $("#corpus-search-results-table-div-loader");
        corpusErrorDiv.empty();
        corpusErrorDiv.html(message);
        corpusErrorDiv.show();
    }

    private fillResultsIntoTable(results: Array<ICorpusSearchResult>) {
        var tableBody = $("#resultsTableBody");
        const abbrevTableBody = $("#resultsAbbrevTableBody");
        tableBody.empty();
        abbrevTableBody.empty();
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

            var tr = $("<tr></tr>");
            tr.addClass("search-result");
            tr.data("bookId", bookId);
            tr.data("author", result.author);
            tr.data("title", result.title);
            tr.data("dating", result.originDate);
            tr.data("pageId", pageId);
            tr.data("pageName", pageContext.name);
            tr.data("acronym", acronym);


            if (verseContext !== null && typeof verseContext !== "undefined") {
                tr.data("verseXmlId", verseContext.verseXmlId);
                tr.data("verseName", verseContext.verseName);
            }

            if (bibleVerseContext !== null && typeof bibleVerseContext !== "undefined") {
                tr.data("bibleBook", bibleVerseContext.bibleBook);
                tr.data("bibleChapter", bibleVerseContext.bibleChapter);
                tr.data("bibleVerse", bibleVerseContext.bibleVerse);
            }

            var tdBefore = document.createElement("td");
            tdBefore.innerHTML = contextStructure.before;

            var tdMatch = $("<td></td>");
            tdMatch.addClass("match");
            tdMatch.html(contextStructure.match);

            var tdAfter = document.createElement("td");
            tdAfter.innerHTML = contextStructure.after;

            tr.append(tdBefore);
            tr.append(tdMatch);
            tr.append(tdAfter);

            tableBody.append(tr);

            if (notes !== null && typeof notes !== "undefined") {

                var notesTr = $("<tr></tr>");
                notesTr.addClass("notes");

                var tdNotes = document.createElement("td");
                tdNotes.colSpan = 2;


                for (var j = 0; j < notes.length; j++) {
                    var noteSpan = document.createElement("span");
                    noteSpan.innerHTML = notes[j];
                    $(noteSpan).addClass("note");
                    tdNotes.appendChild(noteSpan);
                }


                notesTr.append(tdNotes);

                var beforeNotesTr = document.createElement("tr");
                $(beforeNotesTr).addClass("notes spacer");

                var afterNotesTr = document.createElement("tr");
                $(afterNotesTr).addClass("notes spacer");

                tableBody.append(beforeNotesTr);
                tableBody.append(notesTr);
                tableBody.append(afterNotesTr);

            }

            //fill left table with abbrev of corpus name
            var abbrevTr = document.createElement("tr");
            //$(abbrevTr).data("bookXmlId", bookXmlId);
            //$(abbrevTr).data("pageXmlId", pageXmlId);

            var abbrevTd = document.createElement("td");

            var abbrevHref = document.createElement("a");
            abbrevHref.href =
                getBaseUrl() +
                "Editions/Editions/Listing?bookId=" +
                bookId +
                "&searchText=" +
                this.search.getLastQuery() +
                "&page=" +
                pageId;
            abbrevHref.innerHTML = acronym;

            abbrevTd.appendChild(abbrevHref);

            abbrevTr.appendChild(abbrevTd);
            abbrevTableBody.append(abbrevTr);

            if (notes !== null && typeof notes !== "undefined") {

                var abbRevNotesTr = $("<tr></tr>");
                abbRevNotesTr.addClass("notes");

                var abbrevTdNotes = $("<td></td>");

                abbRevNotesTr.append(abbrevTdNotes);

                var beforeAbbrevNotesTr = $("<tr></tr>");
                beforeAbbrevNotesTr.addClass("notes spacer");

                var afterAbbrevNotesTr = $("<tr></tr>");
                afterAbbrevNotesTr.addClass("notes spacer");

                abbrevTableBody.append(beforeAbbrevNotesTr);
                abbrevTableBody.append(abbRevNotesTr);
                abbrevTableBody.append(afterAbbrevNotesTr);

            }
        }


        //scroll from left to center match column in table
        var firstChildTdWidth = $(tableBody).children("tr").first().children("td").first().width();
        var tableContainer = $(tableBody).parents("#corpus-search-results-table-div");
        var tableContainerWidth = tableContainer.width();
        var scrollOffset = firstChildTdWidth - tableContainerWidth / 2;
        tableContainer.scrollLeft(scrollOffset);
    }

    private corpusAdvancedSearchPaged(json: string, pageNumber: number, contextLength: number) {

        if (typeof json === "undefined" || json === null || json === "") return;
        const start = (pageNumber - 1) * this.resultsCountOnPage;
        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();

        this.showLoading();

        const payload: JQuery.PlainObject = {
            json: json,
            start: start,
            count: this.resultsCountOnPage,
            contextLength: contextLength,
            sortingEnum: sortingEnum,
            sortAsc: sortAsc,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery
        };
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusPaged",
            data: payload,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.hideLoading();
                this.fillResultsIntoTable(response["results"]);
                updateQueryStringParameter(this.urlSearchKey, json);
                updateQueryStringParameter(this.urlPageKey, pageNumber);
                updateQueryStringParameter(this.urlSortAscKey, this.sortBar.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.sortBar.getSortCriteria());
                updateQueryStringParameter(this.urlContextSizeKey, contextLength);
            },
            error: response => {
                this.printErrorMessage(this.defaultErrorMessage);
            }
        });
    }

    private corpusBasicSearchPaged(text: string, pageNumber: number, contextLength: number) {
        if (typeof text === "undefined" || text === null || text === "") return;
        const start = (pageNumber - 1) * this.resultsCountOnPage;
        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();

        this.showLoading();

        const payload: JQuery.PlainObject = {
            text: text,
            start: start,
            count: this.resultsCountOnPage,
            contextLength: contextLength,
            sortingEnum: sortingEnum,
            sortAsc: sortAsc,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery
        };

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/TextSearchFulltextPaged",
            data: payload,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.hideLoading();
                this.fillResultsIntoTable(response["results"]);
                updateQueryStringParameter(this.urlSearchKey, text);
                updateQueryStringParameter(this.urlPageKey, pageNumber);
                updateQueryStringParameter(this.urlSortAscKey, this.sortBar.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.sortBar.getSortCriteria());
                updateQueryStringParameter(this.urlContextSizeKey, contextLength);
            },
            error: response => {
                this.printErrorMessage(this.defaultErrorMessage);
            }
        });
    }

    private searchForPageNumber(pageNumber: number) {
        this.actualPage = pageNumber;
        const contextLength = parseInt($("#contextPositionsSelect").val() as string);
        if (this.search.isLastQueryJson()) {
            this.corpusAdvancedSearchPaged(this.search.getLastQuery(), pageNumber, contextLength);
        } else {
            this.corpusBasicSearchPaged(this.search.getLastQuery(), pageNumber, contextLength);
        }
    }

    private createPagination(resultsCount: number) {
        const paginatorContainer = document.getElementById("paginationContainer");
        this.paginator = new Pagination({
            container: paginatorContainer as HTMLDivElement,
            maxVisibleElements: this.paginationMaxVisibleElements,
            pageClickCallback: this.searchForPageNumber.bind(this),
            callPageClickCallbackOnInit: true
        });

        const pages = Math.ceil(resultsCount / this.resultsCountOnPage);
        if (this.initPage && this.initPage <= pages) {
            this.paginator.make(resultsCount, this.resultsCountOnPage, this.initPage);
        } else {
            this.paginator.make(resultsCount, this.resultsCountOnPage);
        }

        const totalResultsDiv = document.getElementById("totalResultCountDiv");
        totalResultsDiv.innerHTML = resultsCount.toString();
    }

    private corpusBasicSearchCount(text: string) {

        if (typeof text === "undefined" || text === null || text === "") return;
        this.actualizeSelectedBooksAndCategoriesInQuery();

        this.showLoading();

        const payload: JQuery.PlainObject = {
            text: text,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery
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
                this.createPagination(count);
                updateQueryStringParameter(this.urlSearchKey, text);
                updateQueryStringParameter(this.urlSelectionKey, this.booksSelector.getSerializedState());
                updateQueryStringParameter(this.urlSortAscKey, this.sortBar.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.sortBar.getSortCriteria());
            },
            error: response => {
                this.printErrorMessage(this.defaultErrorMessage);
            }
        });
    }

    private corpusAdvancedSearchCount(json: string) {

        if (typeof json === "undefined" || json === null || json === "") return;
        this.actualizeSelectedBooksAndCategoriesInQuery();
        this.showLoading();

        const payload: JQuery.PlainObject =
            { json: json, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery };
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusResultsCount",
            data: payload,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                var count = response["count"];
                this.createPagination(count);
                updateQueryStringParameter(this.urlSearchKey, json);
                updateQueryStringParameter(this.urlSelectionKey, this.booksSelector.getSerializedState());
                updateQueryStringParameter(this.urlSortAscKey, this.sortBar.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.sortBar.getSortCriteria());
            },
            error: response => {
                this.printErrorMessage(this.defaultErrorMessage);
            }
        });
    }

    private printDetailInfo(tableRowEl: JQuery) {
        const undefinedReplaceString = "<Nezadáno>";

        $("#detail-author").text(tableRowEl.data("author") ? tableRowEl.data("author") : undefinedReplaceString);
        $("#detail-title").text(tableRowEl.data("title") ? tableRowEl.data("title") : undefinedReplaceString);
        $("#detail-dating").text(tableRowEl.data("dating") ? tableRowEl.data("dating") : undefinedReplaceString);
        $("#detail-dating-century").text(undefinedReplaceString); //TODO ask where is this info stored
        $("#detail-abbrev").text(tableRowEl.data("acronym") ? tableRowEl.data("acronym") : undefinedReplaceString);

        //Edition note
        const editionNoteAnchor = $("#detail-edition-note-href");
        editionNoteAnchor.prop("href", `/EditionNote/EditionNote?bookId=${tableRowEl.data("bookId")}`);

        const folioHref = $("<a></a>");
        folioHref.prop("href",
            `${getBaseUrl()}Editions/Editions/Listing?bookId=${tableRowEl.data("bookId")}&searchText=${
            this.search.getLastQuery()}&page=${tableRowEl.data("pageId")}`);
        folioHref.text(tableRowEl.data("pageName") ? tableRowEl.data("pageName") : undefinedReplaceString);

        $("#detail-folio").empty().append(folioHref);

        $("#detail-vers").text(tableRowEl.data("verseName") ? tableRowEl.data("verseName") : undefinedReplaceString);
        $("#detail-bible-vers-book")
            .text(tableRowEl.data("bibleBook") ? tableRowEl.data("bibleBook") : undefinedReplaceString);
        $("#detail-bible-vers-chapter")
            .text(tableRowEl.data("bibleChapter") ? tableRowEl.data("bibleChapter") : undefinedReplaceString);
        $("#detail-bible-vers-vers")
            .text(tableRowEl.data("bibleVerse") ? tableRowEl.data("bibleVerse") : undefinedReplaceString);
    }
}