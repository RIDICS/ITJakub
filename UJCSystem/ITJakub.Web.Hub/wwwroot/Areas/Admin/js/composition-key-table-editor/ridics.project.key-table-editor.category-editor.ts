class KeyTableCategoryEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private categoryItemList: JQuery;
    private categoryItemListArray: ICategoryContract[];

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        $("#project-layout-content").find("*").off();
        this.createEntryButtonEl.text("Create new category");
        this.changeEntryButtonEl.text("Change category");
        this.deleteEntryButtonEl.text("Delete category");
        this.titleEl.text("Categories");
        this.unbindEventsDialog();
        this.util.getCategoryList().done((data: ICategoryContract[]) => {
            this.categoryCreation();
            this.categoryItemList = this.generateListStructure(data);
            this.categoryItemListArray = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            const numberOfParentCategories = this.categoryItemList.children(".page-list-item").length;
            this.initPagination(numberOfParentCategories, itemsOnPage, this.loadPage.bind(this));
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.categoryRename();
            this.categoryDelete();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private updateContentAfterChange() {

        this.util.getCategoryList().done((data: ICategoryContract[]) => {
            this.categoryItemList = this.generateListStructure(data);
            const numberOfParentCategories = this.categoryItemList.children(".page-list-item").length;
            this.categoryItemListArray = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(numberOfParentCategories, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog("Warning", "Connection to server lost.\nAutomatic page reload is not possible.");
        });
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        listEl.empty();
        const splitArray = this.splitCategoryArray(this.categoryItemList.children(".page-list-item"), pageNumber);
        listEl.append(`<div class="list-group"></div>`);
        listEl.children(".list-group").append(splitArray);
        this.makeSelectable(listEl);
        this.collapseCategories();
    }

    private splitCategoryArray(categoryItemList: JQuery, page: number): JQuery {
        const numberOfListItemsPerPage = this.numberOfItemsPerPage;
        const startIndex = (page - 1) * numberOfListItemsPerPage;
        const endIndex = page * numberOfListItemsPerPage;
        const splitArray = categoryItemList.clone().slice(startIndex, endIndex);
        return splitArray;
    }

    private generateListStructure(categoryItemList: ICategoryContract[]):JQuery {
        const listStart = `<div class="list-group">`;
        const listItemEnd = `</div>`;
        const listEnd = "</div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < categoryItemList.length; i++) {
            const listItemStart =
                `<div class="page-list-item list-group-item" data-category-id="${categoryItemList[i].id}" data-parent-category-id="${categoryItemList[i].parentCategoryId}">`;
            elm += listItemStart;
            elm += categoryItemList[i].description;
            elm += listItemEnd;
        }
        elm += listEnd;
        const html = $.parseHTML(elm);
        const jListEl = $(html);
        const hierarchicallySortedList = this.hierarchicallySortCategories(jListEl);
        return hierarchicallySortedList;
    }

    private generateComboboxFromList(categoryItemList: ICategoryContract[]): JQuery {
        var elm = "";
        for (let i = 0; i < categoryItemList.length; i++) {
            const listItemStart =
                `<option value="${categoryItemList[i].id}" data-parent-category-id="${categoryItemList[i].parentCategoryId}">${categoryItemList[i].description}</option>`;
            elm += listItemStart;
        }
        const html = $.parseHTML(elm);
        const jListEl = $(html);
        return jListEl;
    }

    private hierarchicallySortCategories(jEl: JQuery):JQuery {
        const pageListItem = jEl.children(".page-list-item");
        pageListItem.each((index, element) => {
            const listItemEl = $(element);
            const parentCategoryId = listItemEl.data("parent-category-id");
            if (parentCategoryId !== null) {
                listItemEl.detach();
                listItemEl.addClass("child-category");
                const parentCategoryEl = jEl.find(`*[data-category-id="${parentCategoryId}"]`);
                parentCategoryEl.append(listItemEl);
            }
        });
        return jEl;
    }

    private collapseCategories() {
        const mainCategoriesEls = $(".list-group").children(".page-list-item");
        mainCategoriesEls.each((index, element) => {
            const mainCategoryEl = $(element);
            const childrenCategories = mainCategoryEl.children(".child-category");
            if (childrenCategories.length) {
                childrenCategories.hide();
                mainCategoryEl.append(`<span class="collapse-category-button" title="Toggle collapsed category"><i class="fa fa-arrows-v fa-pull-right" aria-hidden="true"></i></span>`);
                this.trackCollapseCategoryButton(childrenCategories);
            }
        });
    }

    private trackCollapseCategoryButton(childrenCategories:JQuery) {
        $(".collapse-category-button").on("click", () => {
            childrenCategories.toggle();
        });
    }

    private categoryCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showCategoryInputDialog("Name input", "Please input new category description:", "Please choose parent category:");
                const dialogEl = $(".category-change-input-modal-dialog");
                const okButtonEl = dialogEl.find(".info-dialog-ok-button");
                const descriptionTextareaEl = dialogEl.find(".primary-input-dialog-textarea");
                const parentCategoryIdSelectEl = dialogEl.find(".id-method-selection");
                parentCategoryIdSelectEl.empty();
                const newCategoryOption =
                    `<option value="null" data-parent-category-id="null">No parent category</option>`;
                parentCategoryIdSelectEl.append(newCategoryOption);
                parentCategoryIdSelectEl.append(this.generateComboboxFromList(this.categoryItemListArray));
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    const categoryid = selectedPageEl.data("category-id") as number;
                    parentCategoryIdSelectEl.val(categoryid);
                } else {
                    parentCategoryIdSelectEl.val("null");
                }
                okButtonEl.on("click",
                    () => {
                        const categoryString = descriptionTextareaEl.val();
                        if (!categoryString) {
                            this.gui.showInfoDialog("Warning", "You haven't entered anything.");
                        } else {
                            const parentCategoryIdNumber = parentCategoryIdSelectEl.val() as number;
                            const newCategoryAjax = this.util.createNewCategory(categoryString, parentCategoryIdNumber);
                            newCategoryAjax.done(() => {
                                descriptionTextareaEl.val("");
                                this.gui.showInfoDialog("Success", "New category has been created");
                                okButtonEl.off();
                                this.updateContentAfterChange();
                            });
                            newCategoryAjax.fail(() => {
                                this.gui.showInfoDialog("Error", "New category has not been created");
                                okButtonEl.off();
                            });
                        }
                    });
            });
    }

    private categoryRename() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showCategoryInputDialog("Category input", "Please input new category description:", "Please choose new parent category:");
                    const dialogEl = $(".category-change-input-modal-dialog");
                    const textareaEl = dialogEl.find(".primary-input-dialog-textarea");
                    const parentCategoryIdSelectEl = dialogEl.find(".id-method-selection");
                    parentCategoryIdSelectEl.empty();
                    parentCategoryIdSelectEl.append(this.generateComboboxFromList(this.categoryItemListArray));
                    const originalText = selectedPageEl.clone().children().remove().end().text();
                    textareaEl.val(originalText);
                    const id = selectedPageEl.data("category-id") as number;
                    const parentCategoryid = selectedPageEl.data("parent-category-id") as number;
                    parentCategoryIdSelectEl.val(parentCategoryid);
                    const okButtonEl = dialogEl.find(".info-dialog-ok-button");
                    okButtonEl.on("click",
                        () => {
                            const categoryString = textareaEl.val();
                            const newParentCategory = parentCategoryIdSelectEl.val() as number;
                            if (!categoryString) {
                                this.gui.showInfoDialog("Warning", "You haven't entered anything.");
                            } else {
                                const renameAjax = this.util.renameCategory(id, categoryString, newParentCategory);
                                    renameAjax.done(() => {
                                        textareaEl.val("");
                                        this.gui.showInfoDialog("Success", "Category has been renamed");
                                        okButtonEl.off();
                                        this.updateContentAfterChange();
                                    });
                                    renameAjax.fail(() => {
                                        this.gui.showInfoDialog("Error", "Category has not been renamed");
                                        okButtonEl.off();
                                    });
                            }
                        });
                } else {
                    this.gui.showInfoDialog("Info", "Please choose a category");
                }
            });
    }

    private categoryDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog("Confirm", "Are you sure you want to delete this category?");
                    $(".confirmation-ok-button").on("click",
                        () => {
                                const id = selectedPageEl.data("category-id") as number;
                                const deleteAjax = this.util.deleteCategory(id);
                                deleteAjax.done(() => {
                                    $(".confirmation-ok-button").off();
                                    this.gui.showInfoDialog("Success", "Category deletion was successful");
                                    this.updateContentAfterChange();
                                });
                                deleteAjax.fail(() => {
                                    $(".confirmation-ok-button").off();
                                    this.gui.showInfoDialog("Error", "Category deletion was not successful");
                                });
                        });
                } else {
                    this.gui.showInfoDialog("Info", "Please choose a category");
                }
            });
    }
}