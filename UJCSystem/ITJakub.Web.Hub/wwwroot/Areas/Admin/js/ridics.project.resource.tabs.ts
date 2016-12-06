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
