class PageListEditorMain {
    private editDialog: BootstrapDialogWrapper;
    private readonly gui: EditorsGui;
    private readonly errorHandler: ErrorHandler;

    constructor() {
        this.gui = new EditorsGui();
        this.errorHandler = new ErrorHandler();
    }
    
    init(projectId: number) {

        const util = new EditorsUtil();
        const listGenerator = new PageListGenerator();
        const listStructure = new PageListStructure();
        const editor = new PageListEditorTextEditor();

        this.editDialog = new BootstrapDialogWrapper({
            element: $("#project-pages-dialog"),
            autoClearInputs: false
        });

        $(".panel-bottom-buttons .move-page-down").click(() => {
            this.moveList(true, $(".pages"));
        });

        $(".panel-bottom-buttons .move-page-up").click(() => {
            this.moveList(false, $(".pages"));
        });

        $(".page-row").click((event) => {
            const pageId = $(event.currentTarget).data("page-id");
            const pageDetail = $("#page-detail");
            const content = pageDetail.find("#bodyContent");
            const alertHolder = pageDetail.find(".alert-holder");

            alertHolder.empty();
            content.html("<div class=\"loader\"></div>");
            pageDetail.removeClass("hide");

            util.getPageDetail(pageId).done((response) => {
                content.html(response);
            }).fail((error) => {
                const alert = new AlertComponentBuilder(AlertType.Error)
                    .addContent(this.errorHandler.getErrorMessage(error)).buildElement();
                alertHolder.append(alert);
            });
        });

        $("#project-pages-edit-button").click(() => {
            this.editDialog.show();
            this.loadExistingPages(projectId, util, listStructure);
            $(".page-list-editor-content").on("click",
                ".edit-page-list",
                () => {
                    const pageListEl = $(".page-list");
                    if (pageListEl.length) {
                        editor.convertDivListToTextarea(pageListEl);
                    }
                }
            );
            $(".page-list-editor-content").on("click",
                ".generate-page-list",
                () => {
                    const pageListTextareaEl = $(".page-list-edit-textarea");
                    if (pageListTextareaEl.length) {
                        pageListTextareaEl.remove();
                    }
                    const doublePageRadiobuttonEl = $(".doublepage-radiobutton");
                    const doublePageGeneration = doublePageRadiobuttonEl.prop("checked") as boolean;
                    this.startGeneration(listGenerator, projectId, util, listStructure, doublePageGeneration);
                });
            $(".page-list-editor-content").on("click",
                ".move-page-up",
                () => {
                    const selectedPageEl = $(".page-list").children(".ui-selected");
                    if (selectedPageEl.length) {
                        selectedPageEl.each((index, element) => {
                            const selectedItemJEl = $(element as Node as HTMLElement);
                            listStructure.movePageUp(selectedItemJEl);
                        });
                    }
                });

            $(".page-list-editor-content").on("click",
                ".move-page-down",
                () => {
                    const selectedPageEl = $(".page-list").children(".ui-selected");
                    if (selectedPageEl.length) {
                        $(selectedPageEl.get().reverse()).each((index, element) => {
                            const selectedItemJEl = $(element as Node as HTMLElement);
                            listStructure.movePageDown(selectedItemJEl);
                        });
                    }
                }
            );

            $(".page-list-editor-content").on("click",
                ".save-page-list",
                () => {
                    const textAreaEl = $(".page-list-edit-textarea");
                    const pageListDivEl = $(".page-list");
                    if (textAreaEl.length) {
                        const pageListString = textAreaEl.val() as string;
                        const pageListStringArray = pageListString.split("\n");
                        //util.savePageList(pageLingStringArray); TODO use after server functions are done
                    }
                    if (pageListDivEl.length) {
                        const pageItemsEl = pageListDivEl.children(".page-list-item");
                        var pageListStringArray: string[] = [];
                        pageItemsEl.each((index, element) => {
                            const pageEl = $(element as Node as Element);
                            pageListStringArray.push(pageEl.text());
                        });
                        //util.savePageList(pageLingStringArray); TODO use after server functions are done
                    }
                }
            );

            $(".page-list-editor-content").on("click",
                ".cancel-page-list",
                (event) => {
                    event.stopPropagation();
                    const textAreaEl = $(".page-list-edit-textarea");
                    if (textAreaEl.length) {
                        textAreaEl.remove();
                    }
                    this.editDialog.hide();
                    $(".page-list-editor-content").off();
                }
            );
        });
    }

    private enableCheckboxes() {
        const fcCheckbox = $(".book-cover-checkbox");
        const fsCheckbox = $(".book-startpage-checkbox");
        fsCheckbox.prop("disabled", false);
        fcCheckbox.prop("disabled", false);
    }

    private checkmarkFC() {
        const fcCheckbox = $(".book-cover-checkbox");
        fcCheckbox.prop("checked", true);
    }

    private checkmarkFS() {
        const fsCheckbox = $(".book-startpage-checkbox");
        fsCheckbox.prop("checked", true);
    }

    private removeFSPage() {
        const pageListEl = $(".page-list");
        if (pageListEl.length) {
            const listItems = pageListEl.children(".page-list-item");
            listItems.each((index, element) => {
                const listItemEl = $(element as Node as Element);
                if (listItemEl.text().toLocaleLowerCase() === "fs") {
                    listItemEl.remove();
                }
            });
        }
    }

    private removeFCPage() {
        const pageListEl = $(".page-list");
        if (pageListEl.length) {
            const listItems = pageListEl.children(".page-list-item");
            listItems.each((index, element) => {
                const listItemEl = $(element as Node as Element);
                if (listItemEl.text().toLocaleLowerCase() === "fc") {
                    listItemEl.remove();
                }
            });
        }
    }

    private addFSPage() {
        const pageListEl = $(".page-list");
        if (pageListEl.length) {
            const listItem = pageListEl.children(".page-list-item").first();
            const fcPageListItem = listItem.clone().text("FS");
            if (listItem.text().toLocaleLowerCase() === "fc") {
                listItem.after(fcPageListItem);
            } else {
                listItem.before(fcPageListItem);
            }
        }
    }

    private addFCPage() {
        const pageListEl = $(".page-list");
        if (pageListEl.length) {
            const listItem = pageListEl.children(".page-list-item").first();
            listItem.before(listItem.clone().text("FC"));
        }
    }

    private trackSpecialPagesCheckboxesState() {
        const specialPagesControlsEl = $(".special-pages-controls");
        const startpageCheckboxEl = $(".book-startpage-checkbox");
        const bookcoverCheckboxEl = $(".book-cover-checkbox");
        specialPagesControlsEl.off();
        var fsCheckboxPrevState = startpageCheckboxEl.prop("checked") as boolean;
        var fcCheckboxPrevState = bookcoverCheckboxEl.prop("checked") as boolean;
        specialPagesControlsEl.on("click",
            ".book-cover-checkbox",
            () => {
                const fcCheckboxCurrentState = bookcoverCheckboxEl.prop("checked") as boolean;
                if (fcCheckboxPrevState && !fcCheckboxCurrentState) {
                    this.removeFCPage();
                }
                if (!fcCheckboxPrevState && fcCheckboxCurrentState) {
                    this.addFCPage();
                }
                fcCheckboxPrevState = fcCheckboxCurrentState;
            });
        specialPagesControlsEl.on("click",
            ".book-startpage-checkbox",
            () => {
                const fsCheckboxCurrentState = startpageCheckboxEl.prop("checked") as boolean;
                if (fsCheckboxPrevState && !fsCheckboxCurrentState) {
                    this.removeFSPage();
                }
                if (!fsCheckboxPrevState && fsCheckboxCurrentState) {
                    this.addFSPage();
                }
                fsCheckboxPrevState = fsCheckboxCurrentState;
            });
    }

    private loadExistingPages(projectId: number,
        util: EditorsUtil,
        listStructure: PageListStructure) {
        const pageListAjax = util.getPagesList(projectId);
        pageListAjax.done((data: IPage[]) => {
            const pageList: string[] = [];
            this.enableCheckboxes();
            for (let i = 0; i < data.length; i++) {
                const pageName = data[i].name;
                if (pageName.toLocaleLowerCase() === "fc") {
                    this.checkmarkFC();
                }
                if (pageName.toLocaleLowerCase() === "fs") {
                    this.checkmarkFS();
                }
                pageList.push(pageName);
            }
            this.populateList(pageList, listStructure);
            this.trackSpecialPagesCheckboxesState();
        });
        pageListAjax.fail(() => {
            this.gui.showInfoDialog("Error", "Load failure due to server error");
        });
    }

    private startGeneration(listGenerator: PageListGenerator,
        projectId: number,
        util: EditorsUtil,
        listStructure: PageListStructure, doublePage:boolean) {
            const fromFieldValue = $("#project-pages-generate-from").val() as string;
            const toFieldValue = $("#project-pages-generate-to").val() as string;
            if (/\d+/.test(fromFieldValue) && /\d+/.test(toFieldValue)) {
                const from = parseInt(fromFieldValue);
                const to = parseInt(toFieldValue);
                const formatString = $("#project-pages-format").find(":selected").data("format-value") as string;
                const format = PageListFormat[formatString] as number;
                if (!isNaN(from) && !isNaN(to)) {
                    if (to > from) {
                        const pageList = listGenerator.generatePageList(from, to, format, doublePage);
                        this.populateList(pageList, listStructure);
                        this.enableCheckboxes();
                        this.trackSpecialPagesCheckboxesState();
                    } else {
                        this.gui.showInfoDialog("Warning", "Please swap to and from numbers.");
                    }
                }
            } else {
                this.gui.showInfoDialog("Warning", "Please enter a number.");
            }
    }

    private populateList(pageList: string[], listStructure: PageListStructure) {
        const listContainerEl = $(".page-list-container");
        const listEl = listContainerEl.children(".page-list");
        if (!listEl.length) {
            listStructure.createList(pageList, listContainerEl);
        } else {
            this.gui.showInfoDialog("Info", "Page names already exist for this project. Appending generated names to the end of the list.");
            listStructure.appendList(pageList, listEl);
        }
    }

    private moveList(down: boolean, context: JQuery) {
        const distance = Number(context.find(".page-move-distance").val());
        const pages = context.find(".page-row").toArray();
        const newPages = new Array<HTMLElement>(pages.length);
        
        for (let i = 0; i < pages.length; i++) {
            const page = pages[i];
            const isSelected = $(page).find(".selection-checkbox").is(":checked");
            if (isSelected) {
                let newPosition = i;
                if (down) {
                    newPosition += distance;
                } else {
                    newPosition -= distance;
                }

                if (newPosition >= pages.length || newPosition < 0) {
                    newPosition = newPosition % pages.length;
                }
                if (newPosition < 0) {
                    newPosition = pages.length + newPosition;
                }

                $(page).data("position", newPosition+1);
                newPages[newPosition] = page;
            }
        }

        let j = 0;
        for (const page of pages) {
            const isSelected = $(page).find(".selection-checkbox").is(":checked");
            if (!isSelected) {
                while (true) {
                    if (typeof newPages[j] == "undefined") {
                        $(page).data("position", j+1);
                        newPages[j] = page;
                        break;
                    } else {
                        j++;
                    }
                }
            }
        }

        const listing = context.find(".page-listing tbody");
        listing.empty();
        for (const page of newPages) {
            listing.append(page);
        }
    }
}