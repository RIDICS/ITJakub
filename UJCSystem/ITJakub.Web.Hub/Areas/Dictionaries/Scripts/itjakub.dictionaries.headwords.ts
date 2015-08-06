function initDictionaryViewer(categoryIdList: string, bookIdList: string, pageNumber: string) {
    var selectedCategoryIds: Array<number> = [];
    var selectedBookIds: Array<number> = [];
    var defaultPageNumber: number = Number(pageNumber);
    
    try {
        selectedCategoryIds = JSON.parse(categoryIdList);
    } catch (e) { }
    try {
        selectedBookIds = JSON.parse(bookIdList);
    } catch (e) { }
    
    var pageSize = 50;
    var dictionaryViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription", true);
    var dictionaryViewerWrapper = new DictionaryViewerListWrapper(dictionaryViewer, pageSize);

    var searchBox = new SearchBox("#searchbox", "Dictionaries/Dictionaries");
    searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla");
    searchBox.create();

    var updateSearchBox = (state: State) => {
        var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
        searchBox.clearAndDestroy();
        searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla", parametersUrl);
        searchBox.create();
    }


    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = (state: State) => {
        dictionaryViewerWrapper.loadHeadwordList(state);
        updateSearchBox(state);
    };

    var dictionarySelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate, "#dropdownDescriptionDiv");
    dictionarySelector.makeAndRestore(selectedCategoryIds, selectedBookIds);


    $("#cancelFilter").click(() => {
        dictionaryViewer.cancelFilter();
        $("#cancelFilter").addClass("hidden");
    });

    $("#printDescription").click(() => {
        dictionaryViewer.print();
    });

    $("#printList").click(() => {
        dictionaryViewer.printList();
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
    
    dictionaryViewerWrapper.loadDefault(selectedCategoryIds, selectedBookIds, defaultPageNumber);
};

class DictionaryViewerListWrapper {
    private favoriteHeadwords: DictionaryFavoriteHeadwords;
    private pageSize: number;
    private currentPageNumber: number;
    private dictionaryViewer: DictionaryViewer;
    private selectedBookIds: Array<number>;
    private selectedCategoryIds: Array<number>;

    constructor(dictionaryViewer: DictionaryViewer, pageSize: number) {
        this.pageSize = pageSize;
        this.dictionaryViewer = dictionaryViewer;

        window.matchMedia("print").addListener(mql => {
            if (mql.matches) {
                this.dictionaryViewer.loadAllHeadwords();
            }
        });

        this.favoriteHeadwords = new DictionaryFavoriteHeadwords("#saved-word-area", "#saved-word-area .saved-words-body", "#saved-word-area .saved-word-area-more");
        this.favoriteHeadwords.create(this.goToPageWithHeadword.bind(this));
    }

    private goToPageWithHeadword(bookId: string, entryXmlId: string) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordPageNumberById",
            data: {
                selectedBookIds: this.selectedBookIds,
                selectedCategoryIds: this.selectedCategoryIds,
                headwordBookId: bookId,
                headwordEntryXmlId: entryXmlId,
                pageSize: this.pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var resultPageNumber = response;
                this.dictionaryViewer.goToPage(resultPageNumber);
            }
        });
    }

    private addNewFavoriteHeadword(bookId: string, entryXmlId: string) {
        this.favoriteHeadwords.addNewHeadword(bookId, entryXmlId);
    }

    loadDefault(categoryIds: Array<number>, bookIds: Array<number>, defaultPageNumber: number) {
        this.selectedBookIds = bookIds;
        this.selectedCategoryIds = categoryIds;
        this.dictionaryViewer.setDefaultPageNumber(defaultPageNumber);

        this.loadCount();
    }

    loadHeadwordList(state: State) {
        this.selectedBookIds = DropDownSelect.getBookIdsFromState(state);
        this.selectedCategoryIds = DropDownSelect.getCategoryIdsFromState(state);
        this.dictionaryViewer.setDefaultPageNumber(null);

        this.loadCount();
    }

    private loadCount() {
        this.dictionaryViewer.showLoading();
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
                if (resultCount === 0) {
                    this.currentPageNumber = 1;
                    this.updateUrl();
                }

                this.dictionaryViewer.createViewer(resultCount, this.loadHeadwords.bind(this), this.pageSize, null, null, this.addNewFavoriteHeadword.bind(this));
            }
        });
    }

    loadHeadwords(pageNumber: number) {
        this.currentPageNumber = pageNumber;
        this.updateUrl();
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

    private updateUrl() {
        var url = "?categories=" + JSON.stringify(this.selectedCategoryIds)
            + "&books=" + JSON.stringify(this.selectedBookIds)
            + "&page=" + this.currentPageNumber;

        window.history.replaceState(null, null, url);
    }
}
