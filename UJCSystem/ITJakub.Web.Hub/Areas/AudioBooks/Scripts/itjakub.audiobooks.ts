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
    private bookIds: Array<number>;
    private categoryIds: Array<number>;

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
            this.bookIds = selectedIds.selectedBookIds;
            this.categoryIds = selectedIds.selectedCategoryIds;
        };
        callbackDelegate.dataLoadedCallback = () => {
            var selectedIds = this.audibookSelector.getSelectedIds();
            this.bookIds = selectedIds.selectedBookIds;
            this.categoryIds = selectedIds.selectedCategoryIds;
            $("#listResults").removeClass("loader");
            this.search.processSearch();
            //this.search.processSearchQuery("%"); //search for all by default criteria (title)
            //this.search.writeTextToTextField("");
        };
        this.audibookSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "AudioBooks/AudioBooks/GetAudioWithCategories", true, callbackDelegate);
        this.audibookSelector.makeDropdown();

        this.bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", ()=> {this.sortOrderChanged()}, BookTypeEnum.AudioBook);


        $(".searchbar-input.tt-input").change(() => { //prevent clearing input value on blur() 
            this.typeaheadSearchBox.value($(".searchbar-input.tt-input").val());
        });
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
            data: { json: json, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response.books);
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
            data: { text: text, start: start, count: count, sortingEnum: sortingEnum, sortAsc: sortAsc, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.showBooks(response.books);
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
        //if (typeof text === "undefined" || text === null || text === "") return;

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "AudioBooks/AudioBooks/TextSearchCount",
            data: { text: text, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.createPagination(this.bookCountPerPage, (pageNumber: number) => {this.pageClickCallbackForBiblModule(pageNumber)}, response["count"]); //enable pagination
            }
        });
    }

    private audioAdvancedSearch(json: string) {
        this.hideTypeahead();
        if (typeof json === "undefined" || json === null || json === "") return;

        this.bibliographyModule.clearBooks();
        this.bibliographyModule.showLoading();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "AudioBooks/AudioBooks/AdvancedSearchResultsCount",
            data: { json: json, selectedBookIds: this.bookIds, selectedCategoryIds: this.categoryIds },
            dataType: "json",
            contentType: "application/json",
            success: response => {
                this.bibliographyModule.createPagination(this.bookCountPerPage, (pageNumber: number) => { this.pageClickCallbackForBiblModule(pageNumber) }, response["count"]); //enable pagination
            }
        });
    }

    private sortOrderChanged() {
        var textInTextField = this.search.getTextFromTextField();
        this.search.processSearchQuery(this.search.getLastQuery());
        this.search.writeTextToTextField(textInTextField);
    }

    private hideTypeahead() {
        $(".twitter-typeahead").find(".tt-menu").hide();
    }

}

