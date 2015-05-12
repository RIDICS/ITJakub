/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
var DropDownSelect = (function () {
    function DropDownSelect(dropDownSelectContainer, dataUrl, showStar) {
        this.dropDownSelectContainer = dropDownSelectContainer;
        this.dataUrl = dataUrl;
        this.showStar = showStar;
        this.selectedCategoriesIds = new Array();
        this.selectedItemsIds = new Array();
        this.mockupCallbacks();
    }
    //mockup callbacks for advanced search
    DropDownSelect.prototype.mockupCallbacks = function () {
        this.getTypeFromResponseCallback = function (response) {
            return response["type"];
        };
        this.getCategoriesFromResponseCallback = function (response) {
            return response["categories"];
        };
        this.getLeafItemsFromResponseCallback = function (response) {
            return response["books"];
        };
        this.getRootCategoryCallback = function (categories) {
            return categories[""][0];
        };
        this.getCategoryIdCallback = function (category) {
            return category["Id"];
        };
        this.getLeafItemIdCallback = function (leafItem) {
            return leafItem["Id"];
        };
        this.getChildCategoriesCallback = function (categories, currentCategory) {
            return categories[currentCategory["Id"]];
        };
        this.getChildLeafItemsCallback = function (leafItems, currentCategory) {
            return leafItems[currentCategory["Id"]];
        };
        this.getCategoryTextCallback = function (category) {
            return category["Description"];
        };
        this.getLeafItemTextCallback = function (leafItem) {
            return leafItem["Title"];
        };
    };
    DropDownSelect.prototype.getType = function (response) {
        return this.getTypeFromResponseCallback(response);
    };
    DropDownSelect.prototype.getRootCategory = function (categories) {
        return this.getRootCategoryCallback(categories);
    };
    DropDownSelect.prototype.getCategoryId = function (category) {
        return this.getCategoryIdCallback(category);
    };
    DropDownSelect.prototype.getCategoryName = function (category) {
        return this.getCategoryTextCallback(category);
    };
    DropDownSelect.prototype.getLeafItemId = function (leafItem) {
        return this.getLeafItemIdCallback(leafItem);
    };
    DropDownSelect.prototype.getLeafItemName = function (leafItem) {
        return this.getLeafItemTextCallback(leafItem);
    };
    DropDownSelect.prototype.getCategories = function (response) {
        return this.getCategoriesFromResponseCallback(response);
    };
    DropDownSelect.prototype.getLeafItems = function (response) {
        return this.getLeafItemsFromResponseCallback(response);
    };
    DropDownSelect.prototype.getChildCategories = function (categories, currentCategory) {
        return this.getChildCategoriesCallback(categories, currentCategory);
    };
    DropDownSelect.prototype.getChildLeafItems = function (leafItems, currentCategory) {
        return this.getChildLeafItemsCallback(leafItems, currentCategory);
    };
    DropDownSelect.prototype.makeDropdown = function () {
        var dropDownDiv = document.createElement("div");
        $(dropDownDiv).addClass("dropdown-select");
        this.makeHeader(dropDownDiv);
        this.makeBody(dropDownDiv);
        $(this.dropDownSelectContainer).append(dropDownDiv);
    };
    DropDownSelect.prototype.makeHeader = function (dropDownDiv) {
        var dropDownHeadDiv = document.createElement("div");
        $(dropDownHeadDiv).addClass("dropdown-select-header");
        var checkBoxSpan = document.createElement("span");
        $(checkBoxSpan).addClass("dropdown-select-checkbox");
        var checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkBoxSpan.appendChild(checkbox);
        dropDownHeadDiv.appendChild(checkBoxSpan);
        var textSpan = document.createElement("span");
        $(textSpan).addClass("dropdown-select-text");
        textSpan.innerText = ""; //TODO read from parameter when root is not unique or is not description
        dropDownHeadDiv.appendChild(textSpan);
        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("dropdown-select-more");
        $(moreSpan).click(function () {
            var body = $(this).parents(".dropdown-select").children(".dropdown-select-body");
            if (body.is(":hidden")) {
                $(this).children().removeClass("glyphicon-chevron-down");
                $(this).children().addClass("glyphicon-chevron-up");
                body.slideDown();
            }
            else {
                $(this).children().removeClass("glyphicon-chevron-up");
                $(this).children().addClass("glyphicon-chevron-down");
                body.slideUp();
            }
        });
        var iconSpan = document.createElement("span");
        $(iconSpan).addClass("glyphicon glyphicon-chevron-down");
        moreSpan.appendChild(iconSpan);
        dropDownHeadDiv.appendChild(moreSpan);
        dropDownDiv.appendChild(dropDownHeadDiv);
    };
    DropDownSelect.prototype.makeBody = function (dropDownDiv) {
        var dropDownBodyDiv = document.createElement("div");
        $(dropDownBodyDiv).addClass("dropdown-select-body");
        var filterDiv = document.createElement("div");
        $(filterDiv).addClass("dropdown-filter");
        var filterInput = document.createElement("input");
        $(filterInput).addClass("dropdown-filter-input");
        filterInput.placeholder = "Filtrovat podle názvu..";
        $(filterInput).keyup(function () {
            $(this).change();
        });
        $(filterInput).change(function () {
            if ($(this).val() == "") {
                $(this).parents(".dropdown-select-body").children(".concrete-item").show();
            }
            else {
                $(this).parents(".dropdown-select-body").children(".concrete-item").hide().filter(":contains(" + $(this).val() + ")").show();
            }
        });
        filterDiv.appendChild(filterInput);
        var filterClearSpan = document.createElement("span");
        $(filterClearSpan).addClass("dropdown-clear-filter glyphicon glyphicon glyphicon-remove-circle");
        $(filterClearSpan).click(function () {
            $(this).siblings(".dropdown-filter-input").val("").change();
        });
        filterDiv.appendChild(filterClearSpan);
        dropDownBodyDiv.appendChild(filterDiv);
        dropDownDiv.appendChild(dropDownBodyDiv);
        this.downloadData(dropDownBodyDiv);
    };
    DropDownSelect.prototype.downloadData = function (dropDownItemsDiv) {
        var _this = this;
        var loadDiv = document.createElement("div");
        $(loadDiv).addClass("loading");
        $(dropDownItemsDiv).append(loadDiv);
        var self = this;
        $.ajax({
            type: "GET",
            traditional: true,
            data: {},
            url: this.dataUrl,
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                self.type = _this.getType(response);
                var categories = _this.getCategories(response);
                var items = _this.getLeafItems(response);
                $(dropDownItemsDiv).children("div.loading").remove();
                _this.makeTreeStructure(categories, items, dropDownItemsDiv);
            }
        });
    };
    DropDownSelect.prototype.makeTreeStructure = function (categories, leafItems, dropDownItemsDiv) {
        var rootCategory = this.getRootCategory(categories);
        var selectHeader = $(dropDownItemsDiv).parent().children(".dropdown-select-header");
        $(selectHeader).children(".dropdown-select-text").append(this.getCategoryName(rootCategory));
        var checkbox = $(selectHeader).children("span.dropdown-select-checkbox").children("input");
        var info = this.createCallbackInfo(this.getCategoryId(rootCategory), selectHeader);
        var self = this;
        $(checkbox).change(function () {
            if (this.checked) {
                self.addToSelectedCategories(info);
            }
            else {
                self.removeFromSelectedCategories(info);
            }
        });
        var childCategories = this.getChildCategories(categories, rootCategory);
        var childLeafItems = this.getChildLeafItems(leafItems, rootCategory);
        if (typeof (childCategories) !== "undefined") {
            for (var i = 0; i < childCategories.length; i++) {
                var childCategory = childCategories[i];
                this.makeCategoryItem(dropDownItemsDiv, childCategory, categories, leafItems);
            }
        }
        if (typeof (childLeafItems) !== "undefined") {
            for (var i = 0; i < childLeafItems.length; i++) {
                var childBook = childLeafItems[i];
                this.makeLeafItem(dropDownItemsDiv, childBook);
            }
        }
    };
    DropDownSelect.prototype.makeCategoryItem = function (container, currentCategory, categories, leafItems) {
        var itemDiv = document.createElement("div");
        $(itemDiv).addClass("concrete-item"); //TODO add data-item-is-favorite
        $(itemDiv).data("id", this.getCategoryId(currentCategory));
        $(itemDiv).data("name", this.getCategoryName(currentCategory));
        $(itemDiv).data("type", "category");
        var checkbox = document.createElement("input");
        $(checkbox).addClass("concrete-item-checkbox checkbox");
        checkbox.type = "checkbox";
        var info = this.createCallbackInfo(this.getCategoryId(currentCategory), itemDiv);
        var self = this;
        $(checkbox).change(function () {
            if (this.checked) {
                self.addToSelectedCategories(info);
            }
            else {
                self.removeFromSelectedCategories(info);
            }
        });
        itemDiv.appendChild(checkbox);
        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("concrete-item-more");
        $(moreSpan).click(function () {
            var childsDiv = $(this).closest(".concrete-item").children(".child-items");
            if (childsDiv.is(":hidden")) {
                $(this).children().removeClass("glyphicon-chevron-down");
                $(this).children().addClass("glyphicon-chevron-up");
                childsDiv.slideDown();
            }
            else {
                $(this).children().removeClass("glyphicon-chevron-up");
                $(this).children().addClass("glyphicon-chevron-down");
                childsDiv.slideUp();
            }
        });
        var iconSpan = document.createElement("span");
        $(iconSpan).addClass("glyphicon glyphicon-chevron-down");
        moreSpan.appendChild(iconSpan);
        itemDiv.appendChild(moreSpan);
        if (this.showStar) {
            var saveStarSpan = document.createElement("span");
            $(saveStarSpan).addClass("save-item glyphicon glyphicon-star-empty");
            $(saveStarSpan).click(function () {
                $(this).siblings(".delete-item").show();
                $(this).hide();
                //TODO populate request on save to favorites
                if (self.starSaveCategoryCallback) {
                    self.starSaveCategoryCallback(info);
                }
            });
            itemDiv.appendChild(saveStarSpan);
            var deleteStarSpan = document.createElement("span");
            $(deleteStarSpan).addClass("delete-item glyphicon glyphicon-star");
            $(deleteStarSpan).click(function () {
                $(this).siblings(".save-item").show();
                $(this).hide();
                //TODO populate request on delete from favorites
                if (self.starDeleteCategoryCallback) {
                    self.starDeleteCategoryCallback(info);
                }
            });
            itemDiv.appendChild(deleteStarSpan);
        }
        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("concrete-item-name");
        nameSpan.innerText = this.getCategoryName(currentCategory);
        itemDiv.appendChild(nameSpan);
        var childsDiv = document.createElement("div");
        $(childsDiv).addClass("child-items");
        itemDiv.appendChild(childsDiv);
        container.appendChild(itemDiv);
        var childCategories = this.getChildCategories(categories, currentCategory);
        var childLeafItems = this.getChildLeafItems(leafItems, currentCategory);
        if (typeof (childCategories) !== "undefined") {
            for (var i = 0; i < childCategories.length; i++) {
                var childCategory = childCategories[i];
                this.makeCategoryItem(childsDiv, childCategory, categories, leafItems);
            }
        }
        if (typeof (childLeafItems) !== "undefined") {
            for (var i = 0; i < childLeafItems.length; i++) {
                var childLeafItem = childLeafItems[i];
                this.makeLeafItem(childsDiv, childLeafItem);
            }
        }
    };
    DropDownSelect.prototype.makeLeafItem = function (container, currentLeafItem) {
        var itemDiv = document.createElement("div");
        $(itemDiv).addClass("concrete-item"); //TODO add data-item-is-favorite
        $(itemDiv).data("id", this.getLeafItemId(currentLeafItem));
        $(itemDiv).data("name", this.getLeafItemName(currentLeafItem));
        $(itemDiv).data("type", "item");
        var checkbox = document.createElement("input");
        $(checkbox).addClass("concrete-item-checkbox checkbox");
        checkbox.type = "checkbox";
        var info = this.createCallbackInfo(this.getLeafItemId(currentLeafItem), itemDiv);
        var self = this;
        $(checkbox).change(function () {
            if (this.checked) {
                self.addToSelectedItems(info);
            }
            else {
                self.removeFromSelectedItems(info);
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
                if (self.starSaveItemCallback) {
                    self.starSaveItemCallback(info);
                }
            });
            itemDiv.appendChild(saveStarSpan);
            var deleteStarSpan = document.createElement("span");
            $(deleteStarSpan).addClass("delete-item glyphicon glyphicon-star");
            $(deleteStarSpan).click(function () {
                $(this).siblings(".save-item").show();
                $(this).hide();
                //TODO populate request on delete from favorites
                if (self.starDeleteItemCallback) {
                    self.starDeleteItemCallback(info);
                }
            });
            itemDiv.appendChild(deleteStarSpan);
        }
        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("concrete-item-name");
        nameSpan.innerText = this.getLeafItemName(currentLeafItem);
        itemDiv.appendChild(nameSpan);
        container.appendChild(itemDiv);
    };
    DropDownSelect.prototype.createCallbackInfo = function (id, target) {
        var info = new CallbackInfo();
        info.Id = id;
        info.Target = target;
        return info;
    };
    DropDownSelect.prototype.addToSelectedItems = function (info) {
        this.selectedItemsIds.push(info.Id);
        this.selectedChanged();
    };
    DropDownSelect.prototype.removeFromSelectedItems = function (info) {
        this.selectedItemsIds = $.grep(this.selectedItemsIds, function (valueId) {
            return valueId !== info.Id;
        }, false);
        this.selectedChanged();
    };
    DropDownSelect.prototype.addToSelectedCategories = function (info) {
        this.selectedCategoriesIds.push(info.Id);
        this.selectedChanged();
    };
    DropDownSelect.prototype.removeFromSelectedCategories = function (info) {
        this.selectedCategoriesIds = $.grep(this.selectedCategoriesIds, function (valueId) {
            return valueId !== info.Id;
        }, false);
        this.selectedChanged();
    };
    DropDownSelect.prototype.selectedChanged = function () {
        if (this.selectedChangedCallback) {
            this.selectedChangedCallback(this.getState());
        }
    };
    DropDownSelect.prototype.getState = function () {
        var state = new State();
        state.Type = this.type;
        state.SelectedCategoriesIds = this.selectedCategoriesIds;
        state.SelectedItemsIds = this.selectedItemsIds;
        return state;
    };
    return DropDownSelect;
})();
var CallbackInfo = (function () {
    function CallbackInfo() {
    }
    return CallbackInfo;
})();
var State = (function () {
    function State() {
    }
    return State;
})();
//# sourceMappingURL=itjakub.plugins.dropdownselect.js.map