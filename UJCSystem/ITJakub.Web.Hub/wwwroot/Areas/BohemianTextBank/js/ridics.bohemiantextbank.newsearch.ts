function initSearchNew() {
    const searchClass = new BohemianTextBankNew();
    searchClass.initSearch();
}

class BohemianTextBankNew {
    private searchResultsOnPage = 10;//corresponds to amount of results per page that should be on screen
    private contextLength = 50;

    private minContextLength = 30;
    private maxContextLength = 100;
    private minResultsPerPage = 1;
    private maxResultsPerPage = 50;

    private hitBookIds = [];
    private transientResults: ICorpusSearchResult[] = [];

    private paginator: IndefinitePagination;

    private compositionResultListStart = -1;
    private compositionsPerPage = 10;
    private compositionPageIsLast = false;

    private currentBookId = -1;
    private currentBookIndex = 0;
    private currentResultStart = -1;
    private currentViewPage = 1;
    private totalViewPages = 0;

    private defaultErrorMessage =
        "Vyhledávání se nezdařilo. Ujistěte se, zda máte zadáno alespoň jedno kritérium na vyhledávání v textu.";

    private urlSearchKey = "search";
    private urlStartKey = "start"; //TODO in process of changing to start
    private urlSelectionKey = "selected";
    private urlSortAscKey = "sortAsc";
    private urlSortCriteriaKey = "sortCriteria";
    private urlContextSizeKey = "contextSize";
    private urlResultPerPageKey = "resultsPerPage";

    private readyForInit = false;
    private notInitialized = true;

    private bookIdsInQuery = new Array();
    private categoryIdsInQuery = new Array();

    private booksSelector: DropDownSelect2;
    private sortBar: SortBar;

    private enabledOptions = new Array<SearchTypeEnum>();

    private search: Search;

    initSearch() {
        const paginationContainerEl = $("#paginationContainer");
        const paginator = new IndefinitePagination({
            container: paginationContainerEl,
            nextPageCallback: this.formNextPage.bind(this),
            previousPageCallback: this.loadPreviousPage.bind(this),
            loadAllPagesButton: true,
            loadAllPagesCallback: this.loadAllPages.bind(this),
            loadPageCallBack: this.goToPage.bind(this),
            pageDoesntExistCallBack: this.showNoPageWarning.bind(this),
            showSlider: true,
            showInput: true
        });
        this.paginator = paginator;
        paginator.make();
        paginator.disable();

        const contextLengthInputEl = $("#contextPositionsSelect");
        contextLengthInputEl.prop("max", this.maxContextLength);
        contextLengthInputEl.prop("min", this.minContextLength);
        contextLengthInputEl.val(this.contextLength);
        const resultsPerPageInputEl = $("#number-of-results-per-viewing-page");
        resultsPerPageInputEl.prop("max", this.maxResultsPerPage);
        resultsPerPageInputEl.prop("min", this.minResultsPerPage);
        resultsPerPageInputEl.val(this.searchResultsOnPage);

        contextLengthInputEl.on("change", () => {
            const contextLengthString = contextLengthInputEl.val() as string;
            const contextLengthNumber = parseInt(contextLengthString);
            if (!isNaN(contextLengthNumber)) {
                if (contextLengthNumber >= this.minContextLength && contextLengthNumber <= this.maxContextLength) {
                    this.contextLength = contextLengthNumber;
                    updateQueryStringParameter(this.urlContextSizeKey, contextLengthNumber);
                    //TODO reload page on context change?
                }
            }
        });

        resultsPerPageInputEl.on("change", () => {
            const resultsPerPageString = resultsPerPageInputEl.val() as string;
            const resultsPerPageNumber = parseInt(resultsPerPageString);
            if (!isNaN(resultsPerPageNumber)) {
                if (resultsPerPageNumber >= this.minResultsPerPage && resultsPerPageNumber <= this.maxResultsPerPage) {
                    this.searchResultsOnPage = resultsPerPageNumber;
                    updateQueryStringParameter(this.urlResultPerPageKey, resultsPerPageNumber);
                    //TODO reload page on result number change?
                }
            }
        });

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

        $(".corpus-search-settings-advanced-menu").on("click", () => {
            const advancedShowOptionsMenuEl = $(".corpus-search-result-view-properties-advanced");
            advancedShowOptionsMenuEl.slideToggle();
        });

        $(".text-results-table-body").on("click",
            ".result-row",
            (event: JQuery.Event) => {
                var clickedRow = $(event.target as Node as Element).closest(".result-row");

                $(".result-row").not(clickedRow).removeClass("clicked");
                clickedRow.addClass("clicked");

                this.printDetailInfo(clickedRow);
            });

        //$("#corpus-search-results-table-div").scroll((event) => {
        //    $("#corpus-search-results-abbrev-table-div").scrollTop($(event.target as Node as Element).scrollTop());
        //});

        //$("#corpus-search-results-abbrev-table-div").scroll((event) => {
        //    $("#corpus-search-results-table-div").scrollTop($(event.target as Node as Element).scrollTop());
        //});

        this.initializeFromUrlParams();

        this.enabledOptions.push(SearchTypeEnum.Title);
        this.enabledOptions.push(SearchTypeEnum.Author);
        this.enabledOptions.push(SearchTypeEnum.Editor);
        this.enabledOptions.push(SearchTypeEnum.Dating);
        this.enabledOptions.push(SearchTypeEnum.Fulltext);
        this.enabledOptions.push(SearchTypeEnum.Heading);
        this.enabledOptions.push(SearchTypeEnum.Sentence);
        this.enabledOptions.push(SearchTypeEnum.Term);
        this.enabledOptions.push(SearchTypeEnum.TokenDistance);

        const favoritesQueriesConfig: IModulInicializatorConfigurationSearchFavorites = {
            bookType: BookTypeEnum.TextBank,
            queryType: QueryTypeEnum.Search
        };
        this.search = new Search($("#listSearchDiv")[0] as Node as HTMLDivElement,
            this.corpusAdvancedSearchBookHits.bind(this),
            this.corpusBasicSearchBookHits.bind(this),
            favoritesQueriesConfig);
        this.search.limitFullTextSearchToOne();
        this.search.makeSearch(this.enabledOptions);

        const sortBarContainer = "#listResultsHeader";
        const sortBarContainerEl = $(sortBarContainer);
        sortBarContainerEl.empty();
        this.sortBar = new SortBar(this.sortOrderChanged.bind(this));
        const sortBarHtml = this.sortBar.makeSortBar(sortBarContainer);
        sortBarContainerEl.append(sortBarHtml);

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

    private formNextPage() {
        this.currentViewPage = this.paginator.getCurrentPage();
        const hasBeenWrapped = this.paginator.hasBeenWrapped();
        if (hasBeenWrapped) {
            bootbox.alert({
                title: "Attention",
                message: "Page does not exist",
                buttons: {
                    ok: {
                        className: "btn-default"
                    }
                }
            });
            return;
        }
            this.generateViewingPage();
    }

    private initializeFromUrlParams() {
        if (this.readyForInit && this.notInitialized) {

            this.notInitialized = false;

            const contextSize = getQueryStringParameterByName(this.urlContextSizeKey);
            if (contextSize) {
                const contextLengthNumber = parseInt(contextSize);
                if (!isNaN(contextLengthNumber)) {
                    if (contextLengthNumber >= this.minContextLength && contextLengthNumber <= this.maxContextLength) {
                        this.contextLength = contextLengthNumber;
                        $("#contextPositionsSelect").val(contextLengthNumber);
                    }
                }
            }

            const resultPerPage = getQueryStringParameterByName(this.urlResultPerPageKey);
            if (resultPerPage) {
                const resultsPerPageNumber = parseInt(resultPerPage);
                if (!isNaN(resultsPerPageNumber)) {
                    if (resultsPerPageNumber >= this.minResultsPerPage && resultsPerPageNumber <= this.maxResultsPerPage) {
                        this.searchResultsOnPage = resultsPerPageNumber;
                        $("#number-of-results-per-viewing-page").val(resultPerPage);
                    }
                }
            }

            const sortedAsc = getQueryStringParameterByName(this.urlSortAscKey);
            const sortCriteria = parseInt(getQueryStringParameterByName(this.urlSortCriteriaKey));

            if (sortedAsc && sortCriteria) {
                this.sortBar.setSortedAsc(sortedAsc === "true");
                this.sortBar.setSortCriteria(sortCriteria as SortEnum);
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
        if (this.search.isLastQueryJson()) {
            this.corpusAdvancedSearchBookHits(this.search.getLastQuery());
        } else {
            this.corpusBasicSearchBookHits(this.search.getLastQuery());
        }
    }

    private showLoading() {
        $(".text-results-table").hide();
        $("#corpus-search-results-table-div-loader").empty();
        $("#corpus-search-results-table-div-loader").show();
        $("#corpus-search-results-table-div-loader").addClass("loader");
    }


    private hideLoading() {
        $("#corpus-search-results-table-div-loader").removeClass("loader");
        $("#corpus-search-results-table-div-loader").hide();
        $(".text-results-table").show();
    }

    private printErrorMessage(message: string) {
        this.hideLoading();
        const corpusErrorDiv = $("#corpus-search-results-table-div-loader");
        corpusErrorDiv.empty();
        corpusErrorDiv.text(message);
        corpusErrorDiv.show();
    }

    private corpusAdvancedSearchPaged(json: string, start: number, contextLength: number, bookId: number) {
        if (!json) return;

        const count = this.searchResultsOnPage - this.transientResults.length;//TODO test
        this.showLoading();

        const payload: ICorpusLookupAdvancedSearch = {
            json: json,
            start: start,
            count: count,
            contextLength: contextLength,
            snapshotId: bookId,
            selectedCategoryIds: this.categoryIdsInQuery,
            selectedBookIds: this.bookIdsInQuery
        };
        const advancedSearchPageAjax =
            $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusGetPage`, payload);
        this.paginator.disable();
        const viewingPage = (start / count) + 1;
        const compositionListStart = this.compositionResultListStart - this.compositionsPerPage;
        advancedSearchPageAjax.done((response) => {
            this.hideLoading();
            const results: ICorpusSearchResult[] = response["results"];
            this.currentResultStart += count;
            this.calculateAndFlushNumberOfResults(results, count, start + count, compositionListStart, viewingPage);
        });
        advancedSearchPageAjax.fail(() => {
            this.printErrorMessage(this.defaultErrorMessage);
        });
    }

    private fillResultTable(results: ICorpusSearchResult[]) {
        const tableSection = $(".corpus-search-results-div");
        const textColumn = tableSection.find(".result-text-col");
        const textResultTableEl = textColumn.find(".text-results-table-body");
        const undefinedReplaceString = "<Nezadáno>";
        for (let i = 0; i < results.length; i++) {
            const result = results[i];
            const pageContext = result.pageResultContext;
            const verseContext = result.verseResultContext;
            const bibleVerseContext = result.bibleVerseResultContext;
            const contextStructure = pageContext.contextStructure;
            const bookId = result.bookId;
            const pageId = pageContext.id;
            const acronym = result.sourceAbbreviation;
            const notes = result.notes;

            const textResult = $(`<tr class="row result-row"></tr>`);

            textResult.attr("data-bookId", bookId);
            textResult.attr("data-author", result.author);
            textResult.attr("data-title", result.title);
            textResult.attr("data-dating", result.originDate);
            textResult.attr("data-pageId", pageId);
            textResult.attr("data-pageName", pageContext.name);
            textResult.attr("data-acronym", acronym);

            if (verseContext) {
                textResult.attr("data-verseXmlId", verseContext.verseXmlId);
                textResult.attr("data-verseName", verseContext.verseName);
            }

            if (bibleVerseContext) {
                textResult.attr("data-bibleBook", bibleVerseContext.bibleBook);
                textResult.attr("data-bibleChapter", bibleVerseContext.bibleChapter);
                textResult.attr("data-bibleVerse", bibleVerseContext.bibleVerse);
            }

            const contextBefore = $(`<td class="context-before"></td>`);
            contextBefore.text(contextStructure.before);

            const contextMatch = $(`<td class="text-center"></td>`);
            contextMatch.append(`<span class="match">${contextStructure.match}</span>`);

            const contextAfter = $(`<td class="context-after"></td>`);
            contextAfter.text(contextStructure.after);

            const abbrevHref = $("<a></a>");
            abbrevHref.prop("href",
                `${getBaseUrl()}Editions/Editions/Listing?bookId=${bookId}&searchText=${this.search.getLastQuery()
                }&page=${pageId}`);
            abbrevHref.text(acronym ? acronym : undefinedReplaceString);
            const abbrevTd = $(`<td class="abbrev-col"></td>`);
            abbrevTd.append(abbrevHref);

            textResult.append(abbrevTd);
            textResult.append(contextBefore);
            textResult.append(contextMatch);
            textResult.append(contextAfter);

            textResultTableEl.append(textResult);

            if (notes) {

                var notesTr = $("<tr></tr>");
                notesTr.addClass("notes");

                var tdNotes = $("<td></td>");
                tdNotes.attr("colSpan", 2);


                for (var j = 0; j < notes.length; j++) {
                    var noteSpan = $("<span></span>");
                    noteSpan.html(notes[j]);
                    noteSpan.addClass("note");
                    tdNotes.append(noteSpan);
                }


                notesTr.append(tdNotes);

                var beforeNotesTr = $("<tr></tr>");
                beforeNotesTr.addClass("notes spacer");

                var afterNotesTr = $("<tr></tr>");
                afterNotesTr.addClass("notes spacer");

                textResultTableEl.append(beforeNotesTr);
                textResultTableEl.append(notesTr);
                textResultTableEl.append(afterNotesTr);

            }

        }

        $(".text-results-table").tableHeadFixer({ "left": 1, "head": false});

        //scroll from left to center match column in table
        var matchPosition = textResultTableEl.children("tr").first().find(".match").position().left;
        var tableContainerWidth = textColumn.width();
        var scrollOffset = matchPosition - tableContainerWidth / 2;
        textColumn.scrollLeft(scrollOffset);
    }

    private corpusBasicSearchPaged(text: string, start: number, contextLength: number, bookId: number) {
        if (!text) return;
        const count = this.searchResultsOnPage - this.transientResults.length;

        this.showLoading();

        const payload: ICorpusLookupBasicSearch = {
            text: text,
            start: start,
            count: count,
            contextLength: contextLength,
            snapshotId: bookId,
            selectedCategoryIds: this.categoryIdsInQuery,
            selectedBookIds: this.bookIdsInQuery
        };

        console.log(`---PAGE ${this.paginator.getCurrentPage()} INDEX---`);
        console.log(`composition list start: ${this.compositionResultListStart - this.compositionsPerPage}`);
        console.log(`result list start: ${start}`);
        console.log(`result list count: ${count}`);
        console.log(`current bookID: ${this.currentBookId}`);
        console.log(`context length: ${contextLength}`);
        console.log(`---PAGE INDEX END---`);

        const getPageAjax = $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/TextSearchFulltextGetBookPage`,
            payload);
        this.paginator.disable();
        const viewingPage = this.paginator.getCurrentPage();
        const compositionListStart = this.compositionResultListStart - this.compositionsPerPage;
        getPageAjax.done((response) => {
            const results: ICorpusSearchResult[] = response["results"];
            this.currentResultStart += count;
            this.calculateAndFlushNumberOfResults(results, count, start + count, compositionListStart, viewingPage);
        });

        getPageAjax.fail(() => {
            this.printErrorMessage(this.defaultErrorMessage);
        });
        getPageAjax.always(() => {
            this.hideLoading();
        });
    }

    private calculateAndFlushNumberOfResults(results: ICorpusSearchResult[], count: number, currentResultStart: number, compositionListStart: number, viewingPage: number) {
        if (!results.length) {
            this.switchToNextBook();
            return;
        }
        this.transientResults = this.transientResults.concat(results);
        if (results.length < count && this.transientResults.length < this.searchResultsOnPage) {//TODO test
            this.switchToNextBook();
            return;
        }

        if (this.transientResults.length < this.searchResultsOnPage) {//TODO test
            this.loadBookResultPage(this.currentResultStart, this.currentBookId);
        } else {
            this.makeHistoryEntry(currentResultStart, compositionListStart, viewingPage);
            this.flushTransientResults();
            this.paginator.enable();
        }
    }

    private switchToNextBook() {
        this.currentResultStart = 0;//internal list index reset
        this.currentBookIndex++;//external list index shift
        if (this.currentBookIndex > (this.hitBookIds.length - 1)) {
            if (this.compositionPageIsLast) {
                this.paginator.enable();
                if (this.transientResults.length) {
                    const viewingPage = this.paginator.getCurrentPage();
                    this.makeHistoryEntry(this.currentResultStart, this.compositionResultListStart - this.compositionsPerPage, viewingPage);
                    this.flushTransientResults();
                } else {
                    if (this.paginator.isBasicMode()) {//unknown number of pages, thus page was increased by one and needs descreasing
                        this.paginator.updatePage(this.paginator.getCurrentPage() - 1);
                    }
                    bootbox.alert({
                        title: "Attention",
                        message: "This is a last page",
                        buttons: {
                            ok: {
                                className: "btn-default"
                            }
                        }
                    });
                }
                return;
            }else{
                const search = getQueryStringParameterByName(this.urlSearchKey);
                this.currentBookId = -1; //reset book id to get new
                this.currentBookIndex = 0;//reset book index as book array is new
            if (this.search.isLastQueryJson()) {
                this.loadNextCompositionAdvancedResultPage(search);
            } else {
                this.loadNextCompositionResultPage(search);
            }
            return;
            }
        }
        this.currentBookId = this.hitBookIds[this.currentBookIndex];
        this.loadBookResultPage(this.currentResultStart, this.currentBookId);
    }

    private loadBookResultPage(start: number, bookId: number) {
        const contextLength = this.contextLength;
        if (this.search.isLastQueryJson()) {
            this.corpusAdvancedSearchPaged(this.search.getLastQuery(), start, contextLength, bookId);
        } else {
            this.corpusBasicSearchPaged(this.search.getLastQuery(), start, contextLength, bookId);
        }
    }

    private resetIds() {
        this.compositionResultListStart = -1;
        this.currentResultStart = -1;
        this.currentBookIndex = 0;
        this.currentBookId = -1;
    }

    private onSearchStart() {
        const nextPageEl = $(".indefinite-pagination-next-page");
        const totalResultsEl = $(".total-results-count");
        totalResultsEl.hide();
        nextPageEl.prop("disabled", false);
        this.resetIds();
        this.paginator.reset();//reset pagination
        const firstPage = 1;
        this.currentViewPage = firstPage;
        this.emptyResultsTable();
        this.resetHistory();
        this.compositionPageIsLast = false;
        this.totalViewPages = 0;
        this.actualizeSelectedBooksAndCategoriesInQuery();
        this.showLoading();
    }

    private corpusBasicSearchBookHits(text: string) {
        if (!text) return;
        this.paginator.enable();
        this.onSearchStart();
        this.loadNextCompositionResultPage(text);
    }

    private emptyResultsTable() {
        const tableBody = $(".text-results-table-body");
        tableBody.empty();
    }

    private flushTransientResults() {
        this.emptyResultsTable();
        this.fillResultTable(this.transientResults);
        this.transientResults = [];
    }

    /**
     * Generates viewing page
     */
    private generateViewingPage() {
        if (this.currentBookId === -1) {
            this.currentBookId = this.hitBookIds[this.currentBookIndex];
        }
        if (this.currentResultStart === -1) {
            this.currentResultStart = 0;
        }
        this.loadBookResultPage(this.currentResultStart, this.currentBookId);
    }

    private resetHistory() {
        const historyContainerEl = $(".page-history-constainer");
        historyContainerEl.empty();
    }

    private goToPage(pageNumber: number) {
        this.loadPage(pageNumber);
        this.paginator.updatePage(pageNumber);
    }

    private loadPreviousPage() {
        const previousPage = this.paginator.getCurrentPage();
        this.loadPage(previousPage);
    }

    private showNoPageWarning(pageNumber?: number) {
        const pageNumberString = (pageNumber) ? pageNumber.toString() : ""; 
        bootbox.alert({
            title: "Attention",
            message: `Page ${pageNumberString} does not exist`,
            buttons: {
                ok: {
                    className: "btn-default"
                }
            }
        });
    }

    private loadPage(pageNumber: number) {
        const pageHasBeenWrapped = this.paginator.hasBeenWrapped();
        const previousPage = pageNumber - 1;
        if (previousPage === 0 && !pageHasBeenWrapped) {//to load page 1 it's needed to reset indexes
            this.resetIds();
            if (this.search.isLastQueryJson()) {
                this.loadNextCompositionAdvancedResultPage(this.search.getLastQuery());
            } else {
                this.loadNextCompositionResultPage(this.search.getLastQuery());
            }
            return;
        }
        const historyContainerEl = $(".page-history-constainer");
        //const prevPageButtonEl = $(".indefinite-pagination-prev-page");
        const viewingPageEl = historyContainerEl.children(`[data-viewing-page-number=${previousPage}]`);
        if (viewingPageEl.length && !pageHasBeenWrapped) {
            const entry: ICorpusSearchViewingPageHistoryEntry = JSON.parse(viewingPageEl.attr("data-viewing-page-structure"));
            this.compositionResultListStart = entry.compositionResultListStart;
            this.currentResultStart = entry.hitResultStart;
            this.currentBookId = entry.bookId;
            console.log(`---PAGE ${previousPage} LAST INDEX---`);
            console.log(`comosition list start: ${this.compositionResultListStart}`);
            console.log(`result list start: ${this.currentResultStart}`);
            console.log(`current bookID: ${this.hitBookIds[this.currentBookIndex]}`);
            console.log(`current viewing page: ${previousPage}`);
            console.log(`---PAGE INDEX END---`);
            if (this.search.isLastQueryJson()) {
                this.loadNextCompositionAdvancedResultPage(this.search.getLastQuery(), true);
            } else {
                this.loadNextCompositionResultPage(this.search.getLastQuery(), true);
            }
        } else {
            const isBasicPaginationMode = this.paginator.isBasicMode();
            if (isBasicPaginationMode) {
                this.showNoPageWarning();
            } else {
                this.makeTableForPage(pageNumber);
            }
        }
    }

    private makeTableForPage(pageNumber: number) {
        const hitResultTotalStart = (pageNumber - 1) * this.searchResultsOnPage;
        const compositionsPerPage = this.compositionsPerPage;

        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();
        const sortingDirection = sortAsc ? SortDirection.Asc : SortDirection.Desc;
        const selectedBookIds = this.bookIdsInQuery;
        const selectedCategoryIds = this.categoryIdsInQuery;
        const searchQuery = getQueryStringParameterByName(this.urlSearchKey);

        let payload: JQuery.PlainObject;
        let getPagePositionAjax: JQuery.jqXHR;
        if (this.search.isLastQueryJson()) {
            const searchParams: ICorpusListLookupAdvancedSearchParams = {
                json: searchQuery,
                sortBooksBy: sortingEnum,
                sortDirection: sortingDirection,
                selectedBookIds: selectedBookIds,
                selectedCategoryIds: selectedCategoryIds
            };
            payload = {
                hitResultTotalStart: hitResultTotalStart,
                compositionsPerPage: compositionsPerPage,
                searchParams: searchParams
            };
            getPagePositionAjax = $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetPagePositionInListsAdvanced`, payload);
        } else {
            const searchParams: ICorpusListLookupBasicSearchParams = {
                text: searchQuery,
                sortBooksBy: sortingEnum,
                sortDirection: sortingDirection,
                selectedBookIds: selectedBookIds,
                selectedCategoryIds: selectedCategoryIds
            };
            payload = {
                hitResultTotalStart: hitResultTotalStart,
                compositionsPerPage: compositionsPerPage,
                searchParams: searchParams
            };
            getPagePositionAjax = $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetPagePositionInListsBasic`, payload);
        }
        getPagePositionAjax.done((response) => {//TODO interface
            this.currentBookId = response.bookId;
            const start = response.hitResultStart;
            const compositionListStart = response.compositionListStart;
            this.makeHistoryEntry(start, compositionListStart, pageNumber - 1);
            this.loadPage(pageNumber);
        });
        getPagePositionAjax.fail(() => {
            this.printErrorMessage(this.defaultErrorMessage);
        });
    }

    private makeHistoryEntry(start: number, compositionResultListStart: number, viewingPage: number) {
        const historyContainerEl = $(".page-history-constainer");
        const viewingPageEl = historyContainerEl.children(`[data-viewing-page-number=${viewingPage}]`);
        if (viewingPageEl.length) {
            const pageStructure: ICorpusSearchViewingPageHistoryEntry = {
                compositionResultListStart: compositionResultListStart,
                bookId: this.currentBookId,
                hitResultStart: start
            };
            viewingPageEl.attr("data-viewing-page-structure", JSON.stringify(pageStructure));
        } else {
            const historyNewEntryEl = $(`<li></li>`);
            historyNewEntryEl.attr("data-viewing-page-number", viewingPage);
            const pageStructure: ICorpusSearchViewingPageHistoryEntry = {
                compositionResultListStart: compositionResultListStart,
                bookId: this.currentBookId,
                hitResultStart: start
            };
            historyNewEntryEl.attr("data-viewing-page-structure", JSON.stringify(pageStructure));
            historyContainerEl.append(historyNewEntryEl);
        }
    }

    private corpusAdvancedSearchBookHits(json: string) {
        if (!json) return;
        this.paginator.enable();
        this.onSearchStart();
        this.loadNextCompositionAdvancedResultPage(json);
    }

    /**
    * Pagination of external list - list of compositions with hits, basic search
    * @param text
    */
    private loadNextCompositionResultPage(text: string, setIndexFromId?: boolean) {
        if (this.compositionResultListStart === -1) {
            this.compositionResultListStart = 0;
        }
        const start = this.compositionResultListStart;
        const count = this.compositionsPerPage;
        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();
        const sortingDirection = sortAsc ? SortDirection.Asc : SortDirection.Desc;

        const payload: ICorpusListPageLookupBasicSearch = {
            text: text,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery,
            sortBooksBy: sortingEnum,
            sortDirection: sortingDirection,
            start: start,
            count: count
        };

        updateQueryStringParameter(this.urlSearchKey, text);
        updateQueryStringParameter(this.urlSortAscKey, sortAsc);
        updateQueryStringParameter(this.urlSortCriteriaKey, sortingEnum);
        updateQueryStringParameter(this.urlSelectionKey, this.booksSelector.getSerializedState());

        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetHitBookIdsPaged`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                console.log(bookIds);
                this.hideLoading();
                const totalCount = bookIds.totalCount;
                const page = (start / count) + 1;
                const totalPages = Math.ceil(totalCount / count);
                $("#totalCompositionsCountDiv").text(totalCount);
                const snapshotStructureArray = bookIds.list;
                var idList = [];
                snapshotStructureArray.forEach((snapshot) => {
                    idList.push(snapshot.snapshotId);
                });
                this.hitBookIds = idList;
                console.log(this.hitBookIds);
                if (this.hitBookIds.length < this.compositionsPerPage || page === totalPages) {
                    this.compositionPageIsLast = true;
                }
                if (!this.hitBookIds.length) {//TODO requires attention
                    if (this.transientResults.length) {
                        this.flushTransientResults();
                    }
                    bootbox.alert({
                        title: "Attention",
                        message: "No more results",
                        buttons: {
                            ok: {
                                className: "btn-default"
                            }
                        }
                    });
                    $(".indefinite-pagination-next-page").prop("disabled", true);
                } else {
                    if (setIndexFromId) {
                        this.currentBookIndex = $.inArray(this.currentBookId, this.hitBookIds);
                    }
                    this.compositionResultListStart += this.compositionsPerPage;
                    this.generateViewingPage();
                }
            }).fail(() => {
                this.printErrorMessage(this.defaultErrorMessage);
            });
    }
    /**
     * Pagination of external list - list of compositions with hits, advanced search
     * @param json Json request with search request
     */
    private loadNextCompositionAdvancedResultPage(json: string, setIndexFromId?: boolean) {
        if (this.compositionResultListStart === -1) {
            this.compositionResultListStart = 0;
        }
        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();
        const sortingDirection = sortAsc ? SortDirection.Asc : SortDirection.Desc;
        const start = this.compositionResultListStart;
        const count = this.compositionsPerPage;

        updateQueryStringParameter(this.urlSearchKey, json);
        updateQueryStringParameter(this.urlSelectionKey, this.booksSelector.getSerializedState());
        updateQueryStringParameter(this.urlSortAscKey, sortAsc);
        updateQueryStringParameter(this.urlSortCriteriaKey, sortingEnum);

        const payload: ICorpusListPageLookupAdvancedSearch = {
            json: json,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery,
            sortBooksBy: sortingEnum,
            sortDirection: sortingDirection,
            start: start,
            count: count
        };
        console.log(payload);
        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetHitBookIdsPaged`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                this.hideLoading();
                const totalCount = bookIds.totalCount;
                const page = (start / count) + 1;
                const totalPages = Math.ceil(totalCount / count);
                $("#totalCompositionsCountDiv").text(totalCount);
                const snapshotStructureArray = bookIds.list;
                var idList = [];
                snapshotStructureArray.forEach((snapshot) => {
                    idList.push(snapshot.snapshotId);
                });
                this.hitBookIds = idList;
                if (this.hitBookIds.length < this.compositionsPerPage || page === totalPages) {
                    this.compositionPageIsLast = true;
                }
                if (!this.hitBookIds.length) {//TODO requires attention
                    if (this.transientResults.length) {
                        this.emptyResultsTable();
                        this.fillResultTable(this.transientResults);
                        this.transientResults = [];
                    }
                    bootbox.alert({
                        title: "Attention",
                        message: "No more results",
                        buttons: {
                            ok: {
                                className: "btn-default"
                            }
                        }
                    });
                    $(".indefinite-pagination-next-page").prop("disabled", true);
                } else {
                    if (setIndexFromId) {
                        this.currentBookIndex = $.inArray(this.currentBookId, this.hitBookIds);
                    }
                    this.compositionResultListStart += this.compositionsPerPage;
                    this.generateViewingPage();
                }
            }).fail(() => {
                this.printErrorMessage(this.defaultErrorMessage);
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

    private loadAllPages() : JQuery.Deferred<any>{
        const searchQuery = this.search.getLastQuery();
        let ajax: JQuery.jqXHR;
        let payload: JQuery.PlainObject;
        if (this.search.isLastQueryJson()) {
            payload = {
                json: searchQuery,
                selectedSnapshotIds: this.bookIdsInQuery,
                selectedCategoryIds: this.categoryIdsInQuery
            };
            ajax = $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetTotalResultNumberAdvanced`, payload);
        } else {
            payload = {
                text: searchQuery,
                selectedSnapshotIds: this.bookIdsInQuery,
                selectedCategoryIds: this.categoryIdsInQuery
            };
            ajax = $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetTotalResultNumber`, payload);
        }
        const deferred = $.Deferred();
        ajax.done((result) => {
            const totalNumberOfPages = Math.ceil(result.totalCount / this.searchResultsOnPage);
            this.totalViewPages = totalNumberOfPages;
            const totalResultsEl = $(".total-results-count");
            const totalResultsNumberEl = totalResultsEl.children("#totalResultCountDiv");
            totalResultsNumberEl.text(result.totalCount);
            totalResultsEl.show();
            deferred.resolve(totalNumberOfPages);
        });
        ajax.fail(() => {
            deferred.reject();
        });
        return deferred;
    }
}