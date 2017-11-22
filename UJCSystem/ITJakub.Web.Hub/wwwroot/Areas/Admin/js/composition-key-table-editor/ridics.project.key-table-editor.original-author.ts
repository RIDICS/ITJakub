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
        $(".create-key-table-entry-description").text("Create new original author");
        $(".rename-key-table-entry-description").text("Rename original author");
        $(".delete-key-table-entry-description").text("Delete original author");
        {
            const initialPage = 1;
            const initial = true;
            this.loadPage(initialPage, initial);
            this.currentPage = initialPage;
            this.authorRename();
            this.authorDelete();
            this.authorCreation();
        };
    };

    private updateContentAfterChange() {
        const initial = true;
        this.loadPage(this.currentPage, initial);
    }

    private loadPage(pageNumber: number, initial?: boolean) {
        const listEl = $(".selectable-list-div");
        const startIndex = (pageNumber - 1) * this.numberOfItemsPerPage;
        const endIndex = pageNumber * this.numberOfItemsPerPage;
        const pagedAuthorListAjax = this.util.getOriginalAuthorList(startIndex, endIndex);
        pagedAuthorListAjax.done((data: IOriginalAuthorPagedResult) => {
            listEl.empty();
            if (initial) {
                this.initPagination(data.totalCount, this.numberOfItemsPerPage, this.loadPage.bind(this));
            }
            listEl.append(this.generateListStructure(data.list));
            this.makeSelectable(listEl);
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private generateListStructure(originalAuthorItemList: IOriginalAuthor[]): JQuery {
        const listStart = `<div class="list-group">`;
        const listItemEnd = `</div>`;
        const listEnd = "</div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < originalAuthorItemList.length; i++) {
            const listItemStart =
                `<div class="page-list-item list-group-item" data-key-id="${originalAuthorItemList[i].id}"><span class="person-name">${
                    originalAuthorItemList[i].firstName}</span><span class="person-surname">${originalAuthorItemList[i]
                    .lastName}</span>`;
            elm += listItemStart;
            elm += listItemEnd;
        }
        elm += listEnd;
        const html = $.parseHTML(elm);
        const jListEl = $(html);
        return jListEl;
    }

    private authorCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showAuthorInputDialog("Create new author",
                    "Please input new author's name:",
                    "Please input new author's surname:");
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
                const selectedPageEl = $(".list-group").children(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showAuthorInputDialog("Rename author's name",
                        "Please input new author's name:",
                        "Please input new author's surname:");
                    const nameTextareaEl = $(".primary-input-author-textarea");
                    const surnameTextareaEl = $(".secondary-input-author-textarea");
                    const originalName = selectedPageEl.children(".person-name").text();
                    const originalSurname = selectedPageEl.children(".person-surname").text();
                    nameTextareaEl.val(originalName);
                    surnameTextareaEl.val(originalSurname);
                    $(".info-dialog-ok-button").on("click",
                        () => {
                            const nameString = nameTextareaEl.val();
                            const surnameString = surnameTextareaEl.val();
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
                        });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose an author");
                }
            });
    }

    private authorDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog("Confirm", "Are you sure you want to delete this author?");
                    $(".confirmation-ok-button").on("click",
                        () => {
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
                        });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose an author");
                }
            });
    }
}