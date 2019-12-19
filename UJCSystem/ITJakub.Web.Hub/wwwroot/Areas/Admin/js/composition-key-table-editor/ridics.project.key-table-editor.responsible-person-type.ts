class KeyTableResponsiblePersonType extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private responsibleTypeItemList: IResponsibleType[];

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        this.showLoading();
        $("#project-layout-content").find("*").off();
        this.createEntryButtonEl.text(localization.translate("Create", "KeyTable").value);
        this.changeEntryButtonEl.text(localization.translate("Change", "KeyTable").value);
        this.deleteEntryButtonEl.text(localization.translate("Delete", "KeyTable").value);
        this.titleEl.text(localization.translate("ResponsiblePersonTypeHeadline", "KeyTable").value);
        this.unbindEventsDialog();
        this.util.getResponsiblePersonTypeList().done((data: IResponsibleType[]) => {
            this.responsibleTypeItemList = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.translateButtons();
            this.responsibleTypeChange();
            this.responsibleTypeDelete();
            this.responsibleTypeCreation();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("EditorLoadError", "KeyTable").value);
            $("#project-layout-content").empty().append(error.buildElement());
        });
    };

    private updateContentAfterChange() {
        this.showLoading();
        this.util.getResponsiblePersonTypeList().done((data: IResponsibleType[]) => {
            this.responsibleTypeItemList = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog(localization.translate("ConnectionErrorHeadline", "KeyTable").value, localization.translate("ConnectionErrorMessage", "KeyTable").value);
        });
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".key-table-div");
        const splitArray = this.splitArray(this.responsibleTypeItemList, pageNumber);
        listEl.empty();
        listEl.append(this.generateListStructure(splitArray));
        this.translateButtons();
        this.responsibleTypeChange();
        this.responsibleTypeDelete();
        this.makeSelectable(listEl);
    }

    private generateListStructure(responsibleTypeItemList: IResponsibleType[]): JQuery {
        const listStart = `<div class="table-responsive"><table class="table table-hover"><tbody>`;
        const listItemEnd = `</tr>`;
        const listEnd = "</tbody></table></div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < responsibleTypeItemList.length; i++) {
            const listItemStart =
                `<tr class="page-list-item" data-key-id="${responsibleTypeItemList[i].id}" data-responsibility-type="${ResponsibleTypeEnum[responsibleTypeItemList[i].type]}">`;
            elm += listItemStart;
            elm += '<td><div class="empty-glyphicon"></div></td>';
            elm += `<td>${responsibleTypeItemList[i].text}</td>`;
            const changeButton =
                `<td class="key-table-button-cell"><button type="button" class="btn btn-default rename-key-table-entry" data-target="${responsibleTypeItemList[i].id}">
                <i class="fa fa-pencil" aria - hidden="true"> </i>
                <span class="rename-key-table-entry-description"> Rename table of keys entry </span>
                </button>`;
            elm += changeButton;
            const removeButton =
                `<button type="button" class="btn btn-default delete-key-table-entry separate-button" data-target="${responsibleTypeItemList[i].id}">
                <i class="fa fa-trash-o" aria-hidden="true"></i>
                <span class="delete-key-table-entry-description">Delete table of keys entry</span>
                </button></td>`;
            elm += removeButton;
            elm += listItemEnd;
        }
        elm += listEnd;
        const jListEl = $(elm);
        return jListEl;
    }

    private responsibleTypeCreation() {
        $("button.create-key-table-entry").click(
            () => {
                this.gui.showResponsibleTypeInputDialog(localization.translate("RespPerTypeInputHeadline", "KeyTable").value,
                    localization.translate("RespPerTypeNameInput", "KeyTable").value,
                    localization.translate("RespPerTypeInput", "KeyTable").value);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".responsible-type-text-input-dialog-textarea");
                        const typeDescriptionString = textareaEl.val() as string;
                        if (!typeDescriptionString) {
                            this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("RespPerTypeWarningMessage", "KeyTable").value);
                        } else {
                            const responsibilityTypeSelectEl = $(".responsible-type-selection");
                            const responsibilityTypeSelect = responsibilityTypeSelectEl.val() as ResponsibleTypeEnum;
                            const newResponsibleTypeAjax =
                                this.util.createNewResponsiblePersonType(responsibilityTypeSelect,
                                    typeDescriptionString);
                            newResponsibleTypeAjax.done(() => {
                                textareaEl.val("");
                                this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("RespPerTypeCreateSuccess", "KeyTable").value);
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            newResponsibleTypeAjax.fail(() => {
                                this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("RespPerTypeCreateError", "KeyTable").value);
                                $(".info-dialog-ok-button").off();
                            });
                        }
                    });
                $(".info-dialog-close-button").click(
                    () => {
                        const textareaEl = $(".responsible-type-text-input-dialog-textarea");
                        textareaEl.val("");
                    });
            });
    }

    private responsibleTypeChange() {
        $("button.rename-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showResponsibleTypeInputDialog(localization.translate("RespPerTypeInputHeadline", "KeyTable").value,
                    localization.translate("RespPerTypeNameInput", "KeyTable").value,
                    localization.translate("RespPerTypeInput", "KeyTable").value);
                const textareaEl = $(".responsible-type-text-input-dialog-textarea");
                const originalText = selectedPageEl.children()[1].innerText;
                textareaEl.val(originalText);
                const responsibilityType = selectedPageEl.data("responsibility-type") as ResponsibleTypeEnum;
                const responsibilityTypeSelectEl = $(".responsible-type-selection");
                responsibilityTypeSelectEl.val(responsibilityType);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const typeDescriptionString = textareaEl.val() as string;
                        const responsibilityTypeSelect = responsibilityTypeSelectEl.val() as ResponsibleTypeEnum;
                        if (!typeDescriptionString) {
                            this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("RespPerTypeWarningMessage", "KeyTable").value);
                        } else {
                                const responsibleTypeId = selectedPageEl.data("key-id") as number;
                                const renameAjax = this.util.renameResponsiblePersonType(responsibleTypeId,
                                    responsibilityTypeSelect,
                                    typeDescriptionString);
                                renameAjax.done(() => {
                                    textareaEl.val("");
                                    this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("RespPerTypeRenameSuccess", "KeyTable").value);
                                    $(".info-dialog-ok-button").off();
                                    this.updateContentAfterChange();
                                });
                                renameAjax.fail(() => {
                                    this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("RespPerTypeRenameError", "KeyTable").value);
                                    $(".info-dialog-ok-button").off();
                                });
                        }
                    });
                $(".info-dialog-close-button").click(
                    () => {
                        const textareaEl = $(".responsible-type-text-input-dialog-textarea");
                        textareaEl.val("");
                    });
            });
    }

    private responsibleTypeDelete() {
        $("button.delete-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showConfirmationDialog(localization.translate("ModalConfirm", "KeyTable").value, localization.translate("RespPerTypeConfirmMessage", "KeyTable").value);
                $(".confirmation-ok-button").on("click",
                    () => {
                        const id = selectedPageEl.data("key-id") as number;
                        const deleteAjax = this.util.deleteResponsiblePersonType(id);
                        deleteAjax.done(() => {
                            $(".confirmation-ok-button").off();
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("RespPerTypeRemoveSuccess", "KeyTable").value);
                            this.updateContentAfterChange();
                        });
                        deleteAjax.fail(() => {
                            $(".confirmation-ok-button").off();
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("RespPerTypeRemoveError", "KeyTable").value);
                        });
                    });
            });
    }
}