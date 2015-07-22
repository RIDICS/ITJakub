var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var DropDownSelect2 = (function (_super) {
    __extends(DropDownSelect2, _super);
    function DropDownSelect2(dropDownSelectContainer, dataUrl, showStar, callbackDelegate) {
        var _this = this;
        _super.call(this, dropDownSelectContainer, dataUrl, showStar, callbackDelegate);
        this.selectedChangedCallback = callbackDelegate.selectedChangedCallback;
        callbackDelegate.selectedChangedCallback = null;
        callbackDelegate.getRootCategoryCallback = function (categories) {
            return _this.rootCategory.id;
        };
        callbackDelegate.getCategoryIdCallback = function (category) {
            return String(_this.categories[category].id);
        };
        callbackDelegate.getCategoryTextCallback = function (category) {
            return _this.categories[category].name;
        };
        callbackDelegate.getChildCategoriesCallback = function (categories, currentCategory) {
            return _this.categories[currentCategory].subcategoryIds;
        };
        callbackDelegate.getChildLeafItemsCallback = function (items, currentCategory) {
            return _this.categories[currentCategory].bookIds;
        };
        callbackDelegate.getLeafItemIdCallback = function (item) {
            return String(_this.books[item].id);
        };
        callbackDelegate.getLeafItemTextCallback = function (item) {
            return _this.books[item].name;
        };
    }
    DropDownSelect2.prototype.downloadData = function (dropDownItemsDiv) {
        var _this = this;
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
            success: function (response) {
                $(dropDownItemsDiv).children("div.loading").remove();
                _this.processDownloadedData(response);
                _this.makeTreeStructure(_this.categories, _this.books, dropDownItemsDiv);
            }
        });
    };
    DropDownSelect2.prototype.processDownloadedData = function (result) {
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
    };
    DropDownSelect2.prototype.makeLeafItem = function (container, currentLeafItem) {
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
        $(checkbox).change(function (event, propagate) {
            if (this.checked) {
                self.addToSelectedItems(info);
            }
            else {
                self.removeFromSelectedItems(info);
            }
            if (typeof propagate === "undefined" || propagate === null || propagate) {
                self.propagateSelectChange($(this).parent(".concrete-item")[0]);
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
    };
    DropDownSelect2.prototype.propagateRootSelectChange = function (item) {
        this.selectedChangedCallback(this.getState());
    };
    DropDownSelect2.prototype.propagateLeafSelectChange = function (item, info) {
        var sameBookCheckBoxes = this.books[info.ItemId].checkboxes;
        var checkBoxState = $(item).prop("checked");
        for (var i = 0; i < sameBookCheckBoxes.length; i++) {
            var otherCheckBox = sameBookCheckBoxes[i];
            if ($(otherCheckBox).prop("checked") !== checkBoxState) {
                $(otherCheckBox).prop("checked", checkBoxState);
                this.propagateSelectChange($(otherCheckBox).parent(".concrete-item")[0]);
            }
        }
        this.selectedChangedCallback(this.getState());
    };
    DropDownSelect2.prototype.propagateCategorySelectChange = function (item, info) {
        var isChecked = $(item).prop("checked");
        var category = this.categories[info.ItemId];
        var bookIds = [];
        this.getBookIdsForUpdate(category, bookIds);
        for (var i = 0; i < bookIds.length; i++) {
            var book = this.books[bookIds[i]];
            for (var j = 0; j < book.checkboxes.length; j++) {
                var bookCheckBox = book.checkboxes[j];
                if ($(bookCheckBox).prop("checked") !== isChecked) {
                    $(bookCheckBox).prop("checked", isChecked);
                    this.propagateSelectChange($(bookCheckBox).parent(".concrete-item")[0]);
                }
            }
        }
        this.selectedChangedCallback(this.getState());
    };
    DropDownSelect2.prototype.getBookIdsForUpdate = function (category, bookIds) {
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
    };
    DropDownSelect2.prototype.isChecked = function (category, selectedCategories, selectedBooks) {
        var isAllChecked = true;
        for (var i = 0; i < category.subcategoryIds.length; i++) {
            var subcategory = this.categories[category.subcategoryIds[i]];
            var books = [];
            var categories = [];
            if (!this.isChecked(subcategory, categories, books)) {
                isAllChecked = false;
                for (var k = 0; k < categories.length; k++) {
                    selectedCategories.push(categories[k]);
                }
                for (var l = 0; l < books.length; l++) {
                    selectedBooks.push(books[l]);
                }
            }
            else {
                selectedCategories.push(subcategory.id);
            }
        }
        for (var j = 0; j < category.bookIds.length; j++) {
            var book = this.books[category.bookIds[j]];
            if (!book.checkboxes[0].checked)
                isAllChecked = false;
            else
                selectedBooks.push(book.id);
        }
        return isAllChecked;
    };
    DropDownSelect2.prototype.getState = function () {
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
    };
    DropDownSelect2.prototype.getSelectedIds = function () {
        var state = new DropDownSelected();
        var selectedBooks = [];
        var selectedCategories = [];
        if (!this.rootCategory || this.isChecked(this.rootCategory, selectedCategories, selectedBooks)) {
            state.selectedBookIds = [];
            state.selectedCategoryIds = [];
        }
        else {
            state.selectedBookIds = selectedBooks;
            state.selectedCategoryIds = selectedCategories;
        }
        return state;
    };
    DropDownSelect2.getUrlStringFromState = function (state) {
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
    };
    return DropDownSelect2;
})(DropDownSelect);
var DropDownSelected = (function () {
    function DropDownSelected() {
    }
    return DropDownSelected;
})();
var DropDownBook = (function () {
    function DropDownBook() {
    }
    return DropDownBook;
})();
var DropDownCategory = (function () {
    function DropDownCategory() {
    }
    return DropDownCategory;
})();
//# sourceMappingURL=itjakub.plugins.dropdownselect2.js.map