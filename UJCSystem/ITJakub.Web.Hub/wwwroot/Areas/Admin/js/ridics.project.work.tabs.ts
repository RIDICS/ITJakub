class ProjectWorkMetadataTab extends ProjectMetadataTabBase {
    private projectId: number;
    private addPublisherDialog: BootstrapDialogWrapper;
    private addLiteraryKindDialog: BootstrapDialogWrapper;
    private addLiteraryGenreDialog: BootstrapDialogWrapper;
    private addAuthorDialog: BootstrapDialogWrapper;
    private addEditorDialog: BootstrapDialogWrapper;
    private projectClient: ProjectClient;
    private authorTypeahead: SingleSetTypeaheadSearchBox<IOriginalAuthor>;
    private editorTypeahead: SingleSetTypeaheadSearchBox<IResponsiblePerson>;
    private selectedAuthorId: number;
    private selectedResponsiblePersonId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
        this.projectClient = new ProjectClient();

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

        this.authorTypeahead = new SingleSetTypeaheadSearchBox<IOriginalAuthor>("#add-author-search", "Admin/Project", x => `${x.lastName} ${x.firstName}`, null);
        this.authorTypeahead.setDataSet("OriginalAuthor");

        this.editorTypeahead = new SingleSetTypeaheadSearchBox<IResponsiblePerson>("#add-editor-search", "Admin/Project", x => `${x.lastName} ${x.firstName}`, null);
        this.editorTypeahead.setDataSet("ResponsiblePerson");
    }

    getConfiguration(): IProjectMetadataTabConfiguration {
        return {
            $panel: $("#work-metadata-container"),
            $viewButtonPanel: $("#work-metadata-view-button-panel"),
            $editorButtonPanel: $("#work-metadata-editor-button-panel")
        };
    }

    initTab(): void {
        super.initTab();

        var $addResponsibleTypeButton = $("#add-responsible-type-button");
        var $addResponsibleTypeContainer = $("#add-responsible-type-container");

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
            $addResponsibleTypeButton.prop("disabled", false);
            $addResponsibleTypeContainer.hide();
            this.addEditorDialog.show();
        });

        this.authorTypeahead.create((selectedExists, selectConfirmed) => {
            var $firstName = $("#add-author-first-name-preview");
            var $lastName = $("#add-author-last-name-preview");
            if (selectedExists) {
                var author = this.authorTypeahead.getValue();
                $firstName.val(author.firstName);
                $lastName.val(author.lastName);
                this.selectedAuthorId = author.id;
            } else {
                $firstName.val("");
                $lastName.val("");
                this.selectedAuthorId = null;
            }
        });

        this.editorTypeahead.create((selectedExists, selectConfirmed) => {
            var $firstName = $("#add-editor-first-name-preview");
            var $lastName = $("#add-editor-last-name-preview");
            if (selectedExists) {
                var editor = this.editorTypeahead.getValue();
                $firstName.val(editor.firstName);
                $lastName.val(editor.lastName);
                this.selectedResponsiblePersonId = editor.id;
            } else {
                $firstName.val("");
                $lastName.val("");
                this.selectedResponsiblePersonId = null;
            }
        });

        $addResponsibleTypeButton.click(() => {
            $addResponsibleTypeButton.prop("disabled", true);
            $addResponsibleTypeContainer.show();
        });
    }

    private createNewPublisher() {
        var name = $("#add-publisher-name").val();
        var email = $("#add-publisher-email").val();

        if (!name) {
            this.addPublisherDialog.showError("Nebyl vyplněn název nakladatele");
        }

        this.projectClient.createPublisher(name, email, (newPublisherId, errorCode) => {
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

        this.projectClient.createLiteraryKind(name, (newId, errorCode) => {
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

        this.projectClient.createLiteraryGenre(name, (newId, errorCode) => {
            if (errorCode !== null) {
                this.addLiteraryGenreDialog.showError("Chyba při vytváření nového literárního žánru");
                return;
            }

            UiHelper.addCheckboxAndSetChecked($("#work-metadata-literary-genre"), name, newId);
            this.addLiteraryGenreDialog.hide();
        });
    }

    private addAuthor() {
        if ($("#tab-select-existing-author").hasClass("active")) {
            //TODO assign author
        } else {
            //TODO create and assign author
        }

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

class MetadataUiHelper {
    public static addPerson($container: JQuery, label: string, idValue: string | number): HTMLDivElement {
        var rootElement = document.createElement("div");
        var deleteButton = document.createElement("button");
        var deleteGlyphSpan = document.createElement("span");
        var labelSpan = document.createElement("span");

        $(deleteButton)
            .addClass("btn")
            .addClass("btn-default")
            .attr("data-id", idValue)
            .append(deleteGlyphSpan);
        $(deleteGlyphSpan)
            .addClass("glyphicon")
            .addClass("glyphicon-remove");
        $(labelSpan)
            .addClass("text-as-form-control")
            .text(label);
        $(rootElement)
            .append(deleteButton)
            .append(labelSpan)
            .appendTo($container);

        return rootElement;
    }
}