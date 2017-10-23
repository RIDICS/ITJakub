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

    private localization: Localization;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
        this.projectClient = new ProjectClient();

        this.localization = new Localization();

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

        $("#work-metadata-cancel-button").click(() => {
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
            var $authorId = $("#add-author-id-preview");
            if (selectedExists) {
                var author = this.authorTypeahead.getValue();
                $firstName.val(author.firstName);
                $lastName.val(author.lastName);
                $authorId.val(author.id);
                this.selectedAuthorId = author.id;
            } else {
                $firstName.val("");
                $lastName.val("");
                $authorId.val("");
                this.selectedAuthorId = null;
            }
        });

        this.editorTypeahead.create((selectedExists, selectConfirmed) => {
            var $firstName = $("#add-editor-first-name-preview");
            var $lastName = $("#add-editor-last-name-preview");
            var $editorId = $("#add-editor-id-preview");
            if (selectedExists) {
                var editor = this.editorTypeahead.getValue();
                $firstName.val(editor.firstName);
                $lastName.val(editor.lastName);
                $editorId.val(editor.id);
                this.selectedResponsiblePersonId = editor.id;
            } else {
                $firstName.val("");
                $lastName.val("");
                $editorId.val("");
                this.selectedResponsiblePersonId = null;
            }
        });

        $addResponsibleTypeButton.click(() => {
            $addResponsibleTypeButton.prop("disabled", true);
            $addResponsibleTypeContainer.show();
            $("#add-responsible-type-saving-icon").hide();
        });

        $("#add-responsible-type-save").click(this.createResponsibleType.bind(this));

        this.addRemovePersonEvent($("#work-metadata-authors .remove-button, #work-metadata-editors .remove-button"));

        var $saveButton = $("#work-metadata-save-button");
        $saveButton.click(() => {
            this.saveMetadata();
        });
        $(".saving-icon", $saveButton).hide();

        $("#work-metadata-save-error, #work-metadata-save-success").hide();
    }

    private createNewPublisher() {
        var name = $("#add-publisher-name").val();
        var email = $("#add-publisher-email").val();

        if (!name) {
            this.addPublisherDialog.showError(this.localization.translate("MissingPublisherNameError", "Admin").value);
        }

        this.projectClient.createPublisher(name, email, (newPublisherId, errorCode) => {
            if (errorCode !== null) {
                this.addPublisherDialog.showError(this.localization.translate("CreatePublisherError", "Admin").value);
                return;
            }

            UiHelper.addSelectOptionAndSetDefault($("#work-metadata-publisher"), name, newPublisherId);
            this.addPublisherDialog.hide();
        });
    }

    private createNewLiteraryKind() {
        var name = $("#add-literary-kind-name").val();

        if (!name) {
            this.addLiteraryKindDialog.showError(this.localization.translate("MissingName", "Admin").value);
        }

        this.projectClient.createLiteraryKind(name, (newId, errorCode) => {
            if (errorCode !== null) {
                this.addLiteraryKindDialog.showError(this.localization.translate("CreateLiteraryKind", "Admin").value);
                return;
            }

            UiHelper.addCheckboxAndSetChecked($("#work-metadata-literary-kind"), name, newId);
            this.addLiteraryKindDialog.hide();
        });
    }

    private createNewLiteraryGenre() {
        var name = $("#add-literary-genre-name").val();

        if (!name) {
            this.addLiteraryGenreDialog.showError(this.localization.translate("MissingName", "Admin").value);
        }

        this.projectClient.createLiteraryGenre(name, (newId, errorCode) => {
            if (errorCode !== null) {
                this.addLiteraryGenreDialog.showError(this.localization.translate("CreateLiteraryGenre", "Admin").value);
                return;
            }

            UiHelper.addCheckboxAndSetChecked($("#work-metadata-literary-genre"), name, newId);
            this.addLiteraryGenreDialog.hide();
        });
    }

    private addAuthor() {
        var id: number;
        var firstName: string;
        var lastName: string;

        var finishAddingAuthor = () => {
            var element = MetadataUiHelper.addPerson($("#work-metadata-authors"), firstName + " " + lastName, id);
            $(element).addClass("author-item");
            this.addRemovePersonEvent($(".remove-button", element));

            this.addAuthorDialog.hide();
        };

        if ($("#tab-select-existing-author").hasClass("active")) {
            id = $("#add-author-id-preview").val();
            firstName = $("#add-author-first-name-preview").val();
            lastName = $("#add-author-last-name-preview").val();

            finishAddingAuthor();
        } else {
            firstName = $("#add-author-first-name").val();
            lastName = $("#add-author-last-name").val();

            this.projectClient.createAuthor(firstName, lastName, (newAuthorId, errorCode) => {
                if (errorCode != null) {
                    this.addAuthorDialog.showError();
                    return;
                }

                id = newAuthorId;
                finishAddingAuthor();
            });
        }
    }

    private createResponsibleType() {
        var text = $("#add-responsible-type-text").val();
        var type = $("#add-responsible-type-type").val();
        var typeLabel = $("#add-responsible-type-type option:selected").text();

        var $savingIcon = $("#add-responsible-type-saving-icon");
        $savingIcon.show();

        this.projectClient.createResponsibleType(type, text, (newResponsibleTypeId, errorCode) => {
            if (errorCode != null) {
                $savingIcon.hide();
                return;
                //TODO handle error
            }

            $savingIcon.hide();

            $("#add-responsible-type-container").hide();
            $("#add-responsible-type-button").prop("disabled", false);
            $("#add-responsible-type-type").val(0);
            $("#add-responsible-type-text").val("");

            var optionName = `${text} (${typeLabel})`;
            UiHelper.addSelectOptionAndSetDefault($("#add-editor-type"), optionName, newResponsibleTypeId);
        });
    }

    private addEditor() {
        var id: number;
        var firstName: string;
        var lastName: string;

        var finishAddingEditor = () => {
            var element = MetadataUiHelper.addPerson($("#work-metadata-editors"), firstName + " " + lastName, id);
            $(element).addClass("editor-item");
            this.addRemovePersonEvent($(".remove-button", element));

            this.addEditorDialog.hide();
        };

        if ($("#tab-select-existing-editor").hasClass("active")) {
            id = $("#add-editor-id-preview").val();
            firstName = $("#add-editor-first-name-preview").val();
            lastName = $("#add-editor-last-name-preview").val();

            finishAddingEditor();
        } else {
            firstName = $("#add-editor-first-name").val();
            lastName = $("#add-editor-last-name").val();
            var responsibleTypeId = $("#add-editor-type").val();

            this.projectClient.createResponsiblePerson(firstName, lastName, responsibleTypeId, (newResponsiblePersonId, errorCode) => {
                if (errorCode != null) {
                    this.addEditorDialog.showError();
                    return;
                }

                id = newResponsiblePersonId;
                finishAddingEditor();
            });
        }
    }

    private addRemovePersonEvent($removeButton: JQuery) {
        $removeButton.click((event) => {
            $(event.currentTarget).closest(".author-item, .editor-item").remove();
        });
    }

    private saveMetadata() {
        var selectedAuthorIds = new Array<number>();
        var selectedResponsibleIds = new Array<number>();
        var selectedKindIds = new Array<number>();
        var selectedGenreIds = new Array<number>();

        $("#work-metadata-authors .author-item").each((index, elem) => {
            selectedAuthorIds.push($(elem).data("id"));
        });
        $("#work-metadata-editors .editor-item").each((index, elem) => {
            selectedResponsibleIds.push($(elem).data("id"));
        });
        $("#work-metadata-literary-kind input:checked").each((index, elem) => {
            selectedKindIds.push($(elem).val());
        });
        $("#work-metadata-literary-genre input:checked").each((index, elem) => {
            selectedGenreIds.push($(elem).val());
        });

        var data: ISaveMetadataResource = {
            biblText: $("#work-metadata-bibl-text").val(),
            copyright: $("#work-metadata-copyright").val(),
            manuscriptCountry: $("#work-metadata-original-country").val(),
            manuscriptExtent: $("#work-metadata-original-extent").val(),
            manuscriptIdno: $("#work-metadata-original-idno").val(),
            manuscriptRepository: $("#work-metadata-original-repository").val(),
            manuscriptSettlement: $("#work-metadata-original-settlement").val(),
            notAfter: $("#work-metadata-not-after").val(),
            notBefore: $("#work-metadata-not-before").val(),
            originDate: $("#work-metadata-origin-date").val(),
            publishDate: $("#work-metadata-publish-date").val(),
            publishPlace: $("#work-metadata-publish-place").val(),
            publisherId: $("#work-metadata-publisher").val(),
            relicAbbreviation: $("#work-metadata-relic-abbreviation").val(),
            sourceAbbreviation: $("#work-metadata-source-abbreviation").val(),
            subTitle: $("#work-metadata-subtitle").val(),
            title: $("#work-metadata-title").val(),
            authorIdList: selectedAuthorIds,
            literaryGenreIdList: selectedGenreIds,
            literaryKindIdList: selectedKindIds,
            responsiblePersonIdList: selectedResponsibleIds
        };

        var $loadingGlyph = $("#work-metadata-save-button .saving-icon");
        var $buttons = $("#work-metadata-editor-button-panel button");
        var $successAlert = $("#work-metadata-save-success");
        var $errorAlert = $("#work-metadata-save-error");
        $loadingGlyph.show();
        $buttons.prop("disabled", true);
        $successAlert.finish().hide();
        $errorAlert.hide();

        this.projectClient.saveMetadata(this.projectId, data, (resultData, errorCode) => {
            $loadingGlyph.hide();
            $buttons.prop("disabled", false);

            if (errorCode != null) {
                $errorAlert.show();
                return;
            }

            $successAlert.show().delay(3000).fadeOut(2000);
            $("#work-metadata-last-modification").text(resultData.lastModificationText);
            $("#work-metadata-literary-original").text(resultData.literaryOriginalText);
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

    private localization: Localization;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;

        this.localization = new Localization();
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
                    .addContent(this.localization.translate("CreateResourcesError", "Admin").value)
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
            .addClass("remove-button")
            .append(deleteGlyphSpan);
        $(deleteGlyphSpan)
            .addClass("glyphicon")
            .addClass("glyphicon-remove");
        $(labelSpan)
            .addClass("text-as-form-control")
            .text(label);
        $(rootElement)
            .attr("data-id", idValue)
            .append(deleteButton)
            .append(labelSpan)
            .appendTo($container);

        return rootElement;
    }
}