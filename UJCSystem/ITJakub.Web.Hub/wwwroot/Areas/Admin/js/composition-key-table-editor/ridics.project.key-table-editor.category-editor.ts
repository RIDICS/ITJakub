class KeyTableCategoryEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private categoryItemList: JQuery;

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new category");
        $(".rename-key-table-entry").text("Rename category");
        $(".delete-key-table-entry").text("Delete category");
        this.util.getCategoryList().done((data: ICategoryContract[]) => {
            this.categoryCreation();
            this.categoryItemList = this.generateListStructure(data);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
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
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog("Warning", "Connection to server lost.\nAutomatic page reload is not possible.");
        });
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        listEl.empty();
        const splitArray = this.splitCategoryArray(this.categoryItemList.children(".page-list-item"), pageNumber);
        listEl.append(`<div class="page-list"></div>`);
        listEl.children(".page-list").append(splitArray);
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
        const listStart = `<div class="page-list">`;
        const listItemEnd = `</div>`;
        const listEnd = "</div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < categoryItemList.length; i++) {
            const listItemStart =
                `<div class="page-list-item" data-category-id="${categoryItemList[i].id}" data-parent-category-id="${categoryItemList[i].parentCategoryId}">`;
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
        const mainCategoriesEls = $(".page-list").children(".page-list-item");
        mainCategoriesEls.each((index, element) => {
            const mainCategoryEl = $(element);
            const childrenCategories = mainCategoryEl.children(".child-category");
            if (childrenCategories.length) {
                childrenCategories.hide();
                mainCategoryEl.append(`<span class="collapse-category-button" title="Toggle collapsed category"><i class="fa fa-arrows-v" aria-hidden="true"></i></span>`);
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
                this.gui.showDoubleInputDialog("Name input", "Please input new category description:", "Please choose parent category:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const descriptionTextareaEl = $(".primary-input-dialog-textarea");
                        const categoryString = descriptionTextareaEl.val();
                        if (!categoryString) {
                            this.gui.showInfoDialog("Warning", "You haven't entered anything.");
                        } else {
                            const parentCategoryIdSelectEl = $(".id-method-selection");
                            const parentCategorySelectString = parentCategoryIdSelectEl.val() as string;
                            let parentCategoryIdNumber = 0;
                            if (parentCategorySelectString === "new") {
                                parentCategoryIdNumber = null;
                            } else if (parentCategorySelectString === "fromChosen") {
                                const selectedPageEl = $(".page-list").find(".page-list-item-selected");
                                if (!selectedPageEl.length) {
                                    this.gui.showInfoDialog("Info", "Please choose a category");
                                    return;
                                }
                                parentCategoryIdNumber = selectedPageEl.data("category-id") as number;//TODO check when page is not selected
                            }
                            const newCategoryAjax = this.util.createNewCategory(categoryString, parentCategoryIdNumber);
                            newCategoryAjax.done(() => {
                                descriptionTextareaEl.val("");
                                this.gui.showInfoDialog("Success", "New category has been created");
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            newCategoryAjax.fail(() => {
                                this.gui.showInfoDialog("Error", "New category has not been created");
                                $(".info-dialog-ok-button").off();
                            });
                        }
                    });
            });
    }

    private categoryRename() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                const selectedPageEl = $(".page-list").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showSingleInputDialog("Name input", "Please input category name after rename:");
                    const textareaEl = $(".input-dialog-textarea");
                    const originalText = selectedPageEl.text();
                    textareaEl.val(originalText);
                    $(".info-dialog-ok-button").on("click",
                        () => {
                            const categoryString = textareaEl.val();
                            if (!categoryString) {
                                this.gui.showInfoDialog("Warning", "You haven't entered anything.");
                            } else {
                                    const id = selectedPageEl.data("category-id") as number;
                                    const parentCategoryid = selectedPageEl.data("parent-category-id") as number;
                                    const renameAjax = this.util.renameCategory(id, categoryString, parentCategoryid);
                                    renameAjax.done(() => {
                                        textareaEl.val("");
                                        this.gui.showInfoDialog("Success", "Category has been renamed");
                                        $(".info-dialog-ok-button").off();
                                        this.updateContentAfterChange();
                                    });
                                    renameAjax.fail(() => {
                                        this.gui.showInfoDialog("Error", "Category has not been renamed");
                                        $(".info-dialog-ok-button").off();
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
                const selectedPageEl = $(".page-list").find(".page-list-item-selected");
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