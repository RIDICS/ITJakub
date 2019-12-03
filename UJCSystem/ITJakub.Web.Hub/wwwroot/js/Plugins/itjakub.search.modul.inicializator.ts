﻿class SearchModulInicializator extends ModulInicializator {

    protected configuration: ISearchModulInicializatorConfiguration;
    
    constructor(configuration: ISearchModulInicializatorConfiguration) {
        super(configuration);
    }

    protected getDefaultConfiguration() {
        return this.parseConfig({
            base: {
                autosearch: false
            },
            search: {
                processSearchJsonCallback: this.editionAdvancedSearch.bind(this),
                placeholder: localization.translate("SearchInFulltext...", "PluginsJs").value
            }
        }, super.getDefaultConfiguration());
    }

    protected editionAdvancedSearch(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;
        this.actualizeSelectedBooksAndCategoriesInQuery();

        var bibliographyModule = this.getBibliographyModule();
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();
        bibliographyModule.destroyPagination();

        // the same condition in ModulInicializator class
        if (json === "[]") {
            bibliographyModule.showError(localization.translate("SearchCriteriaError", "PluginsJs").value);
            return;
        }

        $.ajax({
            type: "GET",
            traditional: true,
            url: this.configuration.search.url.advancedCount,
            data: { json: json, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.configuration.base.url.searchKey, json);
                updateQueryStringParameter(this.configuration.base.url.sortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.configuration.base.url.sortCriteriaKey, bibliographyModule.getSortCriteria());
            },
            error: () => {
                bibliographyModule.showSearchError();
            }
        });
    }
}

interface ISearchModulInicializatorConfiguration extends IModulInicializatorConfiguration {
    search: ISearchModulInicializatorConfigurationSearch;
}

interface ISearchModulInicializatorConfigurationSearch extends IModulInicializatorConfigurationSearch {
    url: ISearchModulInicializatorConfigurationSearchUrl;
}

interface ISearchModulInicializatorConfigurationSearchUrl extends IModulInicializatorConfigurationSearchUrl {
    //advancedCount: string;
}