class KeyTableCategoryEditor {
    private readonly util: KeyTableViewManager;
    private readonly gui: EditorsGui;
    private categoryItemList: ICategoryContract[];
    private numberOfItemsPerPage = 28;
    private currentPage: number;

    constructor() {
        this.util = new KeyTableViewManager();
        this.gui = new EditorsGui();
    }

    init() {
        this.util.getCategoryList().done((data: ICategoryContract[]) => {
            this.categoryCreation();
            this.categoryItemList = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage);
            this.loadPage(1); //initialise at page 1
            this.currentPage = 1;
            this.categoryRename();
            this.categoryDelete();
        });
    }

    private updateContentAfterChange() {
        this.util.getCategoryList().done((data: ICategoryContract[]) => {
            this.categoryItemList = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage);
        }).fail(() => {
            this.gui.showInfoDialog("Warning", "Connection to server lost.\nAutomatic page reload is not possible.");
        });
    }

    private initPagination(itemsCount: number, itemsOnPage: number) {
        const pagination = new Pagination({
            container: $(".key-table-pagination"),
            pageClickCallback: (pageNumber) => {
                this.loadPage(pageNumber);
                this.currentPage = pageNumber;
            }
        });
        pagination.make(itemsCount, itemsOnPage);
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        const splitArray = this.splitCategoryArray(this.categoryItemList, pageNumber);
        this.generateListStructure(splitArray, listEl);
    }

    private splitCategoryArray(categoryItemList: ICategoryContract[], page: number): ICategoryContract[] {
        const numberOfListItemsPerPage = this.numberOfItemsPerPage;
        const startIndex = (page - 1) * numberOfListItemsPerPage;
        const endIndex = page * numberOfListItemsPerPage;
        const splitArray = categoryItemList.slice(startIndex, endIndex);
        return splitArray;
    }

    private generateListStructure(categoryItemList: ICategoryContract[], jEl: JQuery) {
        jEl.empty();
        const listStart = `<ul class="page-list">`;
        const listItemEnd = `</li>`;
        const listEnd = "</ul>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < categoryItemList.length; i++) {
            const listItemStart =
                `<li class="ui-widget-content page-list-item" data-category-id="${categoryItemList[i].id}">`;
            elm += listItemStart;
            elm += categoryItemList[i].description;
            elm += listItemEnd;
        }
        elm += listEnd;
        const html = $.parseHTML(elm);
        jEl.append(html);
        this.makeSelectable(jEl);
    }

    private makeSelectable(jEl: JQuery) {
        jEl.children(".page-list").selectable();
    }

    private categoryCreation() {
        $(".crud-buttons-div").on("click",
            ".create-new-category",
            () => {
                console.log("Create");
                this.gui.showInputDialog("Name input", "Please input new category name:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const categoryString = textareaEl.val();
                        if (!categoryString) {
                            this.gui.showInfoDialog("Warning", "You haven't entered anything.");
                        } else {
                            const newCategoryAjax = this.util.createNewCategory(categoryString); //TODO parentCategoryId
                            newCategoryAjax.done(() => {
                                textareaEl.val("");
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
            ".rename-category",
            () => {
                this.gui.showInputDialog("Name input", "Please input category name after rename:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const categoryString = textareaEl.val();
                        if (!categoryString) {
                            this.gui.showInfoDialog("Warning", "You haven't entered anything.");
                        } else {
                            const selectedPageEl = $(".page-list").children(".ui-selected");
                            if (selectedPageEl.length) {
                                const id = selectedPageEl.data("category-id") as number;
                                const renameAjax = this.util.renameCategory(id, categoryString); //TODO parentCategoryId
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
                        }
                    });
            });
    }

    private categoryDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-category",
            () => {
                this.gui.showConfirmationDialog("Confirm", "Are you sure you want to delete this category?");
                $(".confirmation-ok-button").on("click",
                    () => {
                        const selectedPageEl = $(".page-list").children(".ui-selected");
                        if (selectedPageEl.length) {
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
                        }
                    });
            });
    }
}