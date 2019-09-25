class ChapterMoveEditor {
    private moveChapterDownButton: JQuery<HTMLElement>;
    private moveChapterUpButton: JQuery<HTMLElement>;
    private moveChapterRightButton: JQuery<HTMLElement>;
    private moveChapterLeftButton: JQuery<HTMLElement>;

    init() {
        this.moveChapterDownButton = $(".panel-bottom-buttons .move-chapter-down");
        this.moveChapterDownButton.click(() => {
            this.moveList(true, $(".chapters"));
        });

        this.moveChapterUpButton = $(".panel-bottom-buttons .move-chapter-up");
        this.moveChapterUpButton.click(() => {
            this.moveList(false, $(".chapters"));
        });

        this.moveChapterRightButton = $(".panel-bottom-buttons .move-chapter-right");
        this.moveChapterRightButton.click(() => {
            this.hierarchyMove(true);
        });

        this.moveChapterLeftButton = $(".panel-bottom-buttons .move-chapter-left");
        this.moveChapterLeftButton.click(() => {
            this.hierarchyMove(false); 
        });
    }
    
    checkMoveButtonsAvailability() {
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

    private hierarchyMove(right: boolean) {
        if ($(".chapter-row .selection-checkbox:checked").length === 0
            || !this.checkHierarchyMoving()
            || (right && !this.checkHierarchyMovingToRight())
            || (!right && !this.checkHierarchyMovingToLeft())) {
            return;
        }
        const selectedChapters = $(".chapter-row .selection-checkbox:checked").closest(".chapter-container");

        if (right) {
            const chaptersContainer = $($(".selection-checkbox:checked").get(0)).closest(".chapter-container");
            $(chaptersContainer).prev().children(".sub-chapters").append(selectedChapters.detach());
        } else {
            const parentContainer = $(selectedChapters.get(0)).parent(".sub-chapters").parent(".chapter-container");
            parentContainer.after(selectedChapters.detach());
        }

        const moveDirection = right ? 1 : -1;
        for (let chapter of selectedChapters.find(".chapter-row").toArray()) {
            const newLevel = Number($(chapter).data("level")) + moveDirection;
            $(chapter).data("level", newLevel);
            $(chapter).children(".ridics-checkbox").attr("style", `margin-left: ${newLevel}em;`);
        }

        this.showUnsavedChangesAlert();
        this.checkMoveButtonsAvailability();
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
        return chapterContainerBefore.length > 0;
    }

    private moveList(down: boolean, context: JQuery) {
        if ($(".chapter-row .selection-checkbox:checked").length === 0 || !this.checkSameSubChapter()) {
            return;
        }

        const distance = Number(context.find(".chapter-move-distance").val());
        const subChaptersElement = $(".chapter-row .selection-checkbox:checked").parents(".sub-chapters").get(0);
        const chapters = $(subChaptersElement).children(".chapter-container").detach();
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
        for (let i = 0; i < chapters.length; i++) {
            const chapter = chapters[i];
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

        for (const chapter of newChapters) {
            subChaptersElement.append(chapter);
        }

        this.showUnsavedChangesAlert();
        this.checkMoveButtonsAvailability();
    }

    private showUnsavedChangesAlert() {
        $("#unsavedChanges").removeClass("hide");
    }
}