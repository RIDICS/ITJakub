class ImageViewerUpload {
    private viewer: ImageViewerContentAddition;
    private errorHandler: ErrorHandler;
    private dropzone: Dropzone;
    private addImageDropzoneDialog: BootstrapDialogWrapper;

    constructor(viewer: ImageViewerContentAddition) {
        this.viewer = viewer;
        this.errorHandler = new ErrorHandler();
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
            //url: null, // Url is not required because it is specified on <form> element
            error: DropzoneHelper.getErrorFunction(),
            autoProcessQueue: false,
            maxFiles: 1,
            uploadMultiple: false,
            paramName: "File",
            init: function() {
                self.dropzone = this;
            },
            params: () => {
                return {
                    comment: $("#new-image-resource-comment").val()
                };
            },
            success: (file, response) => {
                const resultData = response as IImageContract;
                const pageImageEl = $(".page-image");

                pageImageEl.data("image-id", resultData.id)
                    .data("version-id", resultData.versionId);

                this.viewer.addImageContent(pageImageEl, resultData.imageUrl);
                this.addImageDropzoneDialog.hide();
            },
            sending: (file, xhr, formData) => {
                xhr.onreadystatechange = () => {
                    if (xhr.readyState === XMLHttpRequest.DONE && xhr.status !== 200) {
                        const errorMessage = this.errorHandler.getErrorMessageDirect(xhr.responseText, xhr.status);
                        this.addImageDropzoneDialog.showError(errorMessage);
                        this.addImageDropzoneDialog.setSubmitEnabled(false);
                    }
                };
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
    }
}
