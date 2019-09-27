class ImageViewerMain {
    init(projectId: number) {
        const util = new EditorsApiClient();
        const gui = new EditorsGui();
        const contentAddition = new ImageViewerContentAddition(util);
        const upload = new ImageViewerUpload();
        const navigation = new ImageViewerPageNavigation(contentAddition, gui);
        const compositionPagesAjax = util.getPagesList(projectId);
        compositionPagesAjax.done((pages: IPage[]) => {
            navigation.init(pages);
            upload.init();
        });
        compositionPagesAjax.fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error)
                .addContent(localization.translate("ProjectLoadFailed", "RidicsProject").value);
            $("#project-resource-images").empty().append(error.buildElement());
        });
    }
}