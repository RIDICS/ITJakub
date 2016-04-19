class SearchModulInicializator extends ModulInicializator {

    protected configuration: ISearchModulInicializatorConfiguration;

    protected searchDefaultConfiguration={
        search: {
            processSearchJsonCallback: this.editionAdvancedSearch.bind(this) 
        }  
    };

    constructor(configuration: ISearchModulInicializatorConfiguration) {
        super(configuration);
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

    protected editionAdvancedSearch(json: string) {
        if (typeof json === "undefined" || json === null || json === "") return;
        this.actualizeSelectedBooksAndCategoriesInQuery();

        var bibliographyModule = this.getBibliographyModule();
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: this.configuration.searchBox.searchUrl.advanced,
            data: { json: json, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.configuration.base.url.searchKey, json);
                updateQueryStringParameter(this.configuration.base.url.selectionKey, DropDownSelect2.getUrlStringFromState(this.getDropDownSelect().getState()));
                updateQueryStringParameter(this.configuration.base.url.sortAscKey, bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.configuration.base.url.sortCriteriaKey, bibliographyModule.getSortCriteria());
            }
        });
    }
}

interface ISearchModulInicializatorConfiguration extends IModulInicializatorConfiguration {
    searchBox: ISearchModulInicializatorConfigurationSearchBox;
}

interface ISearchModulInicializatorConfigurationSearchBox extends IModulInicializatorConfigurationSearchBox {
    searchUrl: ISearchModulInicializatorConfigurationSearchBoxSearchUrl;
}

interface ISearchModulInicializatorConfigurationSearchBoxSearchUrl extends IModulInicializatorConfigurationSearchBoxSearchUrl {
    advancedCount: string;
}