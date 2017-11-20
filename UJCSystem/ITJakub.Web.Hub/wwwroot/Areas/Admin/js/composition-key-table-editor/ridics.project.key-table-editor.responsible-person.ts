class KeyTableResponsiblePerson extends KeyTableEditorBase{
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private responsiblePersonItemList: IResponsiblePersonContract[];

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new responsible person");
        $(".rename-key-table-entry").text("Rename responsible person");
        $(".delete-key-table-entry").text("Delete responsible person");
        {
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.responsiblePersonRename();
            this.responsiblePersonDelete();
            this.responsiblePersonCreation();
        };
    };
    private updateContentAfterChange() {
        this.loadPage(this.currentPage);
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        const startIndex = (pageNumber - 1) * this.numberOfItemsPerPage;
        const endIndex = pageNumber * this.numberOfItemsPerPage;
        const pagedResponsiblePersonListAjax = this.util.getResponsiblePersonList(startIndex, endIndex);
        pagedResponsiblePersonListAjax.done((data: IResponsiblePersonContract[]) => {
            listEl.empty();
            this.initPagination(data.length, this.numberOfItemsPerPage, this.loadPage.bind(this));
            const generatedListStructure = this.generateResponsiblePersonList(data, listEl);
            listEl.append(generatedListStructure);
            this.makeSelectable(listEl);
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private generateResponsiblePersonList(genreItemList: IResponsiblePersonContract[], jEl: JQuery): JQuery {
        const nameArray = genreItemList.map(a => (a.firstName + " " + a.lastName));
        const idArray = genreItemList.map(a => a.id);
        return this.generateSimpleList(idArray, nameArray, jEl);
    }

    private responsiblePersonCreation() {//TODO
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showAuthorInputDialog("Create new responsible person", "Responsible person's name:", "Responsible person's surname:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const nameTextareaEl = $(".primary-input-author-textarea");
                        const nameString = nameTextareaEl.val();
                        const surnameTextareaEl = $(".secondary-input-author-textarea");
                        const surnameString = surnameTextareaEl.val();
                        const newResponsiblePersonAjax = this.util.createResponsiblePerson(nameString, surnameString);
                        newResponsiblePersonAjax.done(() => {
                            nameTextareaEl.val("");
                            surnameTextareaEl.val("");
                            this.gui.showInfoDialog("Success", "New responsible person has been created");
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newResponsiblePersonAjax.fail(() => {
                            this.gui.showInfoDialog("Error", "New responsible person has not been created");
                            $(".info-dialog-ok-button").off();
                        });
                    });
            });
    }

    private responsiblePersonRename() {//TODO
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                this.gui.showAuthorInputDialog("Rename responsible person", "Responsible person's name:", "Responsible person's surname:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const nameTextareaEl = $(".primary-input-author-textarea");
                        const nameString = nameTextareaEl.val();
                        const surnameTextareaEl = $(".secondary-input-author-textarea");
                        const surnameString = surnameTextareaEl.val();
                        const selectedPageEl = $(".page-list").children(".page-list-item-selected");
                        if (selectedPageEl.length) {
                            const responsiblePersonId = selectedPageEl.data("key-id") as number;
                            const renameAjax = this.util.renameResponsiblePerson(responsiblePersonId, nameString, surnameString);
                            renameAjax.done(() => {
                                nameTextareaEl.val("");
                                surnameTextareaEl.val("");
                                this.gui.showInfoDialog("Success", "Responsible person has been renamed");
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            renameAjax.fail(() => {
                                this.gui.showInfoDialog("Error", "Responsible person has not been renamed");
                                $(".info-dialog-ok-button").off();
                            });
                        } else {
                            this.gui.showInfoDialog("Warning", "Please choose a responsible person");
                        }
                    });
            });
    }

    private responsiblePersonDelete() {//TODO
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                this.gui.showConfirmationDialog("Confirm", "Are you sure you want to delete this responsible person?");
                $(".confirmation-ok-button").on("click",
                    () => {
                        const selectedPageEl = $(".page-list").find(".page-list-item-selected");
                        if (selectedPageEl.length) {
                            const id = selectedPageEl.data("key-id") as number;
                            const deleteAjax = this.util.deleteResponsiblePerson(id);
                            deleteAjax.done(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Success", "Responsible person deletion was successful");
                                this.updateContentAfterChange();
                            });
                            deleteAjax.fail(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Error", "Responsible person deletion was not successful");
                            });
                        } else {
                            this.gui.showInfoDialog("Warning", "Please choose a responsible person");
                        }
                    });
            });
    }
}