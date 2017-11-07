///<reference path="./ridics.project.page-image-viewer.navigation.ts" />
///<reference path="./ridics.project.page-image-viewer.content-placement.ts" />
///<reference path="../editors-common-base/ridics.project.editors.util.ts" />

class ImageViewerMain {
    init(projectId: number) {
        const util = new EditorsUtil();
        const contentAddition = new ImageViewerContentAddition(util);
        const navigation = new ImageViewerPageNavigation(contentAddition);
        const compositionPagesAjax = util.getPagesList(projectId);
        compositionPagesAjax.done((pages: IParentPage[]) => {
            navigation.init(pages);
            const dialog = this.addDropZoneDialog();
            this.attachAndProcessUploadButton(dialog);
        });
        compositionPagesAjax.fail(() => {
            $("#project-resource-images").text("Failed to load project info.");//TODO add styled div
        });
    }

    private addDropZoneDialog():BootstrapDialogWrapper {
        const addResourceDialog = new BootstrapDialogWrapper({
            element: $("#upload-image-dialog"),
            //submitCallback: this.addResource.bind(this),
            autoClearInputs: true
        });
        const dropzoneOptions = DropzoneHelper.getFullConfiguration({
            url: `${getBaseUrl()}Admin/Project/UploadResource`,//TODO check whether it's an actual address
            error: DropzoneHelper.getErrorFunction()
        });
        $("#new-image-upload").dropzone(dropzoneOptions);
        return addResourceDialog;
    }

    private attachAndProcessUploadButton(dialog: BootstrapDialogWrapper) {
        $("#project-resource-images").on("click", ".upload-new-image-button", () => {
            dialog.show();
        });
    }
}