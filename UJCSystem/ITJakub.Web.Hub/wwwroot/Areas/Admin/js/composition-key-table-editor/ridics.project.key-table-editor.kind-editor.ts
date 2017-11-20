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
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new literary kind");
        $(".rename-key-table-entry").text("Rename literary kind");
        $(".delete-key-table-entry").text("Delete literary kind");
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
                        const nameString = textareaEl.val();
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
                this.gui.showSingleInputDialog("Name input", "Please input literary kind after rename:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const literaryKindName = textareaEl.val();
                        const selectedPageEl = $(".page-list").children(".page-list-item-selected");
                        if (selectedPageEl.length) {
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
                        } else {
                            this.gui.showInfoDialog("Warning", "Please choose a literary kind");
                        }
                    });
            });
    }

    private literaryKindDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                this.gui.showConfirmationDialog("Confirmation", "Are you sure you want to delete this literary kind?");
                $(".confirmation-ok-button").on("click",
                    () => {
                        const selectedPageEl = $(".page-list").find(".page-list-item-selected");
                        if (selectedPageEl.length) {
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
                        } else {
                            this.gui.showInfoDialog("Warning", "Please choose a literary kind");
                        }
                    });
            });
    }
}