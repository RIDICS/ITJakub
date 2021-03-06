﻿class KeyTableLiteraryOriginalEditor extends KeyTableEditorBase {
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
        this.createEntryButtonEl.text("Create new literary original");
        this.changeEntryButtonEl.text("Rename literary original");
        this.deleteEntryButtonEl.text("Delete literary original");
        this.titleEl.text("Literary originals");
        this.unbindEventsDialog();
        this.util.getLiteraryOriginalList().done((data: ILiteraryOriginalContract[]) => {
            this.literaryOriginalItemList = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.literaryOriginalRename();
            this.literaryOriginalDelete();
            this.literaryOriginalCreation();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    };

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        const splitArray = this.splitArray(this.literaryOriginalItemList, pageNumber);
        listEl.empty();
        const generatedListStructure = this.generateLiteraryOriginalList(splitArray, listEl);
        listEl.append(generatedListStructure);
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
            this.gui.showInfoDialog("Warning", "Connection to server lost.\nAutomatic page reload is not possible.");
        });
    }

    private literaryOriginalCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showSingleInputDialog("Name input", "Please input new literary original:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const nameString = textareaEl.val() as string;
                        const newLiteraryOriginalAjax = this.util.createNewLiteraryOriginal(nameString);
                        newLiteraryOriginalAjax.done(() => {
                            textareaEl.val("");
                            this.gui.showInfoDialog("Success", "New literary original has been created");
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newLiteraryOriginalAjax.fail(() => {
                            this.gui.showInfoDialog("Error", "New literary original has not been created");
                            $(".info-dialog-ok-button").off();
                        });
                    });
            });
    }

    private literaryOriginalRename() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").children(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showSingleInputDialog("Name input", "Please input literary original after rename:");
                    const textareaEl = $(".input-dialog-textarea");
                    const originalText = selectedPageEl.text();
                    textareaEl.val(originalText);
                    $(".info-dialog-ok-button").on("click",
                        () => {
                            const literaryOriginalName = textareaEl.val() as string;
                                const literaryOriginalId = selectedPageEl.data("key-id") as number;
                                const renameAjax =
                                    this.util.renameLiteraryOriginal(literaryOriginalId, literaryOriginalName);
                                renameAjax.done(() => {
                                    textareaEl.val("");
                                    this.gui.showInfoDialog("Success", "Literary original has been renamed");
                                    $(".info-dialog-ok-button").off();
                                    this.updateContentAfterChange();
                                });
                                renameAjax.fail(() => {
                                    this.gui.showInfoDialog("Error", "Literary original has not been renamed");
                                    $(".info-dialog-ok-button").off();
                                });
                        });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a literary original");
                }
            });
    }

    private literaryOriginalDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog("Confirmation",
                        "Are you sure you want to delete this literary original?");
                    $(".confirmation-ok-button").on("click",
                        () => {
                                const literaryOriginalId = selectedPageEl.data("key-id") as number;
                                const deleteAjax = this.util.deleteLiteraryOriginal(literaryOriginalId);
                                deleteAjax.done(() => {
                                    $(".confirmation-ok-button").off();
                                    this.gui.showInfoDialog("Success", "Literary original deletion was successful");
                                    this.updateContentAfterChange();
                                });
                                deleteAjax.fail(() => {
                                    $(".confirmation-ok-button").off();
                                    this.gui.showInfoDialog("Error", "Literary original deletion was not successful");
                                });
                        });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a literary original");
                }
            });
    }
}