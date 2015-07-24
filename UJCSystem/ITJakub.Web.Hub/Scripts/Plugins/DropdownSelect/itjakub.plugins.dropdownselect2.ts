class DropDownSelect2 extends DropDownSelect {
    private books: IDropDownBookDictionary;
    private categories: IDropDownCategoryDictionary;
    private rootCategory: DropDownCategory;
    private selectedChangedCallback: (state: State) => void;

    constructor(dropDownSelectContainer: string, dataUrl: string, showStar: boolean, callbackDelegate: DropDownSelectCallbackDelegate) {
        super(dropDownSelectContainer, dataUrl, showStar, callbackDelegate);

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
        }
    }

    protected downloadData(dropDownItemsDiv: HTMLDivElement) {
        this.books = {};

        var loadDiv = document.createElement("div");
        $(loadDiv).addClass("loading");
        $(dropDownItemsDiv).append(loadDiv);

        $.ajax({
            type: "GET",
            traditional: true,
            data: {},
            url: this.dataUrl,
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                $(dropDownItemsDiv).children("div.loading").remove();
                this.processDownloadedData(response);
                this.makeTreeStructure(this.categories, this.books, dropDownItemsDiv);
                this.rootCategory.checkBox = <HTMLInputElement>($(dropDownItemsDiv).parent().children(".dropdown-select-header").children("span.dropdown-select-checkbox").children("input").get(0));
            }
        });
    }

    private processDownloadedData(result: IDropDownRequestResult) {
        this.books = {};
        this.categories = {};

        for (var i = 0; i < result.Categories.length; i++) {
            var resultCategory = result.Categories[i];
            var category = new DropDownCategory();
            category.id = resultCategory.Id;
            category.name = resultCategory.Description;
            category.parentCategoryId = resultCategory.ParentCategoryId;
            category.bookIds = [];
            category.subcategoryIds = [];
            this.categories[category.id] = category;

            if (!category.parentCategoryId)
                this.rootCategory = category;
        }

        for (var i = 0; i < result.Categories.length; i++) {
            var resultCategory = result.Categories[i];
            if (!resultCategory.ParentCategoryId)
                continue;

            var parentCategory = this.categories[resultCategory.ParentCategoryId];
            parentCategory.subcategoryIds.push(resultCategory.Id);
        }

        for (var j = 0; j < result.Books.length; j++) {
            var resultBook = result.Books[j];
            var book = new DropDownBook();
            book.id = resultBook.Id;
            book.name = resultBook.Title;
            book.categoryIds = resultBook.CategoryIds;
            book.checkboxes = [];
            this.books[book.id] = book;

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
        var self = this;
        $(checkbox).change(function (event: Event, propagate: boolean) {
            if (this.checked) {
                self.addToSelectedItems(info);
            } else {
                self.removeFromSelectedItems(info);
            }

            if (typeof propagate === "undefined" || propagate === null || propagate) { //Deafault behaviour is to propagate change
                self.propagateSelectChange(<HTMLDivElement>$(this).parent(".concrete-item")[0]);
                self.propagateLeafSelectChange(this, info);
            }
        });

        itemDiv.appendChild(checkbox);

        if (this.showStar) {

            var saveStarSpan = document.createElement("span");
            $(saveStarSpan).addClass("save-item glyphicon glyphicon-star-empty");

            $(saveStarSpan).click(function () {
                $(this).siblings(".delete-item").show();
                $(this).hide();
                //TODO populate request on save to favorites
                if (self.callbackDelegate.starSaveItemCallback) {
                    self.callbackDelegate.starSaveItemCallback(info);
                }
            });

            itemDiv.appendChild(saveStarSpan);

            var deleteStarSpan = document.createElement("span");
            $(deleteStarSpan).addClass("delete-item glyphicon glyphicon-star");

            $(deleteStarSpan).click(function () {
                $(this).siblings(".save-item").show();
                $(this).hide();
                //TODO populate request on delete from favorites
                if (self.callbackDelegate.starDeleteItemCallback) {
                    self.callbackDelegate.starDeleteItemCallback(info);
                }
            });

            itemDiv.appendChild(deleteStarSpan);
        }

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("concrete-item-name");
        nameSpan.innerHTML = this.books[currentLeafItem].name;

        itemDiv.appendChild(nameSpan);

        container.appendChild(itemDiv);
    }

    protected onCreateCategoryCheckBox(categoryId, checkBox: HTMLInputElement) {
        this.categories[categoryId].checkBox = checkBox;
    }

    protected propagateRootSelectChange(item: HTMLInputElement) {
        this.selectedChangedCallback(this.getState());
    }

    protected propagateLeafSelectChange(item: HTMLInputElement, info: CallbackInfo) {
        var sameBookCheckBoxes = this.books[info.ItemId].checkboxes;
        var checkBoxState = $(item).prop("checked");
        for (var i = 0; i < sameBookCheckBoxes.length; i++) {
            var otherCheckBox = sameBookCheckBoxes[i];
            if ($(otherCheckBox).prop("checked") !== checkBoxState) {
                $(otherCheckBox).prop("checked", checkBoxState);
                this.propagateSelectChange(<HTMLDivElement>$(otherCheckBox).parent(".concrete-item")[0]);
            }
        }

        this.selectedChangedCallback(this.getState());
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
                    this.propagateSelectChange(<HTMLDivElement>$(bookCheckBox).parent(".concrete-item")[0]);
                }
            }
        }

        this.selectedChangedCallback(this.getState());
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

        for (var i = 0; i < selectedIds.selectedCategoryIds.length; i++) {
            var id = selectedIds.selectedCategoryIds[i];
            state.SelectedCategories.push(new Category(String(id), this.categories[id].name));
        }
        for (var i = 0; i < selectedIds.selectedBookIds.length; i++) {
            var id = selectedIds.selectedBookIds[i];
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
        } else {
            this.getSelected(this.rootCategory, selectedCategories, selectedBooks);
            state.selectedBookIds = selectedBooks;
            state.selectedCategoryIds = selectedCategories;
        }

        return state;
    }

    static getUrlStringFromState(state: State): string {
        var selectedBooks = state.SelectedItems;
        var selectedCategories = state.SelectedCategories;
        var resultString = "";

        for (var i = 0; i < selectedBooks.length; i++) {
            if (resultString.length > 0)
                resultString += "&";
            resultString += "selectedBookIds=" + selectedBooks[i].Id;
        }

        for (var i = 0; i < selectedCategories.length; i++) {
            if (resultString.length > 0)
                resultString += "&";
            resultString += "selectedCategoryIds=" + selectedCategories[i].Id;
        }

        return resultString;
    }
}

class DropDownSelected {
    selectedBookIds: Array<number>;
    selectedCategoryIds: Array<number>;
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

interface IDropDownBookResult {
    Id: number;
    Title: string;
    CategoryIds: Array<number>;
}

interface IDropDownCategoryResult {
    Id: number;
    Description: string;
    ParentCategoryId: number;
}

interface IDropDownRequestResult {
    Books: Array<IDropDownBookResult>;
    Categories: Array<IDropDownCategoryResult>;
}
