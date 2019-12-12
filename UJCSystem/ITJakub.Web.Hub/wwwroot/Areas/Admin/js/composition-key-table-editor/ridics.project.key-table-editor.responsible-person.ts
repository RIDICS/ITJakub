class KeyTableResponsiblePerson extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;
    private responsiblePersonItemList: IResponsiblePerson[];

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
        this.titleEl.text(localization.translate("ResponsiblePersonHeadline", "KeyTable").value);
        this.unbindEventsDialog();
        const initialPage = 1;
        const initial = true;
        this.loadPage(initialPage, initial);
        this.currentPage = initialPage;
        this.responsiblePersonRename();
        this.responsiblePersonDelete();
        this.responsiblePersonCreation();
    };

    private updateContentAfterChange() {
        this.showLoading();
        const initial = true;
        this.loadPage(this.currentPage, initial);
    }

    private loadPage(pageNumber: number, initial?: boolean) {
        const listEl = $(".selectable-list-div");
        const startIndex = (pageNumber - 1) * this.numberOfItemsPerPage;
        const pagedResponsiblePersonListAjax = this.util.getResponsiblePersonList(startIndex, this.numberOfItemsPerPage);
        pagedResponsiblePersonListAjax.done((data: IPagedResult<IResponsiblePerson>) => {
            listEl.empty();
            if (initial) {
                this.initPagination(data.totalCount, this.numberOfItemsPerPage, this.loadPage.bind(this));
            }
            listEl.append(this.generateListStructure(data.list));
            this.makeSelectable(listEl);
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("EditorLoadError", "KeyTable").value);
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
                `<div class="page-list-item list-group-item" data-key-id="${responsiblePersonItemList[i].id
                    }"><span class="person-name">${responsiblePersonItemList[i].firstName
                    }</span><span class="person-surname">${responsiblePersonItemList[i].lastName}</span>`;
            elm += listItemStart;
            elm += listItemEnd;
        }
        elm += listEnd;
        const jListEl = $(elm);
        return jListEl;
    }

    private responsiblePersonCreation() {
        $(".crud-buttons-div").on("click",
            ".create-key-table-entry",
            () => {
                this.gui.showAuthorInputDialog(localization.translate("RespPerInputHeadline", "KeyTable").value,
                    localization.translate("RespPerNameInput", "KeyTable").value,
                    localization.translate("RespPerSurnameInput", "KeyTable").value);
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
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("RespPerCreateSuccess", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newResponsiblePersonAjax.fail(() => {
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("RespPerCreateError", "KeyTable").value);
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
                    this.gui.showAuthorInputDialog(localization.translate("RespPerInputHeadline", "KeyTable").value,
                        localization.translate("RespPerNameInput", "KeyTable").value,
                        localization.translate("RespPerSurnameInput", "KeyTable").value);
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
                                this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("RespPerRenameSuccess", "KeyTable").value);
                                $(".info-dialog-ok-button").off();
                                this.updateContentAfterChange();
                            });
                            renameAjax.fail(() => {
                                this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("RespPerRenameError", "KeyTable").value);
                                $(".info-dialog-ok-button").off();
                            });
                        });
                } else {
                    this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("RespPerInfoMessage", "KeyTable").value);
                }
            });
    }

    private responsiblePersonDelete() {
        $(".crud-buttons-div").on("click",
            ".delete-key-table-entry",
            () => {
                const selectedPageEl = $(".list-group").find(".page-list-item-selected");
                if (selectedPageEl.length) {
                    this.gui.showConfirmationDialog(localization.translate("ModalConfirm", "KeyTable").value, localization.translate("RespPerConfirmMessage", "KeyTable").value);
                    $(".confirmation-ok-button").on("click",
                        () => {
                            const id = selectedPageEl.data("key-id") as number;
                            const deleteAjax = this.util.deleteResponsiblePerson(id);
                            deleteAjax.done(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("RespPerDeleteSuccess", "KeyTable").value);
                                this.updateContentAfterChange();
                            });
                            deleteAjax.fail(() => {
                                $(".confirmation-ok-button").off();
                                this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("RespPerDeleteError", "KeyTable").value);
                            });
                        });
                } else {
                    this.gui.showInfoDialog(localization.translate("ModalWarning", "KeyTable").value, localization.translate("RespPerInfoMessage", "KeyTable").value);
                }
            });
    }
}