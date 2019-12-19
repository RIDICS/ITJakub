class KeyTableLiteraryOriginalEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private literaryOriginalItemList: ILiteraryOriginalContract[];

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
        this.titleEl.text(localization.translate("LiteraryOriginalHeadline", "KeyTable").value);
        this.unbindEventsDialog();
        this.util.getLiteraryOriginalList().done((data: ILiteraryOriginalContract[]) => {
            this.literaryOriginalItemList = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.translateButtons();
            this.literaryOriginalRename();
            this.literaryOriginalDelete();
            this.literaryOriginalCreation();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("EditorLoadError", "KeyTable").value);
            $("#project-layout-content").empty().append(error.buildElement());
        });
    };

    private loadPage(pageNumber: number) {
        const listEl = $(".key-table-div");
        const splitArray = this.splitArray(this.literaryOriginalItemList, pageNumber);
        listEl.empty();
        const generatedListStructure = this.generateLiteraryOriginalList(splitArray, listEl);
        listEl.append(generatedListStructure);
        this.translateButtons();
        this.literaryOriginalRename();
        this.literaryOriginalDelete();
        this.makeSelectable(listEl);
    }

    private generateLiteraryOriginalList(genreItemList: ILiteraryOriginalContract[], jEl: JQuery): JQuery {
        const nameArray = genreItemList.map(a => a.name);
        const idArray = genreItemList.map(a => a.id);
        return this.generateSimpleList(idArray, nameArray, jEl);
    }

    private updateContentAfterChange() {
        this.showLoading();
        this.util.getLiteraryOriginalList().done((data: ILiteraryOriginalContract[]) => {
            this.literaryOriginalItemList = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog(localization.translate("ConnectionErrorHeadline", "KeyTable").value, localization.translate("ConnectionErrorMessage", "KeyTable").value);
        });
    }

    private literaryOriginalCreation() {
        $("button.create-key-table-entry").click(
            () => {
                this.gui.showSingleInputDialog(localization.translate("LitOriginInputHeadline", "KeyTable").value, localization.translate("LitOriginNameInput", "KeyTable").value);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const nameString = textareaEl.val() as string;
                        const newLiteraryOriginalAjax = this.util.createNewLiteraryOriginal(nameString);
                        newLiteraryOriginalAjax.done(() => {
                            textareaEl.val("");
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("LitOriginCreateSuccess", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newLiteraryOriginalAjax.fail(() => {
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, "New literary original has not been created");
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

    private literaryOriginalRename() {
        $("button.rename-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showSingleInputDialog(localization.translate("LitOriginInputHeadline", "KeyTable").value, localization.translate("LitOriginNameInput", "KeyTable").value);
                const textareaEl = $(".input-dialog-textarea");
                const originalText = selectedPageEl.children()[1].innerText;
                textareaEl.val(originalText);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const literaryOriginalName = textareaEl.val() as string;
                            const literaryOriginalId = selectedPageEl.data("key-id") as number;
                            const renameAjax =
                                this.util.renameLiteraryOriginal(literaryOriginalId, literaryOriginalName);
                            renameAjax.done(() => {
                                textareaEl.val("");
                                this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("LitOriginRenameSuccess", "KeyTable").value);
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            renameAjax.fail(() => {
                                this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("LitOriginRenameError", "KeyTable").value);
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

    private literaryOriginalDelete() {
        $("button.delete-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showConfirmationDialog(localization.translate("ModalConfirm", "KeyTable").value, localization.translate("KindConfirmMessage", "KeyTable").value);
                $(".confirmation-ok-button").on("click",
                    () => {
                        const literaryOriginalId = selectedPageEl.data("key-id") as number;
                        const deleteAjax = this.util.deleteLiteraryOriginal(literaryOriginalId);
                        deleteAjax.done(() => {
                            $(".confirmation-ok-button").off();
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("LitOriginRemoveSuccess", "KeyTable").value);
                            this.updateContentAfterChange();
                        });
                        deleteAjax.fail(() => {
                            $(".confirmation-ok-button").off();
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("LitOriginRemoveError", "KeyTable").value);
                        });
                    });
                });
    }
}