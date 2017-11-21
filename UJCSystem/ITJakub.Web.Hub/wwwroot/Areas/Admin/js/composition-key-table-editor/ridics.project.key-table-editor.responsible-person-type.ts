class KeyTableResponsiblePersonEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private responsibleTypeItemList: IResponsibleType[];

    constructor() {
        super();
        this.util = new KeyTableUtilManager();
        this.gui = new EditorsGui();
    }

    init() {
        $("#project-layout-content").find("*").off();
        $(".create-key-table-entry").text("Create new responsible person type");
        $(".rename-key-table-entry").text("Change responsible person type");
        $(".delete-key-table-entry").text("Delete responsible person type");
        this.util.getResponsiblePersonTypeList().done((data: IResponsibleType[]) => {
            this.responsibleTypeItemList = data;
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
            const initialPage = 1;
            this.loadPage(initialPage);
            this.currentPage = initialPage;
            this.responsibleTypeChange();
            this.responsibleTypeDelete();
            this.responsibleTypeCreation();
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load editor");
            $("#project-layout-content").empty().append(error.buildElement());
        });
    };

    private updateContentAfterChange() {
        this.util.getResponsiblePersonTypeList().done((data: IResponsibleType[]) => {
            this.responsibleTypeItemList = data;
            this.loadPage(this.currentPage);
            const itemsOnPage = this.numberOfItemsPerPage;
            this.initPagination(data.length, itemsOnPage, this.loadPage.bind(this));
        }).fail(() => {
            this.gui.showInfoDialog("Warning", "Connection to server lost.\nAutomatic page reload is not possible.");
        });
    }

    private loadPage(pageNumber: number) {
        const listEl = $(".selectable-list-div");
        const splitArray = this.splitArray(this.responsibleTypeItemList, pageNumber);
        listEl.empty();
        const generatedListStructure = this.generateresponsibleTypeList(splitArray, listEl);
        listEl.append(generatedListStructure);
        this.makeSelectable(listEl);
    }

    private generateresponsibleTypeList(genreItemList: IResponsibleType[], jEl: JQuery): JQuery {
        const nameArray = genreItemList.map(a => a.text);
        const idArray = genreItemList.map(a => a.id);
        return this.generateSimpleList(idArray, nameArray, jEl);
    }

    private responsibleTypeCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showResponsibleTypeInputDialog("Responsible type input",
                    "Please input description:",
                    "Please input responsibility type");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".responsible-type-text-input-dialog-textarea");
                        const typeDescriptionString = textareaEl.val();
                        if (!typeDescriptionString) {
                            this.gui.showInfoDialog("Warning", "You haven't entered anything.");
                        } else {
                            const responsibilityTypeSelectEl = $(".responsible-type-selection");
                            const responsibilityTypeSelect = responsibilityTypeSelectEl.val() as ResponsibleTypeEnum;
                            const newResponsibleTypeAjax =
                                this.util.createNewResponsiblePersonType(responsibilityTypeSelect,
                                    typeDescriptionString);
                            newResponsibleTypeAjax.done(() => {
                                textareaEl.val("");
                                this.gui.showInfoDialog("Success", "New responsibility type has been created");
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            newResponsibleTypeAjax.fail(() => {
                                this.gui.showInfoDialog("Error", "New responsibility type has not been created");
                                $(".info-dialog-ok-button").off();
                            });
                        }
                    });
            });
    }

    private responsibleTypeChange() {
        $(".crud-buttons-div").on("click",
            ".rename-key-table-entry",
            () => {
                this.gui.showResponsibleTypeInputDialog("Responsible type input",
                    "Please input new description:",
                    "Please input new responsibility type");
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const textareaEl = $(".responsible-type-text-input-dialog-textarea");
                        const typeDescriptionString = textareaEl.val();
                        const responsibilityTypeSelectEl = $(".responsible-type-selection");
                        const responsibilityTypeSelect = responsibilityTypeSelectEl.val() as ResponsibleTypeEnum;
                        const selectedPageEl = $(".page-list").children(".page-list-item-selected");
                        if (!typeDescriptionString) {
                            this.gui.showInfoDialog("Warning", "You haven't entered anything.");
                        } else {
                            if (selectedPageEl.length) {
                                const responsibleTypeId = selectedPageEl.data("key-id") as number;
                                const renameAjax = this.util.renameResponsiblePersonType(responsibleTypeId, responsibilityTypeSelect, typeDescriptionString);
                                renameAjax.done(() => {
                                    textareaEl.val("");
                                    this.gui.showInfoDialog("Success", "Responsibility type has been changed");
                                    $(".info-dialog-ok-button").off();
                                    this.updateContentAfterChange();
                                });
                                renameAjax.fail(() => {
                                    this.gui.showInfoDialog("Error", "Responsibility type has not been changed");
                                    $(".info-dialog-ok-button").off();
                                });
                            } else {
                                this.gui.showInfoDialog("Warning", "Please choose a responsibility type");
                            }
                        }
                    });
            });
    }

    private responsibleTypeDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                this.gui.showConfirmationDialog("Confirm", "Are you sure you want to delete this responsibility type?");
                $(".confirmation-ok-button").on("click",
                    () => {
                        const selectedPageEl = $(".page-list").find(".page-list-item-selected");
                        if (selectedPageEl.length) {
                            const id = selectedPageEl.data("key-id") as number;
                            const deleteAjax = this.util.deleteResponsiblePersonType(id);
                            deleteAjax.done(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Success", "Responsibility type deletion was successful");
                                this.updateContentAfterChange();
                            });
                            deleteAjax.fail(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog("Error", "Responsibility type deletion was not successful");
                            });
                        } else {
                            this.gui.showInfoDialog("Warning", "Please choose a responsibility type");
                        }
                    });
            });
    }
}