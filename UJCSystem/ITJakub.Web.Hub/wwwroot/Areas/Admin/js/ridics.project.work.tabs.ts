class ProjectWorkMetadataTab extends ProjectMetadataTabBase {
    private projectId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
    }

    getConfiguration(): IProjectMetadataTabConfiguration {
        return {
            $panel: $("#project-work-metadata"),
            $viewButtonPanel: $("#work-metadata-view-button-panel"),
            $editorButtonPanel: $("#work-metadata-editor-button-panel")
        };
    }

    initTab(): void {
        super.initTab();

        $("#work-metadata-edit-button").click(() => {
            this.enabledEdit();
        });

        $("#work-metadata-cancel-button, #work-metadata-save-button").click(() => {
            this.disableEdit();
        });
    }
}

class ProjectWorkPageListTab extends ProjectModuleTabBase {
    private editDialog: BootstrapDialogWrapper;
    private projectId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
    }

    initTab() {
        this.editDialog = new BootstrapDialogWrapper({
            element: $("#project-pages-dialog"),
            autoClearInputs: false
        });

        $("#project-pages-edit-button").click(() => {
            this.editDialog.show();
        });
    }
}

class ProjectWorkPublicationsTab extends ProjectModuleTabBase {
    private projectId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
    }

    initTab() { }
}

class ProjectWorkCooperationTab extends ProjectModuleTabBase {
    private projectId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
    }

    initTab() { }
}

class ProjectWorkHistoryTab extends ProjectModuleTabBase {
    private projectId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
    }

    initTab() { }
}