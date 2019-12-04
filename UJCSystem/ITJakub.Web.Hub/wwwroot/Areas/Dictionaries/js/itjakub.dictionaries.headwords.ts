﻿function initDictionaryViewer(categoryIdList: string, bookIdList: string, pageNumber: string) {
    var selectedCategoryIds: Array<number> = [];
    var selectedBookIds: Array<number> = [];
    var defaultPageNumber: number = Number(pageNumber);

    try {
        selectedCategoryIds = JSON.parse(categoryIdList);
    } catch (e) { }
    try {
        selectedBookIds = JSON.parse(bookIdList);
    } catch (e) { }
    
    const headwordListSelector = "#headwordList";
    const paginationSelector = "#pagination";
    const headwordDescriptionSelector = "#headwordDescription";
    var pageSize = 50;
    
    var dictionaryViewer = new DictionaryViewer(headwordListSelector, paginationSelector, headwordDescriptionSelector, true);
    var dictionaryViewerWrapper = new DictionaryViewerListWrapper(dictionaryViewer, pageSize);

    var searchBox = new SearchBox("#searchbox", "Dictionaries/Dictionaries");
    searchBox.addDataSet("DictionaryHeadword", localization.translate("DictionaryTerms", "Dictionaries").value);
    searchBox.create();

    var inputElement = $("#searchbox").get(0) as Node as HTMLInputElement;
    var keyboardButton = $("#keyboard-button").get(0) as Node as HTMLButtonElement;
    var keyboardComponent = KeyboardManager.getKeyboard("0");
    //keyboardComponent.registerInput($("#searchbox")[0]);
    keyboardComponent.registerButton(keyboardButton, inputElement, newQuery => searchBox.value(newQuery));

    var updateSearchBox = (state: State) => {
        var parametersUrl = DropDownSelect2.getUrlStringFromState(state);
        searchBox.clearAndDestroy();
        searchBox.addDataSet("DictionaryHeadword", localization.translate("DictionaryTerms", "Dictionaries").value, parametersUrl);
        searchBox.create();
    }


    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = (state: State) => {
        if (state.IsOnlyRootSelected)
            state.SelectedCategories = [];

        dictionaryViewerWrapper.callAfterFavouriteHeadwordsInit(() => {
            dictionaryViewerWrapper.loadHeadwordList(state);
            updateSearchBox(state);
        });
    };

    var dictionarySelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", BookTypeEnum.Dictionary, true, callbackDelegate);
    dictionarySelector.makeAndRestore(selectedCategoryIds, selectedBookIds);


    $("#cancelFilter").on("click",() => {
        dictionaryViewer.cancelFilter();
        $("#cancelFilter").addClass("hidden");
    });

    $("#printDescription").on("click",() => {
        dictionaryViewer.print();
    });

    $("#printList").on("click",() => {
        dictionaryViewer.printList();
    });

    const errorHandler = new ErrorHandler();
    $("#searchButton").on("click", () => {
        var query = $("#searchbox").val() as string;
        var selectedIds = dictionarySelector.getSelectedIds();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordPageNumber",
            data: {
                selectedBookIds: selectedIds.selectedBookIds,
                selectedCategoryIds: selectedIds.isOnlyRootSelected ? [] : selectedIds.selectedCategoryIds,
                query: query,
                pageSize: pageSize
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var resultPageNumber = response;
                dictionaryViewer.goToPage(resultPageNumber);
            },
            error: (jqXHR) => {
                const alert = new AlertComponentBuilder(AlertType.Error).addContent(errorHandler.getErrorMessage(jqXHR));
                $(headwordDescriptionSelector).empty().append(alert.buildElement());
                $(headwordListSelector).empty().append(alert.buildElement());
                $(paginationSelector).empty();
            }            
        });
    });

    dictionaryViewerWrapper.callAfterFavouriteHeadwordsInit(() => {
        dictionaryViewerWrapper.loadDefault(selectedCategoryIds, selectedBookIds, defaultPageNumber);
    });
};

class DictionaryViewerListWrapper {
    private readonly errorHandler: ErrorHandler;
    private favoriteHeadwords: DictionaryFavoriteHeadwords;
    private pageSize: number;
    private currentPageNumber: number;
    private dictionaryViewer: DictionaryViewer;
    private selectedBookIds: Array<number>;
    private selectedCategoryIds: Array<number>;

    constructor(dictionaryViewer: DictionaryViewer, pageSize: number) {
        this.pageSize = pageSize;
        this.dictionaryViewer = dictionaryViewer;
        //this.dictionaryViewer.setFavoriteCallback(this.addNewFavoriteHeadword.bind(this), this.removeFavoriteHeadword.bind(this));
        this.errorHandler = new ErrorHandler();
        
        window.matchMedia("print").addListener(mql => {
            if (mql.matches) {
                this.dictionaryViewer.loadAllHeadwords();
            }
        });

        //this.favoriteHeadwords = new DictionaryFavoriteHeadwords("#saved-word-area", "#saved-word-area .saved-words-body", "#saved-word-area .saved-word-area-more");
        //this.favoriteHeadwords.create(this.goToPageWithHeadword.bind(this), this.favoriteHeadwordsChanged.bind(this));

        //disabled favorites:
        $("#saved-word-area").addClass("hidden");
    }

    callAfterFavouriteHeadwordsInit(callback: () => any) {
        //this.favoriteHeadwords.callAfterInit(callback);

        //disabled favorites:
        this.favoriteHeadwordsChanged([]);
        callback();
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
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var resultPageNumber = response;
                this.dictionaryViewer.goToPage(resultPageNumber);
            },
            error: (jqXHR) => {
                this.showErrors(jqXHR);
            }
        });
    }

    private favoriteHeadwordsChanged(list: Array<IDictionaryFavoriteHeadword>) {
        this.dictionaryViewer.setFavoriteHeadwordList(list);
    }

    private addNewFavoriteHeadword(bookId: string, entryXmlId: string) {
        this.favoriteHeadwords.addNewHeadword(bookId, entryXmlId);
    }

    private removeFavoriteHeadword(bookId: string, entryXmlId: string) {
        this.favoriteHeadwords.removeHeadwordById(bookId, entryXmlId);
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
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var resultCount = response;
                if (resultCount === 0) {
                    this.currentPageNumber = 1;
                    this.updateUrl();
                }

                this.dictionaryViewer.createViewer(resultCount, this.loadHeadwords.bind(this), this.pageSize, null, null);
            },
            error: (jqXHR) => {
                this.showErrors(jqXHR);
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
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                this.dictionaryViewer.showHeadwords(response);
            },
            error: (jqXHR) => {
                this.showErrors(jqXHR);
            }
        });
    }

    private updateUrl() {
        updateQueryStringParameter("categories", JSON.stringify(this.selectedCategoryIds));
        updateQueryStringParameter("books", JSON.stringify(this.selectedBookIds));
        updateQueryStringParameter("page", this.currentPageNumber);
    }
    
    private showErrors(jqXHR: JQueryXHR) {
        const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(jqXHR));
        $("#headwordDescription").empty().append(alert.buildElement());
        $("#headwordList").empty().append(alert.buildElement());
        $("#pagination").empty();
    }
}
