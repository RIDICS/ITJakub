class DropDownSelect2 extends DropDownSelect {
    private books: IDropDownBookDictionary;
    private categories: IDropDownCategoryDictionary;
    private rootCategory: DropDownCategory;

    constructor(dropDownSelectContainer: string, dataUrl: string, showStar: boolean, callbackDelegate: DropDownSelectCallbackDelegate) {
        super(dropDownSelectContainer, dataUrl, showStar, callbackDelegate);

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
            }

            var sameBookCheckBoxes = self.books[info.ItemId].checkboxes;
            for (var i = 0; i < sameBookCheckBoxes.length; i++) {
                var otherCheckBox = sameBookCheckBoxes[i];
                if (otherCheckBox !== this) {
                    $(otherCheckBox).prop("checked", $(this).prop("checked"));
                }
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
