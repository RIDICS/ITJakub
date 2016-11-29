$(document).ready(() => {
    var newProjectDialog = new BootstrapDialogWrapperCopy($("#new-project-dialog"), true);
    // TODO create universal BootstrapDialogWrapper/FavoriteManagementDialog

    $("#new-project-button").click(() => {
        newProjectDialog.show();
    });
});

class BootstrapDialogWrapperCopy {
    private clearInputElements: boolean;
    private element: JQuery;

    constructor(dialogElement: JQuery, clearInputElements: boolean) {
        this.clearInputElements = clearInputElements;
        this.element = dialogElement;

        this.element.on("hidden.bs.modal", () => {
            this.clear();
        });
    }

    public show() {
        this.element.modal({
            show: true,
            backdrop: "static"
        });
    }

    public hide() {
        this.element.modal("hide");
    }

    private clear() {
        if (this.clearInputElements) {
            $("input", this.element).val("");
            $("textarea", this.element).val("");
            $("select", this.element).val("");
        }
    }
}