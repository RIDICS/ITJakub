///<reference path="./ridics.project.page-image-viewer.navigation.ts" />
///<reference path="../editors-common-base/ridics.project.editors.util.ts" />

class ImageViewerMain {
    init(projectId: number) {
        const util = new EditorsUtil();
        const navigation = new ImageViewerPageNavigation();
        const compositionPagesAjax = util.getPagesList(projectId);
        compositionPagesAjax.done((pages: IParentPage[]) => {
            navigation.init(pages);
        });
        compositionPagesAjax.fail(() => {
            $("#project-resource-images").text("Failed to load project info.");//TODO add styled div
        });
    }
}