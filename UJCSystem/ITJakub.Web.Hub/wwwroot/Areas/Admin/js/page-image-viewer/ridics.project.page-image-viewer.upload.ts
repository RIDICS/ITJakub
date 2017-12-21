interface JQuery {//hack, interface extension form d ts doesn't work, check why
    dropzone(options: Dropzone.DropzoneOptions): Dropzone;
}

class ImageViewerUpload {
    private addImageDropzoneDialog: BootstrapDialogWrapper;
    private readonly projectClient: ProjectClient;
    private readonly projectId: number;

    constructor(projectId: number) {
        this.projectId = projectId;
        this.projectClient = new ProjectClient();
    }

    init() {
        this.initDialog();
        this.initDropzone();
        this.attachAndProcessUploadButton();
    }

    private initDialog() {
        this.addImageDropzoneDialog = new BootstrapDialogWrapper({
            element: $("#upload-image-dialog"),
            submitCallback: this.addResource.bind(this),
            autoClearInputs: true
        });  
    }

    private initDropzone(){
        
        const dropzoneOptions = DropzoneHelper.getFullConfiguration({
            url: `${getBaseUrl()}Admin/Project/UploadResource`,//TODO check whether it's an actual address
            error: DropzoneHelper.getErrorFunction()
        });
        $("#new-image-upload").dropzone(dropzoneOptions);
    }

    private attachAndProcessUploadButton() {
        $("#project-resource-images").on("click", ".upload-new-image-button", () => {
            $("#new-image-resource-session-id").val(Guid.generate());
            this.addImageDropzoneDialog.show();
        });
    }

    private addResource() {
        const sessionId = $("#new-image-resource-session-id").val();
        const comment = $("#new-image-resource-comment").val();
        //this.projectClient.processUploadedResources(this.projectId, sessionId, comment, errorCode => {//TODO check correct way to upload
        //    if (errorCode != null) {
        //        this.addImageDropzoneDialog.showError();
        //        return;
        //    }

        //    this.addImageDropzoneDialog.hide();
        //});
    }
}