﻿class ImageViewerMain {
    private readonly client: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private navigation: PageNavigation;
    private upload: ImageViewerUpload;
    private projectId: number;

    constructor() {
        this.client = new EditorsApiClient();
        this.errorHandler = new ErrorHandler();
    }

    init(projectId: number) {
        this.projectId = projectId;
        this.initAddPageDialog();
        
        const gui = new EditorsGui();
        const contentAddition = new ImageViewerContentAddition(this.client);

        this.navigation = new PageNavigation(gui, (pageId: number) => {
            contentAddition.formImageContent(pageId);    
        });
        
        this.navigation.init();

        this.upload = new ImageViewerUpload(contentAddition);
        this.upload.init();
    }

    private loadPages() {
        const pageListing = $(".pages .page-listing");
        const loader = lv.create(null, "lv-circles sm lv-mid lvt-5");
        $(pageListing).html(loader.getElement());
        
        const compositionPagesAjax = this.client.getImagesPageListView(this.projectId);
        const projectImagesElement = $("#project-resource-images");
        compositionPagesAjax.done((data) => {
            $(".pages .page-listing").html(data);
            this.navigation.reinit();
            const uploadImageBtn = $(".upload-new-image-button");
            if (this.navigation.hasPages()) {
                uploadImageBtn.removeAttr("disabled")
            } else {
                uploadImageBtn.attr("disabled", "true")
            }
            
        });
        compositionPagesAjax.fail(() => {
            pageListing.empty();
            const error = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translate("ProjectLoadFailed", "RidicsProject").value);
            projectImagesElement.empty().append(error.buildElement());
        });
        compositionPagesAjax.always(() => {
            projectImagesElement.removeClass("hide");
        });
    }

    private initAddPageDialog() {
        const dialog = $("#addPageDialog");

        $(".add-page-button").on("click", () => {
            dialog.modal("show");
        });

        $("#addPage").on("click", () => {
            const alertHolder = dialog.find(".alert-holder");
            alertHolder.empty();
            
            const lastPage = $(".page-row:last-of-type");
            let position;
            if (lastPage.length) {
                position = Number(lastPage.data("position"))
            } else {
                position = 1;
            }
            
            const newPageName = String(dialog.find("input[name=\"page-name\"]").val());
            if (newPageName !== "") {
                this.client.createPage(this.projectId, newPageName, position).done(() => {
                    dialog.modal("hide");
                    this.loadPages();
                }).fail((error) => {
                    const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error));
                    alertHolder.empty().append(alert.buildElement());
                });
            }
        });
    }
}