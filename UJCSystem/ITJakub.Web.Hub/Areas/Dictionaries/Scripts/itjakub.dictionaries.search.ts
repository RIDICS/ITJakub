$(document).ready(() => {
    var dictionarySearch = new DictionarySearch();
    dictionarySearch.create();
});

class DictionarySearch {
    private tabs: DictionarySearchTabs;
    private callbackDelegate: DropDownSelectCallbackDelegate;
    private dictionaryViewerHeadword: DictionaryViewer;
    private dictionaryViewerFulltext: DictionaryViewer;
    private dictionaryViewerAdvanced: DictionaryViewer;
    private dictionaryWrapperBasic: DictionaryViewerTextWrapper;
    private dictionaryWrapperAdvanced: DictionaryViewerJsonWrapper;
    private search: Search;
    private dictionarySelector: DropDownSelect2;
    private disabledShowOptions: Array<SearchTypeEnum>;
    private typeaheadSearchBox: SearchBox;

    constructor() {
        var pageSize = 25;

        this.tabs = new DictionarySearchTabs();
        this.callbackDelegate = new DropDownSelectCallbackDelegate();
        this.callbackDelegate.selectedChangedCallback = (state) => {
            if (state.IsOnlyRootSelected)
                state.SelectedCategories = [];

            this.updateTypeaheadSearchBox(state);
        };
        this.dictionarySelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", BookTypeEnum.Dictionary, true, this.callbackDelegate);
        
        this.dictionaryViewerHeadword = new DictionaryViewer("#headwords-list", "#headwords-pagination", "#description-headwords", true);
        this.dictionaryViewerFulltext = new DictionaryViewer("#headwords-list-fulltext", "#headwords-pagination-fulltext", "#description-fulltext", true);
        this.dictionaryViewerAdvanced = new DictionaryViewer("#headwords-list-advanced", "#headwords-pagination-advanced", "#description-advanced", true);

        this.dictionaryWrapperBasic = new DictionaryViewerTextWrapper(this.dictionaryViewerHeadword, this.dictionaryViewerFulltext, pageSize, this.tabs, this.dictionarySelector);
        this.dictionaryWrapperAdvanced = new DictionaryViewerJsonWrapper(this.dictionaryViewerAdvanced, pageSize, this.tabs, this.dictionarySelector);

        var favoriteQueriesConfig: IModulInicializatorConfigurationSearchFavorites = {
            bookType: BookTypeEnum.Dictionary,
            queryType: QueryTypeEnum.Search
        };
        this.search = new Search(document.getElementById("dictionarySearchDiv") as HTMLDivElement, this.processSearchJson.bind(this), this.processSearchText.bind(this), favoriteQueriesConfig);
        this.typeaheadSearchBox = new SearchBox(".searchbar-input", "Dictionaries/Dictionaries");
        this.search.setOverrideQueryCallback(text => this.typeaheadSearchBox.value(text));

        this.disabledShowOptions = [
            SearchTypeEnum.Author,
            SearchTypeEnum.Title,
            SearchTypeEnum.Editor,
            SearchTypeEnum.Dating
            // TODO add all disable parameters
        ];
    }

    create() {
        var enabledOptions = new Array<SearchTypeEnum>();
        enabledOptions.push(SearchTypeEnum.Headword);
        enabledOptions.push(SearchTypeEnum.HeadwordDescription);
        enabledOptions.push(SearchTypeEnum.HeadwordDescriptionTokenDistance);
        enabledOptions.push(SearchTypeEnum.Author);
        enabledOptions.push(SearchTypeEnum.Dating);
        enabledOptions.push(SearchTypeEnum.Editor);
        enabledOptions.push(SearchTypeEnum.Title);

        this.dictionarySelector.makeDropdown();
        this.search.makeSearch(enabledOptions);

        this.typeaheadSearchBox.addDataSet("DictionaryHeadword", "Slovníková hesla");
        this.typeaheadSearchBox.create();

        $("#cancelFilter").click(() => {
            this.getCurrentDictionaryViewer().cancelFilter();
            $("#cancelFilter").addClass("hidden");
        });

        $("#printDescription").click(() => {
            this.getCurrentDictionaryViewer().print();
        });

        $("#printList").click(() => {
            this.getCurrentDictionaryViewer().printList();
        });

        window.matchMedia("print").addListener(mql => {
            if (mql.matches) {
                this.getCurrentDictionaryViewer().loadAllHeadwords();
            }
        });
    }

    private updateTypeaheadSearchBox(state: State) {
        var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
        this.typeaheadSearchBox.clearAndDestroy();
        this.typeaheadSearchBox.addDataSet("DictionaryHeadword", "Slovníková hesla", parametersUrl);
        this.typeaheadSearchBox.create();
    }

    private showLoadingBar() {
        $(".dictionary-result-word-search-list").addClass("hidden");
        $(".dictionary-result-word-search-description").addClass("hidden");
        $("#main-loader").removeClass("hidden");
    }

    private processSearchJson(json: string) {
        this.showLoadingBar();
        var filteredJsonForShowing = this.search.getFilteredQuery(json, this.disabledShowOptions);
        this.dictionaryWrapperAdvanced.loadCount(json, filteredJsonForShowing);
    }

    private processSearchText(text: string) {
        this.showLoadingBar();
        this.dictionaryWrapperBasic.loadCount(text);
    }

    private getCurrentDictionaryViewer(): DictionaryViewer {
        var currentTab = this.tabs.getCurrentTab();
        switch (currentTab) {
            case DictionaryTabsEnum.Headwords:
                return this.dictionaryViewerHeadword;
            case DictionaryTabsEnum.Fulltext:
                return this.dictionaryViewerFulltext;
            case DictionaryTabsEnum.Advanced:
                return this.dictionaryViewerAdvanced;
            default:
                return this.dictionaryViewerHeadword;
        }
    }
}

class DictionarySearchTabs {
    private searchTabs: Array<SearchTab>;
    private currentTab: DictionaryTabsEnum;

    constructor() {
        this.searchTabs = [
            new SearchTab("#tab-headwords", "#list-headwords", "#description-headwords"),
            new SearchTab("#tab-fulltext", "#list-fulltext", "#description-fulltext"),
            new SearchTab("#tab-advanced", "#list-advanced", "#description-advanced")
        ];
        this.currentTab = DictionaryTabsEnum.Headwords;
        
        $("#search-tabs li").addClass("hidden");
        $("#search-tabs a").click(e => {
            e.preventDefault();
            $(e.target).tab("show");
            this.show(e.target.getAttribute("href"));
            $(window).trigger("scroll");
        });
    }

    private show(id: string) {
        var index = DictionaryTabsEnum.Headwords;
        switch (id) {
            case "#headwords":
                index = DictionaryTabsEnum.Headwords;
                break;
            case "#fulltext":
                index = DictionaryTabsEnum.Fulltext;
                break;
            case "#advanced":
                index = DictionaryTabsEnum.Advanced;
                break;
        }

        this.currentTab = index;
        var searchTab = this.searchTabs[index];
        $("#headword-description > div").removeClass("active");
        $(".tab-content > div").removeClass("active");
        $(searchTab.descriptionDiv).addClass("active");
        $(searchTab.listDiv).addClass("active");
    }

    showAdvanced() {
        var advancedSearchTab = this.searchTabs[DictionaryTabsEnum.Advanced];
        $("#search-tabs li").addClass("hidden");
        $(advancedSearchTab.tabLi).removeClass("hidden");
        $(advancedSearchTab.tabLi).children().trigger("click");
    }

    showBasic() {
        var advancedSearchTab = this.searchTabs[DictionaryTabsEnum.Advanced];
        var headwordSearchTab = this.searchTabs[DictionaryTabsEnum.Headwords];
        $("#search-tabs li").removeClass("hidden");
        $(advancedSearchTab.tabLi).addClass("hidden");
        $(headwordSearchTab.tabLi).children().trigger("click");
    }

    getCurrentTab(): DictionaryTabsEnum {
        return this.currentTab;
    }
}

enum DictionaryTabsEnum {
    Headwords = 0,
    Fulltext = 1,
    Advanced = 2
}

class SearchTab {
    listDiv: string;
    descriptionDiv: string;
    tabLi: string;

    constructor(tabLi: string, listDiv: string, descriptionDiv: string) {
        this.descriptionDiv = descriptionDiv;
        this.listDiv = listDiv;
        this.tabLi = tabLi;
    }
}

class DictionaryViewerJsonWrapper {
    private categoryDropDown: DropDownSelect2;
    private tabs: DictionarySearchTabs;
    private pageSize: number;
    private dictionaryViewer: DictionaryViewer;
    private json: string;
    private selectedIds: DropDownSelected;

    constructor(dictionaryViewer: DictionaryViewer, pageSize: number, tabs: DictionarySearchTabs, categoryDropDown: DropDownSelect2) {
        this.categoryDropDown = categoryDropDown;
        this.tabs = tabs;
        this.pageSize = pageSize;
        this.dictionaryViewer = dictionaryViewer;
    }

    loadCount(json: string, filteredJsonForShowing: string) {
        this.json = json;
        this.selectedIds = this.categoryDropDown.getSelectedIds();
        if (this.selectedIds.isOnlyRootSelected)
            this.selectedIds.selectedCategoryIds = [];

        this.dictionaryViewer.showLoading();
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaResultsCount",
            data: JSON.stringify({
                "json": json,
                "selectedBookIds": this.selectedIds.selectedBookIds,
                "selectedCategoryIds": this.selectedIds.selectedCategoryIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: (resultCount: number) => {
                this.hideMainLoadingBar();
                $("#search-advanced-count").text(resultCount);
                this.tabs.showAdvanced();
                this.dictionaryViewer.createViewer(resultCount, this.loadHeadwords.bind(this), this.pageSize, filteredJsonForShowing, true);
            }
        });
    }

    private loadHeadwords(pageNumber: number) {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria",
            data: JSON.stringify({
                "json": this.json,
                "start": this.pageSize * (pageNumber - 1),
                "count": this.pageSize,
                "selectedBookIds": this.selectedIds.selectedBookIds,
                "selectedCategoryIds": this.selectedIds.selectedCategoryIds
            }),
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.dictionaryViewer.showHeadwords(response);
            }
        });
    }

    private hideMainLoadingBar() {
        $(".dictionary-result-word-search-list").removeClass("hidden");
        $(".dictionary-result-word-search-description").removeClass("hidden");
        $("#main-loader").addClass("hidden");
    }
}

class DictionaryViewerTextWrapper {
    private categoryDropDown: DropDownSelect2;
    private tabs: DictionarySearchTabs;
    private pageSize: number;
    private headwordViewer: DictionaryViewer;
    private fulltextViewer: DictionaryViewer;
    private text: string;
    private selectedIds: DropDownSelected;

    constructor(headwordViewer: DictionaryViewer, fulltextViewer: DictionaryViewer, pageSize: number, tabs: DictionarySearchTabs, categoryDropDown: DropDownSelect2) {
        this.categoryDropDown = categoryDropDown;
        this.tabs = tabs;
        this.pageSize = pageSize;
        this.headwordViewer = headwordViewer;
        this.fulltextViewer = fulltextViewer;
    }

    loadCount(text: string) {
        this.text = text;
        this.selectedIds = this.categoryDropDown.getSelectedIds();
        if (this.selectedIds.isOnlyRootSelected)
            this.selectedIds.selectedCategoryIds = [];

        $("#search-headword-count").text("");
        $("#search-fulltext-count").text("");
        this.headwordViewer.showLoading();
        this.fulltextViewer.showLoading();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicResultsCount",
            data: {
                text: text,
                selectedBookIds: this.selectedIds.selectedBookIds,
                selectedCategoryIds: this.selectedIds.selectedCategoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: (resultCount: number) => {
                this.showBasicTabsAndCount("#search-headword-count", resultCount);
                this.headwordViewer.createViewer(resultCount, this.loadHeadwords.bind(this), this.pageSize, text);
            },
            error: () => {
                this.showBasicTabsAndCount("#search-headword-count", "!");
            }
        });

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicFulltextResultsCount",
            data: {
                text: text,
                selectedBookIds: this.selectedIds.selectedBookIds,
                selectedCategoryIds: this.selectedIds.selectedCategoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: (resultCount: number) => {
                this.showBasicTabsAndCount("#search-fulltext-count", resultCount);
                this.fulltextViewer.createViewer(resultCount, this.loadFulltextHeadwords.bind(this), this.pageSize, text);
            },
            error: () => {
                this.showBasicTabsAndCount("#search-fulltext-count", "!");
            }
        });
    }

    private showBasicTabsAndCount(container: string, count: string|number|boolean) {
        this.hideMainLoadingBar();
        $(container).text(count);
        this.tabs.showBasic();
    }

    private loadHeadwords(pageNumber: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicHeadword",
            data: {
                text: this.text,
                start: this.pageSize * (pageNumber - 1),
                count: this.pageSize,
                selectedBookIds: this.selectedIds.selectedBookIds,
                selectedCategoryIds: this.selectedIds.selectedCategoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.headwordViewer.showHeadwords(response);
            }
        });
    }

    private loadFulltextHeadwords(pageNumber: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/SearchBasicFulltext",
            data: {
                text: this.text,
                start: this.pageSize * (pageNumber - 1),
                count: this.pageSize,
                selectedBookIds: this.selectedIds.selectedBookIds,
                selectedCategoryIds: this.selectedIds.selectedCategoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.fulltextViewer.showHeadwords(response);
            }
        });
    }

    private hideMainLoadingBar() {
        $(".dictionary-result-word-search-list").removeClass("hidden");
        $(".dictionary-result-word-search-description").removeClass("hidden");
        $("#main-loader").addClass("hidden");
    }
}
