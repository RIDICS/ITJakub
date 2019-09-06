class ImageViewerContentAddition {
    private readonly util: EditorsUtil;

    constructor(util: EditorsUtil) {
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

    private onError(pageImageEl) {
        const imageEl = pageImageEl.children("img");
        imageEl.on("error", () => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent(localization.translate("NoImageOnPage","RidicsProject").value);
            pageImageEl.empty().append(error.buildElement());
        });
    }
}