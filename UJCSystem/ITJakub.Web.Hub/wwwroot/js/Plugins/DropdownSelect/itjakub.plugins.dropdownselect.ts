﻿class DropDownSelectCallbackDelegate {
    // callbacks needs to be implemented

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

    dataLoadedCallback: (rootCategoryId) => void;


    constructor() {
        this.makeDefaults();
    }

    //working callbacks for advanced search
    private makeDefaults() {
        this.getTypeFromResponseCallback = (response): string => {
            return response["type"];
        };

        this.getCategoriesFromResponseCallback = (response): any => {
            return response["categories"];
        };

        this.getLeafItemsFromResponseCallback = (response): any => {
            return response["books"];
        };

        this.getRootCategoryCallback = (categories): any => {
            if (typeof categories !== "undefined" && categories !== null) {
                var rootCategories = categories[""];
                if (typeof rootCategories !== "undefined" && rootCategories !== null && rootCategories.length > 0) {
                    return rootCategories[0];
                }
            }
            return null;
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
}

class DropDownSelect {

    protected dropDownSelectContainer: string;
    protected dataUrl: string;
    protected showStar: boolean;
    protected favoriteCategoryUrl: string;
    protected favoriteLeafUrl: string;
    private type: string;
    private selectedItems: Array<Item>;
    private selectedCategories: Array<Category>;
    protected callbackDelegate: DropDownSelectCallbackDelegate;
    private moreSpan: HTMLSpanElement;
    protected dropDownBodyDiv: HTMLDivElement;
    protected favoriteManager: FavoriteManager;
    protected favoriteDialog: NewFavoriteDialog;
    private headerLoader = lv.create(null, "lv-circles tiniest dropdown-loader-color dropdown-align-loader");

    constructor(dropDownSelectContainer: string, dataUrl: string, showStar: boolean, callbackDelegate: DropDownSelectCallbackDelegate) {
        this.dropDownSelectContainer = dropDownSelectContainer;
        this.dataUrl = dataUrl;
        this.showStar = showStar;
        this.callbackDelegate = callbackDelegate;
        this.selectedCategories = new Array();
        this.selectedItems = new Array();
        this.favoriteManager = new FavoriteManager();
        this.favoriteDialog = NewFavoriteDialogProvider.getInstance(true);
    }

    private getType(response): string {
        return this.callbackDelegate.getTypeFromResponseCallback(response);
    }

    private getRootCategory(categories): any {
        return this.callbackDelegate.getRootCategoryCallback(categories);
    }

    private getCategoryId(category): string {
        return this.callbackDelegate.getCategoryIdCallback(category);
    }

    private getCategoryName(category): string {
        return this.callbackDelegate.getCategoryTextCallback(category);
    }

    private getLeafItemId(leafItem): string {
        return this.callbackDelegate.getLeafItemIdCallback(leafItem);
    }

    private getLeafItemName(leafItem): string {
        return this.callbackDelegate.getLeafItemTextCallback(leafItem);
    }

    private getCategories(response): any {
        return this.callbackDelegate.getCategoriesFromResponseCallback(response);
    }

    private getLeafItems(response): any {
        return this.callbackDelegate.getLeafItemsFromResponseCallback(response);
    }

    private getChildCategories(categories, currentCategory): Array<any> {
        return this.callbackDelegate.getChildCategoriesCallback(categories, currentCategory);
    }

    private getChildLeafItems(leafItems, currentCategory): Array<any> {
        return this.callbackDelegate.getChildLeafItemsCallback(leafItems, currentCategory);
    }

    public setFavoritesFetchUrl(categoryUrl: string, leafUrl: string) {
        this.favoriteCategoryUrl = categoryUrl;
        this.favoriteLeafUrl = leafUrl;
    }

    makeDropdown() {
        this.favoriteDialog.make();

        var dropDownDiv = document.createElement("div");
        $(dropDownDiv).addClass("dropdown-select");

        this.makeHeader(dropDownDiv);
        this.makeBody(dropDownDiv);

        $(document.documentElement).off("click.dropdown");
        $(document.documentElement).on("click.dropdown", (event) => {
            var parents = $(event.target as Node as Element).parents();
            if (!parents.is(dropDownDiv) && !parents.is(".popover")) {
                this.hideBody();
            }
        });

        $(this.dropDownSelectContainer).empty();
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
        $(textSpan).text(""); //TODO read from parameter when root is not unique or is not description

        dropDownHeadDiv.appendChild(textSpan);

        var moreSpan = document.createElement("span");
        $(moreSpan).addClass("dropdown-select-more");

        $(moreSpan).click(()=> {
            if ($(this.dropDownBodyDiv).is(":hidden")) {
                this.showBody();
            } else {
                this.hideBody();
            }
        });

        this.moreSpan = moreSpan;

        var iconSpan = document.createElement("span");
        $(iconSpan).addClass("glyphicon glyphicon-chevron-down");

        moreSpan.appendChild(iconSpan);

        dropDownHeadDiv.appendChild(moreSpan);

        dropDownHeadDiv.append(this.headerLoader.getElement());

        dropDownDiv.appendChild(dropDownHeadDiv);
    }

    showBody() {
        $(this.moreSpan).children().removeClass("glyphicon-chevron-down");
        $(this.moreSpan).children().addClass("glyphicon-chevron-up");
        $(this.dropDownBodyDiv).slideDown("fast");
    }

    private hideBody() {
        $(this.moreSpan).children().removeClass("glyphicon-chevron-up");
        $(this.moreSpan).children().addClass("glyphicon-chevron-down");
        $(this.dropDownBodyDiv).slideUp("fast");
    }

    private makeBody(dropDownDiv: HTMLDivElement) {
        var dropDownBodyDiv = document.createElement("div");
        $(dropDownBodyDiv).addClass("dropdown-select-body");

        this.dropDownBodyDiv = dropDownBodyDiv;

        var filterDiv = document.createElement("div");
        $(filterDiv).addClass("dropdown-filter");

        var filterInput = document.createElement("input");
        $(filterInput).addClass("dropdown-filter-input");
        filterInput.placeholder = localization.translate("FilterByName", "PluginsJs").value;

        $(filterInput).keyup(function() {
            $(this).change();
        });

        $(filterInput).change(function() {
            if ($(this).val() == "") {
                $(this).parents(".dropdown-select-body").find(".concrete-item").show();
            } else {
                $(this).parents(".dropdown-select-body").find(".concrete-item").hide();
                $(this).parents(".dropdown-select-body").find(".concrete-item-name").filter(`:containsCI(${$(this).val()})`).parents(".concrete-item").show();
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

    protected downloadData(dropDownItemsDiv: HTMLDivElement) {
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

                this.makeTreeStructure(categories, items, dropDownItemsDiv);

                var rootCategory = this.getRootCategory(categories);
                var rootCategoryId = this.getCategoryId(rootCategory);
                this.dataLoaded(rootCategoryId);

                this.downloadFavoriteData(dropDownItemsDiv);
            },
            error: jqXHR => {
                $(dropDownItemsDiv).children("div.lv-circles").remove();
                $(dropDownItemsDiv).siblings(".dropdown-select-header").children("div.lv-circles").remove();
                const selectHeader = $(dropDownItemsDiv).parent().children(".dropdown-select-header");
                $(selectHeader).children(".dropdown-select-text").append(localization.translate("LoadingContentError", "PluginsJs").value);
                this.dataLoaded(null);
                this.downloadFavoriteData(dropDownItemsDiv);  
            }
        });
    }

    protected downloadFavoriteData(dropDownItemsDiv: HTMLDivElement) {
        throw new Error("Not implemented");
    }

    protected updateFavoriteIcons(categoryItems: Array<IDropdownFavoriteItem>, leafItems: Array<IDropdownFavoriteItem>, favoriteLabels: Array<IFavoriteLabel>, dropdownItemsDiv: HTMLDivElement) {
        var categoriesDictionary = new DictionaryWrapper<IDropdownFavoriteItem>();
        var leafsDictionary = new DictionaryWrapper<IDropdownFavoriteItem>();
        
        for (var i = 0; i < categoryItems.length; i++) {
            categoriesDictionary.add(categoryItems[i].id, categoryItems[i]);
        }
        for (var j = 0; j < leafItems.length; j++) {
            leafsDictionary.add(leafItems[j].id, leafItems[j]);
        }

        $(".concrete-item", dropdownItemsDiv).each((index, element) => {
            const jqEl = $(element as Node as HTMLElement);
            var id = jqEl.data("id");
            var type = jqEl.data("type");
            var itemName = jqEl.data("name");

            var favoriteItem: IDropdownFavoriteItem = null;
            var favoriteType: FavoriteType = FavoriteType.Unknown;

            if (type === "category") {
                favoriteItem = categoriesDictionary.get(id);
                favoriteType = FavoriteType.Category;
            } else if (type === "item") {
                favoriteItem = leafsDictionary.get(id);
                favoriteType = FavoriteType.Project;
            }

            var favoriteStarContainer = jqEl.children(".save-item");
            var favoriteStar = new FavoriteStar(favoriteStarContainer, favoriteType, id, itemName, this.favoriteDialog, this.favoriteManager, () => this.onFavoritesChanged(favoriteType, id));

            if (favoriteItem != null) {
                favoriteStar.addFavoriteItems(favoriteItem.favoriteInfo);
            }

            favoriteStar.addFavoriteLabels(favoriteLabels);
            favoriteStar.make(null, true);
        });
    }

    protected onFavoritesChanged(favoriteType: FavoriteType, id: number) {
        new NewFavoriteNotification().show();
    }
    
    protected makeTreeStructure(categories, leafItems, dropDownItemsDiv: HTMLDivElement) {
        var rootCategory = this.getRootCategory(categories);

        var selectHeader = $(dropDownItemsDiv).parent().children(".dropdown-select-header");
        $(selectHeader).children(".dropdown-select-text").append(this.getCategoryName(rootCategory));

        $(selectHeader).data("id", this.getCategoryId(rootCategory));
        $(selectHeader).data("name", this.getCategoryName(rootCategory));
        $(selectHeader).data("type", "category");

        var checkbox = $(selectHeader).children("span.dropdown-select-checkbox").children("input")[0] as Node as HTMLInputElement;
        var info = this.createCallbackInfo(this.getCategoryId(rootCategory), this.getCategoryName(rootCategory), selectHeader);
        $(checkbox).change((event: JQuery.Event, propagate: boolean) => {
            var items = $(dropDownItemsDiv).children(".concrete-item").children("input");
            if (checkbox.checked) {
                if (typeof info.ItemId !== "undefined" && info.ItemId !== null) {
                    this.addToSelectedCategories(info);
                    items.prop("checked", false);
                    items.prop("indeterminate", false);
                    items.trigger("change",[false]);
                    $(dropDownItemsDiv).find(".concrete-item").find("input").prop("checked", true);
                } else {
                    items.prop("checked", false);
                    items.prop("indeterminate", false);
                    items.trigger("change", [false]);
                    items.prop("checked", true);
                    items.trigger("change", [false]);
                }
            } else {
                this.removeFromSelectedCategories(info);
                items.prop("checked", false);
                items.trigger("change", [false]);
            }

            if (typeof propagate === "undefined" || propagate === null || propagate) { //Deafault behaviour is to propagate change
                this.propagateRootSelectChange(checkbox);
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
        this.headerLoader.hide();
        }

    protected dataLoaded(rootCategoryId) {
        if (this.callbackDelegate.dataLoadedCallback)
            this.callbackDelegate.dataLoadedCallback(rootCategoryId);
        }

    protected onCreateCategoryCheckBox(categoryId: any, checkBox: HTMLInputElement) { }

    protected propagateRootSelectChange(item: HTMLInputElement) { }

    protected propagateLeafSelectChange(item: HTMLInputElement, info: CallbackInfo) { }

    protected propagateCategorySelectChange(item: HTMLInputElement, info: CallbackInfo) { }

    protected propagateSelectChange(concreteItemSource: HTMLDivElement) {
        var actualItem = $(concreteItemSource).parent().closest(".concrete-item");
        var actualItemInput: JQuery;
        var actualItemChilds: JQuery;
        var checkedChilds: JQuery;
        var indeterminateChilds: JQuery;
        if (actualItem.length === 0) { //for checkbox in header
            actualItem = $(concreteItemSource).parent().closest(".dropdown-select").children(".dropdown-select-header");
            actualItemInput = $(actualItem).children(".dropdown-select-checkbox").children("input");
            actualItemChilds = $(actualItem).parent().closest(".dropdown-select").children(".dropdown-select-body").children(".concrete-item");
            checkedChilds = $(actualItemChilds).children("input:checked");
            indeterminateChilds = <any>$.grep(actualItemChilds.get(),(itemChild) => ($(itemChild).children("input").prop("indeterminate") === true), false);

            if (!actualItem.data("id")) {
                if (actualItemChilds.length !== 0) {
                    if (checkedChilds.length === actualItemChilds.length) {
                        $(actualItemInput).prop("checked", true);
                    } else {
                        $(actualItemInput).prop("checked", false);
                        if (checkedChilds.length === 0 && indeterminateChilds.length === 0) {
                            $(actualItemInput).prop("indeterminate", false);
                        } else {
                            $(actualItemInput).prop("indeterminate", true);
                        }
                    }
                }
                return;
            }

        } else {
            actualItemInput = $(actualItem).children("input");
            actualItemChilds = $(actualItem).children(".child-items").children(".concrete-item");
            checkedChilds = $(actualItemChilds).children("input:checked");
            indeterminateChilds = <any>$.grep(actualItemChilds.get(),(itemChild) => ($(itemChild).children("input").prop("indeterminate") === true), false);
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
            } else {
                this.removeFromSelectedCategories(info);
                $(actualItemInput).prop("checked", false);
                if (checkedChilds.length === 0 && indeterminateChilds.length === 0) {
                    $(actualItemInput).prop("indeterminate", false);
                } else {
                    $(actualItemInput).prop("indeterminate", true);
                    $(actualItemChilds).children("input:checked").trigger("change", [false]); //TODO could be call only when selectedChilds = childs - 1
                }
            }
        }

        if (!actualItem.hasClass("dropdown-select-header")) {
            this.propagateSelectChange(actualItem[0] as Node as HTMLDivElement);    
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
        this.onCreateCategoryCheckBox(this.getCategoryId(currentCategory), checkbox);

        var info = this.createCallbackInfo(this.getCategoryId(currentCategory), this.getCategoryName(currentCategory), itemDiv);

        $(checkbox).change((event: JQuery.Event, propagate: boolean) => {
            var items = $(itemDiv).children(".child-items").children(".concrete-item").children("input");

            if (checkbox.checked) {
                if (typeof info.ItemId !== "undefined" && info.ItemId !== null) {
                    this.addToSelectedCategories(info);
                    $(items).prop("checked", false);
                    $(items).prop("indeterminate", false);
                    $(items).trigger("change", [false]);
                    $(itemDiv).children(".child-items").find(".concrete-item").find("input").prop("checked", true);
                } else {
                    $(items).prop("checked", false);
                    $(items).prop("indeterminate", false);
                    $(items).trigger("change", [false]);
                    $(items).prop("checked", true);
                    $(items).trigger("change", [false]);
                }
            } else {
                this.removeFromSelectedCategories(info);
                $(items).prop("checked", false);
                $(items).prop("indeterminate", false);
                $(items).trigger("change", [false]);
            }

            if (typeof propagate === "undefined" || propagate === null || propagate) { //Deafault behaviour is to propagate change
                this.propagateSelectChange($(checkbox).parent(".concrete-item")[0] as Node as HTMLDivElement);
                this.propagateCategorySelectChange(checkbox, info);
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

            var favoriteStarContainer = document.createElement("span");
            $(favoriteStarContainer).addClass("save-item");
            
            itemDiv.appendChild(favoriteStarContainer);
        }

        var nameSpan = document.createElement("span");
        $(nameSpan).addClass("concrete-item-name");
        $(nameSpan).text(this.getCategoryName(currentCategory));
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

    }

    protected makeLeafItem(container: HTMLDivElement, currentLeafItem: any) {
        var itemDiv = document.createElement("div");
        $(itemDiv).addClass("concrete-item"); //TODO add data-item-is-favorite

        $(itemDiv).data("id", this.getLeafItemId(currentLeafItem));
        $(itemDiv).data("name", this.getLeafItemName(currentLeafItem));
        $(itemDiv).data("type", "item");

        var checkbox = document.createElement("input");
        $(checkbox).addClass("concrete-item-checkbox checkbox");
        checkbox.type = "checkbox";

        var info = this.createCallbackInfo(this.getLeafItemId(currentLeafItem), this.getLeafItemName(currentLeafItem), itemDiv);
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
        nameSpan.innerHTML = this.getLeafItemName(currentLeafItem);

        itemDiv.appendChild(nameSpan);

        container.appendChild(itemDiv);
    }


    protected createCallbackInfo(itemId: string, itemText: string, target: any): CallbackInfo {
        var info = new CallbackInfo();
        info.ItemId = itemId;
        info.ItemText = itemText;
        info.Target = target;
        return info;
    }

    protected addToSelectedItems(info: CallbackInfo) {
        var isSelected = $.grep(this.selectedItems, (item: Item) => (item.Id === info.ItemId), false).length !== 0;
        if (!isSelected) {
            this.selectedItems.push(new Item(info.ItemId, info.ItemText));
            this.selectedChanged();
        }
    }

    protected removeFromSelectedItems(info: CallbackInfo) {
        this.selectedItems = $.grep(this.selectedItems, (item: Item) => (item.Id !== info.ItemId), false);
        this.selectedChanged();
    }

    private addToSelectedCategories(info: CallbackInfo) {
        var isSelected = $.grep(this.selectedCategories, (category: Category) => (category.Id === info.ItemId), false).length !== 0;
        if (!isSelected) {
            this.selectedCategories.push(new Category(info.ItemId, info.ItemText));
            this.selectedChanged();
        }
    }

    private removeFromSelectedCategories(info: CallbackInfo) {
        this.selectedCategories = $.grep(this.selectedCategories, (category: Category) => (category.Id !== info.ItemId), false);
        this.selectedChanged();
    }

    private selectedChanged() {
        if (this.callbackDelegate.selectedChangedCallback) {
            this.callbackDelegate.selectedChangedCallback(this.getState());
        }
    }

    getState(): State {
        var state = new State();
        state.Type = this.type;
        state.SelectedCategories = this.selectedCategories;
        state.SelectedItems = this.selectedItems;
        return state;
    }

    static getBookIdsFromState(state: State): number[] {
        var bookIdList = new Array(state.SelectedItems.length);
        for (var i = 0; i < state.SelectedItems.length; i++) {
            bookIdList[i] = Number(state.SelectedItems[i].Id);
        }
        return bookIdList;
    }

    static getCategoryIdsFromState(state: State): number[] {
        var categoryIdList = new Array(state.SelectedCategories.length);
        for (var i = 0; i < state.SelectedCategories.length; i++) {
            categoryIdList[i] = Number(state.SelectedCategories[i].Id);
        }
        return categoryIdList;
    }
}

class CallbackInfo {
    ItemId: string; //id of item
    ItemText: string; //text of item
    Target: any; //This in caller
}

class State {
    Type: string;
    SelectedItems: Array<Item>;
    SelectedCategories: Array<Category>;
    IsOnlyRootSelected: boolean;
}

class Item {
    Id: string;
    Name: string;

    constructor(id: string, name: string) {
        this.Id = id;
        this.Name = name;
    }
}

class Category {
    Id: string;
    Name: string;

    constructor(id: string, name: string) {
        this.Id = id;
        this.Name = name;
    }
}
