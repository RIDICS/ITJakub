class ProjectResourceMetadataTab extends ProjectMetadataTabBase {
    private resourceId: number;

    constructor(resourceId: number) {
        super();
        this.resourceId = resourceId;
    }

    getConfiguration(): IProjectMetadataTabConfiguration {
        return {
            $panel: $("#project-resource-metadata"),
            $viewButtonPanel: $("#resource-metadata-view-button-panel"),
            $editorButtonPanel: $("#resource-metadata-editor-button-panel")
        };
    }

    initTab(): void {
        super.initTab();

        $("#resource-metadata-edit-button").click(() => {
            this.enabledEdit();
        });

        $("#resource-metadata-cancel-button, #resource-metadata-save-button").click(() => {
            this.disableEdit();
        });
    }
}

class ProjectResourceDiscussionTab extends ProjectModuleTabBase {
    constructor(resourceId: number) {
        super();
    }

    initTab() {
        var $container = $("#resource-discussion-container");
        $(".discussion-thread .discussion-thread .discussion-open-thread-link", $container).hide();
        $(".icon-close", $container).hide();

        $container.children(".discussion-thread").each((index, elem) => {
            var $directSubcontainer = $(elem).children(".discussion-thread");

            if ($directSubcontainer.length === 0) {
                $(".discussion-open-thread-link", elem).hide();
            } else {
                $directSubcontainer.hide();

                $(".discussion-open-thread-link", elem).click((event) => {
                    var $openIcon = $(".icon-open", event.currentTarget);
                    var $closeIcon = $(".icon-close", event.currentTarget);

                    if ($directSubcontainer.is(":visible")) {
                        $directSubcontainer.hide();
                        $openIcon.show();
                        $closeIcon.hide();
                    } else {
                        $directSubcontainer.show();
                        $openIcon.hide();
                        $closeIcon.show();
                    }
                });
            }
        });
    }
}

class ProjectResourcePreviewTab extends ProjectModuleTabBase {
    constructor(resourceId: number) {
        super();
    }

    initTab() {
        const main = new TextEditorMain();
        main.init();
        main.createSlider();
        $(".pages-start").on("scroll resize",() => {main.pageUserOn();});
    }
}