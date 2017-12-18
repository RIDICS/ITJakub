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
    private existingGenres: JQuery = null;
    private existingLitKinds: JQuery = null;

    constructor(projectId: number) {
        super();
        this.projectId = projectId;
        this.projectClient = new ProjectClient();

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
        elm += `<select class="item-select-box">`;
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
            const categoryEl = $(elem);
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
        selectedCategoriesElList.each((index, elem) => {
            const categoryEl = $(elem);
            const catId = categoryEl.data("category-id");
            const catChechbox = categoryTree.getNodeById(catId);
            categoryTree.check(catChechbox);
        });
    }

    private convertCategoryArrayToCategoryTreeObject() {
        const categoryTreeObject = new Array<ICategoryTreeContract>();
        const existingCategoriesEl = $(".all-category-list");
        const existingCategoriesElList = existingCategoriesEl.children(".existing-category-list-item");
        existingCategoriesElList.each((index, elem) => {
            const catEl = $(elem);
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
        var $addResponsibleTypeButton = $("#add-responsible-type-button");
        var $addResponsibleTypeContainer = $("#add-responsible-type-container");

        this.rangePeriodViewSliderClass = new RegExDatingConditionRangePeriodView();
        this.rangePeriodViewSliderClass.makeRangeView(
            $("#select-range-dialog").find(".regex-dating-precision-div")[0] as HTMLDivElement);

        this.createCategoriesNestedStructure();

        this.categoryTree = $("#category-tree").tree({
            primaryKey: "id",
            uiLibrary: "bootstrap",
            checkedField: "categorySelected",
            dataSource: this.convertCategoryArrayToCategoryTreeObject(),
            checkboxes: true,
            cascadeCheck: false
        });

        this.checkSelectedCategoriesInTree(this.categoryTree);

        $("#work-metadata-publisher-email").on("input",
            () => {
                const enteredText = $("#work-metadata-publisher-email").val();
                const emailIsValid = this.validateEmail(enteredText);
                const emailGroupEl = $(".email-group");
                const iconEl = emailGroupEl.find(".form-control-feedback");
                iconEl.show();
                emailGroupEl.addClass("has-feedback");
                if (emailIsValid) {
                    emailGroupEl.addClass("has-success");
                    emailGroupEl.removeClass("has-error");
                    iconEl.addClass("glyphicon-ok");
                    iconEl.removeClass("glyphicon-remove");
                } else {
                    emailGroupEl.addClass("has-error");
                    emailGroupEl.removeClass("has-success");
                    iconEl.addClass("glyphicon-remove");
                    iconEl.removeClass("glyphicon-ok");
                }
            });

        $("#work-metadata-publish-date").on("input",
            () => {
                const enteredText = $("#work-metadata-publish-date").val();
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

        $(document).on("click", ".move-person-up",
            (event) => {
                const targetEl = $(event.target);
                const personEl = targetEl.parents(".editor-item, .author-item");
                const prevPersonEl = personEl.prev(".editor-item, .author-item");
                if (prevPersonEl.length) {
                    personEl.detach();
                    prevPersonEl.before(personEl);
                }
            });

        $(document).on("click", ".move-person-down", 
            (event) => {
                const targetEl = $(event.target);
                const personEl = targetEl.parents(".editor-item, .author-item");
                const nextPersonEl = personEl.next(".editor-item, .author-item");
                if (nextPersonEl.length) {
                    personEl.detach();
                    nextPersonEl.after(personEl);
                }
            });

        $("#category-tree").find("input").prop("disabled", true);

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
            $("#category-tree").children("input").prop("disabled", false);
        });

        $("#work-metadata-cancel-button").click(() => {
            this.disableEdit();
            this.publisherTypeahead.destroy();
            $("#category-tree").children("input").prop("disabled", true);
        });

        $("#add-literary-kind-button").click(() => {
            const kindSelectsEl = $("#work-metadata-literary-kind");
            if (this.existingLitKinds == null) {
                this.parseExistingKinds();
            }
            const isGenre = false;
            const selectBox = this.createGenreKindSelectBoxEl(this.existingLitKinds, isGenre);
            kindSelectsEl.append(selectBox);
        });

        $("#add-literary-genre-button").click(() => {
            const genreSelectsEl = $("#work-metadata-literary-genre");
            if (this.existingGenres == null) {
                this.parseExistingGenres();
            }
            const isGenre = true;
            const selectBox = this.createGenreKindSelectBoxEl(this.existingGenres, isGenre);
            genreSelectsEl.append(selectBox);
        });

        $("#add-author-button").click(() => {
            this.addAuthorDialog.show();
        });

        $(".edit-date-range").click(() => {
            this.selectRangeDialog.show();
        });

        $("#add-editor-button").click(() => {
            $addResponsibleTypeButton.prop("disabled", false);
            $addResponsibleTypeContainer.hide();
            this.addEditorDialog.show();
        });

        $(".generate-bibliography-button").on("click",
            () => {
                this.generateBibliography();
            });

        $(".new-original-author-button").on("click",
            () => {
                const finishAddingResponsiblePersonButton = $(".new-original-author-finish-button");
                $("#add-author-search").prop("disabled", true);
                $(".new-original-author-button").prop("disabled", true);
                $(".author-name-input-row").show();
                finishAddingResponsiblePersonButton.show();
            });

        $(".new-original-author-finish-button").on("click",
            () => {
                const firstNameEl = $("#add-author-first-name");
                const lastNameEl = $("#add-author-last-name");
                const listItemsEl = $(".author-list-items");
                var firstName = firstNameEl.val();
                var lastName = lastNameEl.val();
                var id: number;
                if (firstName === "" || lastName === "") {
                    this.addAuthorDialog.showError("Please enter a name and surname");
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
                    $(".new-original-author-finish-button").hide();
                }).fail(() => {
                    this.addAuthorDialog.showError();
                });
            });

        $(".new-responsible-person-button").on("click",
            () => {
                const finishAddingResponsiblePersonButton = $(".new-responsible-person-finish-button");
                $(".new-responsible-person-button").prop("disabled", true);
                $("#add-editor-search").prop("disabled", true);
                $(".responsible-person-name-input-row").show();
                finishAddingResponsiblePersonButton.show();

            });

        $(".new-responsible-person-finish-button").on("click",
            () => {
                const listItemsEl = $(".responsible-person-list-items");
                const firstNameEl = $("#add-responsible-person-first-name");
                const lastNameEl = $("#add-responsible-person-last-name");
                const firstName = firstNameEl.val();
                const lastName = lastNameEl.val();
                var id: number;
                if (firstName === "" || lastName === "") {
                    this.addEditorDialog.showError("Please enter a name and surname");
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
                        $(".new-responsible-person-finish-button").hide();
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
                $(".new-original-author-finish-button").hide();
            });

        const $authorId = $("#add-author-id-preview");

        $(".author-list-items").on("click",
            ".author-list-item",
            (event: Event) => {
                var targetEl = $(event.target);
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
            (event: Event) => {
                const textAreaEl = $(event.target);
                const enteredText = textAreaEl.val();
                if (enteredText === "") {
                    $(".author-list-item").remove();
                    $authorId.val("");
                    this.selectedAuthorId = null;
                    return;
                }
                $.get(`${getBaseUrl()}Admin/Project/GetTypeaheadOriginalAuthor?query=${enteredText}`).done(
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
            (event: Event) => {
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
            (event: Event) => {
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
                $(".new-responsible-person-finish-button").hide();
            });

        $addResponsibleTypeButton.click(() => {
            $addResponsibleTypeButton.prop("disabled", true);
            $("#responsibility-type-input-elements").hide();
            $addResponsibleTypeContainer.show();
            $("#add-responsible-type-saving-icon").hide();
        });

        $("#add-responsible-type-save").click(this.createResponsibleType.bind(this));

        this.addRemovePersonEvent($("#work-metadata-authors .remove-button, #work-metadata-editors .remove-button"));

        this.addRemoveGenreEvent();

        this.addRemoveLiteraryKindEvent();

        var $saveButton = $("#work-metadata-save-button");
        $saveButton.click(() => {
            this.saveMetadata();
        });
        $(".saving-icon", $saveButton).hide();

        $("#work-metadata-save-error, #work-metadata-save-success").hide();
    }

    private loadProjectsByAuthor(authorId: number) {
        const start = 0; //TODO debug
        const count = 10; //TODO debug
        const projectInfoAjax = this.projectClient.getProjectsByAuthor(authorId, start, count);
        const tableBodyEl = $(".works-produced").find(".works-list-items");
        tableBodyEl.empty();
        tableBodyEl.addClass("loading");
        projectInfoAjax.done((data: IProjectDetailContract[]) => {
            this.generateWorkAuthorTableItem(data);
        }).fail(() => {
            tableBodyEl.text("Error loading works");
        }).always(() => {
            tableBodyEl.removeClass("loading");
        });
    }

    private generateWorkAuthorTableItem(projects: IProjectDetailContract[]) {
        var elm = "";
        projects.forEach((project) => {
            elm += `<div class="col-xs-12 works-list-item">${project.latestMetadata.title}</div>`;
        });
        const html = $.parseHTML(elm);
        this.populateAuthorWorkListItemsTable($(html));
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
        const html = $.parseHTML(elm);
        this.populateResponsiblePersonWorkListItemsTable($(html));
    }

    private populateAuthorWorkListItemsTable(tableItems: JQuery) {
        const authorWorkListEl = $(".works-produced").find(".works-list-items");
        authorWorkListEl.append(tableItems);
    }

    private populateResponsiblePersonWorkListItemsTable(tableItems: JQuery) { //TODO
        const authorWorkListEl = $(".works-participated").find(".works-list-items");
        authorWorkListEl.append(tableItems);
    }

    private loadProjectsByResponsiblePerson(responsiblePersonId: number) {
        const start = 0; //TODO debug
        const count = 10; //TODO debug
        const projectInfoAjax = this.projectClient.getProjectsByResponsiblePerson(responsiblePersonId, start, count);
        const tableBodyEl = $(".works-participated").find(".works-list-items");
        tableBodyEl.empty();
        tableBodyEl.addClass("loading");
        projectInfoAjax.done((data: IProjectDetailContract[]) => {
            this.generateWorkResponsiblePersonItem(data, responsiblePersonId);
        }).fail(() => {
            tableBodyEl.text("Error loading works");
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
        const yearsRegex = new RegExp(/(^\d{1,4}$)|(^\d{1,4}-\d{1,4}$)|(^(\d{1,4},)*\d{1,4}$)/);
        return yearsRegex.test(string);
    }

    private generateBibliography() {
        const bibliographyInputEl = $("#work-metadata-bibl-text");
        const authorEls = $(".author-item");
        const projectTitleEl = $("#work-metadata-title");
        const publishYearsEl = $("#work-metadata-publish-date");
        const publishPlaceEl = $("#work-metadata-publish-place");
        const publisherNameEl = $("#work-metadata-publisher");
        const publishYearsString = publishYearsEl.val();
        const publishPlaceString = publishPlaceEl.val();
        const publisherNameString = publisherNameEl.val();
        const projectTitleString = projectTitleEl.val();
        var authorsStringArray = new Array<string>();
        authorEls.each((index, elem) => {
            const author = $(elem);
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
            id = $("#add-author-id-preview").val();
            firstName = selectedExistingAuthorEl.children(".existing-original-author-name").text();
            lastName = selectedExistingAuthorEl.children(".existing-original-author-surname").text();

            finishAddingAuthor();
        } else {
            this.addAuthorDialog.showError("Please select one author from list");

        }
    }

    private createResponsibleType() {
        $("#responsibility-type-input-elements").show();
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

        const finishAddingEditor = () => {
            var element = MetadataUiHelper.addPerson($("#work-metadata-editors"),
                firstName, `${lastName} - ${responsibilityText}`,
                id,
                responsibilityTypeId);
            element.addClass("editor-item");
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
            $(".responsible-person-list-items").children(".responsible-person-list-item").remove();
            finishAddingEditor();
        } else {
            this.addEditorDialog.showError("Please select one responsible person from list");
        }
    }

    private addRemovePersonEvent($removeButton: JQuery) {
        $removeButton.click((event) => {
            $(event.currentTarget).closest(".author-item, .editor-item").remove();
        });
    }

    private addRemoveGenreEvent() {
        $("#work-metadata-literary-genre").on("click",
            ".remove-button",
            (event) => {
                $(event.currentTarget).closest(".genre-item").remove();
            });
    }

    private addRemoveLiteraryKindEvent() {
        $("#work-metadata-literary-kind").on("click",
            ".remove-button",
            (event) => {
                $(event.currentTarget).closest(".lit-kind-item").remove();
            });
    }

    private validateEmail(mail: string) {
        const emailRegex = new RegExp(/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/);
        if (emailRegex.test(mail)) {
            return true;
        }
        return false;
    }

    private saveMetadata() {
        const keywordFailAlertEl = $("#work-metadata-keyword-fail");
        var selectedAuthorIds = new Array<number>();
        var selectedResponsibleIds = new Array<ISaveProjectResponsiblePerson>();
        var selectedKindIds = new Array<number>();
        var selectedGenreIds = new Array<number>();
        var keywordIdList = new Array<number>();

        $("#work-metadata-authors .author-item").each((index, elem) => {
            selectedAuthorIds.push($(elem).data("id"));
        });
        $("#work-metadata-editors .editor-item").each((index, elem) => {
            var projectResponsible: ISaveProjectResponsiblePerson = {
                responsiblePersonId: $(elem).data("id"),
                responsibleTypeId: $(elem).data("responsible-type-id")
            };
            selectedResponsibleIds.push(projectResponsible);
        });
        $(".lit-kind-item").find("select").each((index, elem) => {
            selectedKindIds.push($(elem).val());
        });
        $(".genre-item").find("select").each((index, elem) => {
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
        var publisherText = "";
        if (this.publisherName == null) {
            publisherText = $("#work-metadata-publisher").val();
        } else {
            publisherText = this.publisherTypeahead.getInputValue();
        }
        createNewKeywordAjax.done((newIds: number[]) => {
            const allKeywordIds = keywordIdList.concat(newIds);
            const data: IOnlySaveMetadataResource = {
                keywordIdList: allKeywordIds,
                categoryIdList: this.categoryTree.getCheckedNodes(),
                literaryKindIdList: selectedKindIds,
                literaryGenreIdList: selectedGenreIds,
                authorIdList: selectedAuthorIds,
                projectResponsiblePersonIdList: selectedResponsibleIds
            };
            this.formMetadataObjectAndSendRequest(data, publisherText);
        }).fail(() => {
            keywordFailAlertEl.show().delay(3000).fadeOut(2000);
            const data: IOnlySaveMetadataResource = {
                keywordIdList: keywordIdList,
                categoryIdList: this.categoryTree.getCheckedNodes(),
                literaryKindIdList: selectedKindIds,
                literaryGenreIdList: selectedGenreIds,
                authorIdList: selectedAuthorIds,
                projectResponsiblePersonIdList: selectedResponsibleIds
            };
            this.formMetadataObjectAndSendRequest(data, publisherText);
        });
    }

    private formMetadataObjectAndSendRequest(contract: IOnlySaveMetadataResource, publisherText: string) {
        var data: ISaveMetadataResource = {
            categoryIdList: contract.categoryIdList,
            keywordIdList: contract.keywordIdList,
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
            publisherEmail: $("#work-metadata-publisher-email").val(),
            publisherText: publisherText,
            relicAbbreviation: $("#work-metadata-relic-abbreviation").val(),
            sourceAbbreviation: $("#work-metadata-source-abbreviation").val(),
            subTitle: $("#work-metadata-subtitle").val(),
            title: $("#work-metadata-title").val(),
            authorIdList: contract.authorIdList,
            literaryGenreIdList: contract.literaryGenreIdList,
            literaryKindIdList: contract.literaryKindIdList,
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

        this.projectClient.saveMetadata(this.projectId, data).done((data) => {
            $successAlert.show().delay(3000).fadeOut(2000);
            $("#work-metadata-last-modification").text(data.lastModificationText);
            $("#work-metadata-literary-original").text(data.literaryOriginalText);
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
    static addPerson($container: JQuery,
        name: string, surname: string,
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