///<reference path="./ridics.project.page-list-editor.util.ts" />
///<reference path="./ridics.project.page-list-editor.generator.ts" />
///<reference path="./ridics.project.page-list-editor.list-structure.ts" />
///<reference path="./ridics.project.page-list-editor.editor.ts" />

class PageListEditorMain {
    private editDialog: BootstrapDialogWrapper;


    init(projectId: number) {

        const util = new PageListEditorUtil();
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
                    editor.convertDivListToTextarea($(".page-list")); //TODO check it exists
                }
            );
            $(".page-list-editor-content").on("click",
                ".generate-page-list",
                () => {
                    const pageListEl = $(".page-list");
                    if (pageListEl.length) {
                        pageListEl.remove();
                    }
                    this.startGeneration(listGenerator, projectId, util, listStructure);
                });
            $(".page-list-editor-content").on("click",
                ".move-page-up",
                () => {
                    const selectedPageEl = $(".page-list").children(".ui-selected");
                    if (selectedPageEl.length) {
                        listStructure.movePageUp(selectedPageEl);
                    }
                });

            $(".page-list-editor-content").on("click",
                ".move-page-down",
                () => {
                    const selectedPageEl = $(".page-list").children(".ui-selected");
                    if (selectedPageEl.length) {
                        listStructure.movePageDown(selectedPageEl);
                    }
                }
            );

            $(".page-list-editor-content").on("click",
                ".save-page-list",
                () => {
                    const textAreaEl = $(".page-list-edit-textarea");
                    if (textAreaEl.length) {
                        const pageListString = textAreaEl.val();
                        const pageLingStringArray = pageListString.split("\n");
                        console.log(pageLingStringArray);
//TODO add logic
                    }
                }
            );

            $(".page-list-editor-content").on("click",
                ".cancel-page-list",
                (event) => {
                    event.stopPropagation();
                    const textAreaEl = $(".page-list-edit-textarea");
                    console.log(textAreaEl);
                    if (textAreaEl.length) {
                        textAreaEl.remove();
                    }
                    this.editDialog.hide();
                    $(".page-list-editor-content").off();
                }
            );
        });
    }

    private startGeneration(listGenerator: PageListGenerator,
        projectId: number,
        util: PageListEditorUtil,
        listStructure: PageListStructure) {
        const fromBook = $(".generate-pages-from-book:checked").val() as boolean;
        if (fromBook) { //TODO dropdown select meaning
            const pageListAjax = util.getPagesList(projectId);
            pageListAjax.done((data: IParentPage[]) => {
                const pageList: string[] = [];
                for (let i = 0; i < data.length; i++) {
                    pageList.push(data[i].name);
                }
                this.populateList(pageList, listStructure);
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
                const fc = $(".book-cover-checkbox:checked").val() as boolean;
                const fs = $(".book-startpage-checkbox:checked").val() as boolean;
                const formatString = $("#project-pages-format").find(":selected").data("format-value") as string;
                const format = PageListFormat[formatString] as number;
                if (!isNaN(from) && !isNaN(to)) {
                    if (to > from) {
                        const pageList = listGenerator.generatePageList(from, to, format, fc, fs);
                        this.populateList(pageList, listStructure);
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