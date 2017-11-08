class ImageViewerContentAddition {
    private readonly util: EditorsUtil;

    constructor(util: EditorsUtil) {
        this.util = util;
    }

    formImageContent(pageId: number) {
        this.util.getImageOnPage(pageId,
            (response) => {
                var url = window.URL;
                const imgUrl = url.createObjectURL(response);
                const pageImageEl = $(".page-image");
                const imageString = `<img src="${imgUrl}">`;
                pageImageEl.text("");
                pageImageEl.append(imageString);
            },
            () => {
                //TODO
            });
    }
}