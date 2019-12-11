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
        
        const loader = lv.create(null, "lv-circles sm lv-mid lvt-5");
        $(pageImageEl).html(loader.getElement());
        
        this.apiClient.getImageResourceByPageId(pageId).done(result => {
            pageImageEl.data("image-id", result.id)
                .data("version-id", result.versionId);
            this.addImageContent(pageImageEl, result.imageUrl);
        }).fail((response) => {
            const errorMessage = response.status === 404
                ? localization.translate("NoImageOnPage", "RidicsProject").value
                : this.errorHandler.getErrorMessage(response);
            const alert = new AlertComponentBuilder(AlertType.Error).addContent(errorMessage);
            pageImageEl.data("image-id", null)
                .data("version-id", null)
                .empty()
                .append(alert.buildElement());
        });
    }

    addImageContent(element: JQuery, imageUrl: string) {
        const imageString = `<img src="${imageUrl}">`;
        
        element.fadeOut(300, () => {
            element.html(imageString);
            this.attachOnErrorEvent(element);
            
            const zoom = 0.1;
            wheelzoom($(".page-image").children("img"), {zoom: zoom});

            let setMinWidth = true;
            $('.page-image img').on('wheel', (e) =>
            {
                const image = $('.page-image img');
                if(setMinWidth)
                {
                    image.css("min-width", image.width());
                    setMinWidth = false;
                }
                
                const width = image.width();
                if((<WheelEvent>e.originalEvent).deltaY < 0)
                {
                    image.css("width", width + width * zoom / 3);    
                } else {
                    image.css("width", width - width * zoom / 3);
                }
            });
        });
        element.fadeIn(300);
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