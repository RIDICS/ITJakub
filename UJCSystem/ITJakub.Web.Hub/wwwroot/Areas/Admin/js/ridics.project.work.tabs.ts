class ProjectWorkMetadataTab extends ProjectMetadataTabBase {
    private projectId: number;
    private addPublisherDialog: BootstrapDialogWrapper;
    private addLiteraryKindDialog: BootstrapDialogWrapper;
    private addLiteraryGenreDialog: BootstrapDialogWrapper;
    private addAuthorDialog: BootstrapDialogWrapper;
    private addEditorDialog: BootstrapDialogWrapper;
    private projectClient: ProjectClient;
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
    }

    getConfiguration(): IProjectMetadataTabConfiguration {
        return {
            $panel: $("#work-metadata-container"),
            $viewButtonPanel: $("#work-metadata-view-button-panel"),
            $editorButtonPanel: $("#work-metadata-editor-button-panel")
        };
    }

    private createNewKeywordsByArray(names: string[]): JQueryXHR {
        const url = `${getBaseUrl()}Admin/Project/CreateKeywordsWithArray`;
        const id = 0; //keyword doesn't have an id yet
        const payload: IKeywordContract[] = [];
        for (let i = 0; i < names.length; i++) {
            payload.push(
                {
                    name: names[i],
                    id: id
                });
        };
        return $.post(url, { request: payload });
    }

    private initKeywords() {
        const selectedKeywordEls = $(".keywords-list-selected").children();
        const engine = new Bloodhound({
            //TODO preparation for ajax autocomplete function
            datumTokenizer: (d: any) => Bloodhound.tokenizers.whitespace(d.label),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: {
                url: `${getBaseUrl()}Admin/Project/KeywordTypeahead?keyword=%QUERY`,
                wildcard: "%QUERY",
                transform: (response: IKeywordContract[]) =>
                    $.map(response,
                        (keyword: IKeywordContract) => ({
                            value: keyword.id,
                            label: keyword.name
                        }))

            }
        });
        engine.initialize();
        $(".keywords-textarea").tokenfield({
            typeahead: [
                {
                    hint: true,
                    highlight: true,
                    minLength: 1
                },
                {
                    source: engine.ttAdapter(),
                    display: "label",
                    limit: 15
                }
            ]
        });
        var tags = [];
        selectedKeywordEls.each((index, element) => {
            const jEl = $(element);
            tags.push({ value: jEl.data("id"), label: jEl.data("name") });
        });
        $(".keywords-textarea").tokenfield("setTokens", tags);
    }

    private returnUniqueElsArray(array: any[]) {
        var seen = {};
        return array.filter(item => seen.hasOwnProperty(item) ? false : (seen[item] = true));
    }

    private createAuthorListStructure(authorList: IOriginalAuthor[], jEl: JQuery):JQuery {
        var elm = "";
        jEl.children(".author-list-item").remove();
        authorList.forEach((item:IOriginalAuthor) => {
            elm += `<div class="author-list-item clearfix" data-author-id="${item.id}"><div class="list-group-item col-xs-6 border-right existing-original-author-name">${item.firstName}</div><div class="list-group-item existing-original-author-surname border-left col-xs-6">${item.lastName}</div></div>`;
        });
        jEl.append(elm);
        return jEl;
    }

    private createResponsiblePersonListStructure(responsiblePersonList: IResponsiblePerson[], jEl: JQuery): JQuery {
        var elm = "";
        jEl.children(".responsible-person-list-item").remove();
        responsiblePersonList.forEach((item: IResponsiblePerson) => {
            elm += `<div class="responsible-person-list-item clearfix" data-responsible-person-id="${item.id}"><div class="list-group-item col-xs-6 border-right existing-responsible-person-name">${item.firstName}</div><div class="list-group-item existing-responsible-person-surname border-left col-xs-6">${item.lastName}</div></div>`;
        });
        jEl.append(elm);
        return jEl;
    }

    initTab(): void {
        super.initTab();
        this.initKeywords();
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

        $(".new-original-author-button").on("click", () => {
            $(".author-list-items").empty();
            $("#add-author-search").prop("disabled", true);
            $(".author-name-input-row").show();
        });

        $(".new-responsible-person-button").on("click", () => {
            $(".responsible-person-list-items").empty();
            $("#add-editor-search").prop("disabled", true);
            $(".responsible-person-name-input-row").show();
        });

        const addAuthorDialogCancelButton = $("#add-author-dialog").find(`[data-dismiss="modal"]`);
        addAuthorDialogCancelButton.on("click", () => {
            $(".author-name-input-row").hide();
            $("#add-author-search").prop("disabled", false);
            $(".author-list-items").empty();
            $(".existing-original-author-selected").removeClass("existing-original-author-selected");
            const newAuthorButtonEl = $(".new-original-author-button");
            newAuthorButtonEl.prop("disabled", false);
        });

        const newAuthorNameEl = $(".add-author-first-name");
        const newAuthorSurnameEl = $(".add-author-last-name");
        const newAuthorButtonEl = $(".new-original-author-button");
        const $authorId = $("#add-author-id-preview");
        var isAuthorSelected = false;

        $(".author-list-items").on("click", ".author-list-item", (event: Event) => {
            var targetEl = $(event.target);
            if (!targetEl.hasClass("author-list-item")) {
                targetEl = targetEl.parents(".author-list-item");
            }
            $(".existing-original-author-selected").not(targetEl).removeClass("existing-original-author-selected");
            targetEl.toggleClass("existing-original-author-selected");
            var authorId = null;
            if ($(".existing-original-author-selected").length) {
                isAuthorSelected = true;
                authorId = $(".existing-original-author-selected").data("author-id");
                $authorId.val(authorId);
                this.selectedAuthorId = authorId;
            } else {
                isAuthorSelected = false;
                $authorId.val("");
                this.selectedAuthorId = null;
            }
            newAuthorButtonEl.prop("disabled", isAuthorSelected);
            newAuthorNameEl.prop("disabled", isAuthorSelected);
            newAuthorSurnameEl.prop("disabled", isAuthorSelected);
        });

        $("#add-author-search").on("input", (event: Event) => {
            const textAreaEl = $(event.target);
            const enteredText = textAreaEl.val();
            if (enteredText === "") {
                $(".author-list-item").remove();
                newAuthorButtonEl.prop("disabled", false);
                newAuthorNameEl.prop("disabled", false);
                newAuthorSurnameEl.prop("disabled", false);
                $authorId.val("");
                this.selectedAuthorId = null;
                return;
            }
            $.get(`${getBaseUrl()}Admin/Project/GetTypeaheadOriginalAuthor?query=${enteredText}`).done(
                (data: IOriginalAuthor[]) => {
                    if (data.length) {
                        $authorId.val("");
                        this.selectedAuthorId = null;
                        this.createAuthorListStructure(data, $(".author-list-items"));
                    } else {
                        $(".author-list-item").remove();
                        $authorId.val("");
                        this.selectedAuthorId = null;
                    }
                });
        });

        const newResponsiblePersonNameEl = $(".add-responsible-person-first-name");
        const newResponsiblePersonSurnameEl = $(".add-responsible-person-last-name");
        const newResponsiblePersonButtonEl = $(".new-responsible-person-button");
        var isResponsiblePersonSelected = false;

        const $editorId = $("#add-editor-id-preview");
        $("#add-editor-search").on("input", (event: Event) => {
            const textAreaEl = $(event.target);
            const enteredText = textAreaEl.val();
            if (enteredText === "") {
                $(".responsible-person-list-item").remove();
                newResponsiblePersonButtonEl.prop("disabled", false);
                newResponsiblePersonNameEl.prop("disabled", false);
                newResponsiblePersonSurnameEl.prop("disabled", false);
                $editorId.val("");
                this.selectedResponsiblePersonId = null;
                return;
            }
            $.get(`${getBaseUrl()}Admin/Project/GetTypeaheadResponsiblePerson?query=${enteredText}`).done(
                (data: IResponsiblePerson[]) => {
                    if (data.length) {
                        $editorId.val("");
                        this.selectedResponsiblePersonId = null;
                        this.createResponsiblePersonListStructure(data, $(".responsible-person-list-items"));
                    } else {
                        $(".responsible-person-list-item").remove();
                        $editorId.val("");
                        this.selectedResponsiblePersonId = null;
                    }
                });
        });

        $(".responsible-person-list-items").on("click", ".responsible-person-list-item", (event: Event) => {
            var targetEl = $(event.target);
            if (!targetEl.hasClass("responsible-person-list-item")) {
                targetEl = targetEl.parents(".responsible-person-list-item");
            }
            $(".existing-responsible-person-selected").not(targetEl).removeClass("existing-responsible-person-selected");
            targetEl.toggleClass("existing-responsible-person-selected");
            var responsiblePersonId = null;
            if ($(".existing-responsible-person-selected").length) {
                isResponsiblePersonSelected = true;
                responsiblePersonId = $(".existing-responsible-person-selected").data("responsible-person-id");
                $editorId.val(responsiblePersonId);
                this.selectedResponsiblePersonId = responsiblePersonId;
            } else {
                isResponsiblePersonSelected = false;
                $editorId.val("");
                this.selectedResponsiblePersonId = null;
            }
            newResponsiblePersonButtonEl.prop("disabled", isAuthorSelected);
            newResponsiblePersonNameEl.prop("disabled", isAuthorSelected);
            newResponsiblePersonSurnameEl.prop("disabled", isAuthorSelected);
        });
        const addResponsiblePersonDialogCancelButton = $("#add-editor-dialog").find(`[data-dismiss="modal"]`);
        addResponsiblePersonDialogCancelButton.on("click", () => {
            $(".responsible-person-name-input-row").hide();
            $("#add-editor-search").prop("disabled", false);
            $(".existing-responsible-person-selected").removeClass("existing-original-author-selected");
            $(".responsible-person-list-items").empty();
            const newResponsiblePersonButtonEl = $(".new-responsible-person-button");
            newResponsiblePersonButtonEl.prop("disabled", false);
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
            this.addPublisherDialog.showError("Nebyl vyplněn název nakladatele");
        }

        this.projectClient.createPublisher(name,
            email,
            (newPublisherId, errorCode) => {
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

        this.projectClient.createLiteraryKind(name,
            (newId, errorCode) => {
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

        this.projectClient.createLiteraryGenre(name,
            (newId, errorCode) => {
                if (errorCode !== null) {
                    this.addLiteraryGenreDialog.showError("Chyba při vytváření nového literárního žánru");
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
            const newAuthorButtonEl = $(".new-original-author-button");
            newAuthorButtonEl.prop("disabled", false);
            $("#add-author-search").prop("disabled", false);
            $(".author-list-items").empty();
            $(".author-name-input-row").hide();
            this.addAuthorDialog.hide();
        };

        const selectedExistingAuthorEl = $(".existing-original-author-selected");
        if (selectedExistingAuthorEl.length) {
            id = $("#add-author-id-preview").val();
            firstName = selectedExistingAuthorEl.children(".existing-original-author-name").text();
            lastName = selectedExistingAuthorEl.children(".existing-original-author-surname").text();

            finishAddingAuthor();
        } else {
            firstName = $("#add-author-first-name").val();
            lastName = $("#add-author-last-name").val();
            if (firstName === "" || lastName === "") {
                this.addAuthorDialog.showError("Please enter a name or choose existing author");
                return;
            }
            const createAuthorAjax = this.projectClient.createAuthor(firstName,
                lastName);
            createAuthorAjax.done((data: number) => {
                id = data;
                finishAddingAuthor();
            }).fail(() => {
                this.addAuthorDialog.showError();
            });

        }
    }

    private createResponsibleType() {
        var text = $("#add-responsible-type-text").val();
        var type = $("#add-responsible-type-type").val();
        var typeLabel = $("#add-responsible-type-type option:selected").text();

        var $savingIcon = $("#add-responsible-type-saving-icon");
        $savingIcon.show();

        this.projectClient.createResponsibleType(type, text).done((newResponsibleTypeId: number) => {
            $("#add-responsible-type-container").hide();
            $("#add-responsible-type-button").prop("disabled", false);
            $("#add-responsible-type-type").val(0);
            $("#add-responsible-type-text").val("");
            var optionName = `${text} (${typeLabel})`;
            UiHelper.addSelectOptionAndSetDefault($("#add-editor-type"), optionName, newResponsibleTypeId);
        }).fail(() => {
            //TODO handle error
        }).always(() => {
            $savingIcon.hide();
        });
    }

    private addEditor() {
        var id: number;
        var responsibilityText: string;
        var responsibilityTypeId: number;
        var firstName: string;
        var lastName: string;

        var finishAddingEditor = () => {
            var element = MetadataUiHelper.addPerson($("#work-metadata-editors"), `${firstName} ${lastName} - ${responsibilityText}`, id, responsibilityTypeId);
            $(element).addClass("editor-item");
            this.addRemovePersonEvent($(".remove-button", element));

            this.addEditorDialog.hide();
        };

        const selectedExistingResponsiblePersonEl = $(".existing-responsible-person-selected");
        if (selectedExistingResponsiblePersonEl.length) {
            id = $("#add-editor-id-preview").val();
            firstName = selectedExistingResponsiblePersonEl.children(".existing-responsible-person-name").text();
            lastName = selectedExistingResponsiblePersonEl.children(".existing-responsible-person-surname").text();
            responsibilityTypeId = $("#add-editor-type").find(":selected").val();
            const responsibilityTextWithParenthesis = $("#add-editor-type").find(":selected").text();
            responsibilityText = responsibilityTextWithParenthesis.replace(/ *\([^)]*\) */g, "");
            finishAddingEditor();
        } else {
            firstName = $("#add-responsible-person-first-name").val();
            lastName = $("#add-responsible-person-last-name").val();
            responsibilityTypeId = $("#add-editor-type").val();
            const responsibilityTextWithParenthesis = $("#add-editor-type").find(":selected").text();
            responsibilityText = responsibilityTextWithParenthesis.replace(/ *\([^)]*\) */g, "");

            this.projectClient.createResponsiblePerson(firstName, lastName).done((newResponsiblePersonId: number) => {
                id = newResponsiblePersonId;
                finishAddingEditor();
            }).fail(() => {
                this.addEditorDialog.showError();
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
        var selectedResponsibleIds = new Array<ISaveProjectResponsiblePerson>();
        var selectedKindIds = new Array<number>();
        var selectedGenreIds = new Array<number>();
        var keywordIdList = new Array<number>();

        $("#work-metadata-authors .author-item").each((index, elem) => {
            selectedAuthorIds.push($(elem).data("id"));
        });
        $("#work-metadata-editors .editor-item").each((index, elem) => {
            var projectResponsible: ISaveProjectResponsiblePerson = {//TODO investigate new responsible person addition
                responsiblePersonId: $(elem).data("id"),
                responsibleTypeId: $(elem).data("responsible-type-id")
            };
            selectedResponsibleIds.push(projectResponsible);
        });
        $("#work-metadata-literary-kind input:checked").each((index, elem) => {
            selectedKindIds.push($(elem).val());
        });
        $("#work-metadata-literary-genre input:checked").each((index, elem) => {
            selectedGenreIds.push($(elem).val());
        });

        const keywordsInputEl = $(".keywords-container").children(".tokenfield").children(".keywords-textarea");
        const keywordsArray = $.map(keywordsInputEl.val().split(","), $.trim);
        const uniqueKeywordArray = this.returnUniqueElsArray(keywordsArray);
        var keywordNonIdList: string[] = [];
        const onlyNumbersRegex = new RegExp(/^[0-9]*$/);
        for (let i = 0; i < uniqueKeywordArray.length; i++) {
            if (onlyNumbersRegex.test(uniqueKeywordArray[i])) {
                keywordIdList.push(uniqueKeywordArray[i]);
            } else {
                keywordNonIdList.push(uniqueKeywordArray[i]);
            }
        }
        const createNewKeywordAjax = this.createNewKeywordsByArray(keywordNonIdList);
        createNewKeywordAjax.done((newIds: number[]) => {
            const allKeywordIds = keywordIdList.concat(newIds);
            var data: ISaveMetadataResource = {
                keywordIdList: allKeywordIds,
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
                projectResponsiblePersonIdList: selectedResponsibleIds
            };
            var $loadingGlyph = $("#work-metadata-save-button .saving-icon");
            var $buttons = $("#work-metadata-editor-button-panel button");
            var $successAlert = $("#work-metadata-save-success");
            var $errorAlert = $("#work-metadata-save-error");
            $loadingGlyph.show();
            $buttons.prop("disabled", true);
            $successAlert.finish().hide();
            $errorAlert.hide();

            this.projectClient.saveMetadata(this.projectId,data).done((data) => {
                $successAlert.show().delay(3000).fadeOut(2000);
                $("#work-metadata-last-modification").text(data.lastModificationText);
                $("#work-metadata-literary-original").text(data.literaryOriginalText);
            }).fail(() => {
                $errorAlert.show();
            }).always(() => {
                $loadingGlyph.hide();
                $buttons.prop("disabled", false);
            });
        }).fail(() => {
            //TODO show that some keyword failed to save, possible duplicates
        });
    }
}

class ProjectWorkPageListTab extends ProjectModuleTabBase {
    private projectId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
    }

    initTab() {
        const main = new PageListEditorMain();
        main.init(this.projectId);
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

        $("#new-snapshot-container").append("<div class=\"loader\"></div>").load(url,
            null,
            (responseText, textStatus, xmlHttpRequest) => {
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

                $(`td:nth-child(${index + 1}) input[type=checkbox]`, this.$container).each((index, elem) => {
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

            var allCheckBox =
                <HTMLInputElement>$(`.subheader th:nth-child(${position}) input[type=checkbox]`, this.$container)
                .get(0);
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

    initTab() {}
}

class ProjectWorkHistoryTab extends ProjectModuleTabBase {
    private projectId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
    }

    initTab() {}
}

class ProjectWorkNoteTab extends ProjectModuleTabBase {
    private readonly projectId: number;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
    }

    initTab() {
        const main = new EditionNote(this.projectId);
        main.init();
    }
}

class MetadataUiHelper {
    public static addPerson($container: JQuery, label: string, idValue: string | number, responsibilityTypeId?: number | string): HTMLDivElement {
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
        if (responsibilityTypeId != null) {
            $(rootElement).attr("data-responsible-type-id", responsibilityTypeId);
        }
        $(rootElement)
            .attr("data-id", idValue)
        .append(deleteButton)
            .append(labelSpan)
            .appendTo($container);

        return rootElement;
    }
}