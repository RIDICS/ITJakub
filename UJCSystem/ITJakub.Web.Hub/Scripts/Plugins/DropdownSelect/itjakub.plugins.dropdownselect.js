/// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
var DropDownSelectCallbackDelegate = (function () {
    function DropDownSelectCallbackDelegate() {
        this.mockup();
    }
    //working callbacks for advanced search
    DropDownSelectCallbackDelegate.prototype.mockup = function () {
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
            if (typeof categories !== "undefined" && categories !== null) {
                var rootCategories = categories[""];
                if (typeof rootCategories !== "undefined" && rootCategories !== null && rootCategories.length > 0) {
                    return rootCategories[0];
                }
            }
            return null;
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
    return DropDownSelectCallbackDelegate;
})();
var DropDownSelect = (function () {
    function DropDownSelect(dropDownSelectContainer, dataUrl, showStar, callbackDelegate) {
        this.dropDownSelectContainer = dropDownSelectContainer;
        this.dataUrl = dataUrl;
        this.showStar = showStar;
        this.callbackDelegate = callbackDelegate;
        this.selectedCategories = new Array();
        this.selectedItems = new Array();
    }
    DropDownSelect.prototype.getType = function (response) {
        return this.callbackDelegate.getTypeFromResponseCallback(response);
    };
    DropDownSelect.prototype.getRootCategory = function (categories) {
        return this.callbackDelegate.getRootCategoryCallback(categories);
    };
    DropDownSelect.prototype.getCategoryId = function (category) {
        return this.callbackDelegate.getCategoryIdCallback(category);
    };
    DropDownSelect.prototype.getCategoryName = function (category) {
        return this.callbackDelegate.getCategoryTextCallback(category);
    };
    DropDownSelect.prototype.getLeafItemId = function (leafItem) {
        return this.callbackDelegate.getLeafItemIdCallback(leafItem);
    };
    DropDownSelect.prototype.getLeafItemName = function (leafItem) {
        return this.callbackDelegate.getLeafItemTextCallback(leafItem);
    };
    DropDownSelect.prototype.getCategories = function (response) {
        return this.callbackDelegate.getCategoriesFromResponseCallback(response);
    };
    DropDownSelect.prototype.getLeafItems = function (response) {
        return this.callbackDelegate.getLeafItemsFromResponseCallback(response);
    };
    DropDownSelect.prototype.getChildCategories = function (categories, currentCategory) {
        return this.callbackDelegate.getChildCategoriesCallback(categories, currentCategory);
    };
    DropDownSelect.prototype.getChildLeafItems = function (leafItems, currentCategory) {
        return this.callbackDelegate.getChildLeafItemsCallback(leafItems, currentCategory);
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
        //checkbox.indeterminate = true;
        checkBoxSpan.appendChild(checkbox);
        dropDownHeadDiv.appendChild(checkBoxSpan);
        var textSpan = document.createElement("span");
        $(textSpan).addClass("dropdown-select-text");
        textSpan.innerText = ""; //TODO read from parameter when root is not unique or is not description
        dropDownHeadDiv.appendChild(textSpan);
        var loadSpan = document.createElement("span");
        $(loadSpan).addClass("dropdown-select-text-loading");
        dropDownHeadDiv.appendChild(loadSpan);
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
                $(this).parents(".dropdown-select-body").find(".concrete-item").show();
            }
            else {
                $(this).parents(".dropdown-select-body").find(".concrete-item").hide();
                $(this).parents(".dropdown-select-body").find(".concrete-item-name").filter(":containsCI(" + $(this).val() + ")").parents(".concrete-item").show();
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
        $(selectHeader).children(".dropdown-select-text-loading").hide();
        $(selectHeader).data("id", this.getCategoryId(rootCategory));
        $(selectHeader).data("name", this.getCategoryName(rootCategory));
        $(selectHeader).data("type", "category");
        var checkbox = $(selectHeader).children("span.dropdown-select-checkbox").children("input");
        var info = this.createCallbackInfo(this.getCategoryId(rootCategory), this.getCategoryName(rootCategory), selectHeader);
        var self = this;
        $(checkbox).change(function (event, propagate) {
            var items = $(dropDownItemsDiv).children(".concrete-item").children("input");
            if (this.checked) {
                if (typeof info.ItemId !== "undefined" && info.ItemId !== null) {
                    self.addToSelectedCategories(info);
                    $(items).prop("checked", false);
                    $(items).trigger("change", [false]);
                    $(dropDownItemsDiv).find(".concrete-item").find("input").prop("checked", true);
                }
                else {
                    $(items).prop("checked", false);
                    $(items).trigger("change", [false]);
                    $(items).prop("checked", true);
                    $(items).trigger("change", [false]);
                }
            }
            else {
                self.removeFromSelectedCategories(info);
                $(items).prop("checked", false);
                $(items).trigger("change", [false]);
            }
        });
        var childCategories = this.getChildCategories(categories, rootCategory);
        var childLeafItems = this.getChildLeafItems(leafItems, rootCategory);
        if (typeof (childCategories) !== "undefined" && childCategories !== null) {
            for (var i = 0; i < childCategories.length; i++) {
                var childCategory = childCategories[i];
                this.makeCategoryItem(dropDownItemsDiv, childCategory, categories, leafItems);
            }
        }
        if (typeof (childLeafItems) !== "undefined" && childLeafItems !== null) {
            for (var i = 0; i < childLeafItems.length; i++) {
                var childBook = childLeafItems[i];
                this.makeLeafItem(dropDownItemsDiv, childBook);
            }
        }
    };
    DropDownSelect.prototype.propagateSelectChange = function (concreteItemSource) {
        var actualItem = $(concreteItemSource).parent().closest(".concrete-item");
        var actualItemInput;
        var actualItemChilds;
        var checkedChilds;
        if (actualItem.length === 0) {
            actualItem = $(concreteItemSource).parent().closest(".dropdown-select").children(".dropdown-select-header");
            actualItemInput = $(actualItem).children(".dropdown-select-checkbox").children("input");
            actualItemChilds = $(actualItem).parent().closest(".dropdown-select").children(".dropdown-select-body").children(".concrete-item");
            checkedChilds = $(actualItemChilds).children("input:checked");
            if (!actualItem.data("id")) {
                if (actualItemChilds.length !== 0) {
                    if (checkedChilds.length === actualItemChilds.length) {
                        $(actualItemInput).prop("checked", true);
                    }
                    else {
                        $(actualItemInput).prop("checked", false);
                        if (checkedChilds.length === 0) {
                            $(actualItemInput).prop("indeterminate", false);
                        }
                        else {
                            $(actualItemInput).prop("indeterminate", true);
                        }
                    }
                }
                return;
            }
        }
        else {
            actualItemInput = $(actualItem).children("input");
            actualItemChilds = $(actualItem).children(".child-items").children(".concrete-item");
            checkedChilds = $(actualItemChilds).children("input:checked");
        }
        var info = this.createCallbackInfo(actualItem.data("id"), actualItem.data("name"), actualItem);
        if (actualItemChilds.length !== 0) {
            if (checkedChilds.length === actualItemChilds.length) {
                var itemsInputs = $(actualItemChilds).children("input");
                this.addToSelectedCategories(info);
                $(actualItemInput).prop("indeterminate", false);
                $(itemsInputs).prop("checked", false);
                $(itemsInputs).trigger("change", [false]);
                $(actualItemChilds).find("input").prop("checked", true);
                $(actualItemInput).prop("checked", true);
            }
            else {
                this.removeFromSelectedCategories(info);
                $(actualItemInput).prop("checked", false);
                if (checkedChilds.length === 0) {
                    $(actualItemInput).prop("indeterminate", false);
                }
                else {
                    $(actualItemInput).prop("indeterminate", true);
                    $(actualItemChilds).children("input:checked").trigger("change", [false]); //TODO could be call only when selectedChilds = childs - 1
                }
            }
        }
        if (!actualItem.hasClass("dropdown-select-header")) {
            this.propagateSelectChange(actualItem[0]);
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
        var info = this.createCallbackInfo(this.getCategoryId(currentCategory), this.getCategoryName(currentCategory), itemDiv);
        var self = this;
        $(checkbox).change(function (event, propagate) {
            var items = $(itemDiv).children(".child-items").children(".concrete-item").children("input");
            if (this.checked) {
                if (typeof info.ItemId !== "undefined" && info.ItemId !== null) {
                    self.addToSelectedCategories(info);
                    $(items).prop("checked", false);
                    $(items).trigger("change", [false]);
                    $(itemDiv).children(".child-items").find(".concrete-item").find("input").prop("checked", true);
                }
                else {
                    $(items).prop("checked", false);
                    $(items).trigger("change", [false]);
                    $(items).prop("checked", true);
                    $(items).trigger("change", [false]);
                }
            }
            else {
                self.removeFromSelectedCategories(info);
                $(items).prop("checked", false);
                $(items).trigger("change", [false]);
            }
            if (typeof propagate === "undefined" || propagate === null || propagate) {
                self.propagateSelectChange($(this).parent(".concrete-item")[0]);
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
                if (self.callbackDelegate.starSaveCategoryCallback) {
                    self.callbackDelegate.starSaveCategoryCallback(info);
                }
            });
            itemDiv.appendChild(saveStarSpan);
            var deleteStarSpan = document.createElement("span");
            $(deleteStarSpan).addClass("delete-item glyphicon glyphicon-star");
            $(deleteStarSpan).click(function () {
                $(this).siblings(".save-item").show();
                $(this).hide();
                //TODO populate request on delete from favorites
                if (self.callbackDelegate.starDeleteCategoryCallback) {
                    self.callbackDelegate.starDeleteCategoryCallback(info);
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
        if (typeof (childCategories) !== "undefined" && childCategories !== null) {
            for (var i = 0; i < childCategories.length; i++) {
                var childCategory = childCategories[i];
                this.makeCategoryItem(childsDiv, childCategory, categories, leafItems);
            }
        }
        if (typeof (childLeafItems) !== "undefined" && childLeafItems !== null) {
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
        var info = this.createCallbackInfo(this.getLeafItemId(currentLeafItem), this.getLeafItemName(currentLeafItem), itemDiv);
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
        nameSpan.innerHTML = this.getLeafItemName(currentLeafItem);
        itemDiv.appendChild(nameSpan);
        container.appendChild(itemDiv);
    };
    DropDownSelect.prototype.createCallbackInfo = function (itemId, itemText, target) {
        var info = new CallbackInfo();
        info.ItemId = itemId;
        info.ItemText = itemText;
        info.Target = target;
        return info;
    };
    DropDownSelect.prototype.addToSelectedItems = function (info) {
        var isSelected = $.grep(this.selectedItems, function (item) { return (item.Id === info.ItemId); }, false).length !== 0;
        if (!isSelected) {
            this.selectedItems.push(new Item(info.ItemId, info.ItemText));
            this.selectedChanged();
        }
    };
    DropDownSelect.prototype.removeFromSelectedItems = function (info) {
        this.selectedItems = $.grep(this.selectedItems, function (item) { return (item.Id !== info.ItemId); }, false);
        this.selectedChanged();
    };
    DropDownSelect.prototype.addToSelectedCategories = function (info) {
        var isSelected = $.grep(this.selectedCategories, function (category) { return (category.Id === info.ItemId); }, false).length !== 0;
        if (!isSelected) {
            this.selectedCategories.push(new Category(info.ItemId, info.ItemText));
            this.selectedChanged();
        }
    };
    DropDownSelect.prototype.removeFromSelectedCategories = function (info) {
        this.selectedCategories = $.grep(this.selectedCategories, function (category) { return (category.Id !== info.ItemId); }, false);
        this.selectedChanged();
    };
    DropDownSelect.prototype.selectedChanged = function () {
        if (this.callbackDelegate.selectedChangedCallback) {
            this.callbackDelegate.selectedChangedCallback(this.getState());
        }
    };
    DropDownSelect.prototype.getState = function () {
        var state = new State();
        state.Type = this.type;
        state.SelectedCategories = this.selectedCategories;
        state.SelectedItems = this.selectedItems;
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
var Item = (function () {
    function Item(id, name) {
        this.Id = id;
        this.Name = name;
    }
    return Item;
})();
var Category = (function () {
    function Category(id, name) {
        this.Id = id;
        this.Name = name;
    }
    return Category;
})();
//# sourceMappingURL=itjakub.plugins.dropdownselect.js.map