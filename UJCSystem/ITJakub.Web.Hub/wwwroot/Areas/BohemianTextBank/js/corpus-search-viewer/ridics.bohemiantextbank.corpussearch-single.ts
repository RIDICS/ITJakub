function initSearchSingle() {
    const searchSingle = new BohemianTextBankSingle();
    searchSingle.initSearch();
}

class BohemianTextBankSingle extends BohemianTextBankBase {

    initSearch() {
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

    private unveilBook(bookContainerEl: JQuery) {//TODO lazyload books logic
        const snapshotId = 1;//TODO debug
        const resultsRowEl = $(`<div class="row"></div>`);
        resultsRowEl.append(`<div class="col-xs-5"></div><div class="col-xs-7"></div>`);
        const paginationRowEl = $(`<div class="row"></div>`);
        const paginationContainerEl = $(`<div class="col-xs-12"></div>`);
        paginationRowEl.append(paginationContainerEl);
        const paginator = new Pagination({
            container: paginationContainerEl,
            pageClickCallback: (pageNumber) => {
                this.goToResultPage.bind(this, pageNumber, snapshotId);
            },
            showSlider: true,
            showInput: true
        });
        bookContainerEl.append(resultsRowEl).append(paginationRowEl);
    }

    private goToResultPage(pageNumber: number, snapshotId: number) {//TODO logic
        const start = (pageNumber - 1) * this.searchResultsOnPage;
        const count = this.searchResultsOnPage;
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

    private corpusBasicSearchBookHits(text: string) {
        if (!text) return;
        //this.onSearchStart();
        //this.loadNextCompositionResultPage(text);TODO logic
    }

    private corpusAdvancedSearchBookHits(json: string) {
        if (!json) return;
        //this.onSearchStart();
        //this.loadNextCompositionAdvancedResultPage(json);TODO logic
    }
}