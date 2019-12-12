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
        const listEl = $(".selectable-list-div");
        const splitArray = this.splitArray(this.responsibleTypeItemList, pageNumber);
        listEl.empty();
        listEl.append(this.generateListStructure(splitArray));
        this.makeSelectable(listEl);
    }

    private generateListStructure(responsibleTypeItemList: IResponsibleType[]): JQuery {
        const listStart = `<div class="list-group">`;
        const listItemEnd = `</div>`;
        const listEnd = "</div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < responsibleTypeItemList.length; i++) {
            const listItemStart =
                `<div class="page-list-item list-group-item" data-key-id="${responsibleTypeItemList[i].id}" data-responsibility-type="${ResponsibleTypeEnum[responsibleTypeItemList[i].type]}">`;
            elm += listItemStart;
            elm += responsibleTypeItemList[i].text;
            elm += listItemEnd;
        }
        elm += listEnd;
        const jListEl = $(elm);
        return jListEl;
    }

    private responsibleTypeCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showResponsibleTypeInputDialog(localization.translate("RespPerTypeTypeInputHeadline", "KeyTable").value,
                    localization.translate("RespPerTypeTypeNameInput", "KeyTable").value,
                    localization.translate("RespPerTypeTypeInput", "KeyTable").value);
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
            });
    }

    private responsibleTypeChange() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").children(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showResponsibleTypeInputDialog(localization.translate("RespPerTypeTypeInputHeadline", "KeyTable").value,
                        localization.translate("RespPerTypeTypeNameInput", "KeyTable").value,
                        localization.translate("RespPerTypeTypeInput", "KeyTable").value);
                    const textareaEl = $(".responsible-type-text-input-dialog-textarea");
                    const originalText = selectedPageEl.text();
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
                } else {
                    this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("RespPerTypeInfoMessage", "KeyTable").value);
                }
            });
    }

    private responsibleTypeDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog(localization.translate("ModalConfirm", "KeyTable").value, localization.translate("RespPerTypeConfirmMessage", "KeyTable").value);
                    $(".confirmation-ok-button").on("click",
                        () => {
                            const id = selectedPageEl.data("key-id") as number;
                            const deleteAjax = this.util.deleteResponsiblePersonType(id);
                            deleteAjax.done(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("RespPerTypeDeleteSuccess", "KeyTable").value);
                                this.updateContentAfterChange();
                            });
                            deleteAjax.fail(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("RespPerTypeDeleteError", "KeyTable").value);
                            });
                        });
                } else {
                    this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("RespPerTypeInfoMessage", "KeyTable").value);
                }
            });
    }
}