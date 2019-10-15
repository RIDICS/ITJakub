class ChapterEditorMain {
    private editDialog: BootstrapDialogWrapper;
    private readonly util: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private readonly moveEditor: ChapterMoveEditor;
    private readonly pagerDisplayPages: number;
    private position = 0;
    private chaptersToSave: IUpdateChapter[];
    private bookPages: Array<BookPage>;
    private actualPageIndex = 0;

    constructor() {
        this.errorHandler = new ErrorHandler();
        this.util = new EditorsApiClient();
        this.moveEditor = new ChapterMoveEditor();
        this.pagerDisplayPages = 5;
    }

    init(projectId: number) {
        this.moveEditor.init();
        this.bookPages = [];
        this.util.getPagesList(projectId).done((pages) => {
            for (let page of pages) {
                const bookPageItem = new BookPage(page.id, page.name, page.position);
                this.bookPages.push(bookPageItem);    
            }
        });

        this.editDialog = new BootstrapDialogWrapper({
            element: $("#projectChaptersDialog"),
            autoClearInputs: false
        });

        $(".save-chapters-button").on("click", () => {
            this.position = 0;
            this.chaptersToSave = [];
            this.getChaptersToSave($(".table > .sub-chapters"));
            this.util.saveChapterList(projectId, this.chaptersToSave).done(() => {
                $("#unsavedChanges").addClass("hide");
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

        $("#projectChaptersDialog").on("click",
            ".create-chapter",
            () => {
                $("#projectChaptersDialog .alert-holder").empty()
                const chapterName = String($("#projectChaptersDialog input[name=\"chapter-name\"]").val());
                if (chapterName === "") {
                    const alert = new AlertComponentBuilder(AlertType.Error)
                        .addContent(localization.translate("ChapterNameRequired", "RidicsProject").value).buildElement();
                    $("#projectChaptersDialog .alert-holder").append(alert);
                    return;
                }
                const selectedPageId = Number($("#projectChaptersDialog .select-page").find("option:selected").val());
                const selectedPageName = `[${$("#projectChaptersDialog .select-page").find("option:selected").text()}]`;
                const chapter = this.createChapterRow(chapterName, selectedPageId, selectedPageName);
                this.initChapterRowClicks(chapter);
                $(".table > .sub-chapters").append(chapter);
                this.editDialog.hide();
            });

        $("#projectChaptersDialog").on("click",
            ".cancel-chapter",
            (event) => {
                event.stopPropagation();
                this.editDialog.hide();
            }
        );

        $("#projectChaptersDialog .select-page").selectpicker({
            liveSearch: true,
            maxOptions: 1
        });
    }

    private getChaptersToSave(subChaptersElements: JQuery<HTMLElement>, parentId: number = null): void {
        const chapters = subChaptersElements.children(".chapter-container");
        
        for (let i = 0; i < chapters.length; i++) {
            const chapterRow = $(chapters[i]).children(".chapter-row");
            const id = Number(chapterRow.data("chapter-id"));
            const newChapter = {
                id: id,
                parentChapterId: parentId,
                position: this.position + 1,
                name: chapterRow.find(".chapter-name").text().trim(),
                beginningPageId: Number(chapterRow.find("option:selected").val()),
                comment: ""
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
            if (newName !== "" && String(nameElement.text()) !== newName) {
                nameElement.text(newName);
                this.showUnsavedChangesAlert();
            }
            
            const newPageName = `[${pageInput.find("option:selected").text()}]`;
            if (newPageName !== "" && String(pageElement.text()) !== newPageName) {
                pageElement.text(newPageName);
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
        
        const content = pageDetail.find(".body-content");
        const textIcon = pageDetail.find(".fa-file-text-o");
        const imageIcon = pageDetail.find(".fa-image");
        const alertHolder = pageDetail.find(".alert-holder");
        

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

        const pagination = this.createPagination(this.bookPages);
        content.empty().html("<div class=\"page-navigation\"></div><div class=\"sub-content\"></div>");
        const paginationEl = content.find(".page-navigation");
        paginationEl.append(pagination);
        this.moveToPage(pageId);
        pageDetail.removeClass("hide");
    }

    private loadPageDetail(pageId: number) {
        const pageDetail = $("#page-detail");
        const textIcon = pageDetail.find(".fa-file-text-o");
        const imageIcon = pageDetail.find(".fa-image");
        const alertHolder = pageDetail.find(".alert-holder");
        alertHolder.empty();

        const content = pageDetail.find(".body-content");
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
        $("#unsavedChanges").removeClass("hide");
    }

    private createChapterRow(name: string, beginningPageId: number, beginningPageName: string, levelOfHierarchy = 0): JQuery<HTMLElement> {
        const newChapter = $("#chapterTemplate").children(".chapter-container").clone();
        newChapter.children(".chapter-row").data("level", levelOfHierarchy);
        newChapter.find(".ridics-checkbox").attr("style", `margin-left: ${levelOfHierarchy}em`);
        newChapter.find(".chapter-name").text(name);
        newChapter.find("input[name=\"chapter-name\"]").val(name);
        newChapter.find(".page-name").text(beginningPageName);
        newChapter.find("input[name=\"page-name\"]").val(beginningPageName);
        newChapter.find(`input[name="page-name"] option[value="${beginningPageId}"]`).attr("selected", "selected");
        
        return newChapter;
    }

    private createPagination(pages: Array<BookPage>): HTMLElement {
    
        var paginationUl: HTMLUListElement = document.createElement("ul");
        paginationUl.classList.add("pagination", "pagination-sm");

        var toLeft = document.createElement("ul");
        toLeft.classList.add("page-navigation-container", "page-navigation-container-left");

        var liElement: HTMLLIElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        var anchor: HTMLAnchorElement = document.createElement("a");
        anchor.href = "#";
        anchor.text = "|<";
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.moveToPageNumber(0);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-left");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.text = "<<";
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.actualPageIndex - 5);
            return false;
        });
        liElement.appendChild(anchor);
        toLeft.appendChild(liElement);

        var toRight = document.createElement("ul");
        toRight.classList.add("page-navigation-container", "page-navigation-container-right");

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.text = ">>";
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
           this.moveToPageNumber(this.actualPageIndex + 5);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("page-navigation page-navigation-right");
        anchor = document.createElement("a");
        anchor.href = "#";
        anchor.text = ">|";
        $(anchor).click((event: JQuery.Event) => {
            event.stopPropagation();
            this.moveToPageNumber(this.bookPages.length - 1);
            return false;
        });
        liElement.appendChild(anchor);
        toRight.appendChild(liElement);

        liElement = document.createElement("li");
        $(liElement).addClass("more-pages more-pages-left");
        liElement.innerHTML = "...";
        paginationUl.appendChild(liElement);

        $.each(pages, (index, page) => {
            liElement = document.createElement("li");
            $(liElement).addClass("page");
            $(liElement).data("page-index", index);
            anchor = document.createElement("a");
            anchor.href = "#";
            anchor.innerHTML = page.text;
            $(anchor).click((event: JQuery.Event) => {
                event.stopPropagation();
                this.moveToPage(page.pageId);
                return false;
            });
            liElement.appendChild(anchor);
            paginationUl.appendChild(liElement);
        });

        liElement = document.createElement("li");
        $(liElement).addClass("more-pages more-pages-right");
        liElement.innerHTML = "...";
        paginationUl.appendChild(liElement);

        var listingContainer = document.createElement("div");
        listingContainer.classList.add("page-navigation-container-helper");
        listingContainer.appendChild(toLeft);
        listingContainer.appendChild(paginationUl);
        listingContainer.appendChild(toRight);

        return listingContainer;
    }

    moveToPageNumber(pageIndex: number) {
        if (pageIndex < 0) {
            pageIndex = 0;
        } else if (pageIndex >= this.bookPages.length) {
            pageIndex = this.bookPages.length - 1;
        }

        this.actualPageIndex = pageIndex;
        this.actualizePagination(pageIndex);
        this.loadPageDetail(this.bookPages[pageIndex].pageId);
    }

    actualizePagination(pageIndex: number) {
        const pager = $("#page-detail").find("ul.pagination");
        pager.find("li.page-navigation").css("visibility", "visible");
        pager.find("li.more-pages").css("visibility", "visible");
        if (pageIndex === 0) {
            pager.find("li.page-navigation-left").css("visibility", "hidden");
            pager.find("li.more-pages-left").css("visibility", "hidden");
        } else if (pageIndex === this.bookPages.length - 1) {
            pager.find("li.page-navigation-right").css("visibility", "hidden");
            pager.find("li.more-pages-right").css("visibility", "hidden");
        }

        const pages = $(pager).find(".page");
        $(pages).css("display", "none");
        $(pages).removeClass("page-active");
        const actualPage = $(pages).filter(function (index) {
            return $(this).data("page-index") === pageIndex;
        });

        const displayPagesOnEachSide = (this.pagerDisplayPages - 1) / 2;
        let displayOnRight = displayPagesOnEachSide;
        let displayOnLeft = displayPagesOnEachSide;
        const pagesOnLeft = pageIndex;
        const pagesOnRight = this.bookPages.length - (pageIndex + 1);
        if (pagesOnLeft <= displayOnLeft) {
            displayOnRight += displayOnLeft - pagesOnLeft;
            pager.find("li.more-pages-left").css("visibility", "hidden");
        } else if (pagesOnRight <= displayOnRight) {
            displayOnLeft += displayOnRight - pagesOnRight;
            pager.find("li.more-pages-right").css("visibility", "hidden");
        }

        const displayedPages = $(pages).filter(function (index) {
            const itemPageIndex = $(this).data("page-index");
            return (itemPageIndex >= pageIndex - displayOnLeft && itemPageIndex <= pageIndex + displayOnRight);
        });
        $(displayedPages).css("display", "inline-block");
        $(actualPage).addClass("page-active");

    }

    moveToPage(pageId: number) {
        let pageIndex = -1;
        for (let i = 0; i < this.bookPages.length; i++) {
            if (this.bookPages[i].pageId === pageId) {
                pageIndex = i;
                break;
            }
        }
        if (pageIndex >= 0 && pageIndex < this.bookPages.length) {
            this.moveToPageNumber(pageIndex);
        }
    }
}