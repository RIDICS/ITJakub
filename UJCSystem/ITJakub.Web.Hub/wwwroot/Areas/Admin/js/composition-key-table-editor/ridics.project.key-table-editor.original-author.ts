class KeyTableOriginalAuthorEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new original author");
        $(".rename-key-table-entry").text("Rename original author");
        $(".delete-key-table-entry").text("Delete original author");
        {
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.authorRename();
            this.authorDelete();
            this.authorCreation();
        };
    };

    private updateContentAfterChange() {
            this.loadPage(this.currentPage);
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        const startIndex = (pageNumber - 1) * this.numberOfItemsPerPage;
        const endIndex = pageNumber * this.numberOfItemsPerPage;
        const pagedAuthorListAjax = this.util.getOriginalAuthorList(startIndex, endIndex);
        pagedAuthorListAjax.done((data: IOriginalAuthorContract[]) => {
            listEl.empty();
            this.initPagination(data.length, this.numberOfItemsPerPage, this.loadPage.bind(this));
            const generatedListStructure = this.generateAuthorList(data, listEl);
            listEl.append(generatedListStructure);
            this.makeSelectable(listEl); 
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private generateAuthorList(genreItemList: IOriginalAuthorContract[], jEl: JQuery): JQuery {
        const nameArray = genreItemList.map(a => (a.firstName+" "+a.lastName));
        const idArray = genreItemList.map(a => a.id);
        return this.generateSimpleList(idArray, nameArray, jEl);
    }

    private authorCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showAuthorInputDialog("Create new author", "Please input new author's name:", "Please input new author's surname:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const nameTextareaEl = $(".primary-input-author-textarea");
                        const nameString = nameTextareaEl.val();
                        const surnameTextareaEl = $(".secondary-input-author-textarea");
                        const surnameString = surnameTextareaEl.val();
                        const newAuthorAjax = this.util.createOriginalAuthor(nameString, surnameString);
                        newAuthorAjax.done(() => {
                            nameTextareaEl.val("");
                            surnameTextareaEl.val("");
                            this.gui.showInfoDialog("Success", "New author has been created");
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newAuthorAjax.fail(() => {
                            this.gui.showInfoDialog("Error", "New author has not been created");
                            $(".info-dialog-ok-button").off();
                        });
                    });
            });
    }

    private authorRename() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                this.gui.showAuthorInputDialog("Rename author's name", "Please input new author's name:", "Please input new author's surname:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const nameTextareaEl = $(".primary-input-author-textarea");
                        const nameString = nameTextareaEl.val();
                        const surnameTextareaEl = $(".secondary-input-author-textarea");
                        const surnameString = surnameTextareaEl.val();
                        const selectedPageEl = $(".page-list").children(".page-list-item-selected");
                        if (selectedPageEl.length) {
                            const authorId = selectedPageEl.data("key-id") as number;
                            const renameAjax = this.util.renameOriginalAuthor(authorId, nameString, surnameString);
                            renameAjax.done(() => {
                                nameTextareaEl.val("");
                                surnameTextareaEl.val("");
                                this.gui.showInfoDialog("Success", "Author has been renamed");
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            renameAjax.fail(() => {
                                this.gui.showInfoDialog("Error", "Author has not been renamed");
                                $(".info-dialog-ok-button").off();
                            });
                        } else {
                            this.gui.showInfoDialog("Warning", "Please choose an author");
                        }
                    });
            });
    }

    private authorDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                this.gui.showConfirmationDialog("Confirm", "Are you sure you want to delete this author?");
                $(".confirmation-ok-button").on("click",
                    () => {
                        const selectedPageEl = $(".page-list").find(".page-list-item-selected");
                        if (selectedPageEl.length) {
                            const id = selectedPageEl.data("key-id") as number;
                            const deleteAjax = this.util.deleteOriginalAuthor(id);
                            deleteAjax.done(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Success", "Author deletion was successful");
                                this.updateContentAfterChange();
                            });
                            deleteAjax.fail(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Error", "Author deletion was not successful");
                            });
                        } else {
                            this.gui.showInfoDialog("Warning", "Please choose an author");
                        }
                    });
            });
    }
}