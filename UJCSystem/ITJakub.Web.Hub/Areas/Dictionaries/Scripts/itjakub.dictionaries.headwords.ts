$(document).ready(() => {
    var pageSize = 50;
    var dictionaryViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription", true);
    var dictionaryViewerWrapper = new DictionaryViewerListWrapper(dictionaryViewer, pageSize);

    var searchBox = new SearchBox("#searchbox", "Dictionaries/Dictionaries");
    searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla");
    searchBox.create();

    var loadHeadwordsFunction = (state: State) => {
        dictionaryViewerWrapper.loadCount(state);
    };

    var updateSearchBox = (state: State) => {
        var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
        searchBox.clearAndDestroy();
        searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla", parametersUrl);
        searchBox.create();
    }


    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = (state: State) => {
        loadHeadwordsFunction(state);
        updateSearchBox(state);
    };

    var dictionarySelector = new DropDownSelect2("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
    
    
    $("#printDescription").click(() => {
        dictionaryViewer.print();
    });

    $("#searchButton").click(() => {
        var query = $("#searchbox").val();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordPageNumber",
            data: {
                selectedBookIds: DropDownSelect.getBookIdsFromState(dictionarySelector.getState()),
                selectedCategoryIds: DropDownSelect.getCategoryIdsFromState(dictionarySelector.getState()),
                query: query,
                pageSize: pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var resultPageNumber = response;
                dictionaryViewer.goToPage(resultPageNumber);
            }
        });
    });

    loadHeadwordsFunction(dictionarySelector.getState());
});

class DictionaryViewerListWrapper {
    private pageSize: number;
    private dictionaryViewer: DictionaryViewer;
    private selectedBookIds: Array<number>;
    private selectedCategoryIds: Array<number>;

    constructor(dictionaryViewer: DictionaryViewer, pageSize: number) {
        this.pageSize = pageSize;
        this.dictionaryViewer = dictionaryViewer;
    }

    loadCount(state: State) {
        this.selectedBookIds = DropDownSelect.getBookIdsFromState(state);
        this.selectedCategoryIds = DropDownSelect.getCategoryIdsFromState(state);
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordCount",
            data: {
                selectedBookIds: this.selectedBookIds,
                selectedCategoryIds: this.selectedCategoryIds
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var resultCount = response;
                this.dictionaryViewer.createViewer(resultCount, this.loadHeadwords.bind(this), this.pageSize);
            }
        });
    }

    loadHeadwords(pageNumber: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordList",
            data: {
                selectedBookIds: this.selectedBookIds,
                selectedCategoryIds: this.selectedCategoryIds,
                page: pageNumber,
                pageSize: this.pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.dictionaryViewer.showHeadwords(response);
            }
        });
    }
}
