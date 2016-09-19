class ListModulInicializator extends ModulInicializator {
    protected configuration: IListModulInicializatorConfiguration;
    
    constructor(configuration: IListModulInicializatorConfiguration) {
        super(configuration);
    }

    protected getDefaultConfiguration() {
        return this.parseConfig({
            searchBox: {
                inputFieldElement: ".searchbar-input",
                searchBoxInputSelector: ".searchbar-input.tt-input",
                dataSet: {
                    name: "Title",
                    groupHeader: "Název",
                    parameterUrlString: null
                }
            }
        }, super.getDefaultConfiguration());
    }

    public init() {
        this.getBibliographyModule();
        this.getSearch();
        this.getSearchBox();
        this.getDropDownSelect();

        this.initializeFromUrlParams();
    }
    
    protected createDropDownSelect() {
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
            var searchBox = this.getSearchBox();
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
            this.configuration.search.favoriteQueries.bookType,
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
}

interface IListModulInicializatorConfiguration extends IModulInicializatorConfiguration {
    searchBox: {
        inputFieldElement?: string;
        controllerPath: string;

        dataSet?: {
            name?: string;
            groupHeader?: string;
            parameterUrlString?: string;
        };
        searchBoxInputSelector?: string;
    };
}
