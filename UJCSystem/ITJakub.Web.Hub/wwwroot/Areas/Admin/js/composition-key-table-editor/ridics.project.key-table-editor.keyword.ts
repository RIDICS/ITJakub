class KeyTableKeywordEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private keywordItemList: IKeywordContract[];
    private readonly gui: EditorsGui;

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
        this.titleEl.text(localization.translate("KeywordHeadline", "KeyTable").value);
        this.unbindEventsDialog();
            const initialPage = 1;
            const initial = true;
            this.loadPage(initialPage, initial);
            this.currentPage = initialPage;
            this.keywordRename();
            this.keywordDelete();
            this.keywordCreation();
    };

    private loadPage(pageNumber: number, initial?: boolean) {
        const listEl = $(".selectable-list-div");
        const startIndex = (pageNumber - 1) * this.numberOfItemsPerPage;
        const pagedResponsiblePersonListAjax = this.util.getKeywordList(startIndex, this.numberOfItemsPerPage);
        pagedResponsiblePersonListAjax.done((data: IPagedResult<IKeywordContract>) => {
            listEl.empty();
            if (initial) {
                this.initPagination(data.totalCount, this.numberOfItemsPerPage, this.loadPage.bind(this));
            }
            const generatedListStructure = this.generateKeywordList(data.list, listEl);
            listEl.append(generatedListStructure);
            this.makeSelectable(listEl);
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("EditorLoadError", "KeyTable").value);
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private updateContentAfterChange() {
        this.showLoading();
        const initial = true;
        this.loadPage(this.currentPage, initial);
    }

    protected generateKeywordList(keywordItemList: IKeywordContract[], jEl: JQuery): JQuery {
        const nameArray = keywordItemList.map(a => a.name);
        const idArray = keywordItemList.map(a => a.id);
        return this.generateSimpleList(idArray, nameArray, jEl);
    }

    private keywordCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showSingleInputDialog(localization.translate("KeywordInputHeadline", "KeyTable").value, localization.translate("KeywordNameInput", "KeyTable").value);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const keywordString = textareaEl.val() as string;
                        const newKeywordAjax = this.util.createNewKeyword(keywordString);
                        newKeywordAjax.done(() => {
                            textareaEl.val("");
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("KeywordCreateSuccess", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newKeywordAjax.fail(() => {
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("KeywordCreateError", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                        });
                    });
            });
    }

    private keywordRename() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").children(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showSingleInputDialog(localization.translate("KeywordInputHeadline", "KeyTable").value, localization.translate("KeywordNameInput", "KeyTable").value);
                    const textareaEl = $(".input-dialog-textarea");
                    const originalText = selectedPageEl.text();
                    textareaEl.val(originalText);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const keywordName = textareaEl.val() as string;
                            const keywordId = selectedPageEl.data("key-id") as number;
                            const renameAjax = this.util.renameKeyword(keywordId, keywordName);
                            renameAjax.done(() => {
                                textareaEl.val("");
                                this.gui.showInfoDialog(localization.translate("Success", "KeyTable").value, localization.translate("KeywordRenameSuccess", "KeyTable").value);
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            renameAjax.fail(() => {
                                this.gui.showInfoDialog(localization.translate("Error", "KeyTable").value, localization.translate("KeywordRenameError", "KeyTable").value);
                                $(".info-dialog-ok-button").off();
                            });
                    });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a keyword");
                }
            });
    }

    private keywordDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog(localization.translate("ModalConfirm", "KeyTable").value, localization.translate("KeywordConfirmMessage", "KeyTable").value);
                $(".confirmation-ok-button").on("click",
                    () => {
                            const id = selectedPageEl.data("key-id") as number;
                            const deleteAjax = this.util.deleteKeyword(id);
                            deleteAjax.done(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("KeywordDeleteSuccess", "KeyTable").value);
                                this.updateContentAfterChange();
                            });
                            deleteAjax.fail(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("KeywordDeleteError", "KeyTable").value);
                            });
                    });
                } else {
                    this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("KeywordInfoMessage", "KeyTable").value);
                }
            });
    }
}