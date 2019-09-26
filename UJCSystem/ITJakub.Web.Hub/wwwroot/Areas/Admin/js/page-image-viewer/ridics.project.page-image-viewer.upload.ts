class ImageViewerUpload {
    private dropzone: Dropzone;
    private addImageDropzoneDialog: BootstrapDialogWrapper;
    private readonly apiClient: EditorsApiClient;
    private readonly projectId: number;

    constructor(projectId: number) {
        this.projectId = projectId;
        this.apiClient = new EditorsApiClient();
    }

    init() {
        this.initDialog();
        this.initDropzone();
        this.attachAndProcessUploadButton();
    }

    private initDialog() {
        this.addImageDropzoneDialog = new BootstrapDialogWrapper({
            element: $("#upload-image-dialog"),
            submitCallback: this.submit.bind(this),
            autoClearInputs: true
        });  
    }

    private initDropzone() {
        const self = this;
        const dropzoneOptions = DropzoneHelper.getFullConfiguration({
            //url: `${getBaseUrl()}Admin/Project/UploadResource`,//TODO check whether it's an actual address
            url: `${getBaseUrl()}Admin/ContentEditor/CreateImageResource`,
            error: DropzoneHelper.getErrorFunction(),
            autoProcessQueue: false,
            maxFiles: 1,
            uploadMultiple: false,
            paramName: "File",
            init: function() {
                self.dropzone = this;
            }
        });
        $("#new-image-upload").dropzone(dropzoneOptions);
    }

    private attachAndProcessUploadButton() {
        $("#project-resource-images").on("click", ".upload-new-image-button", () => {
            const imageEl = $(".page-image");
            const pageId = imageEl.data("page-id");
            const imageId = imageEl.data("image-id");
            const resourceVersionId = imageEl.data("version-id");
            $("#new-image-page-id").val(pageId);
            $("#new-image-image-id").val(imageId);
            $("#new-image-resource-version-id").val(resourceVersionId);

            this.dropzone.removeAllFiles(true);

            this.addImageDropzoneDialog.show();
        });
    }

    private submit() {
        this.dropzone.processQueue();

        //this.projectClient.processUploadedResources(this.projectId, sessionId, comment, errorCode => {//TODO check correct way to upload
        //    if (errorCode != null) {
        //        this.addImageDropzoneDialog.showError();
        //        return;
        //    }

        //    this.addImageDropzoneDialog.hide();
        //});
    }
}
