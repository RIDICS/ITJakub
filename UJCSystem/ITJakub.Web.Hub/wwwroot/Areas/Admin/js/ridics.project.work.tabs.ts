class ProjectWorkMetadataTab extends ProjectMetadataTabBase {
    private projectId: number;
    private addPublisherDialog: BootstrapDialogWrapper;
    private addLiteraryKindDialog: BootstrapDialogWrapper;
    private addLiteraryGenreDialog: BootstrapDialogWrapper;
    private addAuthorDialog: BootstrapDialogWrapper;
    private addEditorDialog: BootstrapDialogWrapper;
    private projectManager: ProjectManager;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
        this.projectManager = new ProjectManager();

        this.addPublisherDialog = new BootstrapDialogWrapper({
            element: $("#add-publisher-dialog"),
            autoClearInputs: true,
            submitCallback: this.createNewPublisher.bind(this)
        });

        this.addLiteraryKindDialog = new BootstrapDialogWrapper({
            element: $("#add-literary-kind-dialog"),
            autoClearInputs: true,
            submitCallback: this.createNewLiteraryKind.bind(this)
        });

        this.addLiteraryGenreDialog = new BootstrapDialogWrapper({
            element: $("#add-literary-genre-dialog"),
            autoClearInputs: true,
            submitCallback: this.createNewLiteraryGenre.bind(this)
        });

        this.addAuthorDialog = new BootstrapDialogWrapper({
            element: $("#add-author-dialog"),
            autoClearInputs: true,
            submitCallback: this.addAuthor.bind(this)
        });

        this.addEditorDialog = new BootstrapDialogWrapper({
            element: $("#add-editor-dialog"),
            autoClearInputs: true,
            submitCallback: this.addEditor.bind(this)
        });
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

        $("#add-publisher-button").click(() => {
            this.addPublisherDialog.show();
        });

        $("#add-literary-kind-button").click(() => {
            this.addLiteraryKindDialog.show();
        });

        $("#add-literary-genre-button").click(() => {
            this.addLiteraryGenreDialog.show();
        });

        $("#add-author-button").click(() => {
            this.addAuthorDialog.show();
        });

        $("#add-editor-button").click(() => {
            this.addEditorDialog.show();
        });
    }

    private createNewPublisher() {
        var name = $("#add-publisher-name").val();
        var email = $("#add-publisher-email").val();

        if (!name) {
            this.addPublisherDialog.showError("Nebyl vyplněn název nakladatele");
        }

        this.projectManager.createPublisher(name, email, (newPublisherId, errorCode) => {
            if (errorCode !== null) {
                this.addPublisherDialog.showError("Chyba při vytváření nového nakladatele");
                return;
            }

            UiHelper.addSelectOptionAndSetDefault($("#work-metadata-publisher"), name, newPublisherId);
            this.addPublisherDialog.hide();
        });
    }

    private createNewLiteraryKind() {
        var name = $("#add-literary-kind-name").val();

        if (!name) {
            this.addLiteraryKindDialog.showError("Nebyl vyplněn název");
        }

        this.projectManager.createLiteraryKind(name, (newId, errorCode) => {
            if (errorCode !== null) {
                this.addLiteraryKindDialog.showError("Chyba při vytváření nového literárního druhu");
                return;
            }

            UiHelper.addCheckboxAndSetChecked($("#work-metadata-literary-kind"), name, newId);
            this.addLiteraryKindDialog.hide();
        });
    }

    private createNewLiteraryGenre() {
        var name = $("#add-literary-genre-name").val();

        if (!name) {
            this.addLiteraryGenreDialog.showError("Nebyl vyplněn název");
        }

        this.projectManager.createLiteraryGenre(name, (newId, errorCode) => {
            if (errorCode !== null) {
                this.addLiteraryGenreDialog.showError("Chyba při vytváření nového literárního žánru");
                return;
            }

            UiHelper.addCheckboxAndSetChecked($("#work-metadata-literary-genre"), name, newId);
            this.addLiteraryGenreDialog.hide();
        });
    }

    private addAuthor() {
        throw Error("Not implemented");
    }

    private addEditor() {
        throw Error("Not implemented");
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

                $(`td:nth-child(${index+1}) input[type=checkbox]`, this.$container).each((index, elem) => {
                    var checkbox2 = <HTMLInputElement>elem;
                    checkbox2.checked = isChecked;
                });
            });
        });

        $("td input[type=checkbox]", this.$container).change((event) => {
            var $parentTd = $(event.currentTarget).closest("td");
            var $parentTr = $parentTd.closest("tr");
            var position = $parentTr.children().index($parentTd) + 1;

            var isAllChecked = true;
            $(`td:nth-child(${position}) input[type=checkbox]`, this.$container).each((index, elem) => {
                if (!(<HTMLInputElement>elem).checked) {
                    isAllChecked = false;
                }
            });

            var allCheckBox = <HTMLInputElement>$(`.subheader th:nth-child(${position}) input[type=checkbox]`, this.$container).get(0);
            allCheckBox.checked = isAllChecked;
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