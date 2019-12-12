﻿class KeyTableGenreEditor extends KeyTableEditorBase{
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private genreItemList: IGenreResponseContract[];

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
        this.titleEl.text(localization.translate("GenreHeadline", "KeyTable").value);
        this.unbindEventsDialog();
        this.util.getLiteraryGenreList().done((data: IGenreResponseContract[]) => {
            this.genreItemList = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.genreRename();
            this.genreDelete();
            this.genreCreation();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("EditorLoadError", "KeyTable").value);
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private updateContentAfterChange() {
        this.showLoading();
        this.util.getLiteraryGenreList().done((data: IGenreResponseContract[]) => {
            this.genreItemList = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog(localization.translate("ConnectionErrorHeadline", "KeyTable").value, localization.translate("ConnectionErrorMessage", "KeyTable").value);
        });
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        const splitArray = this.splitArray(this.genreItemList, pageNumber);
        listEl.empty();
        const generatedListStructure = this.generateGenreList(splitArray, listEl);
        listEl.append(generatedListStructure);
        this.makeSelectable(listEl);
    }

    private generateGenreList(genreItemList: IGenreResponseContract[], jEl: JQuery): JQuery {
        const nameArray = genreItemList.map(a => a.name);
        const idArray = genreItemList.map(a => a.id);
        return this.generateSimpleList(idArray, nameArray, jEl);
    }

    private genreCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showSingleInputDialog(localization.translate("GenreInputHeadline", "KeyTable").value, localization.translate("GenreNameInput", "KeyTable").value);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const genreString = textareaEl.val() as string;
                        const newGenreAjax = this.util.createNewGenre(genreString);
                        newGenreAjax.done(() => {
                            textareaEl.val("");
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("GenreCreateSuccess", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newGenreAjax.fail(() => {
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("GenreCreateError", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                        });
                    });
            });
    }

    private genreRename() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").children(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showSingleInputDialog(localization.translate("GenreInputHeadline", "KeyTable").value, localization.translate("GenreNameInput", "KeyTable").value);
                    const textareaEl = $(".input-dialog-textarea");
                    const originalText = selectedPageEl.text();
                    textareaEl.val(originalText);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const genreName = textareaEl.val() as string;
                            const genreId = selectedPageEl.data("key-id") as number;
                            const renameAjax = this.util.renameGenre(genreId, genreName);
                            renameAjax.done(() => {
                                textareaEl.val("");
                                this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("GenreRenameSuccess", "KeyTable").value);
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            renameAjax.fail(() => {
                                this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("GenreRenameError", "KeyTable").value);
                                $(".info-dialog-ok-button").off();
                            });
                    });
                } else {
                    this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("GenreInfoMessage", "KeyTable").value);
                }
            });
    }

    private genreDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog(localization.translate("ModalConfirm", "KeyTable").value, localization.translate("GenreConfirmMessage", "KeyTable").value);
                    $(".confirmation-ok-button").on("click",
                        () => {
                                const id = selectedPageEl.data("key-id") as number;
                                const deleteAjax = this.util.deleteGenre(id);
                                deleteAjax.done(() => {
                                    $(".confirmation-ok-button").off();
                                    this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("GenreRemoveSuccess", "KeyTable").value);
                                    this.updateContentAfterChange();
                                });
                                deleteAjax.fail(() => {
                                    $(".confirmation-ok-button").off();
                                    this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("GenreRemoveError", "KeyTable").value);
                                });
                        });
                } else {
                    this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("GenreInfoMessage", "KeyTable").value);
                }
            });
    }
}