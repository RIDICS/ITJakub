///<reference path="./ridics.project.page-image-viewer.navigation.ts" />
///<reference path="./ridics.project.page-image-viewer.content-placement.ts" />
///<reference path="./ridics.project.page-image-viewer.upload.ts" />
///<reference path="../editors-common-base/ridics.project.editors.util.ts" />

class ImageViewerMain {
    init(projectId: number) {
        const util = new EditorsUtil();
        const gui = new ImageViewerPageGui();
        const contentAddition = new ImageViewerContentAddition(util);
        const upload = new ImageViewerUpload(projectId);
        const navigation = new ImageViewerPageNavigation(contentAddition, gui);
        const compositionPagesAjax = util.getPagesList(projectId);
        compositionPagesAjax.done((pages: IPage[]) => {
            navigation.init(pages);
            upload.init();
        });
        compositionPagesAjax.fail(() => {
            $("#project-resource-images").text("Failed to load project info.");//TODO add styled div
        });
    }
}