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
            var $directSubcontainer = $(elem as Node as Element).children(".discussion-thread");

            if ($directSubcontainer.length === 0) {
                $(".discussion-open-thread-link", elem as Node as Element).hide();
            } else {
                $directSubcontainer.hide();

                $(".discussion-open-thread-link", elem as Node as Element).click((event) => {
                    var $openIcon = $(".icon-open", event.currentTarget as Node as Element);
                    var $closeIcon = $(".icon-close", event.currentTarget as Node as Element);

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