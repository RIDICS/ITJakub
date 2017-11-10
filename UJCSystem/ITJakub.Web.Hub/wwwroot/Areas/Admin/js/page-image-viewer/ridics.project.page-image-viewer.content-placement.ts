class ImageViewerContentAddition {
    private readonly util: EditorsUtil;

    constructor(util: EditorsUtil) {
        this.util = util;
    }

    formImageContent(pageId: number) {
        const imgUrl = this.util.getImageUrlOnPage(pageId);
        const pageImageEl = $(".page-image");
        const imageString = `<img src="${imgUrl}">`;
        pageImageEl.empty();
        pageImageEl.append(imageString);
    }
}