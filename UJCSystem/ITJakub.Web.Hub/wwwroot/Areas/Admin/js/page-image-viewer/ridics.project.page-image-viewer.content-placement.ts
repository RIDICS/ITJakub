class ImageViewerContentAddition {
    private readonly apiClient: EditorsApiClient;
    private readonly errorHandler: ErrorHandler;

    constructor(apiClient: EditorsApiClient) {
        this.apiClient = apiClient;
        this.errorHandler = new ErrorHandler();
    }

    formImageContent(pageId: number) {
        const pageImageEl = $(".page-image");
        pageImageEl.data("page-id", pageId);

        this.apiClient.getImageResourceByPageId(pageId).done(result => {
            pageImageEl.data("image-id", result.id)
                .data("version-id", result.versionId);
            this.addImageContent(pageImageEl, result.imageUrl);
        }).fail((response) => {
            const errorMessage = response.status === 404
                ? localization.translate("NoImageOnPage", "RidicsProject").value
                : this.errorHandler.getErrorMessage(response);
            const alert = new AlertComponentBuilder(AlertType.Error).addContent(errorMessage);
            pageImageEl.empty().append(alert.buildElement());
        });
    }

    addImageContent(element: JQuery, imageUrl: string) {
        const imageString = `<img src="${imageUrl}">`;
        element.fadeOut(150, () => {
            element.empty();
            element.append(imageString);
            this.attachOnErrorEvent(element);
        });
        element.fadeIn(150);
        element.off();
    }

    private attachOnErrorEvent(pageImageEl) {
        const imageEl = pageImageEl.children("img");
        imageEl.on("error", () => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("NoImageOnPage","RidicsProject").value);
            pageImageEl.empty().append(error.buildElement());
        });
    }
}