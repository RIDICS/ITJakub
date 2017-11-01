///<reference path="./ridics.project.page-list-editor.util.ts" />

class PageListEditorMain {
    private editDialog: BootstrapDialogWrapper;

    init(projectId: number) {
        const util = new ListEditorUtil();

        this.editDialog = new BootstrapDialogWrapper({
            element: $("#project-pages-dialog"),
            autoClearInputs: false
        });

        $("#project-pages-edit-button").click(() => {
            this.editDialog.show();
        });
    }
}