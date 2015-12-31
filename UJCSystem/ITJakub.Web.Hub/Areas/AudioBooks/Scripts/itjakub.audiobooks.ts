var aduioTypeTranslation = [
    "Neznámý",
    "Mp3",
    "Ogg",
    "Wav"
];

enum AudioTypeEnum {
    Unknown = 0,
    Mp3 = 1,
    Ogg = 2,
    Wav = 3,
}


function initAudibooksList() {
    var bookCountPerPage = 5;
    var audibooksList = new AudibooksList(bookCountPerPage);
    audibooksList.create();
}

function translateAudioType(audioType: number): string {
    return aduioTypeTranslation[audioType];
}

function fillLeadingZero(seconds: number): string {
    var secondsString = seconds.toString();
    if (secondsString.length === 1) {
        secondsString = `0${secondsString}`;
    }
    return secondsString;
}

$(document).ready(() => {
    initAudibooksList();
});

class AudibooksList {
    private bookCountPerPage: number;
    private search: Search;
    private typeaheadSearchBox: SearchBox;
    private audibookSelector: DropDownSelect2;
    private bibliographyModule: BibliographyModule;
    private selectedBookIds: Array<number>;
    private selectedCategoryIds: Array<number>;

    private urlSearchKey = "search";
    private urlPageKey = "page";
    private urlSelectionKey = "selected";
    private urlSortAscKey = "sortAsc";
    private urlSortCriteriaKey = "sortCriteria";

    private readyForInit = false;
    private notInitialized = true;
    private initPage: number = null;

    private bookIdsInQuery = new Array();
    private categoryIdsInQuery = new Array();

    constructor(bookCountPerPage: number) {
        this.bookCountPerPage = bookCountPerPage;
    }

    create() {
        var enabledOptions = new Array<SearchTypeEnum>();
        enabledOptions.push(SearchTypeEnum.Author);
        enabledOptions.push(SearchTypeEnum.Dating);
        enabledOptions.push(SearchTypeEnum.Editor);
        enabledOptions.push(SearchTypeEnum.Title);

        this.search = new Search(<any>$("#listSearchDiv")[0], (json: string)=> {this.audioAdvancedSearch(json)}, (text:string) => {this.audioBasicSearch(text)});
        this.search.makeSearch(enabledOptions);

        this.typeaheadSearchBox = new SearchBox(".searchbar-input", "AudioBooks/AudioBooks");
        this.typeaheadSearchBox.addDataSet("Title", "Název");
        this.typeaheadSearchBox.create();
        this.typeaheadSearchBox.value($(".searchbar-input.tt-input").val());

        var callbackDelegate = new DropDownSelectCallbackDelegate();
        callbackDelegate.selectedChangedCallback = (state: State) => {
            var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
            this.typeaheadSearchBox.clearAndDestroy();
            this.typeaheadSearchBox.addDataSet("Title", "Název", parametersUrl);
            this.typeaheadSearchBox.create();
            this.typeaheadSearchBox.value($(".searchbar-input.tt-input").val());

            var selectedIds = this.audibookSelector.getSelectedIds();
            this.selectedBookIds = selectedIds.selectedBookIds;
            this.selectedCategoryIds = selectedIds.selectedCategoryIds;
        };
        callbackDelegate.dataLoadedCallback = () => {
            var selectedIds = this.audibookSelector.getSelectedIds();
            this.selectedBookIds = selectedIds.selectedBookIds;
            this.selectedCategoryIds = selectedIds.selectedCategoryIds;
            $("#listResults").removeClass("loader");
            this.initializeFromUrlParams();
        };
        this.audibookSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "AudioBooks/AudioBooks/GetAudioWithCategories", true, callbackDelegate);
        this.audibookSelector.makeDropdown();

        this.bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", ()=> {this.sortOrderChanged()}, BookTypeEnum.AudioBook);


        $(".searchbar-input.tt-input").change(() => { //prevent clearing input value on blur() 
            this.typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
        });

        this.initializeFromUrlParams();
    }

    private initializeFromUrlParams() {

        if (this.readyForInit && this.notInitialized) {

            this.notInitialized = false;

            var page = getQueryStringParameterByName(this.urlPageKey);

            if (page) {
                this.initPage = parseInt(page);
            }

            var sortedAsc = getQueryStringParameterByName(this.urlSortAscKey);
            var sortCriteria = getQueryStringParameterByName(this.urlSortCriteriaKey);

            if (sortedAsc && sortCriteria) {
                this.bibliographyModule.setSortedAsc(sortedAsc === "true");
                this.bibliographyModule.setSortCriteria(<SortEnum>(<any>(sortCriteria)));
            }

            var selected = getQueryStringParameterByName(this.urlSelectionKey);

            var searched = getQueryStringParameterByName(this.urlSearchKey);
            this.search.writeTextToTextField(searched);

            if (selected) {
                this.audibookSelector.setStateFromUrlString(selected);
            } else {
                this.search.processSearch(); //if not explicitly selected 
            }

        } else if (!this.notInitialized) {
            this.search.processSearch();
        } else {
            this.readyForInit = true;
        }

    }
    
    private actualizeSelectedBooksAndCategoriesInQuery() {
        this.bookIdsInQuery = this.selectedBookIds;
        this.categoryIdsInQuery = this.selectedCategoryIds;
    }

    private audioAdvancedSearchPaged(json: string, pageNumber: number) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        var start = (pageNumber - 1) * this.bibliographyModule.getBooksCountOnPage();
        var count = this.bibliographyModule.getBooksCountOnPage();
        var sortAsc = this.bibliographyModule.isSortedAsc();
        var sortingEnum = this.bibliographyModule.getSortCriteria();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "AudioBooks/AudioBooks/AdvancedSearchPaged",
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response.books);
                updateQueryStringParameter(this.urlSearchKey, json);
                updateQueryStringParameter(this.urlPageKey, pageNumber);
                updateQueryStringParameter(this.urlSortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.bibliographyModule.getSortCriteria());
            }
        });
    }

    private audioBasicSearchPaged(text: string, pageNumber: number) {
        this.hideTypeahead();
        //if (typeof text === "undefined" || text === null || text === "") return;

        var start = (pageNumber - 1) * this.bibliographyModule.getBooksCountOnPage();
        var count = this.bibliographyModule.getBooksCountOnPage();
        var sortAsc = this.bibliographyModule.isSortedAsc();
        var sortingEnum = this.bibliographyModule.getSortCriteria();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "AudioBooks/AudioBooks/TextSearchPaged",
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response.books);
                updateQueryStringParameter(this.urlSearchKey, text);
                updateQueryStringParameter(this.urlPageKey, pageNumber);
                updateQueryStringParameter(this.urlSortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.bibliographyModule.getSortCriteria());
            }
        });
    }

    private pageClickCallbackForBiblModule(pageNumber: number) {

        if (this.search.isLastQueryJson()) {
            this.audioAdvancedSearchPaged(this.search.getLastQuery(), pageNumber);
        } else {
            this.audioBasicSearchPaged(this.search.getLastQuery(), pageNumber);
        }
    }

    private audioBasicSearch(text: string) {
        this.hideTypeahead();
        this.actualizeSelectedBooksAndCategoriesInQuery();
        //if (typeof text === "undefined" || text === null || text === "") return;

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "AudioBooks/AudioBooks/TextSearchCount",
            data: { text: text, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.urlSearchKey, text);
                updateQueryStringParameter(this.urlSelectionKey, DropDownSelect2.getUrlStringFromState(this.audibookSelector.getState()));
                updateQueryStringParameter(this.urlSortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.bibliographyModule.getSortCriteria());
            }
        });
    }

    private audioAdvancedSearch(json: string) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;
        this.actualizeSelectedBooksAndCategoriesInQuery();

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "AudioBooks/AudioBooks/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: this.bookIdsInQuery, selectedCategoryIds: this.categoryIdsInQuery },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.createPagination(response["count"]); //enable pagination
                updateQueryStringParameter(this.urlSearchKey, json);
                updateQueryStringParameter(this.urlSelectionKey, DropDownSelect2.getUrlStringFromState(this.audibookSelector.getState()));
                updateQueryStringParameter(this.urlSortAscKey, this.bibliographyModule.isSortedAsc());
                updateQueryStringParameter(this.urlSortCriteriaKey, this.bibliographyModule.getSortCriteria());
            }
        });
    }

    private sortOrderChanged() {
        this.bibliographyModule.showPage(1);
    }

    private hideTypeahead() {
        $(".twitter-typeahead").find(".tt-menu").hide();
    }

    private createPagination(booksCount: number) {
        var pages = Math.ceil(booksCount / this.bookCountPerPage);
        if (this.initPage && this.initPage <= pages) {
            this.bibliographyModule.createPagination(this.bookCountPerPage, (pageNumber: number) => { this.pageClickCallbackForBiblModule(pageNumber) }, booksCount, this.initPage);
        } else {
            this.bibliographyModule.createPagination(this.bookCountPerPage, (pageNumber: number) => { this.pageClickCallbackForBiblModule(pageNumber) }, booksCount);
        }
    }

}

