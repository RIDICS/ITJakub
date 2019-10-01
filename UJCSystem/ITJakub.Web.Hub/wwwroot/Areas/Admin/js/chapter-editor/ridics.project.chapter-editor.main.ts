class ChapterEditorMain {
    private editDialog: BootstrapDialogWrapper;
    private readonly gui: EditorsGui;
    private readonly util: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private readonly moveEditor: ChapterMoveEditor;
    private position = 0;
    private chaptersToSave: IUpdateChapter[];

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
            this.chaptersToSave = [];
            this.getChaptersToSave($(".chapter-listing table > .sub-chapters"));
            this.util.saveChapterList(projectId, this.chaptersToSave).done(() => {
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

        $("select[name=\"chapter-page\"]").selectpicker({
            liveSearch: true,
            maxOptions: 1
        });
    }

    private getChaptersToSave(subChaptersElements: JQuery<HTMLElement>, parentId: number = null): void {
        const chapters = subChaptersElements.children(".chapter-container").children(".chapter-row");
        const subChaptersEl = subChaptersElements.children(".chapter-container").children(".sub-chapters");

        for (let i = 0; i < chapters.length; i++) {
            const id = Number($(chapters[i]).data("chapter-id"));
            const newChapter = {
                id: id,
                parentChapterId: parentId,
                position: this.position + 1,
                name: $(chapters[i]).find(".chapter-name").text().trim(), 
                beginningPageId: Number($(chapters[i]).find("option:selected").val())
            }
            this.position++;

            this.chaptersToSave.push(newChapter);

            if (subChaptersEl.children(".chapter-container").length !== 0) {
                this.getChaptersToSave(subChaptersEl, newChapter.id);
            }
        }
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
        const chapterRow = editButton.parents(".chapter-row");
        const nameElement = chapterRow.find(".chapter-name");
        const pageElement = chapterRow.find(".page-name");
        
        const nameInput = chapterRow.find("input[name=\"chapter-name\"]");
        const pageInput = chapterRow.find(".select-page.bootstrap-select");
        

        if (editButton.hasClass("fa-pencil")) {
            editButton.switchClass("fa-pencil", "fa-check");
            nameElement.addClass("hide");
            pageElement.addClass("hide");
            nameInput.removeClass("hide");
            pageInput.removeClass("hide");
        } else {
            editButton.switchClass("fa-check", "fa-pencil");
            nameInput.addClass("hide");
            pageInput.addClass("hide");
           
            const newName = String(nameInput.val());
            if (String(nameElement.text()) !== newName) {
                nameElement.text(newName);
                this.showUnsavedChangesAlert();
            }
            
            const newPageName = String(pageInput.find("option:selected").text());
            if (newPageName !== "" && String(pageElement.text()) !== newPageName) {
                pageElement.text(`[${newPageName}]`);
                this.showUnsavedChangesAlert();
            }

            nameElement.removeClass("hide");
            pageElement.removeClass("hide");
        }
    }

    private selectChapter(chapterRow: JQuery) {
        chapterRow.siblings().removeClass("active");
        chapterRow.addClass("active");

        const pageId = chapterRow.data("beginning-page-id");
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

    private createChapterRow(name: string, beginningPageId: number, beginningPageName: string, levelOfHierarchy = 0): JQuery<HTMLElement> {
        const newChapter = $(".chapter-template").children(".chapter-container").clone();
        newChapter.children(".chapter-row").data("level", levelOfHierarchy);
        newChapter.find(".ridics-checkbox").attr("style", `margin-left: ${levelOfHierarchy}em`);
        newChapter.find(".chapter-name").text(name);
        newChapter.find("input[name=\"chapter-name\"]").val(name);
        newChapter.find(".page-name").text(beginningPageName);
        newChapter.find("input[name=\"page-name\"]").val(beginningPageName);

        return newChapter;
    }
}