class KeyTableKindEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private literaryKindItemList: ILiteraryKindContract[];

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
        this.titleEl.text(localization.translate("KindHeadline", "KeyTable").value);
        this.unbindEventsDialog();
        this.util.getLitararyKindList().done((data: ILiteraryKindContract[]) => {
            this.literaryKindItemList = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.translateButtons();
            this.literaryKindRename();
            this.literaryKindDelete();
            this.literaryKindCreation();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("EditorLoadError", "KeyTable").value);
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".key-table-div");
        const splitArray = this.splitArray(this.literaryKindItemList, pageNumber);
        listEl.empty();
        const generatedListStructure = this.generateLiteraryKindList(splitArray, listEl);
        listEl.append(generatedListStructure);
        this.translateButtons();
        this.literaryKindRename();
        this.literaryKindDelete();
        this.makeSelectable(listEl);
    }

    private generateLiteraryKindList(genreItemList: ILiteraryKindContract[], jEl: JQuery): JQuery {
        const nameArray = genreItemList.map(a => a.name);
        const idArray = genreItemList.map(a => a.id);
        return this.generateSimpleList(idArray, nameArray, jEl);
    }

    private updateContentAfterChange() {
        this.showLoading();
        this.util.getLitararyKindList().done((data: ILiteraryKindContract[]) => {
            this.literaryKindItemList = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog(localization.translate("ConnectionErrorHeadline", "KeyTable").value, localization.translate("ConnectionErrorMessage", "KeyTable").value);
        });
    }

    private literaryKindCreation() {
        $("button.create-key-table-entry").click(
            () => {
                this.gui.showSingleInputDialog(localization.translate("KindInputHeadline", "KeyTable").value, localization.translate("KindNameInput", "KeyTable").value);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const nameString = textareaEl.val() as string;
                        const newLiteraryKindAjax = this.util.createNewLiteraryKind(nameString);
                        newLiteraryKindAjax.done(() => {
                            textareaEl.val("");
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("KindCreateSuccess", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newLiteraryKindAjax.fail(() => {
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("KindCreateError", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                        });
                    });
                $(".info-dialog-close-button").click(
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        textareaEl.val("");
                    });
            });
    }

    private literaryKindRename() {
        $("button.rename-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showSingleInputDialog(localization.translate("KindInputHeadline", "KeyTable").value, localization.translate("KindNameInput", "KeyTable").value);
                const textareaEl = $(".input-dialog-textarea");
                const originalText = selectedPageEl.children()[1].innerHTML;
                textareaEl.val(originalText);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const literaryKindName = textareaEl.val() as string;
                        const literaryOriginalId = selectedPageEl.data("key-id") as number;
                        const renameAjax = this.util.renameLiteraryKind(literaryOriginalId, literaryKindName);
                        renameAjax.done(() => {
                            textareaEl.val("");
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("KindRenameSuccess", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        renameAjax.fail(() => {
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("KindRenameError", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                        });
                    });
                $(".info-dialog-close-button").click(
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        textareaEl.val("");
                    });
            });
    }

    private literaryKindDelete() {
        $("button.delete-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showConfirmationDialog(localization.translate("ModalConfirm", "KeyTable").value, localization.translate("KindConfirmMessage", "KeyTable").value);
                $(".confirmation-ok-button").on("click",
                    () => {
                        const literaryKindId = selectedPageEl.data("key-id") as number;
                        const deleteAjax = this.util.deleteLiteraryKind(literaryKindId);
                        deleteAjax.done(() => {
                            $(".confirmation-ok-button").off();
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, "Kind deletion was successful");
                            this.updateContentAfterChange();
                        });
                        deleteAjax.fail(() => {
                            $(".confirmation-ok-button").off();
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, "Kind deletion was not successful");
                        });
                    });
            });
    }
}