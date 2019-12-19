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
        this.showLoading();
        $("#project-layout-content").find("*").off();
        this.createEntryButtonEl.text(localization.translate("Create", "KeyTable").value);
        this.titleEl.text(localization.translate("CategoryHeadline", "KeyTable").value);
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
            this.translateButtons();
            this.categoryRename();
            this.categoryDelete();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("EditorLoadError", "KeyTable").value);
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private updateContentAfterChange() {
        this.showLoading();
        this.util.getCategoryList().done((data: ICategoryContract[]) => {
            const numberOfParentCategories = this.categoryItemList.find(".page-list-item").length;
            this.categoryItemListArray = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(numberOfParentCategories, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog(localization.translate("ConnectionErrorHeadline", "KeyTable").value, localization.translate("ConnectionErrorMessage", "KeyTable").value);
        });
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".key-table-div");
        listEl.empty();
        const splitArray = this.splitCategoryArray(this.categoryItemList, pageNumber);
        listEl.append(this.categoryItemList);
        this.translateButtons();
        this.categoryRename();
        this.categoryDelete();
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
        const listStart = `<div class="table-responsive"><table class="table table-hover"><tbody>`;
        const listItemEnd = `</tr>`;
        const listEnd = "</tbody></table></div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < categoryItemList.length; i++) {
            const listItemStart =
                `<tr class="page-list-item" data-category-id="${categoryItemList[i].id}" data-parent-category-id="${categoryItemList[i].parentCategoryId}">`;
            elm += listItemStart;
            elm += "<td><div class=glyphicon></div></td>";
            elm += "<td>" + categoryItemList[i].description + "</td>";
            const changeButton =
                `<td class="key-table-button-cell"><button type="button" class="btn btn-default rename-key-table-entry" data-target="${categoryItemList[i].id}">
                <i class="fa fa-pencil" aria - hidden="true"> </i>
                <span class="rename-key-table-entry-description"> Rename table of keys entry </span>
                </button>`;
            elm += changeButton;
            const removeButton =
                `<button type="button" class="btn btn-default delete-key-table-entry separate-button" data-target="${categoryItemList[i].id}">
                <i class="fa fa-trash-o" aria-hidden="true"></i>
                <span class="delete-key-table-entry-description">Delete table of keys entry</span>
                </button></td>`;
            elm += removeButton;
            elm += listItemEnd;
        }
        elm += listEnd;
        const jListEl = $(elm);
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
        const jListEl = $(elm);
        return jListEl;
    }

    private hierarchicallySortCategories(jEl: JQuery): JQuery {
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
        const mainCategoriesEls = $(".key-table-div").children(".page-list-item");
        mainCategoriesEls.each((index, element:Node) => {
            const mainCategoryEl = $(element as Element);
            const childrenCategories = mainCategoryEl.children(".child-category");
            if (childrenCategories.length) {
                childrenCategories.hide();
                mainCategoryEl.children("glyphicon").addClass("glyphicon-plus");
                mainCategoryEl.children(".glyphicon").attr("title", `${localization.translate("CollapseButtonTitle", "KeyTable").value}`);
                this.trackCollapseCategoryButton(childrenCategories);
            }
        });
    }

    private trackCollapseCategoryButton(childrenCategories:JQuery<Element>) {
        $(".collapse-category-button").on("click", () => {
            childrenCategories.toggle();
        });
    }

    private categoryCreation() {
        $("button.create-key-table-entry").click(
            () => {
                this.gui.showCategoryInputDialog(localization.translate("CategoryInputHeadline", "KeyTable").value, localization.translate("CategoryNameInput", "KeyTable").value, localization.translate("ParentNameInput", "KeyTable").value);
                const dialogEl = $(".category-change-input-modal-dialog");
                const okButtonEl = dialogEl.find(".info-dialog-ok-button");
                const descriptionTextareaEl = dialogEl.find(".primary-input-dialog-textarea");
                const parentCategoryIdSelectEl = dialogEl.find(".id-method-selection");
                parentCategoryIdSelectEl.empty();
                const newCategoryOption =
                    `<option value="null" data-parent-category-id="null">` + localization.translate("NoParentCategory", "KeyTable").value + `</option>`;
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
                        const categoryString = descriptionTextareaEl.val() as string;
                        if (!categoryString) {
                            this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("CategoryWarningMessage", "KeyTable").value);
                        } else {
                            const parentCategoryIdNumber = parentCategoryIdSelectEl.val() as number;
                            const newCategoryAjax = this.util.createNewCategory(categoryString, parentCategoryIdNumber);
                            newCategoryAjax.done(() => {
                                descriptionTextareaEl.val("");
                                this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("CategoryCreateSuccess").value);
                                okButtonEl.off();
                                this.updateContentAfterChange();
                            });
                            newCategoryAjax.fail(() => {
                                this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("CategoryCreateError", "KeyTable").value);
                                okButtonEl.off();
                            });
                        }
                    });
            });
    }

    private categoryRename() {
        $("button.rename-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showCategoryInputDialog(localization.translate("CategoryInputHeadline", "KeyTable").value, localization.translate("CategoryNameInput", "KeyTable").value, localization.translate("ParentNameInput", "KeyTable").value);
                const dialogEl = $(".category-change-input-modal-dialog");
                const textareaEl = dialogEl.find(".primary-input-dialog-textarea");
                const parentCategoryIdSelectEl = dialogEl.find(".id-method-selection");
                parentCategoryIdSelectEl.empty();
                const newCategoryOption =
                    `<option value="null" data-parent-category-id="null">` + localization.translate("NoParentCategory", "KeyTable").value + `</option>`;
                parentCategoryIdSelectEl.append(newCategoryOption);
                parentCategoryIdSelectEl.append(this.generateComboboxFromList(this.categoryItemListArray));
                const originalText = selectedPageEl.clone().children().remove().end().text();
                textareaEl.val(originalText);
                const id = selectedPageEl.data("category-id") as number;
                const parentCategoryid = selectedPageEl.data("parent-category-id") as number;
                parentCategoryIdSelectEl.val(parentCategoryid);
                const okButtonEl = dialogEl.find(".info-dialog-ok-button");
                okButtonEl.on("click",
                    () => {
                        const categoryString = textareaEl.val() as string;
                        const newParentCategory = parentCategoryIdSelectEl.val() as number;
                        if (!categoryString) {
                            this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("CategoryWarningMessage", "KeyTable").value);
                        } else {
                            const renameAjax = this.util.renameCategory(id, categoryString, newParentCategory);
                                renameAjax.done(() => {
                                    textareaEl.val("");
                                    this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("CategoryRenameSuccess", "KeyTable").value);
                                    okButtonEl.off();
                                    this.updateContentAfterChange();
                                });
                                renameAjax.fail(() => {
                                    this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("CategoryRenameError", "KeyTable").value);
                                    okButtonEl.off();
                                });
                        }
                    });
            });
    }

    private categoryDelete() {
        $("button.delete-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showConfirmationDialog(localization.translate("ModalConfirm", "KeyTable").value, localization.translate("CategoryConfirmMessage", "KeyTable").value);
                $(".confirmation-ok-button").on("click",
                    () => {
                            const id = selectedPageEl.data("category-id") as number;
                            const deleteAjax = this.util.deleteCategory(id);
                            deleteAjax.done(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("CategoryRemoveSuccess", "KeyTable").value);
                                this.updateContentAfterChange();
                            });
                            deleteAjax.fail(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("CategoryRemoveSuccess", "KeyTable").value);
                            });
                    });
            });
    }
}