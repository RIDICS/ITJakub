function initSearchSingle() {
    const searchSingle = new BohemianTextBankSingle();
    searchSingle.initSearch();
}

class BohemianTextBankSingle extends BohemianTextBankBase {
    private lastSnapshotId=-1;
    initSearch() {
        this.lazyloadEventProcess();

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
            this.startAdvancedSearch.bind(this),
            this.startBasicSearch.bind(this),
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

    private goToResultPage(pageNumber: number, snapshotId: number, query: string, contextSize: number) {//TODO logic
        if (!query) return;
        const tableEl = $(`*[data-snapshotId=${snapshotId}]`).find(".text-results-table");
        this.showLoading(tableEl);
        const start = (pageNumber - 1) * this.searchResultsOnPage;
        const count = this.searchResultsOnPage;
        const isAdvancedMode = this.search.isLastQueryJson();
        if (isAdvancedMode) {
            this.loadResultPageAdvanced(query, start, count, contextSize, snapshotId);
        } else {
            this.loadResultPageBasic(query, start, count, contextSize, snapshotId);
        }
    }

    private loadResultPageBasic(text: string, start: number, count: number, contextSize: number, snapshotId: number) {
        const url = `${getBaseUrl()}BohemianTextBank/BohemianTextBank/TextSearchFulltextGetBookPage`;
        const payload: ICorpusLookupBasicSearch = {
            text: text,
            start: start,
            count: count,
            contextLength: contextSize,
            snapshotId: snapshotId,
            selectedCategoryIds: this.categoryIdsInQuery,
            selectedBookIds: this.bookIdsInQuery
        };
        const getPageAjax = $.get(url, payload);
        getPageAjax.done((response) => {
            const results: ICorpusSearchResult[] = response["results"];
            const bookSectionEl = $(`*[data-snapshotId=${snapshotId}]`);
            this.emptyResultsTable(bookSectionEl);
            this.fillResultTable(results, this.search.getLastQuery(), bookSectionEl);
        });
        getPageAjax.fail(() => {
            const loaderEl = $(".corpus-search-results-table-div-loader");
            this.printErrorMessage(this.defaultErrorMessage, loaderEl);
        });
    }

    private emptyResultsTable(bookSectionEl : JQuery) {
        const tableEl = bookSectionEl.find(".text-results-table-body");
        tableEl.empty();
    }

    private loadResultPageAdvanced(json: string,
        start: number,
        count: number,
        contextSize: number,
        snapshotId: number) {
        const url = `${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusGetPage`;
        const payload: ICorpusLookupAdvancedSearch = {
            json: json,
            start: start,
            count: count,
            contextLength: contextSize,
            snapshotId: snapshotId,
            selectedCategoryIds: this.categoryIdsInQuery,
            selectedBookIds: this.bookIdsInQuery
        };
        const getPageAjax = $.get(url, payload);
        getPageAjax.done((response) => {
            const results: ICorpusSearchResult[] = response["results"];
            const bookSectionEl = $(`*[data-snapshotId=${snapshotId}]`);
            this.emptyResultsTable(bookSectionEl);
            this.fillResultTable(results, this.search.getLastQuery(), bookSectionEl);
        });
        getPageAjax.fail(() => {
            const loaderEl = $(`*[data-snapshotId=${snapshotId}]`).find(".corpus-search-results-table-div-loader");
            this.printErrorMessage(this.defaultErrorMessage, loaderEl);
        });
    }

    private generateBookSectionLayout(snapshotId: number, resultNumber: number) {
        const bookSectionsContainer = $(".results-container");
        const bookRowEl = $(`<div class="book-section-row row lazyload" data-snapshotId="${snapshotId}" data-result-number="${resultNumber}"></div>`);
        const bookDetailSection = $(`<div class="col-xs-6 book-detail-section"></div>`);
        const searchResultsSection = $(`<div class="col-xs-6 result-text-col text-center"></div>`);
        const tableEl = $(`<div class="corpus-search-results-table-div-loader">Pro zobrazení výsledků použijte vyhledávání</div><table class="text-results-table"><tbody class="text-results-table-body"></tbody></table>`);
        const searchResultsPaginationSection = $(`<div class="col-xs-12 pagination-section text-center"></div>`);
        searchResultsSection.append(tableEl);
        bookRowEl.append(bookDetailSection).append(searchResultsSection).append(searchResultsPaginationSection);
        bookSectionsContainer.append(bookRowEl);
    }

    private lazyloadEventProcess() {
        $(".corpus-result-and-settings-row").on("lazybeforeunveil", (event) => {
            const targetEl = $(event.target as Node as Element);
            const snapshotId = parseInt(targetEl.attr("data-snapshotId"));
            const totalResults = parseInt(targetEl.attr("data-result-number"));
            if (!isNaN(snapshotId) && !isNaN(totalResults)) {
                this.fillBookSection(snapshotId, totalResults);
                if (snapshotId === this.lastSnapshotId && !this.compositionPageIsLast) {
                    const isAdvancedSearch = this.search.isLastQueryJson();
                    if (isAdvancedSearch) {
                        this.corpusAdvancedSearchBookHits(this.search.getLastQuery());
                    } else {
                        this.corpusBasicSearchBookHits(this.search.getLastQuery());
                    }
                    
                }
            }
        });
    }

    private fillBookSection(snapshotId: number, totalResults: number) {
        const bookSectionEl = $(`*[data-snapshotId=${snapshotId}]`);
        const paginationEl = bookSectionEl.find(".pagination-section");
        const paginator = new Pagination({
            container: paginationEl,
            pageClickCallback: (pageNumber) => {
                this.goToResultPage(pageNumber, snapshotId, this.search.getLastQuery(), this.contextLength);
            },
            showSlider: true,
            showInput: true,
            callPageClickCallbackOnInit: true
        });
        paginator.make(totalResults, this.searchResultsOnPage, 1);
        //TODO fill book details
    }

    private sortOrderChanged() {
        //TODO logic
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

    private onSearchStartSingle() {//TODO logic
        $(".results-container").empty();
        this.compositionResultListStart = - 1;
        this.lastSnapshotId = - 1;
    }

    private startBasicSearch(text: string) {
        if (!text) return;
        this.onSearchStartSingle();
        this.corpusBasicSearchBookHits(text);
    }

    private startAdvancedSearch(json: string) {
        if (!json) return;
        this.onSearchStartSingle();
        this.corpusAdvancedSearchBookHits(json);
    }

    private corpusBasicSearchBookHits(text: string) {
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

        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetHitBookResultNumbers`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                const totalCount = bookIds.totalCount;
                const page = (start / count) + 1;
                const totalPages = Math.ceil(totalCount / count);
                this.compositionResultListStart += this.compositionsPerPage;
                //$("#totalCompositionsCountDiv").text(totalCount);
                const snapshotStructureArray = bookIds.snapshotList;
                var idList: number[] = [];
                snapshotStructureArray.forEach((snapshot) => {
                    idList.push(snapshot.snapshotId);
                    this.generateBookSectionLayout(snapshot.snapshotId, snapshot.resultCount);
                });
                this.hitBookIds = idList;
                this.lastSnapshotId = idList[idList.length - 1];
                if (this.hitBookIds.length < this.compositionsPerPage || page === totalPages) {
                    this.compositionPageIsLast = true;
                }
            }).fail(() => {
                const loaderEl = $(".corpus-search-results-table-div-loader");
                this.printErrorMessage(this.defaultErrorMessage, loaderEl);
            });

    }

    private corpusAdvancedSearchBookHits(json: string) {
        //this.loadNextCompositionAdvancedResultPage(json);TODO logic
    }
}