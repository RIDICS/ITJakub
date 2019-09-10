class ProjectWorkMetadataTab extends ProjectMetadataTabBase {
    private projectId: number;
    private addAuthorDialog: BootstrapDialogWrapper;
    private addEditorDialog: BootstrapDialogWrapper;
    private selectRangeDialog: BootstrapDialogWrapper;
    private rangePeriodViewSliderClass: RegExDatingConditionRangePeriodView;
    private projectClient: ProjectClient;
    private selectedAuthorId: number;
    private selectedResponsiblePersonId: number;
    private publisherTypeahead: SingleSetTypeaheadSearchBox<string>;
    private publisherName: string = null;
    private workModule: ProjectWorkModule;
    private adminApiClient = new AdminApiClient();

    constructor(projectId: number, workModule: ProjectWorkModule) {
        super();
        this.projectId = projectId;
        this.projectClient = new ProjectClient();
        this.workModule = workModule;
        this.publisherTypeahead =
            new SingleSetTypeaheadSearchBox<string>("#work-metadata-publisher", "Admin/Project", x => `${x}`, null);
        this.publisherTypeahead.setDataSet("Publisher");

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

        this.selectRangeDialog = new BootstrapDialogWrapper({
            element: $("#select-range-dialog"),
            autoClearInputs: true,
            submitCallback: this.dateRangeSelected.bind(this)
        });
    }

    getConfiguration(): IProjectMetadataTabConfiguration {
        return {
            $panel: $("#work-metadata-container"),
            $viewButtonPanel: $("#work-metadata-view-button-panel"),
            $editorButtonPanel: $("#work-metadata-editor-button-panel")
        };
    }
    
    private returnUniqueElsArray(array: any[]) {
        var seen = {};
        return array.filter(item => seen.hasOwnProperty(item) ? false : (seen[item] = true));
    }

    private createAuthorListStructure(authorList: IOriginalAuthor[], jEl: JQuery): JQuery {
        var elm = "";
        authorList.forEach((item: IOriginalAuthor) => {
            elm += `<div class="author-list-item clearfix" data-author-id="${item.id
                }"><div class="list-group-item col-xs-6 border-right existing-original-author-name">${item.firstName
                }</div><div class="list-group-item existing-original-author-surname border-left col-xs-6">${
                item.lastName}</div></div>`;
        });
        jEl.append(elm);
        return jEl;
    }

    private createResponsiblePersonListStructure(responsiblePersonList: IResponsiblePerson[], jEl: JQuery): JQuery {
        var elm = "";
        responsiblePersonList.forEach((item: IResponsiblePerson) => {
            elm += `<div class="responsible-person-list-item clearfix" data-responsible-person-id="${item.id
                }"><div class="list-group-item col-xs-6 border-right existing-responsible-person-name">${item.firstName
                }</div><div class="list-group-item existing-responsible-person-surname border-left col-xs-6">${
                item.lastName}</div></div>`;
        });
        jEl.append(elm);
        return jEl;
    }
    
    initTab(): void {
        super.initTab();
        var $addResponsibleTypeButton = $("#add-responsible-type-button");
        
        this.rangePeriodViewSliderClass = new RegExDatingConditionRangePeriodView();
        this.rangePeriodViewSliderClass.makeRangeView(
            $("#select-range-dialog").find(".regex-dating-precision-div")[0] as Node as HTMLDivElement);

        $("#work-metadata-publisher-email").on("input",
            () => {
                const emailGroupEl = $(".email-group");
                const emailToValidateEl = emailGroupEl.children(".email-to-validate");
                const iconEl = emailToValidateEl.children(".form-control-feedback");
                iconEl.show();
                emailGroupEl.addClass("has-feedback");
                $("#email-form").validate({
                    rules: {
                        "email": {
                            required: true,
                            email: true,
                            minlength: 5,
                            maxlength: 100
                        }
                    },
                    highlight: () => {
                        emailGroupEl.removeClass("has-success").addClass("has-error");
                        iconEl.addClass("glyphicon-remove").removeClass("glyphicon-ok");
                    },
                    unhighlight: () => {
                        emailGroupEl.removeClass("has-error").addClass("has-success");
                        iconEl.removeClass("glyphicon-remove").addClass("glyphicon-ok");
                    },
                    errorPlacement: (error, element) => true //disable error messages
                }).form();
            });

        $("#work-metadata-publish-date").on("input",
            () => {
                const enteredText = $("#work-metadata-publish-date").val() as string;
                const yearIsValid = this.publicationYearsValid(enteredText);
                const publishYearsGroupEl = $(".publication-years-group");
                const iconEl = publishYearsGroupEl.find(".form-control-feedback");
                iconEl.show();
                publishYearsGroupEl.addClass("has-feedback");
                if (yearIsValid) {
                    publishYearsGroupEl.addClass("has-success");
                    publishYearsGroupEl.removeClass("has-error");
                    iconEl.addClass("glyphicon-ok");
                    iconEl.removeClass("glyphicon-remove");
                } else {
                    publishYearsGroupEl.addClass("has-error");
                    publishYearsGroupEl.removeClass("has-success");
                    iconEl.addClass("glyphicon-remove");
                    iconEl.removeClass("glyphicon-ok");
                }
            });

        $(document.documentElement).on("click",
            ".move-person-up",
            (event) => {
                const targetEl = $(event.target as Node as Element);
                const personEl = targetEl.parents(".editor-item, .author-item");
                const prevPersonEl = personEl.prev(".editor-item, .author-item");
                if (prevPersonEl.length) {
                    personEl.detach();
                    prevPersonEl.before(personEl);
                }
            });

        $(document.documentElement).on("click",
            ".move-person-down",
            (event) => {
                const targetEl = $(event.target as Node as Element);
                const personEl = targetEl.parents(".editor-item, .author-item");
                const nextPersonEl = personEl.next(".editor-item, .author-item");
                if (nextPersonEl.length) {
                    personEl.detach();
                    nextPersonEl.after(personEl);
                }
            });

        $("#work-metadata-edit-button").click(() => {
            this.enabledEdit();
            this.publisherTypeahead.create((selectedExists, selectConfirmed) => {
                if (selectedExists) {
                    const publisher = this.publisherTypeahead.getValue();
                    this.publisherName = publisher;
                } else {
                    this.publisherName = null;
                }
            });
        });

        $("#work-metadata-cancel-button").click(() => {
            this.disableEdit();
            const metadataTabSelector = "#project-work-metadata";
            var tabPanelEl = $(metadataTabSelector);
            tabPanelEl.empty();
            this.workModule.loadTabPanel(metadataTabSelector);
            this.publisherTypeahead.destroy();
        });

        $("#add-author-button").click(() => {
            this.addAuthorDialog.show();
        });

        $(".edit-date-range").click(() => {
            this.selectRangeDialog.show();
        });

        $("#add-editor-button").click(() => {
            $addResponsibleTypeButton.prop("disabled", false);
            this.addEditorDialog.show();
        });

        $(".generate-bibliography-button").on("click",
            () => {
                this.generateBibliography();
            });

        $(".new-original-author-button").on("click",
            () => {
                const finishAddingResponsiblePersonButtonGroup = $(".new-original-author-button-group");
                $("#add-author-search").prop("disabled", true);
                $(".new-original-author-button").prop("disabled", true);
                $(".author-name-input-row").show();
                finishAddingResponsiblePersonButtonGroup.show();
            });

        $(".new-original-author-cancel-button").on("click",
            () => {
                const finishAddingResponsiblePersonButtonGroup = $(".new-original-author-button-group");
                $("#add-author-search").prop("disabled", false);
                $(".new-original-author-button").prop("disabled", false);
                $(".author-name-input-row").hide();
                finishAddingResponsiblePersonButtonGroup.hide();
            });

        $(".new-original-author-finish-button").on("click",
            () => {
                const firstNameEl = $("#add-author-first-name");
                const lastNameEl = $("#add-author-last-name");
                const listItemsEl = $(".author-list-items");
                var firstName = firstNameEl.val() as string;
                var lastName = lastNameEl.val() as string;
                var id: number;
                if (!firstName || !lastName) {
                    this.addAuthorDialog.showError(localization.translate("EnterValidName", "Admin").value);
                    return;
                }
                const createAuthorAjax = this.projectClient.createAuthor(firstName,
                    lastName);
                createAuthorAjax.done((data: number) => {
                    id = data;
                    const newlyCreatedAuthorArray: IOriginalAuthor[] = [];
                    const newlyCreatedAuthor: IOriginalAuthor = {
                        id: id,
                        lastName: lastName,
                        firstName: firstName
                    };
                    newlyCreatedAuthorArray.push(newlyCreatedAuthor);
                    this.createAuthorListStructure(newlyCreatedAuthorArray, listItemsEl);

                    const newlyCreatedOriginalAuthorEl = $(`[data-author-id=${id}]`);
                    $(".existing-original-author-selected").not(newlyCreatedOriginalAuthorEl)
                        .removeClass("existing-original-author-selected");
                    newlyCreatedOriginalAuthorEl.addClass("existing-original-author-selected");
                    $("#add-author-id-preview").val(id);
                    $(".new-original-author-button").prop("disabled", false);
                    $("#add-author-search").prop("disabled", false);
                    firstNameEl.val("");
                    lastNameEl.val("");
                    $(".author-name-input-row").hide();
                    $(".new-original-author-button-group").hide();
                }).fail(() => {
                    this.addAuthorDialog.showError();
                });
            });

        $(".new-responsible-person-button").on("click",
            () => {
                const finishAddingResponsiblePersonButtonGroup = $(".new-responsible-person-button-group");
                $(".new-responsible-person-button").prop("disabled", true);
                $("#add-editor-search").prop("disabled", true);
                $(".responsible-person-name-input-row").show();
                finishAddingResponsiblePersonButtonGroup.show();
            });

        $(".new-responsible-person-cancel-button").on("click",
            () => {
                const finishAddingResponsiblePersonButtonGroup = $(".new-responsible-person-button-group");
                $(".new-responsible-person-button").prop("disabled", false);
                $("#add-editor-search").prop("disabled", false);
                $(".responsible-person-name-input-row").hide();
                finishAddingResponsiblePersonButtonGroup.hide();
            });

        $(".new-responsible-person-finish-button").on("click",
            () => {
                const listItemsEl = $(".responsible-person-list-items");
                const firstNameEl = $("#add-responsible-person-first-name");
                const lastNameEl = $("#add-responsible-person-last-name");
                const firstName = firstNameEl.val() as string;
                const lastName = lastNameEl.val() as string;
                var id: number;
                if (firstName === "" || lastName === "") {
                    this.addEditorDialog.showError(localization.translate("EnterValidName", "Admin").value);
                    return;
                }
                this.projectClient.createResponsiblePerson(firstName, lastName).done(
                    (newResponsiblePersonId: number) => {
                        id = newResponsiblePersonId;
                        const createdResponsiblePersonArray: IResponsiblePerson[] = [];
                        const createdResponsiblePerson: IResponsiblePerson = {
                            id: id,
                            lastName: lastName,
                            firstName: firstName
                        };
                        createdResponsiblePersonArray.push(createdResponsiblePerson);
                        this.createResponsiblePersonListStructure(createdResponsiblePersonArray, listItemsEl);
                        const newlyCreatedResponsiblePerson = $(`[data-responsible-person-id=${id}]`);
                        $(".existing-responsible-person-selected").not(newlyCreatedResponsiblePerson)
                            .removeClass("existing-responsible-person-selected");
                        newlyCreatedResponsiblePerson.addClass("existing-responsible-person-selected");
                        $("#add-editor-id-preview").val(id);
                        $(".new-responsible-person-button").prop("disabled", false);
                        $("#add-editor-search").prop("disabled", false);
                        firstNameEl.val("");
                        lastNameEl.val("");
                        $(".responsible-person-name-input-row").hide();
                        $(".new-responsible-person-button-group").hide();
                    }).fail(() => {
                    this.addEditorDialog.showError();
                });
            });

        const addAuthorDialogCancelButton = $("#add-author-dialog").find(`[data-dismiss="modal"]`);
        addAuthorDialogCancelButton.on("click",
            () => {
                $(".author-name-input-row").hide();
                $("#add-author-search").prop("disabled", false);
                $(".author-list-items").empty();
                $(".existing-original-author-selected").removeClass("existing-original-author-selected");
                const newAuthorButtonEl = $(".new-original-author-button");
                newAuthorButtonEl.prop("disabled", false);
                $(".new-original-author-button-group").hide();
                const worksParticipatedEl = $(".works-produced");
                const tableBodyEl = worksParticipatedEl.find(".works-list-items");
                tableBodyEl.empty();
            });

        const $authorId = $("#add-author-id-preview");

        $(".author-list-items").on("click",
            ".author-list-item",
            (event) => {
                var targetEl = $(event.target as HTMLElement);
                if (!targetEl.hasClass("author-list-item")) {
                    targetEl = targetEl.parents(".author-list-item");
                }
                $(".existing-original-author-selected").not(targetEl).removeClass("existing-original-author-selected");
                targetEl.toggleClass("existing-original-author-selected");
                var authorId = null;
                if ($(".existing-original-author-selected").length) {
                    authorId = $(".existing-original-author-selected").data("author-id");
                    $authorId.val(authorId);
                    this.selectedAuthorId = authorId;
                    this.loadProjectsByAuthor(authorId);
                } else {
                    $authorId.val("");
                    this.selectedAuthorId = null;
                }
            });

        $("#add-author-search").on("input",
            (event) => {
                const textAreaEl = $(event.target as Node as HTMLElement);
                const enteredText = textAreaEl.val() as string;
                if (enteredText === "") {
                    $(".author-list-item").remove();
                    $authorId.val("");
                    this.selectedAuthorId = null;
                    return;
                }
                this.adminApiClient.getOriginalAuthorTypeahead(enteredText).done(
                    (data: IOriginalAuthor[]) => {
                        if (data.length) {
                            $authorId.val("");
                            const listItemsEl = $(".author-list-items");
                            this.selectedAuthorId = null;
                            listItemsEl.children(".author-list-item").remove();
                            this.createAuthorListStructure(data, listItemsEl);
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
        $("#add-editor-search").on("input",
            (event) => {
                const textAreaEl = $(event.target as Node as HTMLElement);
                const enteredText = textAreaEl.val() as string;
                if (enteredText === "") {
                    $(".responsible-person-list-item").remove();
                    newResponsiblePersonButtonEl.prop("disabled", false);
                    newResponsiblePersonNameEl.prop("disabled", false);
                    newResponsiblePersonSurnameEl.prop("disabled", false);
                    $editorId.val("");
                    this.selectedResponsiblePersonId = null;
                    return;
                }
                this.adminApiClient.getResponsiblePersonTypeahead(enteredText).done(
                    (data: IResponsiblePerson[]) => {
                        if (data.length) {
                            $editorId.val("");
                            this.selectedResponsiblePersonId = null;
                            $(".responsible-person-list-items").children(".responsible-person-list-item").remove();
                            this.createResponsiblePersonListStructure(data, $(".responsible-person-list-items"));
                        } else {
                            $(".responsible-person-list-item").remove();
                            $editorId.val("");
                            this.selectedResponsiblePersonId = null;
                        }
                    });
            });

        $(".responsible-person-list-items").on("click",
            ".responsible-person-list-item",
            (event) => {
                var targetEl = $(event.target);
                if (!targetEl.hasClass("responsible-person-list-item")) {
                    targetEl = targetEl.parents(".responsible-person-list-item");
                }
                $(".existing-responsible-person-selected").not(targetEl)
                    .removeClass("existing-responsible-person-selected");
                targetEl.toggleClass("existing-responsible-person-selected");
                var responsiblePersonId = null;
                if ($(".existing-responsible-person-selected").length) {
                    isResponsiblePersonSelected = true;
                    responsiblePersonId = $(".existing-responsible-person-selected").data("responsible-person-id");
                    $editorId.val(responsiblePersonId);
                    this.selectedResponsiblePersonId = responsiblePersonId;
                    this.loadProjectsByResponsiblePerson(responsiblePersonId);
                } else {
                    isResponsiblePersonSelected = false;
                    $editorId.val("");
                    this.selectedResponsiblePersonId = null;
                }
            });
        const addResponsiblePersonDialogCancelButton = $("#add-editor-dialog").find(`[data-dismiss="modal"]`);
        addResponsiblePersonDialogCancelButton.on("click",
            () => {
                $(".responsible-person-name-input-row").hide();
                $("#add-editor-search").prop("disabled", false);
                $(".existing-responsible-person-selected").removeClass("existing-original-author-selected");
                $(".responsible-person-list-items").empty();
                const newResponsiblePersonButtonEl = $(".new-responsible-person-button");
                newResponsiblePersonButtonEl.prop("disabled", false);
                $(".new-responsible-person-button-group").hide();
                const worksParticipatedEl = $(".works-participated");
                const tableBodyEl = worksParticipatedEl.find(".works-list-items");
                tableBodyEl.empty();
            });

        $addResponsibleTypeButton.click(() => {
            $addResponsibleTypeButton.prop("disabled", true);
            $("#responsibility-type-input-elements").hide();
        });

        this.addRemovePersonEvent($("#work-metadata-authors .remove-button, #work-metadata-editors .remove-button"));
        
        var $saveButton = $("#work-metadata-save-button");
        $saveButton.click(() => {
            this.saveMetadata();
        });
        $(".saving-icon", $saveButton).hide();

        $("#work-metadata-save-error, #work-metadata-save-success").hide();
    }

    private loadProjectsByAuthor(authorId: number) {
        const start = 0;
        const count = 10; //Get first ten works
        const projectInfoAjax = this.projectClient.getProjectsByAuthor(authorId, start, count);
        const worksProducedEl = $(".works-produced");
        const tableBodyEl = worksProducedEl.find(".works-list-items");
        const numberOfWorksEl = worksProducedEl.find(".number-of-works-value");
        tableBodyEl.empty();
        tableBodyEl.addClass("loading");
        projectInfoAjax.done((data: IPagedResult<IProjectDetailContract>) => {
            this.generateWorkAuthorTableItem(data.list);
            numberOfWorksEl.text(data.totalCount);
        }).fail(() => {
            tableBodyEl.text(localization.translate("WorksLoadFailed", "Admin").value);
        }).always(() => {
            tableBodyEl.removeClass("loading");
        });
    }

    private generateWorkAuthorTableItem(projects: IProjectDetailContract[]) {
        var elm = "";
        projects.forEach((project) => {
            elm += `<div class="col-xs-12 works-list-item">${project.latestMetadata.title}</div>`;
        });
        const jqElement = $(elm);
        this.populateAuthorWorkListItemsTable(jqElement);
    }

    private generateWorkResponsiblePersonItem(projects: IProjectDetailContract[], responsiblePersonId: number) {
        var elm = "";
        projects.forEach((project) => {
            const responsiblePeople = project.responsiblePersons;
            const index = responsiblePeople.map((o: IResponsiblePerson) => o.id).indexOf(responsiblePersonId);
            const responsibilityType = responsiblePeople[index].responsibleType.text;
            elm += `<div class="col-xs-12 works-list-item">${project.latestMetadata.title} - ${responsibilityType
                }</div>`;
        });
        const jqElement = $(elm);
        this.populateResponsiblePersonWorkListItemsTable(jqElement);
    }

    private populateAuthorWorkListItemsTable(tableItems: JQuery) {
        const authorWorkListEl = $(".works-produced").find(".works-list-items");
        authorWorkListEl.append(tableItems);
    }

    private populateResponsiblePersonWorkListItemsTable(tableItems: JQuery) {
        const authorWorkListEl = $(".works-participated").find(".works-list-items");
        authorWorkListEl.append(tableItems);
    }

    private loadProjectsByResponsiblePerson(responsiblePersonId: number) {
        const start = 0;
        const count = 10; //Get first ten works
        const projectInfoAjax = this.projectClient.getProjectsByResponsiblePerson(responsiblePersonId, start, count);
        const worksParticipatedEl = $(".works-participated");
        const tableBodyEl = worksParticipatedEl.find(".works-list-items");
        const numberOfWorksEl = worksParticipatedEl.find(".number-of-works-value");
        tableBodyEl.empty();
        tableBodyEl.addClass("loading");
        projectInfoAjax.done((data: IPagedResult<IProjectDetailContract>) => {
            this.generateWorkResponsiblePersonItem(data.list, responsiblePersonId);
            numberOfWorksEl.text(data.totalCount);
        }).fail(() => {
            tableBodyEl.text(localization.translate("WorksLoadFailed", "Admin").value);
        }).always(() => {
            tableBodyEl.removeClass("loading");
        });
    }

    private capitalize(string: string) {
        return string.replace(/(?:^|\s)\S/g, a => a.toLocaleUpperCase());
    };

    private dateRangeSelected() {
        const notBeforeEl = $("#work-metadata-not-before");
        const notAfterEl = $("#work-metadata-not-after");
        const notBefore = this.rangePeriodViewSliderClass.getLowerValue();
        const notAfter = this.rangePeriodViewSliderClass.getHigherValue();
        notBeforeEl.val(notBefore);
        notAfterEl.val(notAfter);
        this.selectRangeDialog.hide();
    }

    private publicationYearsValid(string: string) {
        const yearsRegex = new RegExp(/(^\d{1,4}$)|(^\d{1,4}(-|–)\d{1,4}$)|(^(\d{1,4},)*\d{1,4}$)/);
        return yearsRegex.test(string);
    }

    private generateBibliography() {
        const bibliographyInputEl = $("#work-metadata-bibl-text");
        const authorEls = $(".author-item");
        const projectTitleEl = $("#work-metadata-title");
        const publishYearsEl = $("#work-metadata-publish-date");
        const publishPlaceEl = $("#work-metadata-publish-place");
        const publisherNameEl = $("#work-metadata-publisher");
        const publishYearsString = publishYearsEl.val() as string;
        const publishPlaceString = publishPlaceEl.val() as string;
        const publisherNameString = publisherNameEl.val() as string;
        const projectTitleString = projectTitleEl.val() as string;
        var authorsStringArray = new Array<string>();
        authorEls.each((index, elem) => {
            const author = $(elem as Node as Element);
            const name = author.find(".person-name").text();
            const surname = author.find(".person-surname").text();
            authorsStringArray.push(`${surname.toLocaleUpperCase()}, ${this.capitalize(name)}`);
        });
        const authorsString = authorsStringArray.join(", "); //TODO check correct bibliography format
        const bibliographyString =
            `${authorsString}. ${projectTitleString}. ${publishPlaceString}: ${publisherNameString}, ${
                publishYearsString}.`;
        bibliographyInputEl.val(bibliographyString);
    }

    private addAuthor() {
        var id: number;
        var firstName: string;
        var lastName: string;

        const finishAddingAuthor = () => {
            var element = MetadataUiHelper.addPerson($("#work-metadata-authors"), firstName, lastName, id);
            element.addClass("author-item");
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
            id = parseInt($("#add-author-id-preview").val() as string);
            firstName = selectedExistingAuthorEl.children(".existing-original-author-name").text();
            lastName = selectedExistingAuthorEl.children(".existing-original-author-surname").text();

            finishAddingAuthor();
        } else {
            this.addAuthorDialog.showError(localization.translate("SelectAuthor", "Admin").value);

        }
    }
    
    private addEditor() {
        var id: string;
        var responsibilityText: string;
        var responsibilityTypeId: number;
        var firstName: string;
        var lastName: string;

        const finishAddingEditor = () => {
            var element = MetadataUiHelper.addPerson($("#work-metadata-editors"),
                firstName,
                `${lastName} - ${responsibilityText}`,
                id,
                responsibilityTypeId);
            element.addClass("editor-item");
            this.addRemovePersonEvent($(".remove-button", element));

            this.addEditorDialog.hide();
        };

        const selectedExistingResponsiblePersonEl = $(".existing-responsible-person-selected");
        if (selectedExistingResponsiblePersonEl.length) {
            id = $("#add-editor-id-preview").val() as string;
            firstName = selectedExistingResponsiblePersonEl.children(".existing-responsible-person-name").text();
            lastName = selectedExistingResponsiblePersonEl.children(".existing-responsible-person-surname").text();
            responsibilityTypeId = $("#add-editor-type").find(":selected").val() as number;
            if (typeof responsibilityTypeId == "undefined") {
                this.addEditorDialog.showError(localization.translate("SelectResponsibleType", "Admin").value);
            } else {
                const responsibilityTextWithParenthesis = $("#add-editor-type").find(":selected").text();
                responsibilityText = responsibilityTextWithParenthesis.replace(/ *\([^)]*\) */g, "");
                $(".responsible-person-list-items").children(".responsible-person-list-item").remove();
                finishAddingEditor();
            }
        } else {
            this.addEditorDialog.showError(localization.translate("SelectResponsiblePerson", "Admin").value);
        }
    }

    private addRemovePersonEvent($removeButton: JQuery) {
        $removeButton.click((event) => {
            $(event.currentTarget).closest(".author-item, .editor-item").remove();
        });
    }

    private saveMetadata() {
        const keywordFailAlertEl = $("#work-metadata-keyword-fail");
        var selectedAuthorIds = new Array<number>();
        var selectedResponsibleIds = new Array<ISaveProjectResponsiblePerson>();
        var selectedKindIds = new Array<number>();
        var selectedGenreIds = new Array<number>();
        var keywordIdList = new Array<number>();

        $("#work-metadata-authors .author-item").each((index, elem: Node) => {
            selectedAuthorIds.push($(elem as Element).data("id"));
        });
        $("#work-metadata-editors .editor-item").each((index, elem: Node) => {
            var projectResponsible: ISaveProjectResponsiblePerson = {
                responsiblePersonId: $(elem as Element).data("id"),
                responsibleTypeId: $(elem as Element).data("responsible-type-id")
            };
            selectedResponsibleIds.push(projectResponsible);
        });
        $(".lit-kind-item").find("select").each((index, elem: Node) => {
            selectedKindIds.push(parseInt($(elem as Element).val() as string));
        });
        $(".genre-item").find("select").each((index, elem: Node) => {
            selectedGenreIds.push(parseInt($(elem as Element).val() as string));
        });
        
        var publisherText = "";
        if (this.publisherName == null) {
            publisherText = $("#work-metadata-publisher").val() as string;
        } else {
            publisherText = this.publisherTypeahead.getInputValue();
        }

        const data: IOnlySaveMetadataResource = {
            authorIdList: selectedAuthorIds,
            projectResponsiblePersonIdList: selectedResponsibleIds
        };
        this.formMetadataObjectAndSendRequest(data, publisherText)
    }

    private formMetadataObjectAndSendRequest(contract: IOnlySaveMetadataResource, publisherText: string) {
        var data: ISaveMetadataResource = {
            biblText: $("#work-metadata-bibl-text").val() as string,
            copyright: $("#work-metadata-copyright").val() as string,
            manuscriptCountry: $("#work-metadata-original-country").val() as string,
            manuscriptExtent: $("#work-metadata-original-extent").val() as string,
            manuscriptIdno: $("#work-metadata-original-idno").val() as string,
            manuscriptRepository: $("#work-metadata-original-repository").val() as string,
            manuscriptSettlement: $("#work-metadata-original-settlement").val() as string,
            notAfter: $("#work-metadata-not-after").val() as string,
            notBefore: $("#work-metadata-not-before").val() as string,
            originDate: $("#work-metadata-origin-date").val() as string,
            publishDate: $("#work-metadata-publish-date").val() as string,
            publishPlace: $("#work-metadata-publish-place").val() as string,
            publisherEmail: $("#work-metadata-publisher-email").val() as string,
            publisherText: publisherText,
            relicAbbreviation: $("#work-metadata-relic-abbreviation").val() as string,
            sourceAbbreviation: $("#work-metadata-source-abbreviation").val() as string,
            subTitle: $("#work-metadata-subtitle").val() as string,
            title: $("#work-metadata-title").val() as string,
            authorIdList: contract.authorIdList,
            projectResponsiblePersonIdList: contract.projectResponsiblePersonIdList
        };
        var $loadingGlyph = $("#work-metadata-save-button .saving-icon");
        var $buttons = $("#work-metadata-editor-button-panel button");
        var $successAlert = $("#work-metadata-save-success");
        var $errorAlert = $("#work-metadata-save-error");
        $loadingGlyph.show();
        $buttons.prop("disabled", true);
        $successAlert.finish().hide();
        $errorAlert.hide();
        this.projectClient.saveMetadata(this.projectId, data).done((responseData) => {
            $successAlert.show().delay(3000).fadeOut(2000);
            $("#work-metadata-last-modification").text(responseData.lastModificationText.toLocaleString());
            $("#work-metadata-literary-original").text(responseData.literaryOriginalText);
        }).fail(() => {
            $errorAlert.show();
        }).always(() => {
            $loadingGlyph.hide();
            $buttons.prop("disabled", false);
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

class ProjectWorkCategorizationTab extends ProjectMetadataTabBase {
    private projectId: number;
    private projectClient: ProjectClient;
    private existingGenres: JQuery = null;
    private existingLitKinds: JQuery = null;
    private workModule: ProjectWorkModule;
    private adminApiClient = new AdminApiClient();

    constructor(projectId: number, workModule: ProjectWorkModule) {
        super();
        this.projectId = projectId;
        this.projectClient = new ProjectClient();
        this.workModule = workModule;
    }

    getConfiguration(): IProjectMetadataTabConfiguration {
        return {
            $panel: $("#work-categorization-container"),
            $viewButtonPanel: $("#work-categorization-view-button-panel"),
            $editorButtonPanel: $("#work-categorization-editor-button-panel")
        };
    }

    private initKeywords() {
        const count = 10;
        const selectedKeywordEls = $(".keywords-list-selected").children();
        const engine = new Bloodhound({
            datumTokenizer: (d: any) => Bloodhound.tokenizers.whitespace(d.label),
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: {
                url: `${getBaseUrl()}Admin/Project/KeywordTypeahead?keyword=%QUERY&count=${count}`,
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
        ($(".keywords-textarea") as any).tokenfield({
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
            const jEl = $(element as Node as Element);
            tags.push({ value: jEl.data("id"), label: jEl.data("name") });
        });
        ($(".keywords-textarea") as any).tokenfield("setTokens", tags);
    }

    private returnUniqueElsArray(array: any[]) {
        var seen = {};
        return array.filter(item => seen.hasOwnProperty(item) ? false : (seen[item] = true));
    }

    private categoryTree: any; //TODO investigate d ts

    private parseExistingGenres() {
        const genresEl = $("#all-existing-genres");
        const genreEls = genresEl.children(".existing-genre");
        this.existingGenres = genreEls;
    }

    private parseExistingKinds() {
        const kindsEl = $("#all-existing-lit-kinds");
        const kindEls = kindsEl.children(".existing-kind");
        this.existingLitKinds = kindEls;
    }

    private createGenreKindSelectBoxEl(items: JQuery, isGenre: boolean) {
        var elm = "";
        if (isGenre) {
            elm += `<div class="genre-item clearfix">`;
        } else {
            elm += `<div class="lit-kind-item clearfix">`;
        }
        elm += `<div class="col-xs-9">`;
        elm += `<select class="item-select-box form-control">`;
        items.each((index, elem) => {
            const genreEl = $(elem);
            elm += `<option value="${genreEl.data("id")}">${genreEl.data("name")}</option>`;
        });
        elm += "</select>";
        elm += "</div>";
        elm += `<div class="button-float-right">`;
        elm +=
            `<button class="btn btn-default remove-button"><span class="glyphicon glyphicon-remove"></span></button>`;
        elm += "</div>";
        elm += "</div>";
        return $.parseHTML(elm);
    }

    private createCategoriesNestedStructure() {
        const existingCategoriesEl = $(".all-category-list");
        const existingCategoriesElList = existingCategoriesEl.children(".existing-category-list-item");
        existingCategoriesElList.each((index, elem) => {
            const categoryEl = $(elem as Node as Element);
            const id = categoryEl.data("category-id");
            const childCategories = $(`[data-parent-category-id=${id}]`);
            if (childCategories.length) {
                childCategories.addClass("child-category").detach();
                categoryEl.addClass("parent-category").append(childCategories);
            }
        });
    }

    private checkSelectedCategoriesInTree(categoryTree: any) {
        const selectedCategoriesEl = $(".selected-category-list");
        const selectedCategoriesElList = selectedCategoriesEl.children(".selected-category-list-item");
        if (selectedCategoriesElList.length) {
            selectedCategoriesElList.each((index, elem) => {
                const categoryEl = $(elem as Node as Element);
                const catId = categoryEl.data("category-id");
                const catChechbox = categoryTree.getNodeById(catId);
                if (catChechbox) {
                    categoryTree.check(catChechbox);
                }
            });
        }
    }

    private convertCategoryArrayToCategoryTreeObject() {
        const categoryTreeObject = new Array<ICategoryTreeContract>();
        const existingCategoriesEl = $(".all-category-list");
        const existingCategoriesElList = existingCategoriesEl.children(".existing-category-list-item");
        existingCategoriesElList.each((index, elem) => {
            const catEl = $(elem as Node as HTMLElement);
            var categoryTreeEl = this.convertCateroryElToCategoryTreeEl(catEl);
            categoryTreeObject.push(categoryTreeEl);
        });
        return categoryTreeObject;
    }

    private convertCateroryElToCategoryTreeEl(categoryEl: JQuery) {
        var categoryTreeEl = <ICategoryTreeContract>{};
        categoryTreeEl.id = categoryEl.data("category-id");
        categoryTreeEl.text = categoryEl.data("category-description");
        if (categoryEl.hasClass("parent-category")) {
            const childrenCats = categoryEl.children(".child-category");
            categoryTreeEl.children = Array<ICategoryTreeContract>();
            childrenCats.each((index, elem) => {
                const childCat = $(elem);
                categoryTreeEl.children.push(this.convertCateroryElToCategoryTreeEl(childCat));
            });
        }
        return categoryTreeEl;
    }

    initTab(): void {
        super.initTab();
        this.initKeywords();

        this.createCategoriesNestedStructure();

        const categoryTreeElement = $("#category-tree");
        this.categoryTree = (categoryTreeElement as any).tree({
            primaryKey: "id",
            uiLibrary: "bootstrap",
            checkedField: "categorySelected",
            dataSource: this.convertCategoryArrayToCategoryTreeObject(),
            checkboxes: true,
            cascadeCheck: false
        });

        this.checkSelectedCategoriesInTree(this.categoryTree);

        categoryTreeElement.find("input").prop("disabled", true);
        const categoryTreeLabels = categoryTreeElement.find(".list-group-item span[data-role=\"display\"]");
        categoryTreeLabels.addClass("disabled");
        categoryTreeLabels.click((event) => {
            if (!$(event.currentTarget).hasClass("disabled")) {
                const node = $(event.currentTarget).parent("div").parent(".list-group-item");
                if (node.find("input").prop("checked")) {
                    this.categoryTree.uncheck(node);
                } else {
                    this.categoryTree.check(node);
                }
                
            }
        });


        $("#work-categorization-edit-button").click(() => {
            this.enabledEdit();
            categoryTreeElement.children("input").prop("disabled", false);
            categoryTreeLabels.removeClass("disabled");
        });

        $("#work-categorization-cancel-button").click(() => {
            this.disableEdit();
            const metadataTabSelector = "#project-work-categorization";
            var tabPanelEl = $(metadataTabSelector);
            tabPanelEl.empty();
            this.workModule.loadTabPanel(metadataTabSelector);
            categoryTreeElement.children("input").prop("disabled", true);
            categoryTreeLabels.addClass("disabled");
        });

        $("#add-literary-kind-button").click(() => {
            const kindSelectsEl = $("#work-categorization-literary-kind");
            if (this.existingLitKinds == null) {
                this.parseExistingKinds();
            }
            const isGenre = false;
            const selectBox = this.createGenreKindSelectBoxEl(this.existingLitKinds, isGenre);
            kindSelectsEl.append(selectBox);
        });

        $("#add-literary-genre-button").click(() => {
            const genreSelectsEl = $("#work-categorization-literary-genre");
            if (this.existingGenres == null) {
                this.parseExistingGenres();
            }
            const isGenre = true;
            const selectBox = this.createGenreKindSelectBoxEl(this.existingGenres, isGenre);
            genreSelectsEl.append(selectBox);
        });

        this.addRemoveGenreEvent();

        this.addRemoveLiteraryKindEvent();

        var $saveButton = $("#work-categorization-save-button");
        $saveButton.click(() => {
            this.saveMetadata();
        });
        $(".saving-icon", $saveButton).hide();

        $("#work-categorization-save-error, #work-categorization-save-success").hide();
    }

    private addRemoveGenreEvent() {
        $("#work-categorization-literary-genre").on("click",
            ".remove-button",
            (event) => {
                $(event.currentTarget as Node as Element).closest(".genre-item").remove();
            });
    }

    private addRemoveLiteraryKindEvent() {
        $("#work-categorization-literary-kind").on("click",
            ".remove-button",
            (event) => {
                $(event.currentTarget as Node as Element).closest(".lit-kind-item").remove();
            });
    }

    private saveMetadata() {
        const keywordFailAlertEl = $("#work-categorization-keyword-fail");
        var selectedKindIds = new Array<number>();
        var selectedGenreIds = new Array<number>();
        var keywordIdList = new Array<number>();

        $(".lit-kind-item").find("select").each((index, elem: Node) => {
            selectedKindIds.push(parseInt($(elem as Element).val() as string));
        });
        $(".genre-item").find("select").each((index, elem: Node) => {
            selectedGenreIds.push(parseInt($(elem as Element).val() as string));
        });

        const keywordsInputEl = $(".keywords-container").children(".tokenfield").children(".keywords-textarea");
        const keywordsArray = $.map((keywordsInputEl.val() as string).split(","), $.trim);
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

        this.adminApiClient.createNewKeywordsByArray(keywordNonIdList).done((newIds: number[]) => {
            const allKeywordIds = keywordIdList.concat(newIds);
            const data: IOnlySaveCategorization = {
                keywordIdList: allKeywordIds,
                categoryIdList: this.categoryTree.getCheckedNodes(),
                literaryKindIdList: selectedKindIds,
                literaryGenreIdList: selectedGenreIds
            };
            this.formCategorizationObjectAndSendRequest(data);
        }).fail(() => {
            keywordFailAlertEl.show().delay(3000).fadeOut(2000);
            const data: IOnlySaveCategorization = {
                keywordIdList: keywordIdList,
                categoryIdList: this.categoryTree.getCheckedNodes(),
                literaryKindIdList: selectedKindIds,
                literaryGenreIdList: selectedGenreIds
            };
            this.formCategorizationObjectAndSendRequest(data);
        });
    }

    private formCategorizationObjectAndSendRequest(contract: IOnlySaveCategorization) {
        var data: ISaveCategorization = {
            categoryIdList: contract.categoryIdList,
            keywordIdList: contract.keywordIdList,
            literaryGenreIdList: contract.literaryGenreIdList,
            literaryKindIdList: contract.literaryKindIdList
        };
        var $loadingGlyph = $("#work-categorization-save-button .saving-icon");
        var $buttons = $("#work-categorization-editor-button-panel button");
        var $successAlert = $("#work-categorization-save-success");
        var $errorAlert = $("#work-categorization-save-error");
        $loadingGlyph.show();
        $buttons.prop("disabled", true);
        $successAlert.finish().hide();
        $errorAlert.hide();
        this.projectClient.saveCategorization(this.projectId, data).done(() => {
            $successAlert.show().delay(3000).fadeOut(2000);
        }).fail(() => {
            $errorAlert.show();
        }).always(() => {
            $loadingGlyph.hide();
            $buttons.prop("disabled", false);
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
        var snapshotList = new SnapshotList(this.projectId);
        snapshotList.init();
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

class ProjectWorkForumTab extends ProjectModuleTabBase {
    private readonly projectId: number;
    private readonly projectClient: ProjectClient;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
        this.projectClient = new ProjectClient();
    }

    initTab() {
        var $saveButton = $("#forum-repair-button");
        $saveButton.click(() => {
            this.repairForum();
        });
        $(".saving-icon", $saveButton).hide();

        $("#forum-repair-error, #forum-repair-success").hide();
    }

    private repairForum() {
        var $loadingGlyph = $("#forum-repair-button .saving-icon");
        var $buttons = $("#forum-repair-button");
        var $successAlert = $("#forum-repair-success");
        var $errorAlert = $("#forum-repair-error");
        var $repairBlock = $("#forum-repair-block");
        $loadingGlyph.show();
        $buttons.prop("disabled", true);
        $successAlert.finish().hide();
        $errorAlert.hide();
        this.projectClient.createForum(this.projectId).done((data) => {
            $successAlert.show().delay(1500);
            $("#forum-name").text(data.name);
            $("#forum-url").html("<a href=\""+data.url+"\" >"+data.url+"</a>");
            $repairBlock.delay(5000).fadeOut(1500);
        }).fail(() => {
            $errorAlert.show();
            $buttons.prop("disabled", false);
        }).always(() => {
            $loadingGlyph.hide();
        });
    }
}

class MetadataUiHelper {
    static addPerson($container: JQuery<HTMLDivElement>,
        name: string,
        surname: string,
        idValue: string | number,
        responsibilityTypeId?: number | string): JQuery {
        const rootElement = $(document.createElement("div"));
        const deleteButton = $(document.createElement("button"));
        const movePersonArrows = $(document.createElement("span"));
        const listControls = $(document.createElement("span"));
        const deleteGlyphSpan = $(document.createElement("span"));
        const labelSpan = $(document.createElement("span"));

        deleteButton
            .addClass("btn btn-default remove-button")
            .append(deleteGlyphSpan);
        deleteGlyphSpan
            .addClass("glyphicon glyphicon-remove");
        movePersonArrows.addClass("btn-group-vertical move-button").append(
            `<button type="button" class="btn btn-default move-person-up">
                 <i class="fa fa-chevron-up" aria-hidden="true"></i>
             </button><button type="button" class="btn btn-default move-person-down">
                          <i class="fa fa-chevron-down" aria-hidden="true"></i>
                      </button>`);
        listControls.addClass("person-list-manipulate").append(movePersonArrows).append(deleteButton);
        labelSpan
            .addClass("text-as-form-control")
            .append(`<span class="person-name">${name}</span>`)
            .append(`<span class="person-surname">${surname}</span>`);
        if (responsibilityTypeId != null) {
            rootElement.attr("data-responsible-type-id", responsibilityTypeId);
        }
        rootElement
            .attr("data-id", idValue)
            .append(listControls)
            .append(labelSpan)
            .appendTo($container);

        return rootElement;
    }
}