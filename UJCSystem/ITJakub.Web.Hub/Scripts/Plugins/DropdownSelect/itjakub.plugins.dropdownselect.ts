 /// <reference path="../../typings/jqueryui/jqueryui.d.ts" />


class DropDownSelect {

    private dropDownSelectContainer: string;
    private dataUrl: string;
    private showStar: boolean;
    private type: string;
    private selectedItemsIds: Array<string>;
    private selectedCategoriesIds: Array<string>;

    //callbacks needs to be implemented

    starSaveItemCallback: (info: CallbackInfo) => void;
    starSaveCategoryCallback: (info: CallbackInfo) => void;
    starDeleteItemCallback: (info: CallbackInfo) => void;
    starDeleteCategoryCallback: (info: CallbackInfo) => void;

    selectedChangedCallback: (state: State) => void;

    getTypeFromResponseCallback: (response) => string;
    getRootCategoryCallback: (categories) => any;
    getCategoryIdCallback: (category) => string;
    getCategoryTextCallback: (category) => string;

    getLeafItemIdCallback: (category) => string;
    getLeafItemTextCallback: (category) => string;

    getCategoriesFromResponseCallback: (response) => any;
    getLeafItemsFromResponseCallback: (response) => any;

    getChildCategoriesCallback: (categories, currentCategory) => Array<any>;
    getChildLeafItemsCallback: (leafItems, currentCategory) => Array<any>;
    

    constructor(dropDownSelectContainer: string, dataUrl: string, showStar: boolean) {
        this.dropDownSelectContainer = dropDownSelectContainer;
        this.dataUrl = dataUrl;
        this.showStar = showStar;
        this.selectedCategoriesIds = new Array();
        this.selectedItemsIds = new Array();
        this.mockupCallbacks();
    }

    //mockup callbacks for advanced search
    private mockupCallbacks() {                                         
        this.getTypeFromResponseCallback = (response) : string => {
            return response["type"];
        };

        this.getCategoriesFromResponseCallback = (response): any => {
            return response["categories"];
        };

        this.getLeafItemsFromResponseCallback = (response): any => {
            return response["books"];
        };

        this.getRootCategoryCallback = (categories): any => {
            return categories[""][0];
        };

        this.getCategoryIdCallback = (category): string => {
            return category["Id"];
        };

        this.getLeafItemIdCallback = (leafItem): string => {
            return leafItem["Id"];
        };

        this.getChildCategoriesCallback = (categories, currentCategory): Array<any> => {
            return categories[currentCategory["Id"]];
        };

        this.getChildLeafItemsCallback = (leafItems, currentCategory): Array<any> => {
            return leafItems[currentCategory["Id"]];
        };

        this.getCategoryTextCallback = (category): string => {
            return category["Description"];
        };

        this.getLeafItemTextCallback = (leafItem): string => {
            return leafItem["Title"];
        };
    }

    private getType(response): string {
        return this.getTypeFromResponseCallback(response);
    }

    private getRootCategory(categories): any {
        return this.getRootCategoryCallback(categories);
    }

    private getCategoryId(category): string {
        return this.getCategoryIdCallback(category);
    }

    private getCategoryName(category): string {
        return this.getCategoryTextCallback(category);
    }

    private getLeafItemId(leafItem): string {
        return this.getLeafItemIdCallback(leafItem);
    }

    private getLeafItemName(leafItem): string {
        return this.getLeafItemTextCallback(leafItem);
    }

    private getCategories(response): any {
        return this.getCategoriesFromResponseCallback(response);
    }

    private getLeafItems(response): any {
        return this.getLeafItemsFromResponseCallback(response);
    }

    private getChildCategories(categories, currentCategory): Array<any> {
        return this.getChildCategoriesCallback(categories, currentCategory);
    }

    private getChildLeafItems(leafItems, currentCategory): Array<any> {
        return this.getChildLeafItemsCallback(leafItems, currentCategory);
    }

    makeDropdown() {
        var dropDownDiv = document.createElement("div");
        $(dropDownDiv).addClass("dropdown-select");

        this.makeHeader(dropDownDiv);
        this.makeBody(dropDownDiv);

        $(this.dropDownSelectContainer).append(dropDownDiv);
    }

    private makeHeader(dropDownDiv: HTMLDivElement) {

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

        $(moreSpan).click(function() {
            var body = $(this).parents(".dropdown-select").children(".dropdown-select-body");
            if (body.is(":hidden")) {
                $(this).children().removeClass("glyphicon-chevron-down");
                $(this).children().addClass("glyphicon-chevron-up");
                body.slideDown();
            } else {
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
    }

    private makeBody(dropDownDiv: HTMLDivElement) {
        var dropDownBodyDiv = document.createElement("div");
        $(dropDownBodyDiv).addClass("dropdown-select-body");

        var filterDiv = document.createElement("div");
        $(filterDiv).addClass("dropdown-filter");

        var filterInput = document.createElement("input");
        $(filterInput).addClass("dropdown-filter-input");
        filterInput.placeholder = "Filtrovat podle názvu..";

        $(filterInput).keyup(function() {
            $(this).change();
        });

        $(filterInput).change(function() {
            if ($(this).val() == "") {
                $(this).parents(".dropdown-select-body").children(".concrete-item").show();
            } else {
                $(this).parents(".dropdown-select-body").children(".concrete-item").hide().filter(":contains(" + $(this).val() + ")").show();
            }
        });

        filterDiv.appendChild(filterInput);

        var filterClearSpan = document.createElement("span");
        $(filterClearSpan).addClass("dropdown-clear-filter glyphicon glyphicon glyphicon-remove-circle");

        $(filterClearSpan).click(function() {
            $(this).siblings(".dropdown-filter-input").val("").change();
        });

        filterDiv.appendChild(filterClearSpan);

        dropDownBodyDiv.appendChild(filterDiv);

        dropDownDiv.appendChild(dropDownBodyDiv);

        this.downloadData(dropDownBodyDiv);
    }

    private downloadData(dropDownItemsDiv: HTMLDivElement) {
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
            success: (response) => {
                self.type = this.getType(response);
                var categories = this.getCategories(response);
                var items = this.getLeafItems(response);

                $(dropDownItemsDiv).children("div.loading").remove();

                this.makeTreeStructure(categories, items, dropDownItemsDiv);
            }
        });
    }

    private makeTreeStructure(categories, leafItems, dropDownItemsDiv: HTMLDivElement) {
        var rootCategory = this.getRootCategory(categories);

        var selectHeader = $(dropDownItemsDiv).parent().children(".dropdown-select-header");
        $(selectHeader).children(".dropdown-select-text").append(this.getCategoryName(rootCategory));

        var checkbox = $(selectHeader).children("span.dropdown-select-checkbox").children("input");
        var info = this.createCallbackInfo(this.getCategoryId(rootCategory), selectHeader);
        var self = this;
        $(checkbox).change(function() {
            if (this.checked) {
                self.addToSelectedCategories(info);
            } else {
                self.removeFromSelectedCategories(info);
            }
        });

        var childCategories: Array<any> = this.getChildCategories(categories, rootCategory);
        var childLeafItems: Array<any> = this.getChildLeafItems(leafItems, rootCategory);

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
    }


    private makeCategoryItem(container: HTMLDivElement, currentCategory: any, categories: any, leafItems: any) {

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
        $(checkbox).change(function() {
            if (this.checked) {
                self.addToSelectedCategories(info);
            } else {
                self.removeFromSelectedCategories(info);
            }
        });

        itemDiv.appendChild(checkbox);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("concrete-item-more");

        $(moreSpan).click(function() {
            var childsDiv = $(this).closest(".concrete-item").children(".child-items");
            if (childsDiv.is(":hidden")) {
                $(this).children().removeClass("glyphicon-chevron-down");
                $(this).children().addClass("glyphicon-chevron-up");
                childsDiv.slideDown();
            } else {
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

            $(saveStarSpan).click(function() {
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

            $(deleteStarSpan).click(function() {
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

    }

    private makeLeafItem(container: HTMLDivElement, currentLeafItem: any) {
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
        $(checkbox).change(function() {
            if (this.checked) {
                self.addToSelectedItems(info);
            } else {
                self.removeFromSelectedItems(info);
            }
        });

        itemDiv.appendChild(checkbox);

        if (this.showStar) {

            var saveStarSpan = document.createElement("span");
            $(saveStarSpan).addClass("save-item glyphicon glyphicon-star-empty");

            $(saveStarSpan).click(function() {
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

            $(deleteStarSpan).click(function() {
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
    }


    private createCallbackInfo(id: string, target: any): CallbackInfo {
        var info = new CallbackInfo();
        info.Id = id;
        info.Target = target;
        return info;
    }

    private addToSelectedItems(info: CallbackInfo) {
        this.selectedItemsIds.push(info.Id);
        this.selectedChanged();
    }

    private removeFromSelectedItems(info: CallbackInfo) {
        this.selectedItemsIds = $.grep(this.selectedItemsIds, function(valueId) {
            return valueId !== info.Id;
        }, false);
        this.selectedChanged();
    }

    private addToSelectedCategories(info: CallbackInfo) {
        this.selectedCategoriesIds.push(info.Id);
        this.selectedChanged();
    }

    private removeFromSelectedCategories(info: CallbackInfo) {
        this.selectedCategoriesIds = $.grep(this.selectedCategoriesIds, function(valueId) {
            return valueId !== info.Id;
        }, false);
        this.selectedChanged();
    }

    private selectedChanged() {
        if (this.selectedChangedCallback) {
            this.selectedChangedCallback(this.getState());
        }
    }

    getState(): State {
        var state = new State();
        state.Type = this.type;
        state.SelectedCategoriesIds = this.selectedCategoriesIds;
        state.SelectedItemsIds = this.selectedItemsIds;
        return state;
    }

}

class CallbackInfo {
    Id: string; //id of item
    Target: any; //This in caller
}

class State {
    Type: string;
    SelectedItemsIds: Array<string>;
    SelectedCategoriesIds: Array<string>;
}