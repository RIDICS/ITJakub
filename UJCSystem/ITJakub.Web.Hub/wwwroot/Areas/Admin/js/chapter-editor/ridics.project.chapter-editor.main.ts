class ChapterEditorMain {
    private readonly util: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private readonly moveEditor: ChapterMoveEditor;
    private readonly readerPagination: ReaderPagination;
    private editDialog: BootstrapDialogWrapper;
    private chaptersToSave: IUpdateChapter[];
    private bookPages: Array<BookPage>;
    private position = 0;
    private pageDetail = $("#chaptersPageDetail")
    
    constructor() {
        this.errorHandler = new ErrorHandler();
        this.util = new EditorsApiClient();
        this.moveEditor = new ChapterMoveEditor();
        this.readerPagination = new ReaderPagination(this.pageDetail[0]);
    }

    init(projectId: number) {
        this.moveEditor.init();
        this.readerPagination.init(((pageId, pageIndex, scrollTo) => {
            this.loadPageDetail(pageId);
        }));
        this.bookPages = [];
        this.util.getPagesList(projectId).done((pages) => {
            for (let page of pages) {
                const bookPageItem = new BookPage(page.id, page.name, page.position);
                this.bookPages.push(bookPageItem);    
            }
            this.readerPagination.pages = this.bookPages;
        });

        const createChapterDialog = $("#projectChaptersDialog");
        this.editDialog = new BootstrapDialogWrapper({
            element: createChapterDialog,
            autoClearInputs: false
        });

        const listing = $(".chapter-listing");
        
        $("#generateChapters").on("click", (event) => {
            bootbox.confirm({
                title: localization.translate("Warning").value,
                message: localization.translate("GenerateChaptersWarning", "Admin").value,
                buttons: {
                    confirm: {
                        label: localization.translate("Generate", "Admin").value,
                        className: "btn-default"
                    },
                    cancel: {
                        label: localization.translate("Cancel").value
                    }
                },
                callback: (result => {
                    if(result)
                    {
                        $("#unsavedChanges").addClass("hide");
                        listing.empty().append(`<div class="loader"></div>`);
                        this.util.generateChapterList(projectId).done(() => {
                            this.util.getChapterListView(projectId).done((data) => {
                                listing.html(data);
                                this.initChapterRowClicks($(".table > .sub-chapters"));
                            }).fail((error) => {
                                const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                                listing.empty().append(alert);
                            });
                        }).fail((error) => {
                            const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                            listing.empty().append(alert);
                        });
                    }
                })
            });
        });

        $(".save-chapters-button").on("click", () => {
            if($(".edit-chapter i.fa-check").length > 0) {
                bootbox.alert({
                        title: localization.translate("Error").value,
                        message: localization.translate("ConfirmOrDiscardChapterChanges", "RidicsProject").value,
                        buttons: {
                            ok: {
                                className: "btn-default"
                            }
                        }
                    });
                return;
            }
            
            this.position = 0;
            this.chaptersToSave = [];
            this.getChaptersToSave($(".table > .sub-chapters"));
            listing.empty().append(`<div class="loader"></div>`);
            this.util.saveChapterList(projectId, this.chaptersToSave).done(() => {
                $("#chaptersUnsavedChanges").addClass("hide");
                this.util.getChapterListView(projectId).done((data) => {
                    listing.html(data);
                    this.initChapterRowClicks($(".table > .sub-chapters"));
                }).fail((error) => {
                    const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                    listing.empty().append(alert);
                });                
            }).fail((error) => {
                bootbox.alert({
                    title: localization.translate("Error").value,
                    message: this.errorHandler.getErrorMessage(error),
                    buttons: {
                        ok: {
                            className: "btn-default"
                        }
                    }
                });
            });
        });

        this.initChapterRowClicks($(".table > .sub-chapters"));

        $("#project-chapters-edit-button").click(() => {
            this.editDialog.show();
        });

        createChapterDialog.on("click",
            ".create-chapter",
            () => {
                const alertHolder = createChapterDialog.find(".alert-holder");
                alertHolder.empty();
                const chapterName = String(createChapterDialog.find("input[name=\"chapter-name\"]").val());
                if (chapterName === "") {
                    const alert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(localization.translate("ChapterNameRequired", "RidicsProject").value).buildElement();
                    alertHolder.append(alert);
                    return;
                }
                const selectedOption = createChapterDialog.find(".select-page option:selected");
                const selectedPageId = Number(selectedOption.val());
                const selectedPageName = `[${selectedOption.text()}]`;
                const chapter = this.createChapterRow(chapterName, selectedPageId, selectedPageName);
                const listing = $(".table > .sub-chapters");
                if(listing.children(".alert").length)
                {
                    listing.empty();
                }
                listing.append(chapter);                
                this.initChapterRowClicks(chapter);
                this.editDialog.hide();
                this.showUnsavedChangesAlert();
            });

        createChapterDialog.on("click",
            ".cancel-chapter",
            (event) => {
                event.stopPropagation();
                this.editDialog.hide();
            }
        );

        createChapterDialog.find(".select-page").selectpicker({
            liveSearch: true,
            maxOptions: 1
        });
    }

    private getChaptersToSave(subChaptersElements: JQuery<HTMLElement>, parentId: number = null): void {
        const chapters = subChaptersElements.children(".chapter-container");
        
        for (let i = 0; i < chapters.length; i++) {
            const chapterRow = $(chapters[i]).children(".chapter-row");
            const id = Number(chapterRow.data("chapter-id"));
            const newChapter: IUpdateChapter = {
                id: id,
                parentChapterId: parentId,
                position: this.position + 1,
                name: chapterRow.find(".chapter-name").text().trim(),
                beginningPageId: Number(chapterRow.data("beginning-page-id")),
                comment: null
            };
            
            this.position++;

            this.chaptersToSave.push(newChapter);

            const subChaptersEl = $(chapters[i]).children(".sub-chapters");
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
            const chapterContainer = $(event.currentTarget).parent(".buttons").parent(".chapter-row").parent(".chapter-container");
            chapterContainer.remove();
            this.showUnsavedChangesAlert();
            this.moveEditor.checkMoveButtonsAvailability();
        });

        subChapters.find(".chapter-row .edit-chapter").off();
        subChapters.find(".chapter-row .edit-chapter").on("click", (event) => {
            event.stopPropagation();
            this.editChapter($(event.currentTarget));
        });

        subChapters.find(".chapter-row .discard-chapter-changes").off();
        subChapters.find(".chapter-row .discard-chapter-changes").on("click", (event) => {
            event.stopPropagation();
            this.discardChapterChanges($(event.currentTarget));
        });

        subChapters.find(".chapter-row").off();
        subChapters.find(".chapter-row").on("click", (event) => {
            if($(event.target).parents(".buttons").length === 0) {
                const checkbox = $(event.currentTarget).find(".selection-checkbox");
                checkbox.prop("checked", !checkbox.is(":checked"));
                this.selectChapter($(event.currentTarget));
                this.moveEditor.checkMoveButtonsAvailability();
            }
        });
        
        subChapters.find("select[name=\"chapter-page\"]").selectpicker({
            liveSearch: true,
            maxOptions: 1,
            container: "body"
        });
        
        subChapters.find(".chapter-row input[name=\"chapter-name\"]").on("click", (event) => {
            event.stopPropagation();
        });
    }

    private editChapter(element: JQuery) {
        const editButton = element.find("i.fa");
        const chapterRow = editButton.parents(".chapter-row");
        const nameElement = chapterRow.find(".chapter-name");
        const pageElement = chapterRow.find(".page-name");
        const discardButton = chapterRow.find(".discard-chapter-changes");
        
        const nameInput = chapterRow.find("input[name=\"chapter-name\"]");
        const pageInput = chapterRow.find(".select-page.bootstrap-select");
        
        if (editButton.hasClass("fa-pencil")) {
            editButton.switchClass("fa-pencil", "fa-check");
            nameElement.addClass("hide");
            pageElement.addClass("hide");
            nameInput.removeClass("hide");
            pageInput.removeClass("hide");
            discardButton.removeClass("hide");
        } else {
            editButton.switchClass("fa-check", "fa-pencil");
            nameInput.addClass("hide");
            pageInput.addClass("hide");
            discardButton.addClass("hide");
            
            
            const newName = String(nameInput.val());
            if (newName !== "" && String(nameElement.text()) !== newName) {
                nameElement.text(newName);
                this.showUnsavedChangesAlert();
            }
            
            const newPageName = `[${pageInput.find("option:selected").text()}]`;
            if (newPageName !== "" && String(pageElement.text()) !== newPageName) {
                chapterRow.data("beginning-page-id", Number(pageInput.find("option:selected").val()));
                pageElement.text(newPageName);
                this.showUnsavedChangesAlert();
            }

            nameElement.removeClass("hide");
            pageElement.removeClass("hide");
        }
    }

    private discardChapterChanges(discardButton: JQuery) {        
        const chapterRow = discardButton.parents(".chapter-row");
        const nameElement = chapterRow.find(".chapter-name");
        const pageElement = chapterRow.find(".page-name");
        const editButton = chapterRow.find(".edit-chapter i");
        
        const nameInput = chapterRow.find("input[name=\"chapter-name\"]");
        const pageInput = chapterRow.find(".select-page.bootstrap-select");

        editButton.switchClass("fa-check", "fa-pencil");
        nameInput.addClass("hide");
        pageInput.addClass("hide");
        discardButton.addClass("hide");
        nameElement.removeClass("hide");
        pageElement.removeClass("hide");
        nameInput.val(nameElement.text());

        const oldPageId = chapterRow.data("beginning-page-id");
        pageInput.find(".selectpicker").selectpicker('val', oldPageId);        
    }

    private selectChapter(chapterRow: JQuery) {
        chapterRow.siblings().removeClass("active");
        chapterRow.addClass("active");

        const pageId = chapterRow.data("beginning-page-id");
               
        const content = this.pageDetail.find(".body-content");
        const textIcon = this.pageDetail.find(".fa-file-text-o");
        const imageIcon = this.pageDetail.find(".fa-image");
        const alertHolder = this.pageDetail.find(".alert-holder");
        

        if (typeof pageId == "undefined") {
            textIcon.addClass("hide");
            imageIcon.addClass("hide");
            const alert = new AlertComponentBuilder(AlertType.Info)
                .addContent(localization.translate("EmptyPage", "RidicsProject").value).buildElement();
            alertHolder.empty().append(alert);
            content.empty();
            this.pageDetail.removeClass("hide");
            return;
        }

        
        const pagination = this.readerPagination.createPagination();
        content.empty().html("<div class=\"page-navigation\"></div><div class=\"sub-content\"></div>");
        const paginationEl = content.find(".page-navigation");
        paginationEl.append(pagination);
        this.readerPagination.moveToPage(pageId);
        this.pageDetail.removeClass("hide");
    }

    private loadPageDetail(pageId: number) {
        const textIcon = this.pageDetail.find(".fa-file-text-o");
        const imageIcon = this.pageDetail.find(".fa-image");
        const alertHolder = this.pageDetail.find(".alert-holder");
        alertHolder.empty();

        const content = this.pageDetail.find(".body-content");
        const subcontent = content.find(".sub-content");
        subcontent.empty().append(`<div class="loader"></div>`);

        this.util.getPageDetail(pageId).done((response) => {
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
            subcontent.empty();
        });
    }

    private showUnsavedChangesAlert() {
        $("#chaptersUnsavedChanges").removeClass("hide");
    }

    private createChapterRow(name: string, beginningPageId: number, beginningPageName: string, levelOfHierarchy = 0): JQuery<HTMLElement> {
        const newChapter = $("#chapterTemplate").children(".chapter-container").clone();
        const chapterRow = newChapter.children(".chapter-row");
        chapterRow.data("level", levelOfHierarchy);
        chapterRow.data("beginning-page-id", beginningPageId);
        newChapter.find(".ridics-checkbox").attr("style", `margin-left: ${levelOfHierarchy}em`);
        newChapter.find(".chapter-name").text(name);
        newChapter.find("input[name=\"chapter-name\"]").val(name);
        newChapter.find(".page-name").text(beginningPageName);
        newChapter.find("select[name=\"chapter-page\"]").val(beginningPageName);
        newChapter.find(`select[name="chapter-page"] option[value="${beginningPageId}"]`).attr("selected", "selected");
        
        return newChapter;
    }
}