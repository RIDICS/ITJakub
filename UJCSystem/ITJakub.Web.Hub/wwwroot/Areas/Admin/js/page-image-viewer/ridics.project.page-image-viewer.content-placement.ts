﻿class ImageViewerContentAddition {
    private readonly util: EditorsApiClient;

    constructor(util: EditorsApiClient) {
        this.util = util;
    }

    formImageContent(pageId: number) {
        const imgUrl = this.util.getImageUrlOnPage(pageId);
        const pageImageEl = $(".page-image");
        const imageString = `<img src="${imgUrl}">`;
        pageImageEl.fadeOut(150, () => {
            pageImageEl.empty(); 
            pageImageEl.append(imageString);
            this.onError(pageImageEl);
        });
        pageImageEl.fadeIn(150);
        pageImageEl.off();
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