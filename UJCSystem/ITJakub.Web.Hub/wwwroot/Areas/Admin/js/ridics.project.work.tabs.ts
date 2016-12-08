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

    initTab() {
        var $table = $("#work-snapshots-table");
        $(".duplicate-column", $table).hide();

        $("#work-snapshots-new-button").click((event) => {
            $(".edit-column, .remove-column").hide();
            $(".duplicate-column", $table).show();
            $(event.currentTarget).hide();

            this.openNewSnapshotPanel();
        });
    }

    private openNewSnapshotPanel() {
        var url = getBaseUrl() + "Admin/Project/NewSnapshot?projectId=" + this.projectId;

        $("#new-snapshot-container").append("<div class=\"loader\"></div>").load(url, null, (responseText, textStatus, xmlHttpRequest) => {
            if (xmlHttpRequest.status !== HttpStatusCode.Success) {
                var errorElement = new AlertComponentBuilder(AlertType.Error)
                    .addContent("Chyba při načítání zdrojů k publikaci")
                    .buildElement();
                $("#new-snapshot-container").empty().append(errorElement);
                return;
            }

            this.initNewSnapshotPanel();
        });
    }

    private initNewSnapshotPanel() {
        var textResources = new ProjectWorkPublicationsResource($(".project-dropdown-panel").first());
        textResources.init();
    }
}

class ProjectWorkPublicationsResource {
    private $container: JQuery;

    constructor(panelElement: JQuery) {
        this.$container = panelElement;
    }

    public init() {
        $(".subheader", this.$container).children().each((index, elem) => {
            var $checkbox = $("input[type=checkbox]", elem);
            if ($checkbox.length === 0) return;

            $checkbox.change((event) => {
                var checkbox = <HTMLInputElement>event.currentTarget;
                var isChecked = checkbox.checked;

               // $("td:nth-child()")
            });
        });
    }
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