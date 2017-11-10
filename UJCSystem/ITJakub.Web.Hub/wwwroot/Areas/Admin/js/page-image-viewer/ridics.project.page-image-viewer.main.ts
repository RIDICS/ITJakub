class ImageViewerMain {
    init(projectId: number) {
        const util = new EditorsUtil();
        const gui = new EditorsGui();
        const contentAddition = new ImageViewerContentAddition(util);
        const upload = new ImageViewerUpload(projectId);
        const navigation = new ImageViewerPageNavigation(contentAddition, gui);
        const compositionPagesAjax = util.getPagesList(projectId);
        compositionPagesAjax.done((pages: IPage[]) => {
            navigation.init(pages);
            upload.init();
        });
        compositionPagesAjax.fail(() => {
            const error = new AlertComponentBuilder(AlertType.Error).addContent("Failed to load project info");
            $("#project-resource-images").empty().append(error.buildElement());
        });
    }
}