class ChapterEditorMain {
    private editDialog: BootstrapDialogWrapper;
    private readonly gui: EditorsGui;
    private readonly util: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private readonly moveEditor: ChapterMoveEditor;
    private position = 0;

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
            this.position = 0;
            this.util.saveChapterList(projectId, this.getChapters($(".chapter-listing table > .sub-chapters"))).done(() => {
                $("#unsavedChanges").addClass("hide");
            }).fail((error) => {
                this.gui.showInfoDialog(localization.translate("Error").value, this.errorHandler.getErrorMessage(error));
            });
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

    private getChapters(subChaptersElements: JQuery<HTMLElement>, parentId: number = null): IUpdateChapter[] {
        const chapters = subChaptersElements.children(".chapter-container").children(".chapter-row");
        const subChaptersEl = subChaptersElements.children(".chapter-container").children(".sub-chapters");

        const chapterList: IUpdateChapter[] = [];
        for (let i = 0; i < chapters.length; i++) {
            const newChapter = {
                id: Number($(chapters[i]).data("chapter-id")),
                parentId: parentId,
                position: this.position + 1,
                name: $(chapters[i]).find(".name").text().trim(), //TODO check, add others things
                starts: 0, //TODO page id or page name?
                subChapters: []
            }
            this.position++;

            if (subChaptersEl.children(".chapter-container").length !== 0) {
                newChapter.subChapters = this.getChapters(subChaptersEl, newChapter.id);
            }

            chapterList.push(newChapter);
        }

        return chapterList;
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
            this.moveEditor.checkMoveButtonsAvailability();
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

    private showUnsavedChangesAlert() {
        $("#unsavedChanges").removeClass("hide");
    }

    private createChapterRow(name: string, beginningPageId: number, beginningPageName: string, levelOfHierarchy = 0): string {
        return `<div class="chapter-container">
                    <div class="chapter-row" data-beginning-page-id="${beginningPageId}" data-level="${levelOfHierarchy}">
                        <div class="ridics-checkbox" style="margin-left: ${levelOfHierarchy}em">
                            <label>
                                <input type="checkbox" class="selection-checkbox" />
                                <span class="cr cr-black">
                                    <i class="cr-icon glyphicon glyphicon-ok"></i>
                                </span>
                            </label>
                        </div>
                        <div class="name">
                            <div>
                                <input type="text" name="chapter-name" class="form-control hide" value=" ${name}" />
                                 ${name}
                            </div>
                            <div class="alert alert-danger"></div>
                        </div>
                        <div class="buttons">
                            ${beginningPageName}
                            <a class="edit-chapter btn btn-sm btn-default" title="${localization.translate("EditChapterName", "RidicsProject").value}">
                                <i class="fa fa-pencil"></i>
                            </a>
                            <a class="remove-chapter btn btn-sm btn-default" title="${localization.translate("DeleteChapter", "RidicsProject").value}">
                                <i class="fa fa-trash"></i>
                            </a>
                        </div>
                    </div>
                    <div class="sub-chapters">
                    </div>
                </div>`;
    }
}