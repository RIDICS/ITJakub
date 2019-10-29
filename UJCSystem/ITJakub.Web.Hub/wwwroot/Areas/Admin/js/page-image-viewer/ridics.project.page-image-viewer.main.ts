class ImageViewerMain {
    init(projectId: number) {
        const util = new EditorsApiClient();
        const gui = new EditorsGui();
        const contentAddition = new ImageViewerContentAddition(util);
        const upload = new ImageViewerUpload(contentAddition);
        const navigation = new ImageViewerPageNavigation(contentAddition, gui);
        const compositionPagesAjax = util.getPagesList(projectId);
        const projectImagesElement = $("#project-resource-images");
        compositionPagesAjax.done((pages: IPage[]) => {
            if(pages.length !== 0) {
                navigation.init(pages);
                upload.init();
            } else {
                const error = new AlertComponentBuilder(AlertType.Error)
                    .addContent(localization.translate("NoPages", "RidicsProject").value);
                projectImagesElement.empty().append(error.buildElement());
            }
        });
        compositionPagesAjax.fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translate("ProjectLoadFailed", "RidicsProject").value);
            projectImagesElement.empty().append(error.buildElement());
        });
    }
}