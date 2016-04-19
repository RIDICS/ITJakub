class ListModulInicializator extends ModulInicializator {
    protected bibliographyModule: BibliographyModule;
    protected search: Search;
    protected searchBox: SearchBox;
    protected dropDownSelect: DropDownSelect2;

    protected bookIdsInQuery = new Array();
    protected categoryIdsInQuery = new Array();

    protected selectedBookIds = new Array();
    protected selectedCategoryIds = new Array();

    protected initPage: number = null;
    protected booksCountOnPage = 5;

    protected readyForInit = false;
    protected notInitialized = true;

    protected configuration: IModulInicializatorConfiguration;
    protected defaultConfiguration = {
        base: {
            url: {
                searchKey: "search",
                pageKey: "page",
                selectionKey: "selected",
                sortAscKey: "sortAsc",
                sortCriteriaKey: "sortCriteria"
            }
        },
        bibliographyModule: {
            resultsContainer: "#listResults",
            sortBarContainer: "#listResultsHeader",
            sortChangeCallback: this.sortOrderChanged.bind(this),
            forcedBookType: null,
            customConfigurationPath: null
        },
        search: {
            container: document.getElementById("listSearchDiv") as HTMLDivElement,
            processSearchJsonCallback: this.editionAdvancedSearchPaged.bind(this),
            processSearchTextCallback: this.editionBasicSearch.bind(this)
        },
        searchBox: {
            inputFieldElement: ".searchbar-input",
            searchBoxInputSelector: ".searchbar-input.tt-input",
            dataSet: {
                name: "Title",
                groupHeader: "Název",
                parameterUrlString: null
            }
        },
        dropDownSelect: {
            dropDownSelectContainer: "#dropdownSelectDiv",
            showStar: true
        }
    };

    constructor(configuration: IModulInicializatorConfiguration) {
        super();

        this.configuration = this.parseConfig(configuration, this.defaultConfiguration);
        console.log(this.configuration);
    }

    protected parseConfig(config, defaultConfiguration) {
        const typeOf = typeof defaultConfiguration;
        const typeOfConfig = typeof config;

        switch (typeOf) {
            case "array":
            case "object":
                for (let key in defaultConfiguration) {
                    if (defaultConfiguration.hasOwnProperty(key)) {
                        if (config.hasOwnProperty(key)) {
                            config[key] = this.parseConfig(config[key], defaultConfiguration[key]);
                        } else {
                            config[key] = defaultConfiguration[key];
                        }
                    }
                }

                break;

            case "number":
            case "string":
                if (config === undefined) {
                    config = defaultConfiguration;
                }

                break;
            case "function":
                if (config === undefined) {
                    config = defaultConfiguration;
                } else if (typeOfConfig == "function") {
                    config = config.bind(this);
                } else {
                    console.error(`Config is not typeof function`, defaultConfiguration, config);
                }

                break;

            default:
                console.error(`Unknown defaultConfiguration type: ${typeOf}`, defaultConfiguration, config);
        }

        return config;
    }

    public init() {
        this.getBibliographyModule();
        this.getSearch();
        this.getSearchBox();
        this.getDropDownSelect();

        this.initializeFromUrlParams();
    }

    //---------------------------------------------------

    protected getBibliographyModule() {
        if (this.bibliographyModule === undefined) {
            this.bibliographyModule = this.createBibliographyModule();
        }

        return this.bibliographyModule;
    }

    protected createBibliographyModule() {
        return new BibliographyModule(
            this.configuration.bibliographyModule.resultsContainer,
            this.configuration.bibliographyModule.sortBarContainer,
            this.configuration.bibliographyModule.sortChangeCallback,
            this.configuration.bibliographyModule.forcedBookType,
            this.configuration.bibliographyModule.customConfigurationPath,
            this
        );
    }

    protected sortOrderChanged() {
        this.getBibliographyModule().showPage(1);
    }

    //---------------------------------------------------

    public getSearch() {
        if (this.search === undefined) {
            this.search = this.createSearch();
        }

        return this.search;
    }

    protected createSearch() {
        const search = new Search(
            this.configuration.search.container,
            this.configuration.search.processSearchJsonCallback,
            this.configuration.search.processSearchTextCallback
        );
        search.makeSearch(this.configuration.search.enabledOptions);

        return search;
    }

    protected hideTypeahead() {
        $(".twitter-typeahead").find(".tt-menu").hide();
    };

    protected editionAdvancedSearchPaged(json: string, pageNumber: number = 1) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        var bibliographyModule = this.getBibliographyModule();
        const start = (pageNumber - 1) * bibliographyModule.getBooksCountOnPage();
        const count = bibliographyModule.getBooksCountOnPage();
        const sortAsc = bibliographyModule.isSortedAsc();
        const sortingEnum = bibliographyModule.getSortCriteria();
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: this.configuration.searchBox.searchUrl.advanced,
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                bibliographyModule.showBooks(response.books);
                updateQueryStringParameter(this.configuration.base.url.searchKey, json);
                updateQueryStringParameter(this.configuration.base.url.pageKey, pageNumber);
                updateQueryStringParameter(this.configuration.base.url.sortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.configuration.base.url.sortCriteriaKey, bibliographyModule.getSortCriteria());
            }
        });
    }

    editionBasicSearchPaged(text: string, pageNumber: number = 1) {
        this.hideTypeahead();
        //if (typeof text === "undefined" || text === null || text === "") return;

        var bibliographyModule = this.getBibliographyModule();
        bibliographyModule.runAsyncOnLoad(() => {
            const start = (pageNumber - 1) * bibliographyModule.getBooksCountOnPage();
            const count = bibliographyModule.getBooksCountOnPage();
            const sortAsc = bibliographyModule.isSortedAsc();
            const sortingEnum = bibliographyModule.getSortCriteria();
            bibliographyModule.clearBooks();
            bibliographyModule.showLoading();

            $.ajax({
                type: "GET",
                traditional: true,
                url: this.configuration.searchBox.searchUrl.text,
                data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
                dataType: "json",
                contentType: "application/json",
                success: response => {
                    bibliographyModule.showBooks(response.books);
                    updateQueryStringParameter(this.configuration.base.url.searchKey, text);
                    updateQueryStringParameter(this.configuration.base.url.pageKey, pageNumber);
                    updateQueryStringParameter(this.configuration.base.url.sortAscKey, bibliographyModule.isSortedAsc());
                    updateQueryStringParameter(this.configuration.base.url.sortCriteriaKey, bibliographyModule.getSortCriteria());
                }
            });
        });
    }

    protected actualizeSelectedBooksAndCategoriesInQuery() {
        this.bookIdsInQuery = this.selectedBookIds;
        this.categoryIdsInQuery = this.selectedCategoryIds;
    }

    protected createPagination(booksCount: number) {
        const bibliographyModule = this.getBibliographyModule();

        const pages = Math.ceil(booksCount / this.booksCountOnPage);
        if (this.initPage && this.initPage <= pages) {
            bibliographyModule.createPagination(this.booksCountOnPage, this.pageClickCallbackForBiblModule.bind(this), booksCount, this.initPage);
        } else {
            bibliographyModule.createPagination(this.booksCountOnPage, this.pageClickCallbackForBiblModule.bind(this), booksCount);
        }
    }

    protected pageClickCallbackForBiblModule(pageNumber: number = 1) {
        const search = this.getSearch();

        if (search.isLastQueryJson()) {
            this.editionAdvancedSearchPaged(search.getLastQuery(), pageNumber);
        } else {
            this.editionBasicSearchPaged(search.getLastQuery(), pageNumber);
        }
    }

    protected editionBasicSearch(text: string) {
        this.hideTypeahead();
        this.actualizeSelectedBooksAndCategoriesInQuery();
        //if (typeof text === "undefined" || text === null || text === "") return;

        var bibliographyModule = this.getBibliographyModule();

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: this.configuration.searchBox.searchUrl.textCount,
            data: { text: text, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.configuration.base.url.searchKey, text);
                updateQueryStringParameter(this.configuration.base.url.selectionKey, DropDownSelect2.getUrlStringFromState(this.getDropDownSelect().getState()));
                updateQueryStringParameter(this.configuration.base.url.sortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.configuration.base.url.sortCriteriaKey, bibliographyModule.getSortCriteria());
            }
        });
    }

    //---------------------------------------------------

    protected getSearchBox() {
        if (this.searchBox === undefined) {
            this.searchBox = this.createSearchBox();
        }

        return this.searchBox;
    }

    protected createSearchBox() {
        var searchBox = new SearchBox(
            this.configuration.searchBox.inputFieldElement,
            this.configuration.searchBox.controllerPath
        );
        searchBox.addDataSet(
            this.configuration.searchBox.dataSet.name,
            this.configuration.searchBox.dataSet.groupHeader
        );
        searchBox.create();
        var searchBoxOnput = $(this.configuration.searchBox.searchBoxInputSelector);
        searchBox.value(searchBoxOnput.val());

        searchBoxOnput.change(() => { //prevent clearing input value on blur() 
            searchBox.value(searchBoxOnput.val());
        });

        return searchBox;
    }

    //---------------------------------------------------

    protected getDropDownSelect() {
        if (this.dropDownSelect === undefined) {
            this.dropDownSelect = this.createDropDownSelect(this.getSearchBox());
        }

        return this.dropDownSelect;
    }

    protected createDropDownSelect(searchBox: SearchBox) {
        const callbackDelegate = new DropDownSelectCallbackDelegate();

        callbackDelegate.selectedChangedCallback = (state: State) => {
            this.selectedBookIds = new Array();

            for (let i = 0; i < state.SelectedItems.length; i++) {
                this.selectedBookIds.push(state.SelectedItems[i].Id);
            }

            this.selectedCategoryIds = new Array();

            for (let i = 0; i < state.SelectedCategories.length; i++) {
                this.selectedCategoryIds.push(state.SelectedCategories[i].Id);
            }

            var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
            searchBox.clearAndDestroy();
            searchBox.addDataSet(
                this.configuration.searchBox.dataSet.name,
                this.configuration.searchBox.dataSet.groupHeader,
                parametersUrl
            );
            searchBox.create();
            searchBox.value($(this.configuration.searchBox.searchBoxInputSelector).val());
        };

        const dropDownSelect = new DropDownSelect2(
            this.configuration.dropDownSelect.dropDownSelectContainer,
            this.configuration.dropDownSelect.dataUrl,
            this.configuration.dropDownSelect.showStar,
            callbackDelegate
        );

        callbackDelegate.dataLoadedCallback = () => {
            var selectedIds = dropDownSelect.getSelectedIds();

            this.selectedBookIds = selectedIds.selectedBookIds;
            this.selectedCategoryIds = selectedIds.selectedCategoryIds;
            $("#listResults").removeClass("loader");
            this.initializeFromUrlParams();
        };

        dropDownSelect.makeDropdown();

        return dropDownSelect;
    }

    //---------------------------------------------------

    protected initializeFromUrlParams() {
        if (this.readyForInit && this.notInitialized) {
            this.notInitialized = false;

            const bibliographyModule = this.getBibliographyModule();
            bibliographyModule.runAsyncOnLoad(function () {
                const search = this.getSearch();
                const dropDownSelect = this.getDropDownSelect();

                const page = getQueryStringParameterByName(this.configuration.base.url.pageKey);

                if (page) {
                    this.initPage = parseInt(page);
                }

                const sortedAsc = getQueryStringParameterByName(this.configuration.base.url.sortAscKey);
                const sortCriteria = getQueryStringParameterByName(this.configuration.base.url.sortCriteriaKey);

                if (sortedAsc && sortCriteria) {
                    bibliographyModule.setSortedAsc(sortedAsc === "true");
                    bibliographyModule.setSortCriteria(((sortCriteria) as any) as SortEnum);
                }

                const selected = getQueryStringParameterByName(this.configuration.base.url.selectionKey);
                const searched = getQueryStringParameterByName(this.configuration.base.url.searchKey);

                search.writeTextToTextField(searched);

                if (selected) {
                    dropDownSelect.setStateFromUrlString(selected);
                } else {
                    search.processSearch(); //if not explicitly selected 
                }
            }.bind(this));
        } else if (!this.notInitialized) {
            this.getSearch().processSearch();
        } else {
            this.readyForInit = true;
        }
    }
}

interface IListModulInicializatorConfiguration extends IModulInicializatorConfiguration {
    search: {
        container?: HTMLDivElement;
        processSearchJsonCallback?: (jsonData: string, pageNumber?: number) => void;
        processSearchTextCallback?: (text: string) => void;

        enabledOptions: Array<SearchTypeEnum>;
    };
}
