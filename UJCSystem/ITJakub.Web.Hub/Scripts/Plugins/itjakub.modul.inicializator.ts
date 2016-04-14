﻿class ModulInicializator {
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
    protected defaultConfiguration={
        bibliographyModule: {
            sortChangeCallback: this.sortOrderChanged.bind(this),
            forcedBookType: null,
            customConfigurationPath: null
        },
        search: {
            processSearchJsonCallback: this.editionAdvancedSearchPaged.bind(this),
            processSearchTextCallback: this.editionBasicSearch.bind(this)
        },
        searchBox: {
            dataSet: {
                parameterUrlString: null
            }
        }
    };
    
    constructor(configuration: IModulInicializatorConfiguration) {
        this.configuration = this.parseConfig(configuration, this.defaultConfiguration);
        console.log(this.configuration);
    }

    protected parseConfig(config, defaultConfiguration) {
        const typeOf = typeof defaultConfiguration;

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
            config = defaultConfiguration;

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
            this.configuration.bibliographyModule.customConfigurationPath
        );
    }

    protected sortOrderChanged() {
        this.getBibliographyModule().showPage(1);
    }

    //---------------------------------------------------

    protected getSearch() {
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

    protected editionAdvancedSearchPaged(json: string, pageNumber: number=1) {
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

    editionBasicSearchPaged(text: string, pageNumber: number=1) {
        this.hideTypeahead();
        //if (typeof text === "undefined" || text === null || text === "") return;

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
        const dropDownSelect = new DropDownSelect2(
            this.configuration.dropDownSelect.dropDownSelectContainer,
            this.configuration.dropDownSelect.dataUrl,
            this.configuration.dropDownSelect.showStar,
            callbackDelegate
        );

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
        callbackDelegate.dataLoadedCallback = () => {
            var selectedIds = dropDownSelect.getSelectedIds();

            this.selectedBookIds = selectedIds.selectedBookIds;
            this.selectedCategoryIds = selectedIds.selectedCategoryIds;
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
            console.log(search);

            if (selected) {
                dropDownSelect.setStateFromUrlString(selected);
            } else {
                search.processSearch(); //if not explicitly selected 
            }

        } else if (!this.notInitialized) {
            this.getSearch().processSearch();
        } else {
            this.readyForInit = true;
        }
    }
}

interface IModulInicializatorConfiguration {
    base: {
        url: {
            searchKey: string;
            pageKey: string;
            selectionKey: string;
            sortAscKey: string;
            sortCriteriaKey: string;
        };
    };
    bibliographyModule: {
        resultsContainer: string;
        sortBarContainer: string;
        sortChangeCallback?: () => void;
        forcedBookType?: BookTypeEnum;
        customConfigurationPath?: string;
    };
    search: {
        container: HTMLDivElement;
        processSearchJsonCallback?: (jsonData: string) => void;
        processSearchTextCallback?: (text: string) => void;

        enabledOptions: Array<SearchTypeEnum>;
    };
    searchBox: {
        inputFieldElement: string;
        controllerPath: string;

        dataSet: {
            name: string;
            groupHeader: string;
            parameterUrlString?: string;
        };
        searchBoxInputSelector: string;
        searchUrl: {
            advanced: string;
            text: string;
            textCount: string;
        };
    };
    dropDownSelect: {
        dropDownSelectContainer: string;
        dataUrl: string;
        showStar: boolean;
    };
}