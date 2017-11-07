///<reference path="../editors-common-base/ridics.project.editors.util.ts" />
///<reference path="./ridics.project.page-list-editor.generator.ts" />
///<reference path="./ridics.project.page-list-editor.list-structure.ts" />
///<reference path="./ridics.project.page-list-editor.editor.ts" />

class PageListEditorMain {
    private editDialog: BootstrapDialogWrapper;


    init(projectId: number) {

        const util = new EditorsUtil();
        const listGenerator = new PageListGenerator();
        const listStructure = new PageListStructure();
        const editor = new PageListEditorTextEditor();

        this.editDialog = new BootstrapDialogWrapper({
            element: $("#project-pages-dialog"),
            autoClearInputs: false
        });

        $("#project-pages-edit-button").click(() => {
            this.editDialog.show();
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
                    const pageListEl = $(".page-list");
                    if (pageListEl.length) {
                        pageListEl.remove();
                    }
                    const pageListTextareaEl = $(".page-list-edit-textarea");
                    if (pageListTextareaEl.length) {
                        pageListTextareaEl.remove();
                    }
                    this.startGeneration(listGenerator, projectId, util, listStructure);
                });
            $(".page-list-editor-content").on("click",
                ".move-page-up",
                () => {
                    const selectedPageEl = $(".page-list").children(".ui-selected");
                    if (selectedPageEl.length) {
                        selectedPageEl.each((index, element) => {
                            const selectedItemJEl = $(element);
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
                            const selectedItemJEl = $(element);
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
                        const pageListString = textAreaEl.val();
                        const pageListStringArray = pageListString.split("\n");
                        //util.savePageList(pageLingStringArray); TODO use after server functions are done
                    }
                    if (pageListDivEl.length) {
                        const pageItemsEl = pageListDivEl.children(".page-list-item");
                        var pageListStringArray: string[] = [];
                        pageItemsEl.each((index, element) => {
                            const pageEl = $(element);
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
                const listItemEl = $(element);
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
                const listItemEl = $(element);
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
        $(".special-pages-controls").off();
        var fsCheckboxPrevState = $(".book-startpage-checkbox").prop("checked") as boolean;
        var fcCheckboxPrevState = $(".book-cover-checkbox").prop("checked") as boolean;
        $(".special-pages-controls").on("click",
            ".book-cover-checkbox",
            () => {
                const fcCheckboxCurrentState = $(".book-cover-checkbox").prop("checked") as boolean;
                if (fcCheckboxPrevState && !fcCheckboxCurrentState) {
                    this.removeFCPage();
                }
                if (!fcCheckboxPrevState && fcCheckboxCurrentState) {
                    this.addFCPage();
                }
                fcCheckboxPrevState = fcCheckboxCurrentState;
            });
        $(".special-pages-controls").on("click",
            ".book-startpage-checkbox",
            () => {
                const fsCheckboxCurrentState = $(".book-startpage-checkbox").prop("checked") as boolean;
                if (fsCheckboxPrevState && !fsCheckboxCurrentState) {
                    this.removeFSPage();
                }
                if (!fsCheckboxPrevState && fsCheckboxCurrentState) {
                    this.addFSPage();
                }
                fsCheckboxPrevState = fsCheckboxCurrentState;
            });
    }

    private startGeneration(listGenerator: PageListGenerator,
        projectId: number,
        util: EditorsUtil,
        listStructure: PageListStructure) {
        const fromBook = $(".generate-pages-from-book").prop("checked") as boolean;
        if (fromBook) { //TODO dropdown select meaning
            const pageListAjax = util.getPagesList(projectId);
            pageListAjax.done((data: IParentPage[]) => {
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
                alert("Load failure due to server error.");
            });
        } else {
            const fromFieldValue = $("#project-pages-generate-from").val();
            const toFieldValue = $("#project-pages-generate-to").val();
            if (/\d+/.test(fromFieldValue) && /\d+/.test(toFieldValue)) {
                const from = parseInt(fromFieldValue);
                const to = parseInt(toFieldValue);
                const formatString = $("#project-pages-format").find(":selected").data("format-value") as string;
                const format = PageListFormat[formatString] as number;
                if (!isNaN(from) && !isNaN(to)) {
                    if (to > from) {
                        const pageList = listGenerator.generatePageList(from, to, format);
                        this.populateList(pageList, listStructure);
                        this.enableCheckboxes();
                        this.trackSpecialPagesCheckboxesState();
                    } else {
                        alert("Please swap to and from numbers.");
                    }
                }
            } else {
                alert("Please enter a number.");
            }
        }
    }

    private populateList(pageList: string[], listStructure: PageListStructure) {
        const listEl = $(".page-list-container");
        listStructure.createList(pageList, listEl);
    }
}