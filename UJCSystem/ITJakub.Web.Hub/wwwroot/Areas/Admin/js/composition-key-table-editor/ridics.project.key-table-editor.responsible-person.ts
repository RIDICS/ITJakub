class KeyTableResponsiblePerson extends KeyTableEditorBase{
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private responsiblePersonItemList: IResponsiblePerson[];

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        $("#project-layout-content").find("*").off();
        this.createEntryButtonEl.text("Create new responsible person");
        this.changeEntryButtonEl.text("Rename responsible person");
        this.deleteEntryButtonEl.text("Delete responsible person");
        this.titleEl.text("Responsible people");
        this.unbindEventsDialog();
        {
            const initialPage = 1;
            const initial = true;
            this.loadPage(initialPage, initial);
            this.currentPage = initialPage;
            this.responsiblePersonRename();
            this.responsiblePersonDelete();
            this.responsiblePersonCreation();
        };
    };
    private updateContentAfterChange() {
        const initial = true;
        this.loadPage(this.currentPage, initial);
    }

    private loadPage(pageNumber: number, initial? : boolean) {
        const listEl = $(".selectable-list-div");
        const startIndex = (pageNumber - 1) * this.numberOfItemsPerPage;
        const endIndex = pageNumber * this.numberOfItemsPerPage;
        const pagedResponsiblePersonListAjax = this.util.getResponsiblePersonList(startIndex, endIndex);
        pagedResponsiblePersonListAjax.done((data: IResponsiblePersonPagedResult) => {
            listEl.empty();
            if(initial){this.initPagination(data.totalCount, this.numberOfItemsPerPage, this.loadPage.bind(this));}
            listEl.append(this.generateListStructure(data.list));
            this.makeSelectable(listEl);
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private generateListStructure(responsiblePersonItemList: IResponsiblePerson[]): JQuery {
        const listStart = `<div class="list-group">`;
        const listItemEnd = `</div>`;
        const listEnd = "</div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < responsiblePersonItemList.length; i++) {
            const listItemStart =
                `<div class="page-list-item list-group-item" data-key-id="${responsiblePersonItemList[i].id}"><span class="person-name">${responsiblePersonItemList[i].firstName}</span><span class="person-surname">${responsiblePersonItemList[i].lastName}</span>`;
            elm += listItemStart;
            elm += listItemEnd;
        }
        elm += listEnd;
        const html = $.parseHTML(elm);
        const jListEl = $(html);
        return jListEl;
    }

    private responsiblePersonCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showAuthorInputDialog("Create new responsible person", "Responsible person's name:", "Responsible person's surname:");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const nameTextareaEl = $(".primary-input-author-textarea");
                        const nameString = nameTextareaEl.val() as string;
                        const surnameTextareaEl = $(".secondary-input-author-textarea");
                        const surnameString = surnameTextareaEl.val() as string;
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

    private responsiblePersonRename() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").children(".page-list-item-selected");
                if (selectedPageEl.length) {
                    const nameTextareaEl = $(".primary-input-author-textarea");
                    const surnameTextareaEl = $(".secondary-input-author-textarea");
                    const originalName = selectedPageEl.children(".person-name").text();
                    const originalSurname = selectedPageEl.children(".person-surname").text();
                    nameTextareaEl.val(originalName);
                    surnameTextareaEl.val(originalSurname);
                    this.gui.showAuthorInputDialog("Rename responsible person",
                        "Responsible person's name:",
                        "Responsible person's surname:");
                    $(".info-dialog-ok-button").on("click",
                        () => {
                            const nameString = nameTextareaEl.val() as string;
                            const surnameString = surnameTextareaEl.val() as string;
                                const responsiblePersonId = selectedPageEl.data("key-id") as number;
                                const renameAjax = this.util.renameResponsiblePerson(responsiblePersonId,
                                    nameString,
                                    surnameString);
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
                        });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a responsible person");
                }
            });
    }

    private responsiblePersonDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog("Confirm",
                        "Are you sure you want to delete this responsible person?");
                    $(".confirmation-ok-button").on("click",
                        () => {
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
                        });
                } else {
                    this.gui.showInfoDialog("Warning", "Please choose a responsible person");
                }
            });
    }
}