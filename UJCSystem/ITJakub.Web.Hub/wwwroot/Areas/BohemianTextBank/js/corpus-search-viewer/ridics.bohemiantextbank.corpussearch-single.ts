﻿function initSearchSingleLazyload() {
    const searchSingleLazyload = new BohemianTextBankSingleLazyload();
    searchSingleLazyload.initSearch();
}

function initSearchSinglePaged() {
    const searchSinglePaged = new BohemianTextBankSinglePaged();
    searchSinglePaged.initSearch();
}

class BohemianTextBankSingle extends BohemianTextBankBase {
    protected init() {

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

    protected  goToResultPage(pageNumber: number, snapshotId: number, query: string, contextSize: number) {
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

    protected showPageLoading() {
        const pageLoaderEl = $(".page-loader");
        pageLoaderEl.removeClass("alert alert-info").addClass("loader");
        pageLoaderEl.show();
    }

    protected hidePageLoading() {
        const pageLoaderEl = $(".page-loader");
        pageLoaderEl.hide();
        pageLoaderEl.removeClass("loader");
    }

    protected resultRowClickAttach() {
        $(".text-results-table-body").off("click", ".result-row");
        $(".text-results-table-body").on("click",
            ".result-row",
            (event: JQuery.Event) => {
                console.log(event.target);
                var clickedRow = $(event.target as Node as Element).closest(".result-row");

                clickedRow.siblings(".result-row").removeClass("clicked");
                clickedRow.addClass("clicked");
                const detailSectionEl = clickedRow.parents(".result-text-col").siblings(".book-detail-section");
                this.printDetailInfo(clickedRow, detailSectionEl, this.search.getLastQuery());
            });
    }

    private printDetailInfo(tableRowEl: JQuery, detailSectionEl: JQuery, query: string) {
        const undefinedReplaceString = "<Nezadáno>";
        const detailPholioEl = detailSectionEl.find(".detail-folio");
        const detailVerseEl = detailSectionEl.find(".detail-vers");
        const detailBibleVerseBookEl = detailSectionEl.find(".detail-bible-vers-book");
        const detailBibleVerseChapterEl = detailSectionEl.find(".detail-bible-vers-chapter");
        const detailBibleVerseVerseEl = detailSectionEl.find(".detail-bible-vers-vers");

        const bookId = tableRowEl.data("bookid");
        const folioHref = $("<a></a>");
        const pageId = tableRowEl.data("pageid");
        folioHref.prop("href",
            `${getBaseUrl()}Editions/Editions/Listing?bookId=${bookId}&searchText=${
            query}&page=${pageId}`);
        folioHref.text(tableRowEl.data("pagename") ? tableRowEl.data("pagename") : undefinedReplaceString);

        detailPholioEl.empty().append(folioHref);

        detailVerseEl.text(tableRowEl.data("verseName") ? tableRowEl.data("verseName") : undefinedReplaceString);
        detailBibleVerseBookEl
            .text(tableRowEl.data("bibleBook") ? tableRowEl.data("bibleBook") : undefinedReplaceString);
        detailBibleVerseChapterEl
            .text(tableRowEl.data("bibleChapter") ? tableRowEl.data("bibleChapter") : undefinedReplaceString);
        detailBibleVerseVerseEl
            .text(tableRowEl.data("bibleVerse") ? tableRowEl.data("bibleVerse") : undefinedReplaceString);
    }

    protected loadResultPageBasic(text: string, start: number, count: number, contextSize: number, snapshotId: number) {
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
            this.fillBookDetailsIntoDetailsSection(bookSectionEl);
            this.resultRowClickAttach();
        });
        getPageAjax.fail(() => {
            const loaderEl = $(".corpus-search-results-table-div-loader");
            this.printErrorMessage(this.defaultErrorMessage, loaderEl);
        });
    }

    protected emptyResultsTable(bookSectionEl : JQuery) {
        const tableEl = bookSectionEl.find(".text-results-table-body");
        tableEl.empty();
    }

    protected loadResultPageAdvanced(json: string,
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
            this.fillBookDetailsIntoDetailsSection(bookSectionEl);
            this.resultRowClickAttach();
        });
        getPageAjax.fail(() => {
            const loaderEl = $(`*[data-snapshotId=${snapshotId}]`).find(".corpus-search-results-table-div-loader");
            this.printErrorMessage(this.defaultErrorMessage, loaderEl);
        });
    }

    protected generateBookSectionLayout(snapshotId: number, resultNumber: number) {
        const bookSectionsContainer = $(".results-container");
        const bookRowEl = $(`<div class="book-section-row row lazyload" data-snapshotId="${snapshotId}" data-result-number="${resultNumber}"></div>`);
        const bookDetailSection = $(`<div class="col-xs-4 col-sm-6 book-detail-section"></div>`);
        const searchResultsSection = $(`<div class="col-xs-8 col-sm-6 result-text-col text-center"></div>`);
        const tableEl = $(`<div class="corpus-search-results-table-div-loader"></div><table class="text-results-table"><tbody class="text-results-table-body"></tbody></table>`);
        const searchResultsPaginationSection = $(`<div class="col-xs-12 pagination-section text-center"></div>`);
        searchResultsSection.append(tableEl);
        bookRowEl.append(bookDetailSection).append(searchResultsSection).append(searchResultsPaginationSection);
        bookSectionsContainer.append(bookRowEl);
        this.hidePageLoading();
    }

    protected fillBookDetailsIntoDetailsSection(bookSectionEl: JQuery) {
        const undefinedReplaceString = "<Nezadáno>";
        const resultsSectionEl = bookSectionEl.find(".result-text-col");
        const detailsSectionEl = bookSectionEl.find(".book-detail-section");
        const firstResultRowEl = resultsSectionEl.find(".result-row").first();

        const detailAuthorEl = detailsSectionEl.find(".detail-author");
        const detailTitleEl = detailsSectionEl.find(".detail-title");
        const detailDatingEl = detailsSectionEl.find(".detail-dating");
        const detailDatingCenturyEl = detailsSectionEl.find(".detail-dating-century");
        const detailAbbrevEl = detailsSectionEl.find(".detail-abbrev");
        const editionNoteEl = detailsSectionEl.find(".detail-edition-note-href");

        detailAuthorEl.text(firstResultRowEl.data("author") ? firstResultRowEl.data("author") : undefinedReplaceString);
        detailTitleEl.text(firstResultRowEl.data("title") ? firstResultRowEl.data("title") : undefinedReplaceString);
        detailDatingEl.text(firstResultRowEl.data("dating") ? firstResultRowEl.data("dating") : undefinedReplaceString);
        detailDatingCenturyEl.text(undefinedReplaceString); //TODO ask where is this info stored
        detailAbbrevEl.text(firstResultRowEl.data("acronym") ? firstResultRowEl.data("acronym") : undefinedReplaceString);

        //Edition note
        const editionNoteAnchor = editionNoteEl;
        const bookId = firstResultRowEl.data("bookid");
        editionNoteAnchor.prop("href", `/EditionNote/EditionNote?bookId=${bookId}`);
    }

    protected fillBookSection(snapshotId: number, totalResults: number) {
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
        this.createBookDetailsSection(bookSectionEl);
    }

    protected sortOrderChanged() {
        //TODO logic
    }

    protected createBookDetailsSection(bookSectionEl: JQuery) {
        const bookDetailEl = bookSectionEl.find(".book-detail-section");
        const docSectionEl = $(`<div class="row result-info-section">
                        <div class="col-sm-2 col-xs-12 label">Doc</div>
                        <div class="col-sm-10 col-xs-12">
                            <div class="corpus-search-detail">
                                <div class="table-row row">
                                    <div class="col-sm-3 col-xs-12 label">Autor</div>
                                    <div class="detail-author col-sm-9 col-xs-12 break-word">&lt;Nezadáno&gt;</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-sm-3 col-xs-12 label">Titul</div>
                                    <div class="detail-title col-sm-9 col-xs-12 break-word">&lt;Nezadáno&gt;</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-sm-3 col-xs-12 label">Datace pramene</div>
                                    <div class="detail-dating col-sm-9 col-xs-12 break-word">&lt;Nezadáno&gt;</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-sm-3 col-xs-12 label">Datace století</div>
                                    <div class="detail-dating-century col-sm-9 col-xs-12 break-word">&lt;Nezadáno&gt;</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-sm-3 col-xs-12 label">Zkratka památky</div>
                                    <div class="detail-abbrev col-sm-9 col-xs-12 break-word">&lt;Nezadáno&gt;</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-xs-12 col-no-padding">
                                        <a class="detail-edition-note-href" href="#">Ediční poznámka</a>
                                    </div>
                                </div>
                            </div>
                        </div></div>`);
        const pholioSection = $(`<div class="row result-info-section">
                        <div class="col-sm-2 col-xs-12 label">Folio</div>
                        <div class="col-sm-10 col-xs-12 col-no-padding">
                            <div class="detail-folio">&lt;Nezadáno&gt;</div>
                            </div>
                        </div>`);
        const verseSection = $(`<div class="row result-info-section">
                        <div class="col-sm-2 col-xs-12 label">Vers</div>
                        <div class="col-sm-10 col-xs-12 col-no-padding">
                            <div class="detail-vers">&lt;Nezadáno&gt;</div>
                            </div>
                        </div>`);
        const bibleVerse = $(`<div class="row result-info-section">
                        <div class="col-sm-2 col-xs-12 label">Bible vers</div>
                        <div class="col-sm-10 col-xs-12">
                                <div class="corpus-search-detail">
                                    <div class="table-row row">
                                        <div class="col-sm-3 col-xs-12 label">Kniha</div>
                                        <div class="detail-bible-vers-book col-sm-9 col-xs-12 break-word">&lt;Nezadáno&gt;</div>
                                    </div>
                                    <div class="table-row row">
                                        <div class="col-sm-3 col-xs-12 label">Kapitola</div>
                                        <div class="detail-bible-vers-chapter col-sm-9 col-xs-12 break-word">&lt;Nezadáno&gt;</div>
                                    </div>
                                    <div class="table-row row">
                                        <div class="col-sm-3 col-xs-12 label">Verš</div>
                                        <div class="detail-bible-vers-vers col-sm-9 col-xs-12 break-word">&lt;Nezadáno&gt;</div>
                                    </div>
                                </div>
                        </div>
                    </div>`);
        const detailStructureContainer = $(`<div class="corpus-search-detail-container"></div>`);
        detailStructureContainer.append(docSectionEl);
        detailStructureContainer.append(pholioSection);
        detailStructureContainer.append(verseSection);
        detailStructureContainer.append(bibleVerse);
        bookDetailEl.append(detailStructureContainer);
    }

    protected showNoPageWarning(pageNumber?: number) {
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
}

class BohemianTextBankSingleLazyload extends BohemianTextBankSingle {
    private lastSnapshotId = -1;

    initSearch() {
        this.lazyloadEventProcess();
        this.init();
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
    }

    private onSearchStartSingleLazyload() {
        $(".results-container").empty();
        const viewingSettingsChangedWarningEl = $(".search-settings-changed-warning");
        viewingSettingsChangedWarningEl.slideUp();
        this.compositionResultListStart = - 1;
        this.lastSnapshotId = - 1;
        this.compositionPageIsLast = false;
    }

    private startBasicSearch(text: string) {
        if (!text) return;
        this.showPageLoading();
        this.onSearchStartSingleLazyload();
        this.corpusBasicSearchBookHits(text);
    }

    private startAdvancedSearch(json: string) {
        if (!json) return;
        this.showPageLoading();
        this.onSearchStartSingleLazyload();
        this.corpusAdvancedSearchBookHits(json);
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

    private corpusAdvancedSearchBookHits(json: string) {
        if (this.compositionResultListStart === -1) {
            this.compositionResultListStart = 0;
        }
        const start = this.compositionResultListStart;
        const count = this.compositionsPerPage;
        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();
        const sortingDirection = sortAsc ? SortDirection.Asc : SortDirection.Desc;

        const payload: ICorpusListPageLookupAdvancedSearch = {
            json: json,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery,
            sortBooksBy: sortingEnum,
            sortDirection: sortingDirection,
            start: start,
            count: count
        };

        updateQueryStringParameter(this.urlSearchKey, json);
        updateQueryStringParameter(this.urlSortAscKey, sortAsc);
        updateQueryStringParameter(this.urlSortCriteriaKey, sortingEnum);
        updateQueryStringParameter(this.urlSelectionKey, this.booksSelector.getSerializedState());

        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetHitBookResultNumbers`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                const totalCount = bookIds.totalCount;
                $("#totalCompositionsCountDiv").text(totalCount);
                if (!totalCount) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                    noResultAlert.addContent("No results");
                    $(".results-container").append(noResultAlert.buildElement());
                    return;
                }
                const page = (start / count) + 1;
                const totalPages = Math.ceil(totalCount / count);
                this.compositionResultListStart += this.compositionsPerPage;
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
                $("#totalCompositionsCountDiv").text(totalCount);
                if (!totalCount) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                    noResultAlert.addContent("No results");
                    $(".results-container").append(noResultAlert.buildElement());
                    return;
                }
                const page = (start / count) + 1;
                const totalPages = Math.ceil(totalCount / count);
                this.compositionResultListStart += this.compositionsPerPage;
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
}

class BohemianTextBankSinglePaged extends BohemianTextBankSingle {
    private paginationInitialised = false;
    private mainPaginator: Pagination;
    initSearch() {
        this.lazyloadEventProcess();
        this.init();
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
    }

    private lazyloadEventProcess() {
        $(".corpus-result-and-settings-row").on("lazybeforeunveil", (event) => {
            const targetEl = $(event.target as Node as Element);
            const snapshotId = parseInt(targetEl.attr("data-snapshotId"));
            const totalResults = parseInt(targetEl.attr("data-result-number"));
            if (!isNaN(snapshotId) && !isNaN(totalResults)) {
                this.fillBookSection(snapshotId, totalResults);
            }
        });
    }
    private onSearchStartSinglePaged() {
        $(".results-container").empty();
        const viewingSettingsChangedWarningEl = $(".search-settings-changed-warning");
        viewingSettingsChangedWarningEl.slideUp();
        this.compositionResultListStart = - 1;
        if (this.mainPaginator) {
            this.mainPaginator = null;
            $(".main-pagination-container").empty();
        }
        this.paginationInitialised = false;
    }

    private startBasicSearch(text: string) {
        if (!text) return;
        this.showPageLoading();
        this.onSearchStartSinglePaged();
        this.corpusBasicSearchBookHits(text, 1);
    }

    private startAdvancedSearch(json: string) {
        if (!json) return;
        this.showPageLoading();
        this.onSearchStartSinglePaged();
        this.corpusAdvancedSearchBookHits(json, 1);
    }

    private createMainPaginator(totalBooks: number) {
        if (this.paginationInitialised) {
            return;
        } else {
            const paginationContainerEl = $(".main-pagination-container");
            const query = this.search.getLastQuery();
            const isAdvancedMode = this.search.isLastQueryJson();
            if (isAdvancedMode) {
                this.mainPaginator = new Pagination({
                    container: paginationContainerEl,
                    pageClickCallback: (pageNumber) => {
                        $(".results-container").empty();
                        this.showPageLoading();
                        this.corpusAdvancedSearchBookHits(query, pageNumber);
                    },
                    showSlider: false,
                    showInput: false,
                    callPageClickCallbackOnInit: false
                });
            } else {
                this.mainPaginator = new Pagination({
                    container: paginationContainerEl,
                    pageClickCallback: (pageNumber) => {
                        $(".results-container").empty();
                        this.showPageLoading();
                        this.corpusBasicSearchBookHits(query, pageNumber);
                    },
                    showSlider: false,
                    showInput: false,
                    callPageClickCallbackOnInit: false
                });
            }
            this.mainPaginator.make(totalBooks, this.compositionsPerPage, 1);
        }
    }

    private corpusAdvancedSearchBookHits(json: string, page: number) {
        if (this.compositionResultListStart === -1) {
            this.compositionResultListStart = 0;
        }
        const count = this.compositionsPerPage;
        const start = (page - 1) * count;
        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();
        const sortingDirection = sortAsc ? SortDirection.Asc : SortDirection.Desc;

        const payload: ICorpusListPageLookupAdvancedSearch = {
            json: json,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery,
            sortBooksBy: sortingEnum,
            sortDirection: sortingDirection,
            start: start,
            count: count
        };

        updateQueryStringParameter(this.urlSearchKey, json);
        updateQueryStringParameter(this.urlSortAscKey, sortAsc);
        updateQueryStringParameter(this.urlSortCriteriaKey, sortingEnum);
        updateQueryStringParameter(this.urlSelectionKey, this.booksSelector.getSerializedState());

        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetHitBookResultNumbers`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                const totalCount = bookIds.totalCount;
                $("#totalCompositionsCountDiv").text(totalCount);
                if (!totalCount) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                    noResultAlert.addContent("No results");
                    $(".results-container").append(noResultAlert.buildElement());
                    return;
                }
                this.createMainPaginator(totalCount);
                this.paginationInitialised = true;
                const totalPages = Math.ceil(totalCount / count);
                const snapshotStructureArray = bookIds.snapshotList;
                var idList: number[] = [];
                snapshotStructureArray.forEach((snapshot) => {
                    idList.push(snapshot.snapshotId);
                    this.generateBookSectionLayout(snapshot.snapshotId, snapshot.resultCount);
                });
                this.hitBookIds = idList;
                if (this.hitBookIds.length < this.compositionsPerPage || page === totalPages) {
                    this.compositionPageIsLast = true;
                }
            }).fail(() => {
                const loaderEl = $(".corpus-search-results-table-div-loader");
                this.printErrorMessage(this.defaultErrorMessage, loaderEl);
            });
    }

    private corpusBasicSearchBookHits(text: string, page : number) {
        if (this.compositionResultListStart === -1) {
            this.compositionResultListStart = 0;
        }
        const count = this.compositionsPerPage;
        const start = (page - 1) * count;
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
                $("#totalCompositionsCountDiv").text(totalCount);
                if (!totalCount) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                    noResultAlert.addContent("No results");
                    $(".results-container").append(noResultAlert.buildElement());
                    return;
                }
                this.createMainPaginator(totalCount);
                this.paginationInitialised = true;
                const totalPages = Math.ceil(totalCount / count);
                this.compositionResultListStart += this.compositionsPerPage;
                const snapshotStructureArray = bookIds.snapshotList;
                var idList: number[] = [];
                snapshotStructureArray.forEach((snapshot) => {
                    idList.push(snapshot.snapshotId);
                    this.generateBookSectionLayout(snapshot.snapshotId, snapshot.resultCount);
                });
                this.hitBookIds = idList;
                if (this.hitBookIds.length < this.compositionsPerPage || page === totalPages) {
                    this.compositionPageIsLast = true;
                }
            }).fail(() => {
                const loaderEl = $(".corpus-search-results-table-div-loader");
                this.printErrorMessage(this.defaultErrorMessage, loaderEl);
            });
    }
}