class ModulInicializator {
    protected bibliographyModule: BibliographyModule;
    protected search: Search;
    protected searchBox: SearchBox;
    protected dropDownSelect: DropDownSelect2;

    protected bookIdsInQuery = new Array();
    protected categoryIdsInQuery = new Array();
    
    protected initPage: number = null;
    protected booksCountOnPage = 5;

    protected readyForInit = false;
    protected notInitialized = true;

    protected configuration: IModulInicializatorConfiguration;
    protected defaultConfiguration = {
        base: {
            autosearch: true,
            searchOnFill: true,
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
            processSearchJsonCallback: this.editionAdvancedSearch.bind(this),
            processSearchTextCallback: this.editionBasicSearch.bind(this)
        },
        dropDownSelect: {
            dropDownSelectContainer: "#dropdownSelectDiv",
            showStar: true
        }
    };

    constructor(configuration: IModulInicializatorConfiguration) {
        this.configuration = this.parseConfig(configuration, this.getDefaultConfiguration());
        console.log(this.configuration);
    }

    protected getDefaultConfiguration() {
        return this.defaultConfiguration;
    }

    protected parseConfig(config, defaultConfiguration) {
        const typeOf = typeof defaultConfiguration;
        const typeOfConfig = typeof config;

        switch (typeOf) {
            case "array":
            case "object":
                if (config === undefined) {
                    config = defaultConfiguration;
                }
                else {
                    for (let key in defaultConfiguration) {
                        if (defaultConfiguration.hasOwnProperty(key)) {
                            if (config.hasOwnProperty(key)) {
                                config[key] = this.parseConfig(config[key], defaultConfiguration[key]);
                            } else {
                                config[key] = defaultConfiguration[key];
                            }
                        }
                    }
                }

                break;

            case "boolean":
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
            this.configuration.search.processSearchTextCallback,
            this.configuration.search.favoriteQueries
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
            url: this.configuration.search.url.advanced,
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

    protected editionBasicSearchPaged(text: string, pageNumber: number = 1) {
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
                url: this.configuration.search.url.text,
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
        var selectedIds = this.dropDownSelect.getSelectedIds();
        this.bookIdsInQuery = selectedIds.selectedBookIds;
        this.categoryIdsInQuery = selectedIds.selectedCategoryIds;
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

    protected editionAdvancedSearch(json: string) {
        this.hideTypeahead();
        this.actualizeSelectedBooksAndCategoriesInQuery();
        if (typeof json === "undefined" || json === null || json === "") return;

        var bibliographyModule = this.getBibliographyModule();

        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: this.configuration.search.url.advancedCount,
            data: { json: json, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.configuration.base.url.searchKey, json);
                updateQueryStringParameter(this.configuration.base.url.sortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.configuration.base.url.sortCriteriaKey, bibliographyModule.getSortCriteria());
            }
        });
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
            url: this.configuration.search.url.textCount,
            data: { text: text, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.configuration.base.url.searchKey, text);
                updateQueryStringParameter(this.configuration.base.url.sortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.configuration.base.url.sortCriteriaKey, bibliographyModule.getSortCriteria());
            }
        });
    }

    //---------------------------------------------------

    protected getDropDownSelect() {
        if (this.dropDownSelect === undefined) {
            this.dropDownSelect = this.createDropDownSelect();
        }

        return this.dropDownSelect;
    }

    protected createDropDownSelect() {
        const callbackDelegate = new DropDownSelectCallbackDelegate();

        callbackDelegate.selectedChangedCallback = (state: State) => {
            var serializedState = this.dropDownSelect.getSerializedState();
            updateQueryStringParameter(this.configuration.base.url.selectionKey, serializedState);
        };

        const dropDownSelect = new DropDownSelect2(
            this.configuration.dropDownSelect.dropDownSelectContainer,
            this.configuration.dropDownSelect.dataUrl,
            this.configuration.search.favoriteQueries.bookType,
            this.configuration.dropDownSelect.showStar,
            callbackDelegate
        );

        callbackDelegate.dataLoadedCallback = () => {
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
            bibliographyModule.runAsyncOnLoad(() =>{
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
                    dropDownSelect.restoreFromSerializedState(selected);
                }
                if (this.configuration.base.autosearch || (
                    this.configuration.base.searchOnFill
                    && search.getTextFromTextField().length > 0
                )) {
                    search.processSearch(); //if not explicitly selected 
                }
            });
        } else if (!this.notInitialized) {
            this.getSearch().processSearch();
        } else {
            this.readyForInit = true;
        }
    }
}

interface IModulInicializatorConfiguration {
    base?: {
        autosearch?: boolean;
        searchOnFill?: boolean;
        url?: {
            searchKey?: string;
            pageKey?: string;
            selectionKey?: string;
            sortAscKey?: string;
            sortCriteriaKey?: string;
        };
    };
    bibliographyModule: {
        resultsContainer?: string;
        sortBarContainer?: string;
        sortChangeCallback?: () => void;
        forcedBookType?: BookTypeEnum;
        customConfigurationPath?: string;
    };
    search: IModulInicializatorConfigurationSearch;
    dropDownSelect: {
        dropDownSelectContainer?: string;
        dataUrl: string;
        showStar?: boolean;
    };
}

interface IModulInicializatorConfigurationSearch {
    container?: HTMLDivElement;
    processSearchJsonCallback?: (jsonData: string, pageNumber?:number) => void;
    processSearchTextCallback?: (text: string) => void;

    enabledOptions: Array<SearchTypeEnum>;

    url: IModulInicializatorConfigurationSearchUrl;
    favoriteQueries: IModulInicializatorConfigurationSearchFavorites;
}

interface IModulInicializatorConfigurationSearchUrl {
    advanced: string;
    text: string;
    advancedCount: string;
    textCount: string;
}

interface IModulInicializatorConfigurationSearchFavorites {
    bookType: BookTypeEnum;
    queryType: QueryTypeEnum;
}
