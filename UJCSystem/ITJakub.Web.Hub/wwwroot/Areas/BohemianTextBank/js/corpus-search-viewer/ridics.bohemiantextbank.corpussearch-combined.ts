﻿function initSearchCombined() {
    const searchCombined = new BohemianTextBankCombined();
    searchCombined.initSearch();
}

class BohemianTextBankCombined extends BohemianTextBankBase{

    private transientResults: ICorpusSearchResult[] = [];

    private paginator: IndefinitePagination;

    private paginationContainerEl = $("#paginationContainer");

    //string for localisation
    private attentionString = localization.translate("Warning", "BohemianTextBank").value;
    private lastResultPageString = localization.translate("LastPageOfResults", "BohemianTextBank").value;
    private lastResultPageDetailString = localization.translate("LastPageOfResultsDetail", "BohemianTextBank").value;
    private firstResultPageDetailString = localization.translate("FirstPageOfResultsDetail", "BohemianTextBank").value;
    private allResults = localization.translate("AllResults", "BohemianTextBank").value;

    private loadAllResultsButtonContent = $(`<div>${this.allResults}</div>`);

    initSearch() {
        const paginator = new IndefinitePagination({
            container: this.paginationContainerEl,
            nextPageCallback: this.formNextPage.bind(this),
            previousPageCallback: this.loadPreviousPage.bind(this),
            loadAllPagesButton: true,
            loadAllPagesCallback: this.loadAllPages.bind(this),
            loadAllPagesButtonContent: this.loadAllResultsButtonContent,
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
        const viewingSettingsChangedRefreshEl = $(".search-settings-changed-refresh");

        const contextSizeWarningEl = $(".context-size-warning");
        const contextSizeAlert = new AlertComponentBuilder(AlertType.Error);
        contextSizeAlert.addContent(this.contextSizeWarningMessage);
        contextSizeWarningEl.append(contextSizeAlert.buildElement());

        contextLengthInputEl.on("change", () => {
            if(this.atLeastOnSearchDone){
                viewingSettingsChangedRefreshEl.slideDown();
            }
            const contextLengthString = contextLengthInputEl.val() as string;
            const contextLengthNumber = parseInt(contextLengthString);
            if (!isNaN(contextLengthNumber)) {
                const contextSizeWarningEl = $(".context-size-warning");
                if (contextLengthNumber >= this.minContextLength && contextLengthNumber <= this.maxContextLength) {
                    contextSizeWarningEl.slideUp();
                    this.contextLength = contextLengthNumber;
                    updateQueryStringParameter(this.urlContextSizeKey, contextLengthNumber);
                } else {
                    contextSizeWarningEl.slideDown();
                }
            }
        });

        const numberOfPositionsWarningEl = $(".number-of-positions-size-warning");
        const numberOfPositions = new AlertComponentBuilder(AlertType.Error);
        numberOfPositions.addContent(this.numberOfResultsPerPageWarningMessage);
        numberOfPositionsWarningEl.append(numberOfPositions.buildElement());

        resultsPerPageInputEl.on("change", () => {
            if (this.atLeastOnSearchDone) {
                viewingSettingsChangedRefreshEl.slideDown();
            }
            const resultsPerPageString = resultsPerPageInputEl.val() as string;
            const resultsPerPageNumber = parseInt(resultsPerPageString);
            if (!isNaN(resultsPerPageNumber)) {
                const numberOfPositionsWarningEl = $(".number-of-positions-size-warning");
                if (resultsPerPageNumber >= this.minResultsPerPage && resultsPerPageNumber <= this.maxResultsPerPage) {
                    numberOfPositionsWarningEl.slideUp();
                    this.searchResultsOnPage = resultsPerPageNumber;
                    updateQueryStringParameter(this.urlResultPerPageKey, resultsPerPageNumber);
                } else {
                    numberOfPositionsWarningEl.slideDown();
                }
            }
        });
        
        $("#paragraphCheckbox").on("change", () => {
            var checkbox = $("#paragraphCheckbox");
            var mainDiv = $("#corpus-search-div");

            if (checkbox.is(":checked")) {
                mainDiv.addClass("show-paragraph");
            } else {
                mainDiv.removeClass("show-paragraph");
            }
        });

        $(".text-results-table-body").on("click",
            ".result-row",
            (event) => {
                var clickedRow = $(event.target).closest(".result-row");

                $(".result-row").not(clickedRow).removeClass("clicked");
                clickedRow.addClass("clicked");
                const detailSectionEl = $(".corpus-search-detail");
                this.printDetailInfo(clickedRow, detailSectionEl, this.search.getLastQuery());
            });

        this.initializeFromUrlParams();

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
            this.corpusAdvancedSearchBookHits.bind(this),
            this.corpusBasicSearchBookHits.bind(this),
            favoritesQueriesConfig);
        this.search.limitFullTextSearchToOne();
        this.search.makeSearch(this.enabledOptions, false);

        $(".results-refresh-button").on("click", () => {
            const query = this.search.getLastQuery();
            if (query) {
                const advancedMode = this.search.isLastQueryJson();
                if (advancedMode) {
                    this.corpusAdvancedSearchBookHits(query);
                } else {
                    this.corpusBasicSearchBookHits(query);
                }
            }
        });

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
        this.showCurrentPage(this.currentViewPage);
        this.paginator.disable();
        const hasBeenWrapped = this.paginator.hasBeenWrapped();
        if (hasBeenWrapped) {
            this.showWarning(this.lastResultPageString, this.lastResultPageDetailString);
        } else {
            const tableEl = $(".text-results-table");
            this.showLoading(tableEl);
            this.generateViewingPage();
        }
            
    }

    private sortOrderChanged() {
        if (this.search.isLastQueryJson()) {
            this.corpusAdvancedSearchBookHits(this.search.getLastQuery());
        } else {
            this.corpusBasicSearchBookHits(this.search.getLastQuery());
        }
    }

    private corpusAdvancedSearchPaged(json: string, start: number, contextLength: number, bookId: number) {
        if (!json) return;

        const count = this.searchResultsOnPage - this.transientResults.length;

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
            $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedCorpusSearchGetResultsPage`, payload as JQuery.PlainObject);
        const viewingPage = this.paginator.getCurrentPage();
        const compositionListStart = this.compositionResultListStart - this.compositionsPerPage;
        advancedSearchPageAjax.done((response) => {
            const results: ICorpusSearchResult[] = response["results"];
            this.currentResultStart += count;
            this.calculateAndFlushNumberOfResults(results, count, start + count, compositionListStart, viewingPage);
        });
        advancedSearchPageAjax.fail(() => {
            const loaderEl = $(".corpus-search-results-table-div-loader");
            this.printErrorMessage(this.defaultErrorMessage, loaderEl);
        });
    }

    private printDetailInfo(tableRowEl: JQuery, detailSectionEl: JQuery, query: string) {
        const detailAuthorEl = detailSectionEl.find(".detail-author");
        const detailTitleEl = detailSectionEl.find(".detail-title");
        const detailDatingEl = detailSectionEl.find(".detail-dating");
        const detailDatingCenturyEl = detailSectionEl.find(".detail-dating-century");
        const detailAbbrevEl = detailSectionEl.find(".detail-abbrev");
        const editionNoteEl = detailSectionEl.find(".detail-edition-note-href");
        const detailPholioEl = detailSectionEl.find(".detail-folio");
        const detailVerseEl = detailSectionEl.find(".detail-vers");
        const detailBibleVerseBookEl = detailSectionEl.find(".detail-bible-vers-book");
        const detailBibleVerseChapterEl = detailSectionEl.find(".detail-bible-vers-chapter");
        const detailBibleVerseVerseEl = detailSectionEl.find(".detail-bible-vers-vers");

        detailAuthorEl.text(tableRowEl.data("author") ? tableRowEl.data("author") : this.undefinedReplaceString);
        detailTitleEl.text(tableRowEl.data("title") ? tableRowEl.data("title") : this.undefinedReplaceString);
        detailDatingEl.text(tableRowEl.data("dating") ? tableRowEl.data("dating") : this.undefinedReplaceString);
        detailDatingCenturyEl.text(this.undefinedReplaceString); //TODO ask where is this info stored
        detailAbbrevEl.text(tableRowEl.data("acronym") ? tableRowEl.data("acronym") : this.undefinedReplaceString);

        //Edition note
        const editionNoteAnchor = editionNoteEl;
        const bookId = tableRowEl.data("bookid");
        editionNoteAnchor.prop("href", `/EditionNote/EditionNote?bookId=${bookId}`);

        const folioHref = $("<a></a>");
        const pageId = tableRowEl.data("pageid");
        folioHref.prop("href",
            `${getBaseUrl()}Editions/Editions/Listing?bookId=${bookId}&searchText=${
            query}&page=${pageId}`);
        folioHref.text(tableRowEl.data("pagename") ? tableRowEl.data("pagename") : this.undefinedReplaceString);

        detailPholioEl.empty().append(folioHref);

        detailVerseEl.text(tableRowEl.data("verseName") ? tableRowEl.data("verseName") : this.undefinedReplaceString);
        detailBibleVerseBookEl
            .text(tableRowEl.data("bibleBook") ? tableRowEl.data("bibleBook") : this.undefinedReplaceString);
        detailBibleVerseChapterEl
            .text(tableRowEl.data("bibleChapter") ? tableRowEl.data("bibleChapter") : this.undefinedReplaceString);
        detailBibleVerseVerseEl
            .text(tableRowEl.data("bibleVerse") ? tableRowEl.data("bibleVerse") : this.undefinedReplaceString);
    }

    private corpusBasicSearchPaged(text: string, start: number, contextLength: number, bookId: number) {
        if (!text) return;
        const count = this.searchResultsOnPage - this.transientResults.length;

        const payload: ICorpusLookupBasicSearch = {
            text: text,
            start: start,
            count: count,
            contextLength: contextLength,
            snapshotId: bookId,
            selectedCategoryIds: this.categoryIdsInQuery,
            selectedBookIds: this.bookIdsInQuery
        };

        const getPageAjax = $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/BasicCorpusSearchGetResultsPage`,
            payload as JQuery.PlainObject);
        const viewingPage = this.paginator.getCurrentPage();
        const compositionListStart = this.compositionResultListStart - this.compositionsPerPage;
        getPageAjax.done((response) => {
            const results: ICorpusSearchResult[] = response["results"];
            this.currentResultStart += count;
            this.calculateAndFlushNumberOfResults(results, count, start + count, compositionListStart, viewingPage);
        });
        getPageAjax.fail(() => {
            const loaderEl = $(".corpus-search-results-table-div-loader");
            this.printErrorMessage(this.defaultErrorMessage, loaderEl);
        });
    }

    private calculateAndFlushNumberOfResults(results: ICorpusSearchResult[], count: number, currentResultStart: number, compositionListStart: number, viewingPage: number) {
        if (!results.length) {
            this.switchToNextBook();
            return;
        }
        this.transientResults = this.transientResults.concat(results);
        if (results.length < count && this.transientResults.length < this.searchResultsOnPage) {
            this.switchToNextBook();
            return;
        }

        if (this.transientResults.length < this.searchResultsOnPage) {
            this.loadBookResultPage(this.currentResultStart, this.currentBookId);
        } else {
            this.makeHistoryEntry(currentResultStart, compositionListStart, viewingPage);
            this.flushTransientResults();
            this.paginator.enable();
        }
    }

    private switchToNextBook() {
        this.currentBookIndex++;//external list index shift
        if (this.currentBookIndex > (this.hitBookIds.length - 1)) {
            if (this.compositionPageIsLast) {//no more books and no more composition pages
                this.paginator.enable();
                if (this.transientResults.length) {
                    const viewingPage = this.paginator.getCurrentPage();
                    this.makeHistoryEntry(this.currentResultStart, this.compositionResultListStart - this.compositionsPerPage, viewingPage);
                    this.flushTransientResults();
                } else {
                    if (this.paginator.isBasicMode()) {//unknown number of pages, thus page was increased by one and needs descreasing
                        this.paginator.updatePage(this.paginator.getCurrentPage() - 1);
                        this.showCurrentPage(this.paginator.getCurrentPage());
                    }
                    const tableEl = $(".text-results-table");
                    this.hideLoading(tableEl);
                    bootbox.alert({
                        title: this.lastResultPageString,
                        message: this.lastResultPageDetailString,
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
                this.currentResultStart = 0;//internal list index reset
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
        this.currentResultStart = 0;//internal list index reset
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
        this.atLeastOnSearchDone = true;
        const nextPageEl = $(".indefinite-pagination-next-page");
        const totalResultsEl = $(".total-results-count");
        const viewingSettingsChangedRefreshEl = $(".search-settings-changed-refresh");
        viewingSettingsChangedRefreshEl.slideUp();
        totalResultsEl.hide();
        nextPageEl.prop("disabled", false);
        this.resetIds();
        this.paginator.reset();//reset pagination
        this.paginator.disable();
        const firstPage = 1;
        this.currentViewPage = firstPage;
        this.showCurrentPage(firstPage);
        this.emptyResultsTable();
        this.resetHistory();
        this.compositionPageIsLast = false;
        this.totalViewPages = 0;
        this.updateSelectedBooksAndCategoriesInQuery();
        const tableEl = $(".text-results-table");
        this.showLoading(tableEl);
    }

    private corpusBasicSearchBookHits(text: string) {
        if (!text) return;
        this.paginator.enable();
        this.onSearchStart();
        this.loadNextCompositionResultPage(text);
    }

    private flushTransientResults() {
        this.emptyResultsTable();
        const tableSectionEl = $(".corpus-search-results-div");
        this.fillResultTable(this.transientResults, this.search.getLastQuery(), tableSectionEl);
        this.transientResults = [];
    }

    private emptyResultsTable() {
        const tableBody = $(".text-results-table-body");
        tableBody.empty();
    }

    private showNoPageWarning(pageNumber?: number) {
        const pageNumberString = (pageNumber) ? pageNumber.toString() : "";
        const message = localization.translateFormat("PageWithNumberDoesNotExists", [pageNumberString], "BohemianTextBank").value;
        this.showWarning(this.attentionString, message);
    }

    private showWarning(title: string, message: string) {
        this.paginator.enable();
        bootbox.alert({
            title: title,
            message: message,
            buttons: {
                ok: {
                    className: "btn-default"
                }
            }
        });
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
        this.showCurrentPage(pageNumber);
        this.paginator.disable();
        this.loadPage(pageNumber);
        this.paginator.updatePage(pageNumber);
    }

    private loadPreviousPage() {
        const previousPage = this.paginator.getCurrentPage();
        this.showCurrentPage(previousPage);
        this.paginator.disable();
        this.loadPage(previousPage);
    }

    private loadPage(pageNumber: number) {
        const tableEl = $(".text-results-table");
        const pageHasBeenWrapped = this.paginator.hasBeenWrapped();
        if (!pageHasBeenWrapped) {
            this.showLoading(tableEl);
        }
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
        const viewingPageEl = historyContainerEl.children(`[data-viewing-page-number=${previousPage}]`);
        if (viewingPageEl.length && !pageHasBeenWrapped) {
            const entry: ICorpusSearchViewingPageHistoryEntry = JSON.parse(viewingPageEl.attr("data-viewing-page-structure"));
            this.compositionResultListStart = entry.compositionResultListStart;
            this.currentResultStart = entry.hitResultStart;
            this.currentBookId = entry.bookId;
            if (this.search.isLastQueryJson()) {
                this.loadNextCompositionAdvancedResultPage(this.search.getLastQuery(), true);
            } else {
                this.loadNextCompositionResultPage(this.search.getLastQuery(), true);
            }
        } else {
            const isBasicPaginationMode = this.paginator.isBasicMode();
            if (isBasicPaginationMode || this.paginator.hasBeenWrapped()) {
                this.showWarning(this.attentionString, this.firstResultPageDetailString);
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
            getPagePositionAjax = $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetPagePositionInAllResultPages`, payload);
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
            getPagePositionAjax = $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/BasicSearchGetPagePositionInAllResultPages`, payload);
        }
        getPagePositionAjax.done((response: CorpusSearchPagePosition) => {
            this.currentBookId = response.bookId;
            const start = response.hitResultStart;
            const compositionListStart = response.compositionListStart;
            this.makeHistoryEntry(start, compositionListStart, pageNumber - 1);
            this.loadPage(pageNumber);
        });
        getPagePositionAjax.fail(() => {
            const loaderEl = $(".corpus-search-results-table-div-loader");
            this.printErrorMessage(this.defaultErrorMessage, loaderEl);
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

        updateQueryStringParameter(this.urlSearchKey, text);
        updateQueryStringParameter(this.urlSelectionKey, this.booksSelector.getSerializedState());
        updateQueryStringParameter(this.urlSortAscKey, sortAsc);
        updateQueryStringParameter(this.urlSortCriteriaKey, sortingEnum);
        
        const payload: ICorpusListPageLookupBasicSearch = {
            text: text,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery,
            sortBooksBy: sortingEnum,
            sortDirection: sortingDirection,
            start: start,
            count: count
        };

        const ajax = this.basicApiClient.basicSearchGetResultSnapshotListPageOfIdsWithoutResultNumbers(payload);
        this.processLoadedNextCompositionResultPage(ajax, start, count, setIndexFromId);
    }
    /**
     * Pagination of external list - list of compositions with hits, advanced search
     * @param json Json request with search request
     */
    private loadNextCompositionAdvancedResultPage(json: string, setIndexFromId?: boolean) {
        if (this.compositionResultListStart === -1) {
            this.compositionResultListStart = 0;
        }
        const start = this.compositionResultListStart;
        const count = this.compositionsPerPage;
        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();
        const sortingDirection = sortAsc ? SortDirection.Asc : SortDirection.Desc;
        
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
        const ajax = this.basicApiClient.advancedSearchGetResultSnapshotListPageOfIdsWithoutResultNumbers(payload);
        this.processLoadedNextCompositionResultPage(ajax, start, count, setIndexFromId);
    }

    private processLoadedNextCompositionResultPage(ajaxResult: JQuery.jqXHR<ICoprusSearchSnapshotResult>, start: number, count: number, setIndexFromId?: boolean) {
        ajaxResult
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                const totalCount = bookIds.totalCount;
                const page = (start / count) + 1;
                const totalPages = Math.ceil(totalCount / count);
                $("#totalCompositionsCountDiv").text(totalCount);
                const snapshotStructureArray = bookIds.snapshotList;
                var idList: number[] = [];
                snapshotStructureArray.forEach((snapshot) => {
                    idList.push(snapshot.snapshotId);
                });
                this.hitBookIds = idList;
                if (this.hitBookIds.length < this.compositionsPerPage || page === totalPages) {
                    this.compositionPageIsLast = true;
                }
                if (!this.hitBookIds.length) {
                    if (this.transientResults.length) {
                        this.flushTransientResults();
                    } else {
                        const tableEl = $(".text-results-table");
                        this.hideLoading(tableEl);
                        const alert = new AlertComponentBuilder(AlertType.Info);
                        alert.addContent(localization.translate("NoResults", "BohemianTextBank").value);
                        this.emptyResultsTable();
                        $(".text-results-table-body").append(alert.buildElement());
                    }
                } else {
                    if (setIndexFromId) {
                        this.currentBookIndex = $.inArray(this.currentBookId, this.hitBookIds);
                    }
                    this.compositionResultListStart += this.compositionsPerPage;
                    this.generateViewingPage();
                }
            }).fail(() => {
                const loaderEl = $(".corpus-search-results-table-div-loader");
                this.printErrorMessage(this.defaultErrorMessage, loaderEl);
            });
    }

    private loadAllPages() : JQuery.Deferred<any>{
        const searchQuery = this.search.getLastQuery();
        let ajax: JQuery.jqXHR;
        if (this.search.isLastQueryJson()) {
            const payload: CorpusSearchTotalResultCountAdvanced = {
                json: searchQuery,
                selectedSnapshotIds: this.bookIdsInQuery,
                selectedCategoryIds: this.categoryIdsInQuery
            };
            ajax = $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetTotalResultNumber`, payload as JQuery.PlainObject);
        } else {
            const payload: CorpusSearchTotalResultCountBasic = {
                text: searchQuery,
                selectedSnapshotIds: this.bookIdsInQuery,
                selectedCategoryIds: this.categoryIdsInQuery
            };
            ajax = $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/BasicSearchGetTotalResultNumber`, payload as JQuery.PlainObject);
        }
        const deferred = $.Deferred();
        ajax.done((result) => {
            const totalNumberOfPages = Math.ceil(result.totalCount / this.searchResultsOnPage);
            this.totalViewPages = totalNumberOfPages;
            const totalResultsEl = $(".total-results-count");
            const totalResultsNumberEl = totalResultsEl.children("#totalResultCountDiv");
            totalResultsNumberEl.text(result.totalCount);
            totalResultsEl.show();
            $("#currentPageContainer").hide();
            deferred.resolve(totalNumberOfPages);
        });
        ajax.fail(() => {
            deferred.reject();
        });
        return deferred;
    }

    private showCurrentPage(pageNumber: number) {
        $("#currentPageValue").text(pageNumber);
        if (this.paginator.isBasicMode()) {
            $("#currentPageContainer").show();
        } else {
            $("#currentPageContainer").hide();
        }
    }
}