class ImageViewerContentAddition {
    private readonly apiClient: EditorsApiClient;

    constructor(apiClient: EditorsApiClient) {
        this.apiClient = apiClient;
    }

    formImageContent(pageId: number) {
        const pageImageEl = $(".page-image");
        pageImageEl.data("page-id", pageId);

        this.apiClient.getImageResourceByPageId(pageId).done(result => {
            pageImageEl.data("image-id", result.id)
                .data("version-id", result.versionId);
            this.addImageContent(pageImageEl, result.imageUrl);
        }).fail(() => {
            //TODO show correct error
        });
        //const imgUrl = this.apiClient.getImageUrlOnPage(pageId);
    }

    addImageContent(element: JQuery, imageUrl: string) {
        const imageString = `<img src="${imageUrl}">`;
        element.fadeOut(150, () => {
            element.empty();
            element.append(imageString);
            this.onError(element);
        });
        element.fadeIn(150);
        element.off();
    }

    private onError(pageImageEl) {
        const imageEl = pageImageEl.children("img");
        imageEl.on("error", () => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("NoImageOnPage","RidicsProject").value);
            pageImageEl.empty().append(error.buildElement());
        });
    }
}