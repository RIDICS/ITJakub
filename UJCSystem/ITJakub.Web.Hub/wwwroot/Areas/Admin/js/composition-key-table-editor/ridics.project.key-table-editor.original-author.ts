class KeyTableOriginalAuthorEditor extends KeyTableEditorBase {
    private readonly util: KeyTableUtilManager;
    private readonly gui: EditorsGui;

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
        this.titleEl.text(localization.translate("OriginalAuthorHeadline", "KeyTable").value);
        this.unbindEventsDialog();
        {
            const initialPage = 1;
            const initial = true;
            this.loadPage(initialPage, initial);
            this.currentPage = initialPage;
            this.translateButtons();
            this.authorRename();
            this.authorDelete();
            this.authorCreation();
        };
    };

    private updateContentAfterChange() {
        this.showLoading();
        const initial = true;
        this.loadPage(this.currentPage, initial);
    }

    private loadPage(pageNumber: number, initial?: boolean) {
        const listEl = $(".key-table-div");
        const startIndex = (pageNumber - 1) * this.numberOfItemsPerPage;
        const pagedAuthorListAjax = this.util.getOriginalAuthorList(startIndex, this.numberOfItemsPerPage);
        pagedAuthorListAjax.done((data: IPagedResult<IOriginalAuthor>) => {
            listEl.empty();
            if (initial) {
                this.initPagination(data.totalCount, this.numberOfItemsPerPage, this.loadPage.bind(this));
            }
            listEl.append(this.generateListStructure(data.list));
            this.translateButtons();
            this.authorRename();
            this.authorDelete();
            this.makeSelectable(listEl);
        }).fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("EditorLoadError", "KeyTable").value);
            $("#project-layout-content").empty().append(error.buildElement());
        });
    }

    private generateListStructure(originalAuthorItemList: IOriginalAuthor[]): JQuery {
        const listStart = `<div class="table-responsive"><table class="table table-hover"><tbody>`;
        const listItemEnd = `</tr>`;
        const listEnd = "</tbody></table></div>";
        var elm = "";
        elm += listStart;
        for (let i = 0; i < originalAuthorItemList.length; i++) {
            const listItemStart =
                `<tr class="page-list-item" data-key-id="${originalAuthorItemList[i].id}"><td><div class="glyphicon empty-glyphicon"></div></td><td class="person-name">${
                    originalAuthorItemList[i].firstName}</td><td class="person-surname">${originalAuthorItemList[i]
                    .lastName}</td>`;
            elm += listItemStart;
            const changeButton =
                `<td class="key-table-button-cell"><button type="button" class="btn btn-default rename-key-table-entry" data-target="${originalAuthorItemList[i].id}">
                <i class="fa fa-pencil" aria - hidden="true"> </i>
                <span class="rename-key-table-entry-description"> Rename table of keys entry </span>
                </button>`;
            elm += changeButton;
            const removeButton =
                `<button type="button" class="btn btn-default delete-key-table-entry separate-button" data-target="${originalAuthorItemList[i].id}">
                <i class="fa fa-trash-o" aria-hidden="true"></i>
                <span class="delete-key-table-entry-description">Delete table of keys entry</span>
                </button></td>`;
            elm += removeButton;
            elm += listItemEnd;
        }
        elm += listEnd;
        const jListEl = $(elm);
        return jListEl;
    }

    private authorCreation() {
        $("button.create-key-table-entry").click(
            () => {
                this.gui.showAuthorInputDialog(localization.translate("OrigAuthInputHeadline", "KeyTable").value,
                    localization.translate("OrigAuthNameInput", "KeyTable").value,
                    localization.translate("OrigAuthSurnameInput", "KeyTable").value);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const nameTextareaEl = $(".primary-input-author-textarea");
                        const nameString = nameTextareaEl.val() as string;
                        const surnameTextareaEl = $(".secondary-input-author-textarea");
                        const surnameString = surnameTextareaEl.val() as string;
                        const newAuthorAjax = this.util.createOriginalAuthor(nameString, surnameString);
                        newAuthorAjax.done(() => {
                            nameTextareaEl.val("");
                            surnameTextareaEl.val("");
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("OrigAuthCreateSuccess", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        newAuthorAjax.fail(() => {
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("OrigAuthCreateError", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                        });
                    });
                $(".info-dialog-close-button").click(
                    () => {
                        const nameTextareaEl = $(".primary-input-author-textarea");
                        const surnameTextareaEl = $(".secondary-input-author-textarea");
                        nameTextareaEl.val("");
                        surnameTextareaEl.val("");
                    });
            });
    }

    private authorRename() {
        $("button.rename-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showAuthorInputDialog(localization.translate("OrigAuthInputHeadline", "KeyTable").value,
                    localization.translate("OrigAuthNameInput", "KeyTable").value,
                    localization.translate("OrigAuthSurnameInput", "KeyTable").value);
                const nameTextareaEl = $(".primary-input-author-textarea");
                const surnameTextareaEl = $(".secondary-input-author-textarea");
                const originalName = selectedPageEl.children(".person-name").text();
                const originalSurname = selectedPageEl.children(".person-surname").text();
                nameTextareaEl.val(originalName);
                surnameTextareaEl.val(originalSurname);
                $(".info-dialog-ok-button").on("click",
                    () => {
                        const nameString = nameTextareaEl.val() as string;
                        const surnameString = surnameTextareaEl.val() as string;
                        const authorId = selectedPageEl.data("key-id") as number;
                        const renameAjax = this.util.renameOriginalAuthor(authorId, nameString, surnameString);
                        renameAjax.done(() => {
                            nameTextareaEl.val("");
                            surnameTextareaEl.val("");
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("OrigAuthRenameSuccess", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                            this.updateContentAfterChange();
                        });
                        renameAjax.fail(() => {
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("OrigAuthRenameError", "KeyTable").value);
                            $(".info-dialog-ok-button").off();
                        });
                    });
                $(".info-dialog-close-button").click(
                    () => {
                        const nameTextareaEl = $(".primary-input-author-textarea");
                        const surnameTextareaEl = $(".secondary-input-author-textarea");
                        nameTextareaEl.val("");
                        surnameTextareaEl.val("");
                    });
            });

    }

    private authorDelete() {
        $("button.delete-key-table-entry").click(
            (event) => {
                const itemSelector = '*[data-key-id=' + event.currentTarget.dataset["target"] + ']';
                const selectedPageEl = $(itemSelector);
                this.gui.showConfirmationDialog(localization.translate("ModalConfirm", "KeyTable").value, localization.translate("OrigAuthConfirmMessage", "KeyTable").value);
                $(".confirmation-ok-button").on("click",
                    () => {
                        const id = selectedPageEl.data("key-id") as number;
                        const deleteAjax = this.util.deleteOriginalAuthor(id);
                        deleteAjax.done(() => {
                            $(".confirmation-ok-button").off();
                            this.gui.showInfoDialog(localization.translate("ModalSuccess", "KeyTable").value, localization.translate("OrigAuthRemoveSuccess", "KeyTable").value);
                            this.updateContentAfterChange();
                        });
                        deleteAjax.fail(() => {
                            $(".confirmation-ok-button").off();
                            this.gui.showInfoDialog(localization.translate("ModalError", "KeyTable").value, localization.translate("OrigAuthRemoveError", "KeyTable").value);
                        });
                    });
                });
    }
}