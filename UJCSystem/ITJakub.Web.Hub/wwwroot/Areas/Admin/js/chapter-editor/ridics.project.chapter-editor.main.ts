class ChapterEditorMain {
    private editDialog: BootstrapDialogWrapper;
    private readonly gui: EditorsGui;
    private readonly util: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private moveChapterDownButton: JQuery<HTMLElement>;
    private moveChapterUpButton: JQuery<HTMLElement>;
    private moveChapterLeftButton: JQuery<HTMLElement>;
    private moveChapterRightButton: JQuery<HTMLElement>;

    constructor() {
        this.gui = new EditorsGui();
        this.errorHandler = new ErrorHandler();
        this.util = new EditorsApiClient();
    }
    
    init(projectId: number) {
        this.editDialog = new BootstrapDialogWrapper({
            element: $("#project-chapters-dialog"),
            autoClearInputs: false
        });

        this.moveChapterDownButton = $(".panel-bottom-buttons .move-chapter-down");
        this.moveChapterDownButton.click(() => {
            this.moveList(true, $(".chapters"));
        });

        this.moveChapterUpButton = $(".panel-bottom-buttons .move-chapter-up");
        this.moveChapterUpButton.click(() => {
            this.moveList(false, $(".chapters"));
        });

        this.moveChapterLeftButton = $(".panel-bottom-buttons .move-chapter-left");
        this.moveChapterLeftButton.click(() => {

        });

        this.moveChapterRightButton = $(".panel-bottom-buttons .move-chapter-right");
        this.moveChapterRightButton.click(() => {

        });

        $(".save-chapters-button").on("click", () => {
            const chapters = $(".chapter-row").toArray();
            //TODO save chapters
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
            this.checkMoveButtonsAvailability();
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
            this.checkMoveButtonsAvailability();
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

    private checkSameSubChapter() {
        const selectedCheckboxes = $(".chapter-row .selection-checkbox:checked");
        const subChaptersElement = selectedCheckboxes.parents(".sub-chapters").get(0);

        const chapters = $(subChaptersElement).children(".chapter-container").toArray();
        let selectedChapters = 0;
        for (let chapter of chapters) {
            if ($(chapter).children(".chapter-row").children(".ridics-checkbox").find(".selection-checkbox")
                .is(":checked")) {
                selectedChapters++;
            }
        }

        return selectedChapters === selectedCheckboxes.length;
    }

    private checkHierarchyMoving(): boolean {
        const selectedCheckboxes = $(".chapter-row .selection-checkbox:checked");
        if (selectedCheckboxes.length === 0)
            return true;

        const subChaptersElement = selectedCheckboxes.parents(".sub-chapters").get(0);
        const chapters = $(subChaptersElement).children(".chapter-container").toArray();
        let selectedChapters = 0;
        let selectedLevel = $(chapters[0]).data("level");
        for (let chapter of chapters) {
            if (selectedLevel !== $(chapter).data("level")) {
                return false;
            }
            else if ($(chapter).children(".chapter-row").children(".ridics-checkbox").find(".selection-checkbox")
                .is(":checked")) {
                selectedChapters++;
                if (selectedChapters === selectedCheckboxes.length) {
                    return true;
                }
            } else if (selectedChapters > 0) {
                return false;
            }
        }

        return selectedChapters === selectedCheckboxes.length;
    }

    private checkHierarchyMovingToLeft(): boolean {
        const selectedCheckboxes = $(".chapter-row .selection-checkbox:checked");
        if (selectedCheckboxes.length === 0)
            return true;

        const chapterRow = selectedCheckboxes.parents(".chapter-row").get(0);
        return $(chapterRow).data("level") > 0;
    }

    private checkHierarchyMovingToRight(): boolean {
        const selectedCheckboxes = $(".chapter-row .selection-checkbox:checked");
        if (selectedCheckboxes.length === 0)
            return true;

        const chapterRow = $(selectedCheckboxes.get(0)).parents(".chapter-row");
        const chapterContainerBefore = $(chapterRow).parent(".chapter-container").prev();
        return chapterContainerBefore.length > 0 && chapterContainerBefore.find(".sub-chapters").length === 0;
    }

    private moveList(down: boolean, context: JQuery) {
        if ($(".chapter-row .selection-checkbox:checked").length === 0 || !this.checkSameSubChapter()) {
            return;
        }

        const distance = Number(context.find(".chapter-move-distance").val());
        const subChaptersElement = $(".chapter-row .selection-checkbox:checked").parents(".sub-chapters").get(0);
        const chapters = $(subChaptersElement).children(".chapter-container").toArray();
        const newChapters = new Array<HTMLElement>(chapters.length);
 
        for (let i = 0; i < chapters.length; i++) {
            const chapter = chapters[i];
            const isSelected = $(chapter).children(".chapter-row").children(".ridics-checkbox").find(".selection-checkbox").is(":checked");
            if (isSelected) {
                let newPosition = i;
                if (down) {
                    newPosition += distance;
                } else {
                    newPosition -= distance;
                }

                if (newPosition >= chapters.length || newPosition < 0) {
                    newPosition = newPosition % chapters.length;
                }
                if (newPosition < 0) {
                    newPosition = chapters.length + newPosition;
                }

                newChapters[newPosition] = chapter;
            }
        }

        let j = 0;
        for (const chapter of chapters) {
            const isSelected = $(chapter).children(".chapter-row").children(".ridics-checkbox").find(".selection-checkbox").is(":checked");
            if (!isSelected) {
                while (true) {
                    if (typeof newChapters[j] == "undefined") {
                        newChapters[j] = chapter;
                        break;
                    } else {
                        j++;
                    }
                }
            }
        }

        $(subChaptersElement).empty();
        for (const chapter of newChapters) {
            subChaptersElement.append(chapter);
        }

        this.showUnsavedChangesAlert();
        this.initChapterRowClicks($(subChaptersElement));
    }

    private showUnsavedChangesAlert() {
        $("#unsavedChanges").removeClass("hide");
    }

    private checkMoveButtonsAvailability() {
        if (this.checkSameSubChapter()) {
            this.moveChapterDownButton.removeAttr("disabled");
            this.moveChapterUpButton.removeAttr("disabled");
        } else {
            this.moveChapterDownButton.attr("disabled", "disabled");
            this.moveChapterUpButton.attr("disabled", "disabled");
        }

        if (this.checkHierarchyMoving()) {
            if (this.checkHierarchyMovingToLeft()) {
                this.moveChapterLeftButton.removeAttr("disabled");
            } else {
                this.moveChapterLeftButton.attr("disabled", "disabled");
            }

            if (this.checkHierarchyMovingToRight()) {
                this.moveChapterRightButton.removeAttr("disabled");
            } else {
                this.moveChapterRightButton.attr("disabled", "disabled");
            }
        } else {
            this.moveChapterLeftButton.attr("disabled", "disabled");
            this.moveChapterRightButton.attr("disabled", "disabled");
        }
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