﻿class DropDownSelect2 extends DropDownSelect {
    private bookIdList: Array<number>;
    private categoryIdList: Array<number>;
    private books: IDropDownBookDictionary;
    private categories: IDropDownCategoryDictionary;
    private rootCategory: DropDownCategory;
    private selectedChangedCallback: (state: State) => void;
    private restoreCategoryIds: Array<number>;
    private restoreBookIds: Array<number>;
    private descriptionDiv: HTMLDivElement;
    private cancelDiv: HTMLDivElement;
    private isLoaded: boolean;
    private favoriteBook: FavoriteBook;
    private bookType: BookTypeEnum;
    private overrideSelectedBookCountVal: number;

    private static selectedBookUrlKey = "selectedBookIds";
    private static selectedCategoryUrlKey = "selectedCategoryIds";

    constructor(dropDownSelectContainer: string, dataUrl: string, bookType:BookTypeEnum, showStar: boolean, callbackDelegate: DropDownSelectCallbackDelegate) {
        super(dropDownSelectContainer, dataUrl, showStar, callbackDelegate);

        this.bookType = bookType;
        this.selectedChangedCallback = callbackDelegate.selectedChangedCallback;
        callbackDelegate.selectedChangedCallback = null;

        callbackDelegate.getRootCategoryCallback = (categories) => {
            return this.rootCategory.id;
        };
        callbackDelegate.getCategoryIdCallback = (category) => {
            return String(this.categories[category].id);
        };
        callbackDelegate.getCategoryTextCallback = (category) => {
            return this.categories[category].name;
        };
        callbackDelegate.getChildCategoriesCallback = (categories, currentCategory) => {
            return this.categories[currentCategory].subcategoryIds;
        };
        callbackDelegate.getChildLeafItemsCallback = (items, currentCategory) => {
            return this.categories[currentCategory].bookIds;
        };
        callbackDelegate.getLeafItemIdCallback = (item) => {
            return String(this.books[item].id);
        };
        callbackDelegate.getLeafItemTextCallback = (item) => {
            return this.books[item].name;
        };
        this.isLoaded = false;
        this.overrideSelectedBookCountVal = null;
    }

    makeAndRestore(categoryIds: Array<number>, bookIds: Array<number>) {
        this.restoreCategoryIds = categoryIds;
        this.restoreBookIds = bookIds;

        this.makeDropdown();
    }

    makeDropdown() {
        super.makeDropdown();

        this.isLoaded = false;
        this.descriptionDiv = document.createElement("div");
        $(this.descriptionDiv).addClass("dropdown-description");
        $(this.dropDownSelectContainer).append(this.descriptionDiv);

        this.cancelDiv = document.createElement("div");
        $(this.cancelDiv).addClass("dropdown-cancel");
        $(this.dropDownSelectContainer).append(this.cancelDiv);
        
        if (this.showStar) {
            var cancelButton = this.createCancelButton();
            $(this.cancelDiv).append(cancelButton);

            var favoriteDropdownDiv = document.createElement("div");
            this.favoriteBook = new FavoriteBook($(favoriteDropdownDiv), this.bookType, this);
            this.favoriteBook.make();

            $(this.dropDownSelectContainer).append(favoriteDropdownDiv);
        }
    }

    private createCancelButton(): HTMLElement {
        const elem = $(`<a href="#" class="btn btn-sm btn-default" title="${localization.translate("CancelBookFilter", "PluginsJs").value}"><span class="glyphicon glyphicon-remove"></span></a>`);
        elem.click(() => {
            this.favoriteBook.resetSelected();
        });
        return elem.get(0);
    }

    setSelected(categoryIds: Array<number>, bookIds: Array<number>) {
        if (categoryIds.length === 0 && bookIds.length === 0) {
            this.rootCategory.checkBox.checked = false;
            this.rootCategory.checkBox.indeterminate = false;
        }
        $("input[type=checkbox]", this.dropDownBodyDiv).prop("checked", false);
        this.restore(categoryIds, bookIds);
    }

    restore(categoryIds: Array<number>, bookIds: Array<number>) {
        this.restoreCategoryIds = categoryIds;
        this.restoreBookIds = bookIds;

        this.doRestore();
    }

    protected onFavoritesChanged(favoriteType: FavoriteType, id: number): void {
        this.favoriteBook.loadData();
        new NewFavoriteNotification().show();
    }

    private doRestore() {
        var categoriesCount = 0;
        var booksCount = 0;

        if (this.restoreCategoryIds) {
            for (var i = 0; i < this.restoreCategoryIds.length; i++) {
                var category = this.categories[this.restoreCategoryIds[i]];
                if (!category) {
                    continue;
                }
                category.checkBox.checked = true;
                this.propagateSelectChange($(category.checkBox).parent(".concrete-item")[0] as Node as HTMLDivElement);
                categoriesCount++;
            }
        }

        if (this.restoreBookIds) {
            for (var j = 0; j < this.restoreBookIds.length; j++) {
                var book = this.books[this.restoreBookIds[j]];
                if (!book) {
                    continue;
                }

                for (var k = 0; k < book.checkboxes.length; k++) {
                    var checkbox = book.checkboxes[k];
                    if (checkbox.checked)
                        continue;

                    checkbox.checked = true;
                    this.propagateSelectChange($(checkbox).parent(".concrete-item")[0] as Node as HTMLDivElement);
                }
            }
            booksCount = this.getSelectedBookCount();
        }
        
        this.updateSelectionInfo(categoriesCount, booksCount);
    }

    protected downloadData(dropDownItemsDiv: HTMLDivElement) {
        this.books = {};

        var loadDiv = lv.create(null, "lv-circles lv-mid sm lvb-1 lvt-1");
        $(dropDownItemsDiv).append(loadDiv.getElement());
        
        $.ajax({
            type: "GET",
            traditional: true,
            data: {},
            url: this.dataUrl,
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                loadDiv.remove();
                this.processDownloadedData(response);
                this.makeTreeStructure(this.categories, this.books, dropDownItemsDiv);
                this.rootCategory.checkBox = ($(dropDownItemsDiv).parent().children(".dropdown-select-header").children("span.dropdown-select-checkbox").children("input").get(0) as Node as HTMLInputElement);
                this.doRestore();
                this.isLoaded = true;
                this.dataLoaded(this.rootCategory.id);

                this.downloadFavoriteData(dropDownItemsDiv);
            },
            error: () => {
                $(dropDownItemsDiv).children("div.lv-circles").remove();
                $(dropDownItemsDiv).siblings(".dropdown-select-header").children("div.lv-circles").remove();
                const selectHeader = $(dropDownItemsDiv).parent().children(".dropdown-select-header");
                $(selectHeader).children(".dropdown-select-text").append(localization.translate("LoadingContentError", "PluginsJs").value);
                this.isLoaded = true;
                this.dataLoaded(null);
                this.downloadFavoriteData(dropDownItemsDiv);
            }
        });
    }

    public downloadFavoriteData(dropDownItemsDiv: HTMLDivElement): void {
        if (!this.showStar) {
            return;
        }

        var loadedFavoriteCategories: Array<IDropdownFavoriteItem> = null;
        var loadedFavoriteBooks: Array<IDropdownFavoriteItem> = null;
        var loadedFavoriteLabels: Array<IFavoriteLabel> = null;

        var isAllLoaded = () => (loadedFavoriteCategories != null &&
            loadedFavoriteBooks != null &&
            loadedFavoriteLabels != null);

        this.favoriteManager.getFavoritesForCategories(this.categoryIdList, (favoriteCategories) => {
            loadedFavoriteCategories = favoriteCategories;

            if (isAllLoaded()) {
                this.updateFavoriteIcons(loadedFavoriteCategories, loadedFavoriteBooks, loadedFavoriteLabels, dropDownItemsDiv);
            }
        });

        this.favoriteManager.getFavoritesForBooks(null, this.bookIdList, (favoriteBooks) => {
            loadedFavoriteBooks = favoriteBooks;

            if (isAllLoaded()) {
                this.updateFavoriteIcons(loadedFavoriteCategories, loadedFavoriteBooks, loadedFavoriteLabels, dropDownItemsDiv);
            }
        });

        this.favoriteManager.getLatestFavoriteLabels(favoriteLabels => {
            loadedFavoriteLabels = favoriteLabels;

            if (isAllLoaded()) {
                this.updateFavoriteIcons(loadedFavoriteCategories, loadedFavoriteBooks, loadedFavoriteLabels, dropDownItemsDiv);
            }
        });
    }

    private processDownloadedData(result: IDropDownRequestResult) {
        this.books = {};
        this.categories = {};
        this.bookIdList = [];
        this.categoryIdList = [];

        for (let i = 0; i < result.categories.length; i++) {
            let resultCategory = result.categories[i];
            var category = new DropDownCategory();
            category.id = resultCategory.id;
            category.name = resultCategory.description;
            category.parentCategoryId = resultCategory.parentCategoryId;
            category.bookIds = [];
            category.subcategoryIds = [];
            this.categories[category.id] = category;
            this.categoryIdList.push(category.id);

            if (category.parentCategoryId == null)
                this.rootCategory = category;
        }

        for (let i = 0; i < result.categories.length; i++) {
            let resultCategory = result.categories[i];
            if (resultCategory.parentCategoryId == null)
                continue;

            var parentCategory = this.categories[resultCategory.parentCategoryId];
            parentCategory.subcategoryIds.push(resultCategory.id);
        }

        for (var j = 0; j < result.books.length; j++) {
            var resultBook = result.books[j];
            var book = new DropDownBook();
            book.id = resultBook.id;
            book.name = resultBook.title;
            book.categoryIds = resultBook.categoryIds;
            book.checkboxes = [];
            this.books[book.id] = book;
            this.bookIdList.push(book.id);

            for (var k = 0; k < book.categoryIds.length; k++) {
                var categoryId = book.categoryIds[k];
                if (this.categories[categoryId])
                    this.categories[categoryId].bookIds.push(book.id);
            }
        }
    }
    
    protected makeLeafItem(container: HTMLDivElement, currentLeafItem: any) {
        var itemDiv = document.createElement("div");
        $(itemDiv).addClass("concrete-item"); //TODO add data-item-is-favorite

        $(itemDiv).data("id", this.books[currentLeafItem].id);
        $(itemDiv).data("name", this.books[currentLeafItem].name);
        $(itemDiv).data("type", "item");

        var checkbox = document.createElement("input");
        $(checkbox).addClass("concrete-item-checkbox checkbox");
        checkbox.type = "checkbox";

        this.books[currentLeafItem].checkboxes.push(checkbox);

        var info = this.createCallbackInfo(String(this.books[currentLeafItem].id), this.books[currentLeafItem].name, itemDiv);
        $(checkbox).change((event: JQuery.Event, propagate: boolean) => {
            if (checkbox.checked) {
                this.addToSelectedItems(info);
            } else {
                this.removeFromSelectedItems(info);
            }

            if (typeof propagate === "undefined" || propagate === null || propagate) { //Deafault behaviour is to propagate change
                this.propagateSelectChange($(checkbox).parent(".concrete-item")[0] as Node as HTMLDivElement);
                this.propagateLeafSelectChange(checkbox, info);
            }
        });

        itemDiv.appendChild(checkbox);

        if (this.showStar) {
            var favoriteStarContainer = document.createElement("span");
            $(favoriteStarContainer).addClass("save-item");
            
            itemDiv.appendChild(favoriteStarContainer);
        }

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("concrete-item-name");
        nameSpan.innerHTML = this.books[currentLeafItem].name;

        itemDiv.appendChild(nameSpan);

        container.appendChild(itemDiv);
    }

    private updateSelectionInfo(categoriesCount: number, booksCount: number) {
        var categoriesCountString = String(categoriesCount);
        var booksCountString = String(booksCount);

        if (!this.rootCategory.checkBox.indeterminate) {
            categoriesCountString = localization.translate("All1", "PluginsJs").value;
            booksCountString = localization.translate("All2", "PluginsJs").value;
        }

        if (this.overrideSelectedBookCountVal != null) {
            booksCountString = this.overrideSelectedBookCountVal.toString();
        }
        
        var infoDiv = document.createElement("div");
        var infoText = localization.translate("SearchCategories", "PluginsJs").value + categoriesCountString + "<br>"
            + localization.translate("SearchBooks", "PluginsJs").value + booksCountString;
        
        infoDiv.innerHTML = infoText;

        $(this.descriptionDiv).empty();
        $(this.descriptionDiv).append(infoDiv);
    }

    private getSelectedBookCount(): number {
        var count = 0;
        for (var i = 0; i < this.bookIdList.length; i++) {
            var bookId = this.bookIdList[i];
            var firstCheckBox = this.books[bookId].checkboxes[0];
            if (firstCheckBox.checked)
                count++;
        }
        return count;
    }

    protected onCreateCategoryCheckBox(categoryId, checkBox: HTMLInputElement) {
        this.categories[categoryId].checkBox = checkBox;
    }

    private onSelectionChanged() {
        var currentState = this.getState();
        var categoriesCount = currentState.SelectedCategories.length;
        var booksCount = this.getSelectedBookCount();

        this.selectedChangedCallback(currentState);
        this.updateSelectionInfo(categoriesCount, booksCount);
    }

    protected propagateRootSelectChange(item: HTMLInputElement) {
        this.onSelectionChanged();
    }

    protected propagateLeafSelectChange(item: HTMLInputElement, info: CallbackInfo) {
        var sameBookCheckBoxes = this.books[info.ItemId].checkboxes;
        var checkBoxState = $(item).prop("checked");
        for (var i = 0; i < sameBookCheckBoxes.length; i++) {
            var otherCheckBox = sameBookCheckBoxes[i];
            if ($(otherCheckBox).prop("checked") !== checkBoxState) {
                $(otherCheckBox).prop("checked", checkBoxState);
                this.propagateSelectChange($(otherCheckBox).parent(".concrete-item")[0] as Node as HTMLDivElement);
            }
        }

        this.onSelectionChanged();
    }

    protected propagateCategorySelectChange(item: HTMLInputElement, info: CallbackInfo) {
        var isChecked = $(item).prop("checked");
        var category = this.categories[info.ItemId];
        var bookIds: Array<number> = [];

        this.getBookIdsForUpdate(category, bookIds);

        for (var i = 0; i < bookIds.length; i++) {
            var book = this.books[bookIds[i]];

            for (var j = 0; j < book.checkboxes.length; j++) {
                var bookCheckBox = book.checkboxes[j];
                if ($(bookCheckBox).prop("checked") !== isChecked) {
                    $(bookCheckBox).prop("checked", isChecked);
                    this.propagateSelectChange($(bookCheckBox).parent(".concrete-item")[0] as Node as HTMLDivElement);
                }
            }
        }

        this.onSelectionChanged();
    }

    private getBookIdsForUpdate(category: DropDownCategory, bookIds: Array<number>) {
        for (var i = 0; i < category.bookIds.length; i++) {
            var bookId = category.bookIds[i];
            if ($.inArray(bookId, bookIds) === -1) {
                bookIds.push(bookId);
            }
        }
        for (var j = 0; j < category.subcategoryIds.length; j++) {
            var subcategoryId = category.subcategoryIds[j];
            var subcategory = this.categories[subcategoryId];
            this.getBookIdsForUpdate(subcategory, bookIds);
        }
    }

    private getSelected(category: DropDownCategory, selectedCategories: Array<number>, selectedBooks: Array<number>) {
        if (!category.checkBox.indeterminate) {
            if (category.checkBox.checked) {
                selectedCategories.push(category.id);
            }
            return;
        }

        for (var i = 0; i < category.subcategoryIds.length; i++) {
            var subcategory = this.categories[category.subcategoryIds[i]];
            this.getSelected(subcategory, selectedCategories, selectedBooks);
        }

        for (var j = 0; j < category.bookIds.length; j++) {
            var book = this.books[category.bookIds[j]];
            if (book.checkboxes[0].checked)
                selectedBooks.push(book.id);
        }
    }

    getState(): State {
        var selectedIds = this.getSelectedIds();
        var state = new State();
        state.SelectedCategories = [];
        state.SelectedItems = [];
        state.IsOnlyRootSelected = selectedIds.isOnlyRootSelected;

        for (let i = 0; i < selectedIds.selectedCategoryIds.length; i++) {
            let id = selectedIds.selectedCategoryIds[i];
            state.SelectedCategories.push(new Category(String(id), this.categories[id].name));
        }
        for (let i = 0; i < selectedIds.selectedBookIds.length; i++) {
            let id = selectedIds.selectedBookIds[i];
            state.SelectedItems.push(new Item(String(id), this.books[id].name));
        }

        return state;
    }

    getSelectedIds(): DropDownSelected {
        var state = new DropDownSelected();
        var selectedBooks = [];
        var selectedCategories = [];
        if (!this.rootCategory || !this.rootCategory.checkBox.indeterminate) {
            state.selectedBookIds = [];
            state.selectedCategoryIds = [];
            state.isOnlyRootSelected = true;
            if (this.rootCategory) {
                state.selectedCategoryIds.push(this.rootCategory.id);
            }
        } else {
            this.getSelected(this.rootCategory, selectedCategories, selectedBooks);
            state.selectedBookIds = selectedBooks;
            state.selectedCategoryIds = selectedCategories;
            state.isOnlyRootSelected = false;
        }

        return state;
    }

    isDataLoaded(): boolean {
        return this.isLoaded;
    }

    static getUrlStringFromState(state: State): string {
        var selectedBooks = state.SelectedItems;
        var selectedCategories = state.SelectedCategories;
        var resultString = "";

        for (let i = 0; i < selectedBooks.length; i++) {
            if (resultString.length > 0)
                resultString += "&";
            resultString += DropDownSelect2.selectedBookUrlKey + "=" + selectedBooks[i].Id;
        }

        for (let i = 0; i < selectedCategories.length; i++) {
            if (resultString.length > 0)
                resultString += "&";
            resultString += DropDownSelect2.selectedCategoryUrlKey + "=" + selectedCategories[i].Id;
        }

        return resultString;
    }

    getSerializedState(): string {
        var state = this.getSelectedIds();
        return JSON.stringify(state);
    }

    restoreFromSerializedState(serializedState: string) {
        var state: DropDownSelected = JSON.parse(serializedState);
        this.restore(state.selectedCategoryIds, state.selectedBookIds);
    }

    getFavoriteBookComponent(): FavoriteBook {
        return this.favoriteBook;
    }

    overrideSelectedBookCount(newCount: number) {
        this.overrideSelectedBookCountVal = newCount;
    }

    hasBooksLoaded(): boolean {
        return this.bookIdList && this.bookIdList.length > 0;
    }
}

class DropDownSelected {
    selectedBookIds: Array<number>;
    selectedCategoryIds: Array<number>;
    isOnlyRootSelected: boolean;
}

class DropDownBook {
    id: number;
    name: string;
    checkboxes: Array<HTMLInputElement>;
    categoryIds: Array<number>;
}

class DropDownCategory {
    id: number;
    name: string;
    parentCategoryId: number;
    subcategoryIds: Array<number>;
    bookIds: Array<number>;
    checkBox: HTMLInputElement;
}

interface IDropDownBookDictionary {
    [bookId: string]: DropDownBook;
}

interface IDropDownCategoryDictionary {
    [categoryId: string]: DropDownCategory;
}

