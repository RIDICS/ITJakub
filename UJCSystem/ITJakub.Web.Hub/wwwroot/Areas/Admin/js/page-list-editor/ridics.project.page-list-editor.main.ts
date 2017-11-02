///<reference path="./ridics.project.page-list-editor.util.ts" />
///<reference path="./ridics.project.page-list-editor.generator.ts" />
///<reference path="./ridics.project.page-list-editor.list-structure.ts" />

class PageListEditorMain {
    private editDialog: BootstrapDialogWrapper;


    init(projectId: number) {

        const util = new PageListEditorUtil();
        const listGenerator = new PageListGenerator();
        const listStructure = new PageListStructure();

        this.editDialog = new BootstrapDialogWrapper({
            element: $("#project-pages-dialog"),
            autoClearInputs: false
        });

        $("#project-pages-edit-button").click(() => {
            this.editDialog.show();
            $(".project-pages-dialog").on("click",
                ".generate-page-list",
                () => {
                    const pageListEl = $(".page-list");
                    if (pageListEl.length) {
                        pageListEl.remove();
                    }
                    this.startGeneration(listGenerator, projectId, util, listStructure);
                });
            $(".project-pages-dialog").on("click",
                ".move-page-up",
                () => {
                    const selectedPageEl = $(".page-list").children(".ui-selected");
                    if (selectedPageEl.length) {
                        listStructure.movePageUp(selectedPageEl);
                    }
                });

            $(".project-pages-dialog").on("click",
                ".move-page-down",
                () => {
                    const selectedPageEl = $(".page-list").children(".ui-selected");
                    if (selectedPageEl.length) {
                        listStructure.movePageDown(selectedPageEl);
                    }
                }
            );
        });
    }

    private startGeneration(listGenerator: PageListGenerator, projectId: number, util: PageListEditorUtil, listStructure: PageListStructure) {
        const fromBook = $(".generate-pages-from-book:checked").val() as boolean;
        if (fromBook) {//TODO dropdown select meaning
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