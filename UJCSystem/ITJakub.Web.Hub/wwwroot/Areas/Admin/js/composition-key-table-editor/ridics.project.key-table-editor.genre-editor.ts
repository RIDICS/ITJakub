class KeyTableGenreEditor extends KeyTableEditorBase{
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private genreItemList: IGenreResponseContract[];

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry-description").text("Create new genre");
        $(".rename-key-table-entry-description").text("Rename genre");
        $(".delete-key-table-entry-description").text("Delete genre");
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
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private updateContentAfterChange() {
        this.util.getLiteraryGenreList().done((data: IGenreResponseContract[]) => {
            this.genreItemList = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog("Warning", "Connection to server lost.\nAutomatic page reload is not possible.");
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
                this.gui.showSingleInputDialog("Name input", "Please input new genre name:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".input-dialog-textarea");
                        const genreString = textareaEl.val();
                        const newGenreAjax = this.util.createNewGenre(genreString);
                        newGenreAjax.done(() => {
                            textareaEl.val("");
                            this.gui.showInfoDialog("Success", "New genre has been created");
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newGenreAjax.fail(() => {
                            this.gui.showInfoDialog("Error", "New genre has not been created");
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
                    this.gui.showSingleInputDialog("Name input", "Please input genre name after rename:");
                    const textareaEl = $(".input-dialog-textarea");
                    const originalText = selectedPageEl.text();
                    textareaEl.val(originalText);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const genreName = textareaEl.val();
                            const genreId = selectedPageEl.data("key-id") as number;
                            const renameAjax = this.util.renameGenre(genreId, genreName);
                            renameAjax.done(() => {
                                textareaEl.val("");
                                this.gui.showInfoDialog("Success", "Genre has been renamed");
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            renameAjax.fail(() => {
                                this.gui.showInfoDialog("Error", "Genre has not been renamed");
                                $(".info-dialog-ok-button").off();
                            });
                    });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a genre");
                }
            });
    }

    private genreDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog("Confirm", "Are you sure you want to delete this genre?");
                    $(".confirmation-ok-button").on("click",
                        () => {
                                const id = selectedPageEl.data("key-id") as number;
                                const deleteAjax = this.util.deleteGenre(id);
                                deleteAjax.done(() => {
                                    $(".confirmation-ok-button").off();
                                    this.gui.showInfoDialog("Success", "Genre deletion was successful");
                                    this.updateContentAfterChange();
                                });
                                deleteAjax.fail(() => {
                                    $(".confirmation-ok-button").off();
                                    this.gui.showInfoDialog("Error", "Genre deletion was not successful");
                                });
                        });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a genre");
                }
            });
    }
}