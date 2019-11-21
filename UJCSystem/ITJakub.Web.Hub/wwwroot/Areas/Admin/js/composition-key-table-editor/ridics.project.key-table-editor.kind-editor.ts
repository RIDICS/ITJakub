﻿class KeyTableKindEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private literaryKindItemList: ILiteraryKindContract[];

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        $("#project-layout-content").find("*").off();
        this.createEntryButtonEl.text(localization.translate("CreateKind", "KeyTable").value);
        this.changeEntryButtonEl.text(localization.translate("RenameKind", "KeyTable").value);
        this.deleteEntryButtonEl.text(localization.translate("DeleteKind", "KeyTable").value);
        this.titleEl.text(localization.translate("KindHeadline", "KeyTable").value);
        this.unbindEventsDialog();
        this.util.getLitararyKindList().done((data: ILiteraryKindContract[]) => {
            this.literaryKindItemList = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.literaryKindRename();
            this.literaryKindDelete();
            this.literaryKindCreation();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        const splitArray = this.splitArray(this.literaryKindItemList, pageNumber);
        listEl.empty();
        const generatedListStructure = this.generateLiteraryKindList(splitArray, listEl);
        listEl.append(generatedListStructure);
        this.makeSelectable(listEl);
    }

    private generateLiteraryKindList(genreItemList: ILiteraryKindContract[], jEl: JQuery): JQuery {
        const nameArray = genreItemList.map(a => a.name);
        const idArray = genreItemList.map(a => a.id);
        return this.generateSimpleList(idArray, nameArray, jEl);
    }

    private updateContentAfterChange() {
        this.util.getLitararyKindList().done((data: ILiteraryKindContract[]) => {
            this.literaryKindItemList = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog("Warning", "Connection to server lost.\nAutomatic page reload is not possible.");
        });
    }

    private literaryKindCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showSingleInputDialog("Name input", "Please input new literary kind name:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const nameString = textareaEl.val() as string;
                        const newLiteraryKindAjax = this.util.createNewLiteraryKind(nameString);
                        newLiteraryKindAjax.done(() => {
                            textareaEl.val("");
                            this.gui.showInfoDialog("Success", "New literary kind has been created");
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newLiteraryKindAjax.fail(() => {
                            this.gui.showInfoDialog("Error", "New literary kind has not been created");
                            $(".info-dialog-ok-button").off();
                        });
                    });
            });
    }

    private literaryKindRename() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").children(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showSingleInputDialog("Name input", "Please input literary kind after rename:");
                    const textareaEl = $(".input-dialog-textarea");
                    const originalText = selectedPageEl.text();
                    textareaEl.val(originalText);
                    $(".info-dialog-ok-button").on("click",
                        () => {
                            const literaryKindName = textareaEl.val() as string;
                            const literaryOriginalId = selectedPageEl.data("key-id") as number;
                            const renameAjax = this.util.renameLiteraryKind(literaryOriginalId, literaryKindName);
                            renameAjax.done(() => {
                                textareaEl.val("");
                                this.gui.showInfoDialog("Success", "Literary kind has been renamed");
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            renameAjax.fail(() => {
                                this.gui.showInfoDialog("Error", "Literary kind has not been renamed");
                                $(".info-dialog-ok-button").off();
                            });
                        });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a literary kind");
                }
            });
    }

    private literaryKindDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog("Confirmation",
                        "Are you sure you want to delete this literary kind?");
                    $(".confirmation-ok-button").on("click",
                        () => {
                            const literaryKindId = selectedPageEl.data("key-id") as number;
                            const deleteAjax = this.util.deleteLiteraryKind(literaryKindId);
                            deleteAjax.done(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Success", "Kind deletion was successful");
                                this.updateContentAfterChange();
                            });
                            deleteAjax.fail(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Error", "Kind deletion was not successful");
                            });
                        });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a literary kind");
                }
            });
    }
}