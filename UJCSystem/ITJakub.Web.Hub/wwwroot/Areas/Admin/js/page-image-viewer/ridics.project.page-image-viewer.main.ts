class ImageViewerMain {
    private readonly client: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;
    private navigation: ImageViewerPageNavigation;
    private upload: ImageViewerUpload;
    private projectId: number;

    constructor() {
        this.client = new EditorsApiClient();
        this.errorHandler = new ErrorHandler();
    }

    init(projectId: number) {
        this.projectId = projectId;
        const gui = new EditorsGui();
        const contentAddition = new ImageViewerContentAddition(this.client);
        
        this.navigation = new ImageViewerPageNavigation(contentAddition, gui);
        this.navigation.init();
        
        this.upload = new ImageViewerUpload(contentAddition);        
        this.upload.init();
        
        this.initAddPageDialog();
    }

    private loadPages() {
        const compositionPagesAjax = this.client.getImagesPageListView(this.projectId);
        const projectImagesElement = $("#project-resource-images");
        compositionPagesAjax.done((data) => {
            $(".pages .page-listing").html(data);
            this.navigation.reinit();
        });
        compositionPagesAjax.fail(() => {
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
            
            /* const listContainerEl = $(".page-listing tbody");
            if (!listContainerEl.children().length) {
                $("#noPagesAlert").remove();
            }*/

            const newPageName = String(dialog.find("input[name=\"page-name\"]").val());
            if (newPageName !== "") {
                this.client.createPage(this.projectId, newPageName, position).done(() => {
                    dialog.modal("hide");
                    this.loadPages();
                }).fail((error) => {
                    const alert = new AlertComponentBuilder(AlertType.Error).addContent(this.errorHandler.getErrorMessage(error));
                    alertHolder.empty().append(alert.buildElement());
                });


                /* const html = this.createPageRow(newPageName);
                 listContainerEl.append(html);
 
                 this.showUnsavedChangesAlert();
                 this.initPageRowClicks();*/
            }
        });
    }
}