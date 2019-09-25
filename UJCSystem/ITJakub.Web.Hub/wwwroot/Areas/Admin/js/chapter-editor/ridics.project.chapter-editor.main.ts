class ChapterEditorMain {
    private editDialog: BootstrapDialogWrapper;
    private readonly gui: EditorsGui;
    private readonly util: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private readonly moveEditor: ChapterMoveEditor;
    private moveChapterDownButton: JQuery<HTMLElement>;
    private moveChapterUpButton: JQuery<HTMLElement>;
    private moveChapterLeftButton: JQuery<HTMLElement>;
    private moveChapterRightButton: JQuery<HTMLElement>;

    constructor() {
        this.gui = new EditorsGui();
        this.errorHandler = new ErrorHandler();
        this.util = new EditorsApiClient();
        this.moveEditor = new ChapterMoveEditor();
    }
    
    init(projectId: number) {
        this.moveEditor.init();
        this.editDialog = new BootstrapDialogWrapper({
            element: $("#project-chapters-dialog"),
            autoClearInputs: false
        });
        
        $(".save-chapters-button").on("click", () => {
            const chapters = $(".chapter-row").toArray();
            // TODO save chapters
           /* const pageListArray: IUpdatePage[] = [];
            for (let i = 0; i < pages.length; i++) {
                pageListArray.push({
                    id: $(pages[i]).data("page-id"),
                    position: i + 1,
                    name: $(pages[i]).find(".name").text().trim()
                });
            }
            this.util.savePageList(projectId, pageListArray).done(() => {
                $("#unsavedChanges").addClass("hide");
            }).fail((error) => {
                this.gui.showInfoDialog(localization.translate("Error").value, this.errorHandler.getErrorMessage(error));
            });*/
        });

        this.initChapterRowClicks($($(".sub-chapters").get(0)));

        $("#project-chapters-edit-button").click(() => {
            this.editDialog.show();
            
        });

        $(".chapter-list-editor-content").on("click",
            ".create-chapter",
            () => {
                //TODO create chapter
            });

        $(".chapter-list-editor-content").on("click",
            ".cancel-chapter",
            (event) => {
                event.stopPropagation();
                this.editDialog.hide();
                $(".chapter-list-editor-content").off();
            }
        );
    }

    private initChapterRowClicks(subChapters: JQuery<HTMLElement>) {
        subChapters.find(".chapter-row .ridics-checkbox").change(() => {
            this.moveEditor.checkMoveButtonsAvailability();
        });

        subChapters.find(".chapter-row .ridics-checkbox label").off();
        subChapters.find(".chapter-row .ridics-checkbox label").click((event) => {
            event.stopPropagation(); //stop propagation to prevent loading detail, while is clicked on the checkbox
        });


        subChapters.find(".chapter-row .remove-chapter").off();
        subChapters.find(".chapter-row .remove-chapter").click((event) => {
            event.stopPropagation();
            const chapterRow = $(event.currentTarget).parents(".chapter-row");
            chapterRow.remove();
            this.showUnsavedChangesAlert();
        });

        subChapters.find(".chapter-row .edit-chapter").off();
        subChapters.find(".chapter-row .edit-chapter").on("click", (event) => {
            event.stopPropagation();
            this.editChapter($(event.currentTarget));
        });

        subChapters.find(".chapter-row").off();
        subChapters.find(".chapter-row").on("click", (event) => {
            const checkbox = $(event.currentTarget).find(".selection-checkbox");
            checkbox.prop("checked", !checkbox.is(":checked"));
            this.selectChapter($(event.currentTarget));
            this.moveEditor.checkMoveButtonsAvailability();
        });
    }

    private editChapter(element: JQuery) {
        const editButton = element.find("i.fa");
        const chapterRow = editButton.parents(".page-chapter");
        const nameElement = chapterRow.find(".name");
        //TODO fix, edit also page number
        const nameInput = chapterRow.find("input[name=\"chapter-name\"]");
        if (editButton.hasClass("fa-pencil")) {
            editButton.switchClass("fa-pencil", "fa-check");
            nameInput.removeClass("hide");
        } else {
            nameInput.addClass("hide");
            editButton.switchClass("fa-check", "fa-pencil");
            const newName = String(nameInput.val());
            if (String(nameElement.text()) !== newName) {
                nameElement.text(newName);
                this.showUnsavedChangesAlert();
            }
        }
    }

    private selectChapter(chapterRow: JQuery) {
        chapterRow.siblings().removeClass("active");
        chapterRow.addClass("active");

        const pageId = chapterRow.data("page-id");
        const pageDetail = $("#page-detail");
        const alertHolder = pageDetail.find(".alert-holder");
        const content = pageDetail.find(".body-content");
        const textIcon = pageDetail.find(".fa-file-text-o");
        const imageIcon = pageDetail.find(".fa-image");
        alertHolder.empty();

        if (typeof pageId == "undefined") {
            textIcon.addClass("hide");
            imageIcon.addClass("hide");
            const alert = new AlertComponentBuilder(AlertType.Info)
                .addContent(localization.translate("EmptyPage", "RidicsProject").value).buildElement();
            alertHolder.empty().append(alert);
            content.empty();
            pageDetail.removeClass("hide");
            return;
        }

        content.empty().html("<div class=\"sub-content\"></div>");
        const subcontent = content.find(".sub-content");
        subcontent.addClass("loader");
        pageDetail.removeClass("hide");
        
        this.util.getPageDetail(pageId).done((response) => {
            subcontent.removeClass("loader");
            subcontent.html(response);

            if (content.find(".page-text").length > 0) {
                textIcon.removeClass("hide");
            } else {
                textIcon.addClass("hide");
            }

            if (content.find(".image-preview").length > 0) {
                imageIcon.removeClass("hide");
            } else {
                imageIcon.addClass("hide");
            }
        }).fail((error) => {
            const alert = new AlertComponentBuilder(AlertType.Error)
                .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
            alertHolder.empty().append(alert);
            subcontent.removeClass("loader").empty();
        });
    }

    private populateList(pageList: string[]) {
        const listContainerEl = $(".page-listing tbody");
        if (listContainerEl.length) {
            this.gui.showInfoDialog(localization.translate("Info").value, localization.translate("AddingGeneratedNames", "RidicsProject").value);
        }

        for (let page of pageList) {
            const html = this.createChapterRow(page);
            listContainerEl.append(html);
        }

        this.showUnsavedChangesAlert();
       // this.initChapterRowClicks();
    }

    private showUnsavedChangesAlert() {
        $("#unsavedChanges").removeClass("hide");
    }

    //TODO fix this
    private createChapterRow(name: string): string {
        return `<tr class="chapter-row">
                    <td class="ridics-checkbox">
                        <label>
                            <input type="checkbox" class="selection-checkbox">
                            <span class="cr cr-black">
                                <i class="cr-icon glyphicon glyphicon-ok"></i>
                            </span>
                        </label>
                    </td>
                    <td>
                        <div>
                            <input type="text" name="page-name" class="form-control hide" value="${name}" />
                            <div class="name">
                                ${name}
                            </div>
                        </div>
                        <div class="alert alert-danger"></div>
                    </td>
                    <td class="buttons">
                        <a class="edit-page btn btn-sm btn-default">
                            <i class="fa fa-pencil"></i>
                        </a>
                        <a class="remove-page btn btn-sm btn-default">
                            <i class="fa fa-trash"></i>
                        </a>
                    </td>
                </tr>`;
    }
}