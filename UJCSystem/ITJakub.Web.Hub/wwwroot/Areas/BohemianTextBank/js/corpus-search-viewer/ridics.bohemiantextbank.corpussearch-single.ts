function initSearchSingleLazyload() {
    const searchSingleLazyload = new BohemianTextBankSingleLazyload();
    searchSingleLazyload.initSearch();
}

function initSearchSinglePaged() {
    const searchSinglePaged = new BohemianTextBankSinglePaged();
    searchSinglePaged.initSearch();
}

class BohemianTextBankSingle extends BohemianTextBankBase {
    protected bootstrapXsScreenWidth = 768;

    //string for localisation
    protected errorDisplayingResultsMessage = "Chyba při zobrazení výsledků";
    protected noResultsMessage = "Žadné výsledky";
    protected docString = "Doc";
    protected authorString = "Autor";
    protected titleString = "Titul";
    protected sourceDateString = "Datace pramene";
    protected sourceDateCenturyString = "Datace století";
    protected sourceAbbreviationString = "Zkratka památky";
    protected editionNoteString = "Ediční poznámka";
    protected pholioString = "Folio";
    protected verseString = "Verš";
    protected bibleVerseString = "Bible verš";
    protected bookString = "Book";
    protected chapterString = "Kapitola";
    /**
     * Initialises event listeners, sets initial parameters, creates gui elements
     */
    protected init() {

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

        this.stickPanelToTop();

        contextLengthInputEl.on("change", () => {
            if (this.atLeastOnSearchDone) {
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
    /**
     * Executes page generation and loading logic after paginator selected a page
     * @param pageNumber Page number
     * @param snapshotId Book identifier
     * @param query Search query
     * @param contextSize Size of corpus context
     */
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
    /**
     * Shows page loading animation
     */
    protected showPageLoading() {
        const pageLoaderEl = $(".page-loader");
        pageLoaderEl.removeClass("alert alert-info").addClass("loader");
        pageLoaderEl.show();
    }
    /**
     * Hides page loading animation
     */
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
    /**
     * Displays info in a book details section about specific search result
     * @param tableRowEl JQuery object containing search result
     * @param detailSectionEl JQuery object containing book details section
     * @param query Search query
     */
    private printDetailInfo(tableRowEl: JQuery, detailSectionEl: JQuery, query: string) {
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
    /**
 * Get results in a book containing search results of basic search
 * @param text Search query
 * @param start Start of results
 * @param count Amount of results to load
 * @param contextSize Size of corpus context
 * @param snapshotId Book identifier
 */
    protected loadResultPageBasic(text: string, start: number, count: number, contextSize: number, snapshotId: number) {
        const url = `${getBaseUrl()}BohemianTextBank/BohemianTextBank/BasicCorpusSearchGetResultsPage`;
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
            if (!bookSectionEl.length) {
                this.hidePageLoading();
                const noResultAlert = new AlertComponentBuilder(AlertType.Error);
                noResultAlert.addContent(this.errorDisplayingResultsMessage);
                $(".results-container").empty().append(noResultAlert.buildElement());
                return;
            }
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

    protected emptyResultsTable(bookSectionEl : JQuery) {
        const tableEl = bookSectionEl.find(".text-results-table-body");
        tableEl.empty();
    }
    /**
     * Get results in a book containing advanced search results
     * @param json JSON containing search query
     * @param start Start of results
     * @param count Amount of results to load
     * @param contextSize Size of corpus context
     * @param snapshotId Book identifier
     */
    protected loadResultPageAdvanced(json: string,
        start: number,
        count: number,
        contextSize: number,
        snapshotId: number) {
        const url = `${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedCorpusSearchGetResultsPage`;
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
            if (!bookSectionEl.length) {
                this.hidePageLoading();
                const noResultAlert = new AlertComponentBuilder(AlertType.Error);
                noResultAlert.addContent(this.errorDisplayingResultsMessage);
                $(".results-container").empty().append(noResultAlert.buildElement());
                return;
            }
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
    /**
     * Dynamically generation details section and result section for a book
     * @param snapshotId Book identifier
     * @param resultNumber Amount of search results in the book
     */
    protected generateBookSectionLayout(snapshotId: number, resultNumber: number) {
        const bookSectionsContainer = $(".results-container");
        const bookRowEl = $(`<div class="book-section-row row lazyload" data-snapshotId="${snapshotId}" data-result-number="${resultNumber}"></div>`);
        const bookDetailSection = $(`<div class="col-xs-4 col-md-6 book-detail-section"></div>`);
        const searchResultsSection = $(`<div class="col-xs-8 col-md-6 result-text-col text-center"></div>`);
        const tableEl = $(`<div class="corpus-search-results-table-div-loader"></div><table class="text-results-table"><tbody class="text-results-table-body"></tbody></table>`);
        const searchResultsPaginationSection = $(`<div class="col-xs-12 pagination-section text-center"></div>`);
        searchResultsSection.append(tableEl);
        bookRowEl.append(bookDetailSection).append(searchResultsSection).append(searchResultsPaginationSection);
        bookSectionsContainer.append(bookRowEl);
        this.hidePageLoading();
    }
    /**
     * Fill details section with static info - book details 
     * @param bookSectionEl Details section of book section
     */
    protected fillBookDetailsIntoDetailsSection(bookSectionEl: JQuery) {
        const resultsSectionEl = bookSectionEl.find(".result-text-col");
        const detailsSectionEl = bookSectionEl.find(".book-detail-section");
        const firstResultRowEl = resultsSectionEl.find(".result-row").first();

        const detailAuthorEl = detailsSectionEl.find(".detail-author");
        const detailTitleEl = detailsSectionEl.find(".detail-title");
        const detailDatingEl = detailsSectionEl.find(".detail-dating");
        const detailDatingCenturyEl = detailsSectionEl.find(".detail-dating-century");
        const detailAbbrevEl = detailsSectionEl.find(".detail-abbrev");
        const editionNoteEl = detailsSectionEl.find(".detail-edition-note-href");

        detailAuthorEl.text(firstResultRowEl.data("author") ? firstResultRowEl.data("author") : this.undefinedReplaceString);
        detailTitleEl.text(firstResultRowEl.data("title") ? firstResultRowEl.data("title") : this.undefinedReplaceString);
        detailDatingEl.text(firstResultRowEl.data("dating") ? firstResultRowEl.data("dating") : this.undefinedReplaceString);
        detailDatingCenturyEl.text(this.undefinedReplaceString); //TODO ask where is this info stored
        detailAbbrevEl.text(firstResultRowEl.data("acronym") ? firstResultRowEl.data("acronym") : this.undefinedReplaceString);

        //Edition note
        const editionNoteAnchor = editionNoteEl;
        const bookId = firstResultRowEl.data("bookid");
        editionNoteAnchor.prop("href", `/EditionNote/EditionNote?bookId=${bookId}`);
    }
    /**
     * Create paginator and details section for each book
     * @param snapshotId Book identifier
     * @param totalResults Amount of search results
     */
    protected fillBookSection(snapshotId: number, totalResults: number) {
        const bookSectionEl = $(`*[data-snapshotId=${snapshotId}]`);
        const paginationEl = bookSectionEl.find(".pagination-section");
        const paginator = new Pagination({
            container: paginationEl,
            pageClickCallback: (pageNumber) => {
                this.goToResultPage(pageNumber, snapshotId, this.search.getLastQuery(), this.contextLength);
            },
            showSlider: false,
            showInput: false,
            callPageClickCallbackOnInit: true
        });
        paginator.make(totalResults, this.searchResultsOnPage, 1);
        this.createBookDetailsSection(bookSectionEl);
    }

    /**
     * Creates book details section structure
     * @param bookSectionEl JQuery container for details section
     */
    protected createBookDetailsSection(bookSectionEl: JQuery) {
        const bookDetailEl = bookSectionEl.find(".book-detail-section");
        const docSectionEl = $(`<div class="row result-info-section">
                        <div class="col-md-2 col-xs-12 label">${this.docString}</div>
                        <div class="col-md-10 col-xs-12">
                            <div class="corpus-search-detail">
                                <div class="table-row row">
                                    <div class="col-md-4 col-xs-12 label">${this.authorString}</div>
                                    <div class="detail-author col-md-8 col-xs-12 break-word">${this.undefinedReplaceString}</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-md-4 col-xs-12 label">${this.titleString}</div>
                                    <div class="detail-title col-md-8 col-xs-12 break-word">${this.undefinedReplaceString}</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-md-4 col-xs-12 label">${this.sourceDateString}</div>
                                    <div class="detail-dating col-md-8 col-xs-12 break-word">${this.undefinedReplaceString}</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-md-4 col-xs-12 label">${this.sourceDateCenturyString}</div>
                                    <div class="detail-dating-century col-md-8 col-xs-12 break-word">${this.undefinedReplaceString}</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-md-4 col-xs-12 label">${this.sourceAbbreviationString}</div>
                                    <div class="detail-abbrev col-md-8 col-xs-12 break-word">${this.undefinedReplaceString}</div>
                                </div>
                                <div class="table-row row">
                                    <div class="col-xs-12 col-no-padding">
                                        <a class="detail-edition-note-href" href="#">${this.editionNoteString}</a>
                                    </div>
                                </div>
                            </div>
                        </div></div>`);
        const pholioSection = $(`<div class="row result-info-section">
                        <div class="col-md-2 col-xs-12 label">${this.pholioString}</div>
                        <div class="col-md-10 col-xs-12 col-no-padding">
                            <div class="detail-folio">${this.undefinedReplaceString}</div>
                            </div>
                        </div>`);
        const verseSection = $(`<div class="row result-info-section">
                        <div class="col-md-2 col-xs-12 label">${this.verseString}</div>
                        <div class="col-md-10 col-xs-12 col-no-padding">
                            <div class="detail-vers">${this.undefinedReplaceString}</div>
                            </div>
                        </div>`);
        const bibleVerse = $(`<div class="row result-info-section">
                        <div class="col-md-2 col-xs-12 label">${this.bibleVerseString}</div>
                        <div class="col-md-10 col-xs-12">
                                <div class="corpus-search-detail">
                                    <div class="table-row row">
                                        <div class="col-md-4 col-xs-12 label">${this.bookString}</div>
                                        <div class="detail-bible-vers-book col-md-8 col-xs-12 break-word">${this.undefinedReplaceString}</div>
                                    </div>
                                    <div class="table-row row">
                                        <div class="col-md-4 col-xs-12 label">${this.chapterString}</div>
                                        <div class="detail-bible-vers-chapter col-md-8 col-xs-12 break-word">${this.undefinedReplaceString}</div>
                                    </div>
                                    <div class="table-row row">
                                        <div class="col-md-4 col-xs-12 label">${this.verseString}</div>
                                        <div class="detail-bible-vers-vers col-md-8 col-xs-12 break-word">${this.undefinedReplaceString}</div>
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

    protected stickPanelToTop() {
        const pageEl = $(".page-body-content");
        const panelSelector = ".corpus-search-result-controls-container";
        const panelEl = $(panelSelector);
        var stickyBits: any;
        const pageOffset = pageEl.offset().top;
        const checkWidth = () => {
            const pageWidth = pageEl.width();
            if (pageWidth >= this.bootstrapXsScreenWidth) {
                stickyBits = stickybits(panelSelector, { stickyBitStickyOffset: pageOffset });
            } else {
                if (stickyBits) {
                    stickyBits.cleanup();
                    panelEl.css("position", "");
                    panelEl.css("top", "");
                }
            }
        };
        checkWidth();
        $(window as any).on("resize", ()=>{
            checkWidth();
        });
    }
}
//Unpaged lazyloaded search, each book gets a section
class BohemianTextBankSingleLazyload extends BohemianTextBankSingle {
    private lastSnapshotId = -1;
    /**
     * Initial page preparation for searching
     */
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

        $(".results-refresh-button").on("click", () => {
            const query = this.search.getLastQuery();
            if (query) {
                const advancedMode = this.search.isLastQueryJson();
                if (advancedMode) {
                    this.startAdvancedSearch(query);
                } else {
                    this.startBasicSearch(query);
                }
            }
        });

        const sortBarContainer = "#listResultsHeader";
        const sortBarContainerEl = $(sortBarContainer);
        sortBarContainerEl.empty();
        this.sortBar = new SortBar(this.sortOrderChanged.bind(this));
        const sortBarHtml = this.sortBar.makeSortBar(sortBarContainer);
        sortBarContainerEl.append(sortBarHtml);
    }
    /**
     * Returns page to initial state
     */
    private onSearchStartSingleLazyload() {
        this.atLeastOnSearchDone = true;
        $(".results-container").empty();
        const viewingSettingsChangedRefreshEl = $(".search-settings-changed-refresh");
        viewingSettingsChangedRefreshEl.slideUp();
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

    private sortOrderChanged() {
        const query = this.search.getLastQuery();
        if(!query) {
            return;
        }
        const advancedMode = this.search.isLastQueryJson();
        $(".results-container").empty();
        this.showPageLoading();
        if (advancedMode) {
            this.corpusAdvancedSearchBookHits(query);
        } else {
            this.corpusBasicSearchBookHits(query);
        }
    }
    /**
     * Executes book section generation on appear in viewport
     */
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
    /**
     * Loads books' identifiers that correspond to search parameters
     * @param json Advanced search json query 
     */
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

        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetResultSnapshotListPageOfIdsWithResultNumbers`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                const totalCount = bookIds.totalCount;
                const snapshotStructureArray = bookIds.snapshotList;
                $("#totalCompositionsCountDiv").text(totalCount);
                if (!totalCount) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                    noResultAlert.addContent(this.noResultsMessage);
                    $(".results-container").empty().append(noResultAlert.buildElement());
                    return;
                }
                if (!snapshotStructureArray.length) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Error);
                    noResultAlert.addContent(this.defaultErrorMessage);
                    $(".results-container").empty().append(noResultAlert.buildElement());
                    return;
                }
                const page = (start / count) + 1;
                const totalPages = Math.ceil(totalCount / count);
                this.compositionResultListStart += this.compositionsPerPage;
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
                this.hidePageLoading();
                const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                noResultAlert.addContent(this.defaultErrorMessage);
                $(".results-container").append(noResultAlert.buildElement());
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

        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/BasicSearchGetResultSnapshotListPageOfIdsWithResultNumbers`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                const totalCount = bookIds.totalCount;
                const snapshotStructureArray = bookIds.snapshotList;
                $("#totalCompositionsCountDiv").text(totalCount);
                if (!totalCount) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                    noResultAlert.addContent(this.noResultsMessage);
                    $(".results-container").empty().append(noResultAlert.buildElement());
                    return;
                }
                if (!snapshotStructureArray.length) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Error);
                    noResultAlert.addContent(this.defaultErrorMessage);
                    $(".results-container").empty().append(noResultAlert.buildElement());
                    return;
                }
                const page = (start / count) + 1;
                const totalPages = Math.ceil(totalCount / count);
                this.compositionResultListStart += this.compositionsPerPage;
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
                this.hidePageLoading();
                const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                noResultAlert.addContent(this.defaultErrorMessage);
                $(".results-container").empty().append(noResultAlert.buildElement());
            });
    }
}
//Paged search, each book gets a section
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

        $(".results-refresh-button").on("click", () => {
            const query = this.search.getLastQuery();
            if (query) {
                const advancedMode = this.search.isLastQueryJson();
                if (advancedMode) {
                    this.startAdvancedSearch(query);
                } else {
                    this.startBasicSearch(query);
                }
            }
        });

        const sortBarContainer = "#listResultsHeader";
        const sortBarContainerEl = $(sortBarContainer);
        sortBarContainerEl.empty();
        this.sortBar = new SortBar(this.sortOrderChanged.bind(this));
        const sortBarHtml = this.sortBar.makeSortBar(sortBarContainer);
        sortBarContainerEl.append(sortBarHtml);
    }
    /**
     * Book section filling logic on appear in the viewport
     */
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
    /**
     * Resets page to initial state
     */
    private onSearchStartSinglePaged() {
        this.atLeastOnSearchDone = true;
        $(".results-container").empty();
        const viewingSettingsChangedRefreshEl = $(".search-settings-changed-refresh");
        viewingSettingsChangedRefreshEl.slideUp();
        this.compositionResultListStart = - 1;
        if (this.mainPaginator) {
            this.mainPaginator = null;
            $(".main-pagination-container").empty();
        }
        this.paginationInitialised = false;
        this.actualizeSelectedBooksAndCategoriesInQuery();
    }

    /**
     * Initialises parameters for basic search
     * @param text Search query text
     */
    private startBasicSearch(text: string) {
        if (!text) return;
        const firstPage = 1;
        this.showPageLoading();
        this.onSearchStartSinglePaged();
        this.corpusBasicSearchBookHits(text, firstPage);
    }
    /**
     * Initialises parameters for advanced search
     * @param json Json containing search query
     */
    private startAdvancedSearch(json: string) {
        if (!json) return;
        const firstPage = 1;
        this.showPageLoading();
        this.onSearchStartSinglePaged();
        this.corpusAdvancedSearchBookHits(json, firstPage);
    }

    private actualizeSelectedBooksAndCategoriesInQuery() {
        const selectedIds = this.booksSelector.getSelectedIds();
        this.bookIdsInQuery = selectedIds.selectedBookIds;
        this.categoryIdsInQuery = selectedIds.selectedCategoryIds;
    }

    /**
     * Reloads search results on change of sort ordering
     */
    private sortOrderChanged() {
        const query = this.search.getLastQuery();
        const firstPage = 1;
        const advancedMode = this.search.isLastQueryJson();
        $(".results-container").empty();
        this.showPageLoading();
        if (advancedMode) {
            this.corpusAdvancedSearchBookHits(query, firstPage);
        } else {
            this.corpusBasicSearchBookHits(query, firstPage);
        }
    }
    /**
     * Creates main paginator to navigate through pages containing multiple books
     * @param totalBooks Total number of books with search results
     */
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
    /**
     * Get one page with book identifiers
     * @param json Search query of advanced search
     * @param page Page containing book identifiers
     */
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

        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetResultSnapshotListPageOfIdsWithResultNumbers`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                const totalCount = bookIds.totalCount;
                const snapshotStructureArray = bookIds.snapshotList;
                $("#totalCompositionsCountDiv").text(totalCount);
                if (!totalCount) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                    noResultAlert.addContent(this.noResultsMessage);
                    $(".results-container").empty().append(noResultAlert.buildElement());
                    return;
                }
                if (!snapshotStructureArray.length) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Error);
                    noResultAlert.addContent(this.defaultErrorMessage);
                    $(".results-container").empty().append(noResultAlert.buildElement());
                    return;
                }
                this.createMainPaginator(totalCount);
                this.paginationInitialised = true;
                const totalPages = Math.ceil(totalCount / count);
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
                this.hidePageLoading();
                const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                noResultAlert.addContent(this.defaultErrorMessage);
                $(".results-container").empty().append(noResultAlert.buildElement());
            });
    }

    /**
    * Get one page with book identifiers
    * @param text Search query of basic search
    * @param page Page containing book identifiers
    */
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

        $.post(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/BasicSearchGetResultSnapshotListPageOfIdsWithResultNumbers`, payload)
            .done((bookIds: ICoprusSearchSnapshotResult) => {
                const totalCount = bookIds.totalCount;
                const snapshotStructureArray = bookIds.snapshotList;
                $("#totalCompositionsCountDiv").text(totalCount);
                if (!totalCount) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                    noResultAlert.addContent(this.noResultsMessage);
                    $(".results-container").empty().append(noResultAlert.buildElement());
                    return;
                }
                if (!snapshotStructureArray.length) {
                    this.hidePageLoading();
                    const noResultAlert = new AlertComponentBuilder(AlertType.Error);
                    noResultAlert.addContent(this.defaultErrorMessage);
                    $(".results-container").empty().append(noResultAlert.buildElement());
                    return;
                }
                this.createMainPaginator(totalCount);
                this.paginationInitialised = true;
                const totalPages = Math.ceil(totalCount / count);
                this.compositionResultListStart += this.compositionsPerPage;
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
                this.hidePageLoading();
                const noResultAlert = new AlertComponentBuilder(AlertType.Info);
                noResultAlert.addContent(this.defaultErrorMessage);
                $(".results-container").empty().append(noResultAlert.buildElement());
            });
    }
}