class SnapshotList {
    private readonly projectId: number;
    
    constructor(projectId: number) {
        this.projectId = projectId;
    }

    init() {
        const $table = $("#work-snapshots-table");
        $(".duplicate-column", $table).hide();

        $("#work-snapshots-new-button").click((event) => {
            $(".edit-column, .remove-column").hide();
            $(".duplicate-column", $table).show();
            $(event.currentTarget as Node as Element).hide();

            this.openNewSnapshotPanel();
        });
    }

    private openNewSnapshotPanel() {
        const url = getBaseUrl() + "Admin/Project/NewSnapshot?projectId=" + this.projectId;

        $("#new-snapshot-container").append("<div class=\"loader\"></div>").load(url,
            null,
            (responseText, textStatus, xmlHttpRequest) => {
                if (xmlHttpRequest.status !== HttpStatusCode.Success) {
                    var errorElement = new AlertComponentBuilder(AlertType.Error)
                        .addContent(localization.translate("CreateResourcesError", "Admin").value)
                        .buildElement();
                    $("#new-snapshot-container").empty().append(errorElement);
                    return;
                }

                this.initSnapshotEditor();
            });
    }

    private initSnapshotEditor() {
        const snapshotEditor = new SnapshotEditor(this.projectId);
        snapshotEditor.init();
    }
}