class ProfessionalLiteratureModulInicializator extends ListModulInicializator {
    protected configuration: IProfessionalLiteratureModulInicializatorConfiguration;

    constructor(configuration: IProfessionalLiteratureModulInicializatorConfiguration) {
        super(configuration);

        this.configuration.search.processSearchJsonCallback = this.advancedSearch.bind(this);
    }

    protected advancedSearch(json: string) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        this.actualizeSelectedBooksAndCategoriesInQuery();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();
        
        $.ajax({
            type: "GET",
            traditional: true,
            url: this.configuration.searchBox.searchUrl.advancedCount,
            data: { json: json, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.configuration.base.url.searchKey, json);
                updateQueryStringParameter(this.configuration.base.url.selectionKey, DropDownSelect2.getUrlStringFromState(this.getDropDownSelect().getState()));
                updateQueryStringParameter(this.configuration.base.url.sortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.configuration.base.url.sortCriteriaKey, this.bibliographyModule.getSortCriteria());
            }
        });
    }

    protected editionAdvancedSearchPaged(json: string, pageNumber: number) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        const start = (pageNumber - 1) * this.bibliographyModule.getBooksCountOnPage();
        const count = this.bibliographyModule.getBooksCountOnPage();
        const sortAsc = this.bibliographyModule.isSortedAsc();
        const sortingEnum = this.bibliographyModule.getSortCriteria();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: this.configuration.searchBox.searchUrl.advanced,
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response["books"]);
                updateQueryStringParameter(this.configuration.base.url.searchKey, json);
                updateQueryStringParameter(this.configuration.base.url.pageKey, pageNumber);
                updateQueryStringParameter(this.configuration.base.url.sortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.configuration.base.url.sortCriteriaKey, this.bibliographyModule.getSortCriteria());
            }
        });
    }
}

interface IProfessionalLiteratureModulInicializatorConfiguration extends IModulInicializatorConfiguration {
    searchBox: IProfessionalLiteratureModulInicializatorConfigurationSearchBox;
}

interface IProfessionalLiteratureModulInicializatorConfigurationSearchBox extends IModulInicializatorConfigurationSearchBox {
    searchUrl: IProfessionalLiteratureModulInicializatorConfigurationSearchBoxSearchUrl;
}

interface IProfessionalLiteratureModulInicializatorConfigurationSearchBoxSearchUrl extends IModulInicializatorConfigurationSearchBoxSearchUrl {
    advancedCount: string;
}
