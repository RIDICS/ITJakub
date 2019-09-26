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
        
        const dropzoneOptions = DropzoneHelper.getFullConfiguration({
            //url: `${getBaseUrl()}Admin/Project/UploadResource`,//TODO check whether it's an actual address
            url: `${getBaseUrl()}Admin/ContentEditor/CreateImageResource`,
            error: DropzoneHelper.getErrorFunction(),
            autoProcessQueue: false,
            maxFiles: 1,
        });
        this.dropzone = $("#new-image-upload").dropzone(dropzoneOptions);
    }

    private attachAndProcessUploadButton() {
        $("#project-resource-images").on("click", ".upload-new-image-button", () => {
            // TODO set correct input values, like:
            $("#new-image-page-id").val("555"); // TODO this is mock

            this.addImageDropzoneDialog.show();
        });
    }

    private submit() {
        const sessionId = $("#new-image-resource-session-id").val() as string;
        const comment = $("#new-image-resource-comment").val() as string;

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