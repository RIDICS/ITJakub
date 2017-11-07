class ImageViewerContentAddition {
    private readonly util: EditorsUtil;

    constructor(util: EditorsUtil) {
        this.util = util;
    }

    formImageContent(pageId: number) {
        const imageAjax = this.util.getImageOnPage(pageId);
        imageAjax.done(() => {
            //TODO add logic
        });
        imageAjax.fail(() => {
            //TODO add logic
        });
    }
}