function initSearchCombined() {
    const searchCombined = new BohemianTextBankCombined();
    searchCombined.initSearch();
}

class BohemianTextBankCombined extends BohemianTextBankBase{

    private transientResults: ICorpusSearchResult[] = [];

    private paginator: IndefinitePagination;

    private compositionPageIsLast = false;

    private paginationContainerEl = $("#paginationContainer");

    initSearch() {
        const paginator = new IndefinitePagination({
            container: this.paginationContainerEl,
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
        const viewingSettingsChangedWarningEl = $(".search-settings-changed-warning");

        const contextSizeWarningEl = $(".context-size-warning");
        const contextSizeAlert = new AlertComponentBuilder(AlertType.Error);
        contextSizeAlert.addContent(`Context size must be between ${this.minContextLength} and ${this.maxContextLength}`);
        contextSizeWarningEl.append(contextSizeAlert.buildElement());

        contextLengthInputEl.on("change", () => {
            viewingSettingsChangedWarningEl.slideDown();
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
        numberOfPositions.addContent(`Number of results must be between ${this.minResultsPerPage} and ${this.maxResultsPerPage}`);
        numberOfPositionsWarningEl.append(numberOfPositions.buildElement());

        resultsPerPageInputEl.on("change", () => {
            viewingSettingsChangedWarningEl.slideDown();
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

                this.printDetailInfo(clickedRow, this.search.getLastQuery());
            });

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
        this.paginator.disable();
        const hasBeenWrapped = this.paginator.hasBeenWrapped();
        if (hasBeenWrapped) {
            this.showNoPageWarning();
        } else {
            this.showLoading();
             this.generateViewingPage();
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
            $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusGetPage`, payload);
        const viewingPage = this.paginator.getCurrentPage();
        const compositionListStart = this.compositionResultListStart - this.compositionsPerPage;
        advancedSearchPageAjax.done((response) => {
            const results: ICorpusSearchResult[] = response["results"];
            this.currentResultStart += count;
            this.calculateAndFlushNumberOfResults(results, count, start + count, compositionListStart, viewingPage);
        });
        advancedSearchPageAjax.fail(() => {
            this.printErrorMessage(this.defaultErrorMessage);
        });
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

        const getPageAjax = $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/TextSearchFulltextGetBookPage`,
            payload);
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
                    }
                    this.hideLoading();
                    bootbox.alert({
                        title: "Attention",
                        message: "Last result page",
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
        const nextPageEl = $(".indefinite-pagination-next-page");
        const totalResultsEl = $(".total-results-count");
        const viewingSettingsChangedWarningEl = $(".search-settings-changed-warning");
        viewingSettingsChangedWarningEl.slideUp();
        totalResultsEl.hide();
        nextPageEl.prop("disabled", false);
        this.resetIds();
        this.paginator.reset();//reset pagination
        this.paginator.disable();
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

    private flushTransientResults() {
        this.emptyResultsTable();
        this.fillResultTable(this.transientResults, this.search.getLastQuery());
        this.transientResults = [];
    }

    private showNoPageWarning(pageNumber?: number) {
        this.paginator.enable();
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
        this.paginator.disable();
        this.loadPage(pageNumber);
        this.paginator.updatePage(pageNumber);
    }

    private loadPreviousPage() {
        const previousPage = this.paginator.getCurrentPage();
        this.paginator.disable();
        this.loadPage(previousPage);
    }

    private loadPage(pageNumber: number) {
        this.showLoading();
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
        const historyContainerEl = $(".page-history-constainer");;
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
        getPagePositionAjax.done((response: CorpusSearchPagePosition) => {
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
                if (!this.hitBookIds.length) {
                    if (this.transientResults.length) {
                        this.flushTransientResults();
                    } else {
                        this.hideLoading();
                        const alert = new AlertComponentBuilder(AlertType.Info);
                        alert.addContent("No results");
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
        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetHitBookIdsPaged`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
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
                if (!this.hitBookIds.length) {
                    if (this.transientResults.length) {
                        this.flushTransientResults();
                    } else {
                        this.hideLoading();
                        const alert = new AlertComponentBuilder(AlertType.Info);
                        alert.addContent("No results");
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
                this.printErrorMessage(this.defaultErrorMessage);
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
            ajax = $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetTotalResultNumberAdvanced`, payload);
        } else {
            const payload: CorpusSearchTotalResultCountBasic = {
                text: searchQuery,
                selectedSnapshotIds: this.bookIdsInQuery,
                selectedCategoryIds: this.categoryIdsInQuery
            };
            ajax = $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetTotalResultNumber`, payload);
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