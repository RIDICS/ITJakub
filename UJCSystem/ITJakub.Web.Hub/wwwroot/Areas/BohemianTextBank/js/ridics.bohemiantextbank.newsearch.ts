function initSearchNew() {
    const searchClass = new BohemianTextBankNew();
    searchClass.initSearch();
}

class BohemianTextBankNew {
    private approximateNumberOfResultsPerPage = 4; //preferred number of results to be displayed in page
    private resultsPerPage = 3; //corresponds to amount of results per page that server can return
    private hitBookIds = [];

    private compositionResultListStart = -1;
    private compositionsPerPage = 10;

    private currentBookId = -1;
    private currentBookIndex = 0;
    private currentResultStart = -1;
    private currentAmountOfResultsInPage = 0;
    private currentViewPage = 1;

    private pageTable = [];

    private contextLength = -1;

    private resultsCountOnPage = 20;

    private paginationMaxVisibleElements = 5;

    private defaultErrorMessage =
        "Vyhledávání se nezdařilo. Ujistěte se, zda máte zadáno alespoň jedno kritérium na vyhledávání v textu.";

    private urlSearchKey = "search";
    private urlStartKey = "start"; //TODO in process of changing to start
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

    private enabledOptions = new Array<SearchTypeEnum>();

    private search: Search;

    initSearch() {

        $(".search-result-next-page").on("click",
            () => {
                this.emptyResultsTable();
                this.currentViewPage++;
                console.log(`Current view page ${this.currentViewPage}`);
                this.loadNextPage();
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

        $("#search-results-div").on("click",
            ".search-result-item",
            (event: JQuery.Event) => {
                var clickedRow = $(event.target as Node as Element).closest(".search-result-item");
                if (clickedRow.hasClass("notes")) {
                    return;
                }

                $(".search-result-item").not(clickedRow).removeClass("clicked");
                clickedRow.addClass("clicked");

                this.printDetailInfo(clickedRow);
            });

        $("#contextPositionsSelect").change(() => {
            //this.searchForBook(this.currentPage);TODO reload page on context change?
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
        //if (paginator) {TODO
        //    paginator.goToPage(1);
        //}
    }

    private showLoading() {
        $("#search-results-div").hide();
        $("#corpus-search-results-table-div-loader").empty();
        $("#corpus-search-results-table-div-loader").show();
        $("#corpus-search-results-table-div-loader").addClass("loader");
    }


    private hideLoading() {
        $("#corpus-search-results-table-div-loader").removeClass("loader");
        $("#corpus-search-results-table-div-loader").hide();
        $("#search-results-div").show();
    }

    private printErrorMessage(message: string) {
        this.hideLoading();
        const corpusErrorDiv = $("#corpus-search-results-table-div-loader");
        corpusErrorDiv.empty();
        corpusErrorDiv.html(message);
        corpusErrorDiv.show();
    }

    private corpusAdvancedSearchPaged(json: string, start: number, contextLength: number, bookId: number) {
        if (!json) return;

        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();

        this.showLoading();

        const payload: JQuery.PlainObject = {
            json: json,
            start: start,
            count: this.resultsPerPage,
            contextLength: contextLength,
            sortingEnum: sortingEnum,
            sortAsc: sortAsc,
            bookId: bookId,
            selectedCategoryIds: this.categoryIdsInQuery
        };

        const advancedSearchPageAjax =
            $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchCorpusGetPage`, payload);

        advancedSearchPageAjax.done((response) => {
            this.hideLoading();
            const results: ICorpusSearchResult[] = response["results"];
            const numberOfResults = results.length;
            if (numberOfResults === 0) {
                this.switchToNextBook();
                return;
            }
            console.log(`Results: ${numberOfResults}`);
            console.log(`Book: ${bookId}`);
            console.log(`Start: ${start}`);
            this.currentResultStart += this.resultsPerPage;
            this.currentAmountOfResultsInPage += numberOfResults;
            this.fillResultTable(results);
            updateQueryStringParameter(this.urlSearchKey, json);
            //updateQueryStringParameter(this.urlPageKey, pageNumber);TODO change to start
            updateQueryStringParameter(this.urlSortAscKey, this.sortBar.isSortedAsc());
            updateQueryStringParameter(this.urlSortCriteriaKey, this.sortBar.getSortCriteria());
            updateQueryStringParameter(this.urlContextSizeKey, contextLength);
            this.optionallyLoadMoreResultsForPage();
        });
        advancedSearchPageAjax.fail(() => {
            this.printErrorMessage(this.defaultErrorMessage);
        });
    }

    private fillResultTable(results: ICorpusSearchResult[]) {
        const tableBody = $("#search-results-div");
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

            const tableRow = $(`<div class="search-result-item list-group-item row"></div>`);
            const abbrevColumn = $(`<div class="col-xs-2"></div>`);
            const resultColumn = $(`<div class="col-xs-10 text-center"></div>`);
            const notesSpacerEl = $(`<div class="col-xs-12 notes spacer list-group-item row"></div>`);
            const notesEl = $(`<div class="col-xs-12 notes list-group-item row"></div>`);

            tableRow.attr("data-bookId", bookId);
            tableRow.attr("data-author", result.author);
            tableRow.attr("data-title", result.title);
            tableRow.attr("data-dating", result.originDate);
            tableRow.attr("data-pageId", pageId);
            tableRow.attr("data-pageName", pageContext.name);
            tableRow.attr("data-acronym", acronym);

            if (verseContext) {
                tableRow.attr("data-verseXmlId", verseContext.verseXmlId);
                tableRow.attr("data-verseName", verseContext.verseName);
            }

            if (bibleVerseContext) {
                tableRow.attr("data-bibleBook", bibleVerseContext.bibleBook);
                tableRow.attr("data-bibleChapter", bibleVerseContext.bibleChapter);
                tableRow.attr("data-bibleVerse", bibleVerseContext.bibleVerse);
            }

            const contextBefore = $(`<span class="context-before"></span>`);
            contextBefore.text(contextStructure.before);

            const contextMatch = $(`<span class="match"></span>`);
            contextMatch.text(contextStructure.match);

            const contextAfter = $(`<span class="context-after"></span>`);
            contextAfter.text(contextStructure.after);

            const abbrevHref = $("<a></a>");
            abbrevHref.prop("href",
                `${getBaseUrl()}Editions/Editions/Listing?bookId=${bookId}&searchText=${this.search.getLastQuery()
                }&page=${pageId}`);
            abbrevHref.text(acronym);
            abbrevColumn.append(abbrevHref);
            
            resultColumn.append(contextBefore);
            resultColumn.append(contextMatch);
            resultColumn.append(contextAfter);

            tableRow.append(abbrevColumn);
            tableRow.append(resultColumn);

            if (notes) {
                for (var j = 0; j < notes.length; j++) {
                    var noteSpan = $(`<span class="note">${notes[j]}</span>`);
                    notesEl.append(noteSpan);
                }
                tableRow.append(notesSpacerEl);//TODO finish note placement
                tableRow.append(notesEl);
                tableRow.append(notesSpacerEl);
            }

            tableBody.append(tableRow);

        }
    }

    private corpusBasicSearchPaged(text: string, start: number, contextLength: number, bookId: number) {
        if (!text) return;
        const sortingEnum = this.sortBar.getSortCriteria();
        const sortAsc = this.sortBar.isSortedAsc();

        this.showLoading();

        const payload: JQuery.PlainObject = {
            text: text,
            start: start,
            count: this.resultsPerPage,
            contextLength: contextLength,
            sortingEnum: sortingEnum,
            sortAsc: sortAsc,
            bookId: bookId
        };

        const getPageAjax = $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/TextSearchFulltextGetBookPage`,
            payload);

        getPageAjax.done((response) => {
            this.hideLoading();
            const results: ICorpusSearchResult[] = response["results"];
            const numberOfResults = results.length;
            if (numberOfResults === 0) {
                this.switchToNextBook();
                return;
            }
            console.log(`Results: ${numberOfResults}`);
            console.log(`Book: ${bookId}`);
            console.log(`Start: ${start}`);
            console.log(`Count: ${this.resultsPerPage}`);
            this.currentResultStart += this.resultsPerPage;
            this.currentAmountOfResultsInPage += numberOfResults;
            this.fillResultTable(results);
            updateQueryStringParameter(this.urlSearchKey, text);
            updateQueryStringParameter(this.currentResultStart, start);
            updateQueryStringParameter(this.urlSortAscKey, sortAsc);
            updateQueryStringParameter(this.urlSortCriteriaKey, sortingEnum);
            updateQueryStringParameter(this.urlContextSizeKey, contextLength);
            this.optionallyLoadMoreResultsForPage();
        });

        getPageAjax.fail(() => {
            this.printErrorMessage(this.defaultErrorMessage);
        });
    }

    private optionallyLoadMoreResultsForPage() {
        if (this.currentAmountOfResultsInPage < this.approximateNumberOfResultsPerPage) {
            this.loadBookResultPage(this.currentResultStart, this.currentBookId);
        } else {
            this.currentAmountOfResultsInPage = 0;
        }
    }

    private switchToNextBook() {
        this.currentResultStart = 0;//internal list index reset
        this.currentBookIndex++;//external list index shift
        if (this.currentBookIndex > (this.hitBookIds.length - 1)) {
            const search = getQueryStringParameterByName(this.urlSearchKey);
            if (this.search.isLastQueryJson()) {
                this.loadNextCompositionAdvancedResultPage(search);
            } else {
                this.loadNextCompositionResultPage(search);
            }
            return;
        }
        this.currentBookId = this.hitBookIds[this.currentBookIndex];
        this.loadBookResultPage(this.currentResultStart, this.currentBookId);
    }

    private loadBookResultPage(start: number, bookId: number) {
        console.log("load");
        const contextLength = parseInt($("#contextPositionsSelect").val() as string);
        if (this.search.isLastQueryJson()) {
            this.corpusAdvancedSearchPaged(this.search.getLastQuery(), start, contextLength, bookId);
        } else {
            this.corpusBasicSearchPaged(this.search.getLastQuery(), start, contextLength, bookId);
        }
    }

    private corpusBasicSearchBookHits(text: string) {
        if (!text) return;
        const nextPageEl = $(".search-result-next-page");
        nextPageEl.prop("disabled", false);
        this.compositionResultListStart = -1;
        this.emptyResultsTable();
        this.resetHistory();
        this.actualizeSelectedBooksAndCategoriesInQuery();

        this.showLoading();

        this.loadNextCompositionResultPage(text);
    }

    private emptyResultsTable() {
        const tableBody = $("#search-results-div");
        tableBody.empty();
    }

    /**
     * Generates viewing page
     */
    private loadNextPage() {
        if (this.currentBookId === -1) {
            this.currentBookId = this.hitBookIds[this.currentBookIndex];
        }
        if (this.currentResultStart === -1) {
            this.currentResultStart = 0;
        }
        this.loadBookResultPage(this.currentResultStart, this.currentBookId);
    }

    private resetHistory() {
        this.currentViewPage = 1;
        //TODO reset generated history here
    }

    private makePageTableList(bookId: number, page: number, viewingPage: number) {
        this.pageTable = [];
        //TODO implement logic
    }

    private corpusAdvancedSearchBookHits(json: string) {
        if (!json) return;
        const nextPageEl = $(".search-result-next-page");
        nextPageEl.prop("disabled", false);
        this.compositionResultListStart = -1;
        this.emptyResultsTable();
        this.resetHistory();

        this.actualizeSelectedBooksAndCategoriesInQuery();
        this.showLoading();

        this.loadNextCompositionAdvancedResultPage(json);
    }

    /**
    * Pagination of external list - list of compositions with hits, basic search
    * @param text
    */
    private loadNextCompositionResultPage(text: string) {
        if (this.compositionResultListStart === -1) {
            this.compositionResultListStart = 0;
        }
        const payload: JQuery.PlainObject = {
            text: text,
            start: this.compositionResultListStart,
            count: this.compositionsPerPage,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery
        };

        console.log(`External composition list start: ${this.compositionResultListStart}`);
        console.log(`External composition list count: ${this.compositionsPerPage}`);

        $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetHitBookIdsPaged`, payload)
            .done((bookIds: IPagedResultArray<number>) => {
                console.log(bookIds);
                const count = bookIds.totalCount;
                $("#totalResultCountDiv").text(count);
                this.hitBookIds = bookIds.list;
                if (!this.hitBookIds) {
                    bootbox.alert({
                        title: "Attention",
                        message: "No more pages",
                        buttons: {
                            ok: {
                                className: "btn-default"
                            }
                        }
                    });
                    $(".search-result-next-page").prop("disabled", true);
                } else {
                    this.currentBookId = -1;//reset book id to get new
                    this.currentBookIndex = 0;//reset book index as book array is new
                    this.compositionResultListStart += this.compositionsPerPage;
                    this.loadNextPage();
                    updateQueryStringParameter(this.urlSearchKey, text);
                    updateQueryStringParameter(this.urlSelectionKey, this.booksSelector.getSerializedState());
                    updateQueryStringParameter(this.urlSortAscKey, this.sortBar.isSortedAsc());
                    updateQueryStringParameter(this.urlSortCriteriaKey, this.sortBar.getSortCriteria());
                }
            }).fail(() => {
                this.printErrorMessage(this.defaultErrorMessage);
            });
    }
    /**
     * Pagination of external list - list of compositions with hits, advanced search
     * @param json Json request with search request
     */
    private loadNextCompositionAdvancedResultPage(json: string) {
        if (this.compositionResultListStart === -1) {
            this.compositionResultListStart = 0;
        }
        const payload: JQuery.PlainObject = {
            json: json,
            start: this.compositionResultListStart,
            count: this.compositionsPerPage,
            selectedBookIds: this.bookIdsInQuery,
            selectedCategoryIds: this.categoryIdsInQuery
        };

        $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/AdvancedSearchGetHitBookIdsPaged`, payload)
            .done((bookIds: IPagedResultArray<number>) => {
                console.log(`Composition list start: ${this.compositionResultListStart}`);
                console.log(`Composition list count: ${this.compositionsPerPage}`);
                console.log(`Book ids in page: ${bookIds}`);
                const count = bookIds.totalCount;
                $("#totalResultCountDiv").text(count);
                this.hitBookIds = bookIds.list;
                if (!this.hitBookIds) {
                    bootbox.alert({
                        title: "Attention",
                        message: "No more pages",
                        buttons: {
                            ok: {
                                className: "btn-default"
                            }
                        }
                    });
                    $(".search-result-next-page").prop("disabled", true);
                } else {
                    this.currentBookId = -1;//reset book id to get new
                    this.currentBookIndex = 0;//reset book index as book array is new
                    this.compositionResultListStart += this.compositionsPerPage;
                    this.loadNextPage();
                    //updateQueryStringParameter(this.urlSearchKey, text);TODO
                    updateQueryStringParameter(this.urlSelectionKey, this.booksSelector.getSerializedState());
                    updateQueryStringParameter(this.urlSortAscKey, this.sortBar.isSortedAsc());
                    updateQueryStringParameter(this.urlSortCriteriaKey, this.sortBar.getSortCriteria());
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

    private calculateNumberOfPages() { //TODO not used yet
        var pageStructure = [];
        const hitBookIdsAjax = $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetHitBookIds`,
            { text: "", selectedBookIds: null, selectedCategoryIds: null } as JQuery.PlainObject);
        hitBookIdsAjax.done((bookIds: number[]) => {
            bookIds.forEach((bookId: number) => {
                const bookHitPagesAjax =
                    $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetNumberOfPagesByBook`,
                        { bookId: bookId } as JQuery.PlainObject);
                bookHitPagesAjax.done((pages: number[]) => {
                    var transientResults = [];
                    var numberOfTransientPages = 0;
                    pages.forEach((page: number) => {
                        const numberOfHitsPerPageAjax =
                            $.get(`${getBaseUrl()}BohemianTextBank/BohemianTextBank/GetResultNumberByBookByPage`,
                                { bookId: bookId } as JQuery.PlainObject);
                        numberOfHitsPerPageAjax.done((resultNumber: number) => {
                            if (resultNumber < (this.approximateNumberOfResultsPerPage - 5)
                            ) { //if page has less than minimum results, hold in temporary array
                                transientResults.push({ bookId: bookId, page: page });
                                numberOfTransientPages += resultNumber;
                            } else { //more than minimum pages
                                if (transientResults) {
                                    transientResults.push({
                                        bookId: bookId,
                                        page: page
                                    }); //if more than min number of pages and temp array exist, flush them
                                    pageStructure.push(transientResults);
                                    transientResults.length = 0;
                                    numberOfTransientPages = 0;
                                } else { //more than min number of pages, no temp array
                                    pageStructure.push([{ bookId: bookId, page: page }]);
                                }
                            }
                            if (numberOfTransientPages >= (this.approximateNumberOfResultsPerPage - 5)
                            ) { //no page with more than min number of hits, but temp array is too big
                                pageStructure.push(transientResults);
                                transientResults.length = 0;
                                numberOfTransientPages = 0;
                            }
                        }).fail(() => {
                            //TODO
                        });
                    });
                }).fail(() => {
                    //TODO
                });
            });
        }).fail(() => {
            //TODO
        });
    }
}