function initLemmatization(tokenId: string) {
    var lemmatization = new Lemmatization("#mainContainer");
    lemmatization.make();

    if (tokenId)
        lemmatization.load(Number(tokenId));
}

class Lemmatization {
    private mainContainer: string;
    private searchBox: LemmatizationSearchBox;
    private lemmatizationCharacteristic: LemmatizationCharacteristicEditor;
    private currentTokenItem: IToken;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.searchBox = new LemmatizationSearchBox("#mainSearchInput");
        this.lemmatizationCharacteristic = new LemmatizationCharacteristicEditor();
    }

    public make() {
        $(this.mainContainer).empty();
        this.searchBox.setDataSet("Token");
        this.searchBox.create((selectedExists: boolean, selectionConfirmed: boolean) => {
            if (selectedExists || this.searchBox.getInputValue() === "") {
                $("#addNewTokenButton").addClass("hidden");
                $("#loadButton").removeClass("hidden");
            } else {
                $("#addNewTokenButton").removeClass("hidden");
                $("#loadButton").addClass("hidden");
            }

            if (selectionConfirmed) {
                this.loadToken(<IToken>this.searchBox.getValue());
            }
        });
        this.lemmatizationCharacteristic.init();
        LemmatizationCanonicalForm.init();

        $("#loadButton").click(() => {
            var tokenItem = <IToken>this.searchBox.getValue();
            this.loadToken(tokenItem);
        });

        $("#addNewTokenButton").click(() => {
            this.showAddNewToken();
        });

        $("#addNewCharacteristic").click(() => {
            this.lemmatizationCharacteristic.show(this.currentTokenItem.Id, (newTokenCharacteristic) => {
                this.addNewCharacteristic(newTokenCharacteristic);
            });
        });

        $("#save-token").click(() => {
            this.saveNewToken();
        });

        $("#showEditToken").click(() => {
            $("#edit-token-text").val(this.currentTokenItem.Text);
            $("#edit-token-description").val(this.currentTokenItem.Description);
            $("#editTokenDialog").modal({
                show: true,
                backdrop: "static"
            });
        });

        $("#save-edited-token").click(() => {
            this.saveEditedToken();
        });
    }

    public load(tokenId: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/GetToken",
            data: {
                tokenId: tokenId
            },
            dataType: "json",
            contentType: "application/json",
            success: (token) => {
                this.loadToken(token);
                this.searchBox.setValue(token.Text);
            }
        });
    }

    private loadToken(tokenItem: IToken) {
        this.currentTokenItem = tokenItem;
        $(".content").removeClass("hidden");
        $("#specificToken").text(tokenItem.Text);
        $("#specificTokenDescription").text(tokenItem.Description);

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/GetTokenCharacteristic",
            data: {
                tokenId: tokenItem.Id
            },
            dataType: "json",
            contentType: "application/json",
            success: (list) => {
                this.loadCharacteristics(list);
            }
        });
    }

    private loadCharacteristics(list: Array<ITokenCharacteristic>) {
        $(this.mainContainer).empty();

        for (var i = 0; i < list.length; i++) {
            var containerDiv = document.createElement("div");
            var characteristicItem = list[i];
            var characteristicTable = new LemmatizationCharacteristicTable(this.lemmatizationCharacteristic, characteristicItem, containerDiv);
            characteristicTable.make();

            $(this.mainContainer).append(containerDiv);
        }
    }

    private addNewCharacteristic(item: ITokenCharacteristic) {
        var containerDiv = document.createElement("div");
        var characteristicTable = new LemmatizationCharacteristicTable(this.lemmatizationCharacteristic, item, containerDiv);
        characteristicTable.make();

        $(this.mainContainer).append(containerDiv);
    }

    private showAddNewToken() {
        var tokenName = this.searchBox.getInputValue();
        $("#new-token").val(tokenName);
        $("#new-token-description").val("");
        
        $("#newTokenDialog").modal({
            show: true,
            backdrop: "static"
        });
    }

    private saveNewToken() {
        var token = $("#new-token").val();
        var description = $("#new-token-description").val();

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/CreateToken",
            data: JSON.stringify({
                token: token,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: (newTokenId) => {
                $("#newTokenDialog").modal("hide");
                $("#addNewTokenButton").addClass("hidden");
                this.searchBox.reload();

                var tokenItem: IToken = {
                    Id: newTokenId,
                    Text: token,
                    Description: description
                };
                this.loadToken(tokenItem);
            }
        });
    }

    private saveEditedToken() {
        var description = $("#edit-token-description").val();

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/EditToken",
            data: JSON.stringify({
                tokenId: this.currentTokenItem.Id,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                $("#editTokenDialog").modal("hide");
                $("#specificTokenDescription").text(description);
                this.currentTokenItem.Description = description;
            }
        });
    }
}

class LemmatizationCharacteristicEditor {
    private currentValue: string;
    private tokenId: number;
    private tokenCharacteristic: ITokenCharacteristic;
    private itemSavedCallback: (newTokenCharacteristic: ITokenCharacteristic) => void;

    init() {
        $("#newTokenCharacteristic select").on("change", () => {
            this.updateTag();
        });

        $("#saveCharacteristic").click(() => {
            if (this.tokenCharacteristic)
                this.saveEdit();
            else
                this.saveNew();
        });
    }

    private updateTag() {
        this.currentValue = this.getNewValue();
        $("#result-characteristic-tag").text("Tag=\"" + this.currentValue + "\"");
    }

    private getNewValue(): string {
        var selects = $("#newTokenCharacteristic select");
        var result = "";

        for (var i = 0; i < selects.length; i++) {
            var value = selects.filter("#description-" + i).val();
            result += value;
        }

        return result;
    }

    private clear() {
        $("#newTokenCharacteristic select").each((index: number, element: HTMLSelectElement) => {
            element.selectedIndex = 0;
        });
        $("#new-token-text-description").val("");
        this.updateTag();
    }

    private loadData(tokenCharacteristic: ITokenCharacteristic) {
        var characteristic = tokenCharacteristic.MorphologicalCharacteristic;
        $("#new-token-text-description").val(tokenCharacteristic.Description);
        $("#newTokenCharacteristic select").each((index: number, element: HTMLSelectElement) => {
            element.selectedIndex = 0;
            var charValue = characteristic.charAt(index);
            for (var i = 0; i < element.children.length; i++) {
                var option = element.children[i];
                var value = $(option).attr("value");
                if (value === charValue) {
                    element.selectedIndex = i;
                    break;
                }
            }
        });
        this.updateTag();
    }

    getValue(): string {
        return this.currentValue;
    }

    show(tokenId: number, itemSavedCallback: (newTokenCharacteristic: ITokenCharacteristic) => void ) {
        this.tokenId = tokenId;
        this.tokenCharacteristic = null;
        this.itemSavedCallback = itemSavedCallback;
        this.clear();
        $("#newTokenCharacteristic").modal({
            show: true,
            backdrop: "static"
        });
    }

    showEdit(tokenCharacteristic: ITokenCharacteristic, tokenSavedCallback: () => void) {
        this.tokenCharacteristic = tokenCharacteristic;
        this.itemSavedCallback = tokenSavedCallback;
        this.loadData(tokenCharacteristic);
        $("#newTokenCharacteristic").modal({
            show: true,
            backdrop: "static"
        });
    }

    private saveNew() {
        var description = $("#new-token-text-description").val();
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/AddTokenCharacteristic",
            data: JSON.stringify({
                tokenId: this.tokenId,
                morphologicalCharacteristic: this.currentValue,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: (newTokenCharacteristicId) => {
                $("#newTokenCharacteristic").modal("hide");

                var newTokenCharacteristic: ITokenCharacteristic = {
                    Id: newTokenCharacteristicId,
                    Description: description,
                    MorphologicalCharacteristic: this.currentValue,
                    CanonicalFormList: []
                }

                this.itemSavedCallback(newTokenCharacteristic);
            }
        });
    }

    private saveEdit() {
        var description = $("#new-token-text-description").val();
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/EditTokenCharacteristic",
            data: JSON.stringify({
                tokenCharacteristicId: this.tokenCharacteristic.Id,
                morphologicalCharacteristic: this.currentValue,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                $("#newTokenCharacteristic").modal("hide");

                this.tokenCharacteristic.Description = description;
                this.tokenCharacteristic.MorphologicalCharacteristic = this.currentValue;
                
                this.itemSavedCallback(null);
            }
        });
    }
}

class LemmatizationCharacteristicTable {
    private characteristicEditor: LemmatizationCharacteristicEditor;
    private container: HTMLDivElement;
    private tbody: HTMLTableSectionElement;
    private item: ITokenCharacteristic;
    private descriptionDiv: HTMLDivElement;
    private morphologicalSpan: HTMLSpanElement;
    
    constructor(characteristicEditor: LemmatizationCharacteristicEditor, item: ITokenCharacteristic, container: HTMLDivElement) {
        this.characteristicEditor = characteristicEditor;
        this.container = container;
        this.item = item;
    }

    make() {
        $(this.container).empty();

        var morphologicalDiv = document.createElement("div");
        var morphologicalLabelDiv = document.createElement("div");
        var morphologicalContentSpan = document.createElement("span");
        var descriptionDiv = document.createElement("div");
        var descriptionLabelDiv = document.createElement("div");
        var descriptionContentDiv = document.createElement("div");
        var tableDiv = document.createElement("div");
        var editCharacteristicDiv = document.createElement("div");
        var editCharacteristicButton = document.createElement("button");
        var delimeter = document.createElement("hr");

        $(editCharacteristicButton)
            .addClass("lemmatization-edit")
            .addClass("lemmatization-big-left-space")
            .text("Upravit")
            .click(() => {
                this.characteristicEditor.showEdit(this.item, this.updateUiAfterSave.bind(this));
            });
        $(editCharacteristicDiv)
            .append(editCharacteristicButton);

        $(morphologicalLabelDiv)
            .addClass("lemmatization-label")
            .addClass("lemmatization-big-label-width")
            .text("Morfologická charakteristika:");
        $(morphologicalContentSpan)
            .addClass("lemmatization-label-content-big")
            .text(this.item.MorphologicalCharacteristic);
        $(morphologicalDiv)
            .append(morphologicalLabelDiv)
            .append(morphologicalContentSpan);

        $(descriptionLabelDiv)
            .addClass("lemmatization-label")
            .addClass("lemmatization-label-description")
            .addClass("lemmatization-big-label-width")
            .text("Popis:");
        $(descriptionContentDiv)
            .addClass("lemmatization-description")
            .addClass("lemmatization-big-left-space")
            .text(this.item.Description);
        $(descriptionDiv)
            .append(descriptionLabelDiv)
            .append(descriptionContentDiv);

        $(tableDiv)
            .addClass("lemmatization-characteristic");

        var table = document.createElement("table");
        var thead = document.createElement("thead");
        var headerTr = document.createElement("tr");
        var th1 = document.createElement("th");
        var th2 = document.createElement("th");
        var th3 = document.createElement("th");
        var th4 = document.createElement("th");
        var tbody = document.createElement("tbody");
        this.tbody = tbody;

        $(th1).addClass("column-commands")
            .text("");
        $(th2).addClass("column-canonical-form")
            .text("Kanonická forma");
        $(th3).addClass("column-type")
            .text("Typ");
        $(th4).addClass("column-description")
            .text("Popis");
        $(headerTr)
            .append(th1)
            .append(th2)
            .append(th3)
            .append(th4);
        $(thead).append(headerTr);
        $(table)
            .addClass("lemmatization-table")
            .append(thead)
            .append(tbody);

        for (var i = 0; i < this.item.CanonicalFormList.length; i++) {
            var canonicalFormItem = this.item.CanonicalFormList[i];
            var canonicalForm = new LemmatizationCanonicalForm(this.item.Id, tbody, canonicalFormItem);
            canonicalForm.make(this.addNewEmptyRow.bind(this));
        }
        this.addNewEmptyRow();

        $(tableDiv).append(table);

        $(this.container)
            .append(morphologicalDiv)
            .append(descriptionDiv)
            .append(editCharacteristicDiv)
            .append(tableDiv)
            .append(delimeter);

        this.descriptionDiv = descriptionContentDiv;
        this.morphologicalSpan = morphologicalContentSpan;
    }

    private addNewEmptyRow() {
        var canonicalForm = new LemmatizationCanonicalForm(this.item.Id, this.tbody);
        canonicalForm.make(this.addNewEmptyRow.bind(this));
    }

    private updateUiAfterSave() {
        $(this.descriptionDiv).text(this.item.Description);
        $(this.morphologicalSpan).text(this.item.MorphologicalCharacteristic);
    }
}

class LemmatizationCanonicalForm {
    private static searchBox: LemmatizationSearchBox;
    private static hyperSearchBox: LemmatizationSearchBox;
    private newCanonicalFormCreatedCallback: (form: ICanonicalForm) => void;
    private tableBody: HTMLTableSectionElement;
    private canonicalForm: ICanonicalForm;
    private tokenCharacteristicId: number;
    private containerCanonicalForm: HTMLDivElement;
    private containerType: HTMLDivElement;
    private containerDescription: HTMLDivElement;
    private editButton: HTMLButtonElement;
    private hyperLemmatization: LemmatizationHyperCanonicalForm;

    constructor(tokenCharacteristicId: number, tableBody: HTMLTableSectionElement, canonicalForm: ICanonicalForm = null) {
        this.tableBody = tableBody;
        this.canonicalForm = canonicalForm;
        this.tokenCharacteristicId = tokenCharacteristicId;
    }

    make(newCanonicalFormCreatedCallback: (form: ICanonicalForm) => void) {
        this.newCanonicalFormCreatedCallback = newCanonicalFormCreatedCallback;
        var tr = document.createElement("tr");
        var td1 = document.createElement("td");
        var td2 = document.createElement("td");
        var td3 = document.createElement("td");
        var td4 = document.createElement("td");
        var containerCanonicalForm = document.createElement("div");
        var containerType = document.createElement("div");
        var containerDescription = document.createElement("div");
        var editButton = document.createElement("button");
        var editButtonContent = document.createElement("span");
        $(editButtonContent)
            .addClass("glyphicon")
            .addClass(this.canonicalForm ? "glyphicon-pencil" : "glyphicon-plus");
        $(editButton).append(editButtonContent);
        $(editButton).click(() => {
            if (this.canonicalForm)
                this.showEditDialog();
            else
                this.showCreateDialog();
        });

        if (this.canonicalForm) {
            $(containerCanonicalForm).text(this.canonicalForm.Text);
            $(containerType).text(LemmatizationCanonicalForm.typeToString(this.canonicalForm.Type));
            $(containerDescription).text(this.canonicalForm.Description);
        }


        $(td1).addClass("column-commands")
            .append(editButton);
        $(td2).append(containerCanonicalForm);
        $(td3).append(containerType);
        $(td4).append(containerDescription);
        $(tr).addClass("canonical-form-row")
            .append(td1)
            .append(td2)
            .append(td3)
            .append(td4);

        // hyper canonical form
        var hyperTr = document.createElement("tr");
        var hyperTd1 = document.createElement("td");
        var hyperTd2 = document.createElement("td");
        var hyperTd3 = document.createElement("td");
        var hyperTd4 = document.createElement("td");
        var containerHyperForm = document.createElement("div");
        var containerHyperType = document.createElement("div");
        var containerHyperDescription = document.createElement("div");

        var editHyperButton = document.createElement("button");
        var setHyperButton = document.createElement("button");
        var editHyperLabel = document.createElement("span");
        var setHyperLabel = document.createElement("span");
        $(editHyperLabel)
            .addClass("glyphicon")
            .addClass("glyphicon-pencil");
        $(setHyperLabel)
            .addClass("glyphicon")
            .addClass("glyphicon-edit");
        $(editHyperButton)
            .addClass("hidden")
            .append(editHyperLabel);
        $(setHyperButton)
            .addClass("hidden")
            .append(setHyperLabel);

        if (this.canonicalForm) {
            if (this.canonicalForm.HyperCanonicalForm) {
                var hyperCanonicalForm = this.canonicalForm.HyperCanonicalForm;

                $(containerHyperForm).text(hyperCanonicalForm.Text);
                $(containerHyperType).text(LemmatizationCanonicalForm.hyperTypeToString(hyperCanonicalForm.Type));
                $(containerHyperDescription).text(hyperCanonicalForm.Description);
                $(editHyperButton).removeClass("hidden");
            }
            $(setHyperButton).removeClass("hidden");
        }

        $(editHyperButton).click(() => {
            this.showEditHyperDialog();
        });
        $(setHyperButton).click(() => {
            if (this.canonicalForm.HyperCanonicalForm)
                this.showSetHyperDialog();
            else
                this.showCreateHyperDialog();
        });

        $(hyperTd1)
            .addClass("column-commands")
            .append(setHyperButton)
            .append(editHyperButton);
        $(hyperTd2).append(containerHyperForm);
        $(hyperTd3).append(containerHyperType);
        $(hyperTd4).append(containerHyperDescription);
        $(hyperTr).addClass("hyper-canonical-form-row")
            .append(hyperTd1)
            .append(hyperTd2)
            .append(hyperTd3)
            .append(hyperTd4);

        $(this.tableBody).append(tr);
        $(this.tableBody).append(hyperTr);

        this.containerCanonicalForm = containerCanonicalForm;
        this.containerType = containerType;
        this.containerDescription = containerDescription;
        this.editButton = editButton;

        this.hyperLemmatization = new LemmatizationHyperCanonicalForm();
        this.hyperLemmatization.containerName = containerHyperForm;
        this.hyperLemmatization.containerType = containerHyperType;
        this.hyperLemmatization.containerDescription = containerHyperDescription;
        this.hyperLemmatization.editButton = editHyperButton;
        this.hyperLemmatization.setButton = setHyperButton;
    }

    private showCreateDialog() {
        $("#newCanonicalFormDialog").modal({
            show: true,
            backdrop: "static"
        });
        $("#save-form").off("click");
        $("#save-form").click(() => {
            this.createItem();
        });

        $("#new-form").val("");
        $("#new-form-description").val("");
        $("#new-form-existing-description").val("");
        LemmatizationCanonicalForm.searchBox.setValue("");
    }

    private showEditDialog() {
        $("#editCanonicalFormDialog").modal({
            show: true,
            backdrop: "static"
        });
        $("#save-edited-form").off("click");
        $("#save-edited-form").click(() => {
            this.editItem();
        });

        $("#edit-form-text").val(this.canonicalForm.Text);
        $("#edit-form-type").val(String(this.canonicalForm.Type));
        $("#edit-form-description").val(this.canonicalForm.Description);
    }

    private showSetHyperDialog() {
        this.showCreateHyperDialog();
    }

    private showCreateHyperDialog() {
        $("#newHyperCanonicalFormDialog").modal({
            show: true,
            backdrop: "static"
        });
        $("#save-hyper").off("click");
        $("#save-hyper").click(() => {
            this.createHyperItem();
        });

        $("#new-hyper").val("");
        $("#new-hyper-description").val("");
        $("#new-hyper-existing-description").val("");
        LemmatizationCanonicalForm.hyperSearchBox.setValue("");
    }

    private showEditHyperDialog() {
        $("#editHyperCanonicalFormDialog").modal({
            show: true,
            backdrop: "static"
        });
        $("#save-edited-hyper").off("click");
        $("#save-edited-hyper").click(() => {
            this.editHyperItem();
        });

        $("#edit-hyper-text").val(this.canonicalForm.HyperCanonicalForm.Text);
        $("#edit-hyper-type").val(String(this.canonicalForm.HyperCanonicalForm.Type));
        $("#edit-hyper-description").val(this.canonicalForm.HyperCanonicalForm.Description);
    }

    private createItem() {
        var name = $("#new-form").val();
        var formType = Number($("#new-form-type").val());
        var description = $("#new-form-description").val();

        if ($("#tab-create-new").hasClass("active")) {
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/CreateCanonicalForm",
                data: JSON.stringify({
                    tokenCharacteristicId: this.tokenCharacteristicId,
                    text: name,
                    type: formType,
                    description: description
                }),
                dataType: "json",
                contentType: "application/json",
                success: (newCanonicalFormId) => {
                    this.updateAfterItemCreation(newCanonicalFormId, name, formType, description);
                }
            });
        } else {
            var searchBox = LemmatizationCanonicalForm.searchBox;
            var currentItem: ICanonicalForm = <ICanonicalForm>(searchBox.getValue());

            if (!currentItem)
                return; //TODO show error

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/AddCanonicalForm",
                data: JSON.stringify({
                    tokenCharacteristicId: this.tokenCharacteristicId,
                    canonicalFormId: currentItem.Id
                }),
                dataType: "json",
                contentType: "application/json",
                success: () => {
                    this.updateAfterItemCreation(currentItem.Id, currentItem.Text, currentItem.Type, currentItem.Description);
                }
            });
        }
    }

    private createHyperItem() {
        var name = $("#new-hyper").val();
        var formType = Number($("#new-hyper-type").val());
        var description = $("#new-hyper-description").val();

        if ($("#tab2-create-new").hasClass("active")) {
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/CreateHyperCanonicalForm",
                data: JSON.stringify({
                    canonicalFormId: this.canonicalForm.Id,
                    text: name,
                    type: formType,
                    description: description
                }),
                dataType: "json",
                contentType: "application/json",
                success: (newHyperCanonicalFormId) => {
                    this.updateAfterHyperItemCreation(newHyperCanonicalFormId, name, formType, description);
                }
            });
        } else {
            var searchBox = LemmatizationCanonicalForm.hyperSearchBox;
            var currentItem: IHyperCanonicalForm = <IHyperCanonicalForm>(searchBox.getValue());

            if (!currentItem)
                return; //TODO show error

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/SetHyperCanonicalForm",
                data: JSON.stringify({
                    canonicalFormId: this.canonicalForm.Id,
                    hyperCanonicalFormId: currentItem.Id
                }),
                dataType: "json",
                contentType: "application/json",
                success: () => {
                    this.updateAfterHyperItemCreation(currentItem.Id, currentItem.Text, currentItem.Type, currentItem.Description);
                }
            });
        }
    }

    private editItem() {
        var text = $("#edit-form-text").val();
        var formType = Number($("#edit-form-type").val());
        var description = $("#edit-form-description").val();

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/EditCanonicalForm",
            data: JSON.stringify({
                canonicalFormId: this.canonicalForm.Id,
                text: text,
                type: formType,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                this.updateAfterItemEdit(text, formType, description);
            }
        });
    }

    private editHyperItem() {
        var text = $("#edit-hyper-text").val();
        var formType = Number($("#edit-hyper-type").val());
        var description = $("#edit-hyper-description").val();

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/EditHyperCanonicalForm",
            data: JSON.stringify({
                hyperCanonicalFormId: this.canonicalForm.HyperCanonicalForm.Id,
                text: text,
                type: formType,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                this.updateAfterHyperItemEdit(text, formType, description);
            }
        });
    }

    private updateAfterItemEdit(text: string, formType: CanonicalFormTypeEnum, description: string) {
        this.canonicalForm.Text = text;
        this.canonicalForm.Type = formType;
        this.canonicalForm.Description = description;

        $(this.containerCanonicalForm).text(this.canonicalForm.Text);
        $(this.containerDescription).text(this.canonicalForm.Description);
        $(this.containerType).text(LemmatizationCanonicalForm.typeToString(this.canonicalForm.Type));
        $("#editCanonicalFormDialog").modal("hide");
    }

    private updateAfterHyperItemEdit(text: string, formType: HyperCanonicalFormTypeEnum, description: string) {
        var hyperCanonicalForm = this.canonicalForm.HyperCanonicalForm;
        hyperCanonicalForm.Text = text;
        hyperCanonicalForm.Type = formType;
        hyperCanonicalForm.Description = description;

        $(this.hyperLemmatization.containerName).text(hyperCanonicalForm.Text);
        $(this.hyperLemmatization.containerDescription).text(hyperCanonicalForm.Description);
        $(this.hyperLemmatization.containerType).text(LemmatizationCanonicalForm.hyperTypeToString(hyperCanonicalForm.Type));
        $("#editHyperCanonicalFormDialog").modal("hide");
    }

    private updateAfterItemCreation(newId: number, name: string, formType: CanonicalFormTypeEnum, description: string) {
        this.canonicalForm = {
            Id: newId,
            Text: name,
            Type: formType,
            Description: description,
            HyperCanonicalForm: null
        };

        $(this.containerCanonicalForm).text(this.canonicalForm.Text);
        $(this.containerDescription).text(this.canonicalForm.Description);
        $(this.containerType).text(LemmatizationCanonicalForm.typeToString(this.canonicalForm.Type));
        $(this.editButton).children()
            .removeClass("glyphicon-plus")
            .addClass("glyphicon-pencil");
        $(this.hyperLemmatization.setButton).removeClass("hidden");

        this.newCanonicalFormCreatedCallback(this.canonicalForm);
        $("#newCanonicalFormDialog").modal("hide");
    }

    private updateAfterHyperItemCreation(newId: number, name: string, formType: HyperCanonicalFormTypeEnum, description: string) {
        this.canonicalForm.HyperCanonicalForm = {
            Id: newId,
            Text: name,
            Type: formType,
            Description: description
        }

        var hyperCanonicalForm = this.canonicalForm.HyperCanonicalForm;
        $(this.hyperLemmatization.containerName).text(hyperCanonicalForm.Text);
        $(this.hyperLemmatization.containerDescription).text(hyperCanonicalForm.Description);
        $(this.hyperLemmatization.containerType).text(LemmatizationCanonicalForm.hyperTypeToString(hyperCanonicalForm.Type));
        $(this.hyperLemmatization.editButton).removeClass("hidden");

        $("#newHyperCanonicalFormDialog").modal("hide");
    }

    static init() {
        var createOption = (value: CanonicalFormTypeEnum): HTMLOptionElement => {
            var label = LemmatizationCanonicalForm.typeToString(value);
            var element = document.createElement("option");
            $(element).attr("value", value);
            $(element).text(label);
            return element;
        }

        var createHyperOption = (value: HyperCanonicalFormTypeEnum): HTMLOptionElement => {
            var label = LemmatizationCanonicalForm.hyperTypeToString(value);
            var element = document.createElement("option");
            $(element).attr("value", value);
            $(element).text(label);
            return element;
        };

        $("#new-form-type")
            .append(createOption(CanonicalFormTypeEnum.Lemma))
            .append(createOption(CanonicalFormTypeEnum.LemmaOld))
            .append(createOption(CanonicalFormTypeEnum.Stemma))
            .append(createOption(CanonicalFormTypeEnum.StemmaOld));

        $("#new-form-existing-type")
            .append(createOption(CanonicalFormTypeEnum.Lemma))
            .append(createOption(CanonicalFormTypeEnum.LemmaOld))
            .append(createOption(CanonicalFormTypeEnum.Stemma))
            .append(createOption(CanonicalFormTypeEnum.StemmaOld));

        $("#edit-form-type")
            .append(createOption(CanonicalFormTypeEnum.Lemma))
            .append(createOption(CanonicalFormTypeEnum.LemmaOld))
            .append(createOption(CanonicalFormTypeEnum.Stemma))
            .append(createOption(CanonicalFormTypeEnum.StemmaOld));

        $("#new-hyper-type")
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperLemma))
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperStemma));

        $("#new-hyper-existing-type")
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperLemma))
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperStemma));

        $("#edit-hyper-type")
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperLemma))
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperStemma));

        var searchBox = new LemmatizationSearchBox("#new-form-existing-input");
        var hyperSearchBox = new LemmatizationSearchBox("#new-hyper-existing-input");

        var selectedChangedCallback = (selectedExist, selectionConfirmed) => {
            var currentItem = searchBox.getValue();
            var description = currentItem ? currentItem.Description : "";
            $("#new-form-existing-description").val(description);
        };
        var hyperSelectedChangedCallback = (selectedExist, selectionConfirmed) => {
            var currentItem = hyperSearchBox.getValue();
            var description = currentItem ? currentItem.Description : "";
            $("#new-hyper-existing-description").val(description);
        };

        searchBox.setDataSet("CanonicalForm", "type=0");
        searchBox.create(selectedChangedCallback);
        hyperSearchBox.setDataSet("HyperCanonicalForm", "type=0");
        hyperSearchBox.create(hyperSelectedChangedCallback);

        $("#new-form-existing-type").on("change", (e) => {
            var value = $(e.target).val();
            searchBox.setDataSet("CanonicalForm", "type=" + value);
            searchBox.create(selectedChangedCallback);
            searchBox.reload();
        });
        $("#new-hyper-existing-type").on("change", (e) => {
            var value = $(e.target).val();
            hyperSearchBox.setDataSet("HyperCanonicalForm", "type=" + value);
            hyperSearchBox.create(hyperSelectedChangedCallback);
            hyperSearchBox.reload();
        });

        LemmatizationCanonicalForm.searchBox = searchBox;
        LemmatizationCanonicalForm.hyperSearchBox = hyperSearchBox;
    }

    static typeToString(canonicalFormType: CanonicalFormTypeEnum): string {
        switch (canonicalFormType) {
            case CanonicalFormTypeEnum.Lemma:
                return "Lemma";
            case CanonicalFormTypeEnum.LemmaOld:
                return "Staré lemma";
            case CanonicalFormTypeEnum.Stemma:
                return "Stemma";
            case CanonicalFormTypeEnum.StemmaOld:
                return "Staré stemma";
            default:
                return "";
        }
    }

    static hyperTypeToString(hyperCanonicalForm: HyperCanonicalFormTypeEnum): string {
        switch (hyperCanonicalForm) {
            case HyperCanonicalFormTypeEnum.HyperLemma:
                return "Hyperlemma";
            case HyperCanonicalFormTypeEnum.HyperStemma:
                return "Hyperstemma";
            default:
                return "";
        }
    }
}

class LemmatizationHyperCanonicalForm {
    public editButton: HTMLButtonElement;
    public setButton: HTMLButtonElement;
    public containerName: HTMLDivElement;
    public containerType: HTMLDivElement;
    public containerDescription: HTMLDivElement;
}

class LemmatizationSearchBox {
    private inputField: string;
    private suggestionTemplate: (item: any) => string;
    private urlWithController: string;
    private options: Twitter.Typeahead.Options;
    private dataset: Twitter.Typeahead.Dataset;
    private bloodhound: Bloodhound<string>;
    private currentItem: ITypeaheadItem;

    constructor(inputFieldElement: string, suggestionTemplate: (item: any) => string = null) {
        this.inputField = inputFieldElement;
        this.suggestionTemplate = suggestionTemplate;
        this.urlWithController = getBaseUrl() + "Lemmatization";

        this.options = {
            hint: true,
            highlight: false,
            minLength: 1
        };
    }
    
    setValue(value: any): void {
        $(this.inputField).typeahead('val', value);
    }

    getValue(): ITypeaheadItem {
        return this.currentItem;
    }

    getInputValue(): string {
        return <any>($(this.inputField).typeahead("val"));
    }

    create(selectionChangedCallback: (selectedExists: boolean, selectConfirmed: boolean) => void): void {
        var self = this;
        $(this.inputField).typeahead(this.options, this.dataset);
        $(this.inputField).bind("typeahead:render", <any>function(e, ...datums) {
            var isEmpty = $(".tt-menu", e.target.parentNode).hasClass("tt-empty");
            if (isEmpty) {
                self.currentItem = null;
                selectionChangedCallback(false, false);
                return;
            }

            var currentText = self.getInputValue();
            var suggestionElements = $(".suggestion", e.target.parentNode);
            for (var i = 0; i < suggestionElements.length; i++) {
                if ($(suggestionElements[i]).text() === currentText) {
                    self.currentItem = datums[i];
                    selectionChangedCallback(true, false);
                    return;
                }
            }
            self.currentItem = null;
            selectionChangedCallback(false, false);
        });
        $(this.inputField).bind("typeahead:select", <any>function (e, datum) {
            self.currentItem = datum;
            selectionChangedCallback(true, true);
        });
        $(this.inputField).bind("typeahead:autocomplete", <any>function (e, datum) {
            self.currentItem = datum;
            selectionChangedCallback(true, false);
        });
    }

    destroy(): void {
        $(this.inputField).typeahead("destroy");
        $(this.inputField).unbind("typeahead:render");
        $(this.inputField).unbind("typeahead:select");
        $(this.inputField).unbind("typeahead:autocomplete");
    }

    reload() {
        this.clearCache();
        var value = this.getInputValue();
        this.setValue("");
        this.setValue(value);
    }

    clearCache(): void {
        if (this.bloodhound) {
            this.bloodhound.clear();
            this.bloodhound.clearPrefetchCache();
            this.bloodhound.clearRemoteCache();
        }
    }

    setDataSet(name: string, parameterUrlString: string = null): void {
        this.clearCache();
        this.destroy();
        var remoteUrl: string = this.urlWithController + "/GetTypeahead" + name + "?query=%QUERY";

        if (parameterUrlString != null) {
            remoteUrl += "&" + parameterUrlString;
        }

        var remoteOptions: Bloodhound.RemoteOptions<string> = {
            url: remoteUrl,
            wildcard: "%QUERY"
        };

        var bloodhound: Bloodhound<string> = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.whitespace,
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            limit: 10,
            remote: remoteOptions
        });

        var suggestionTemplate = this.suggestionTemplate ? this.suggestionTemplate : this.getDefaultSuggestionTemplate;
        var dataset: Twitter.Typeahead.Dataset = {
            name: name,
            source: bloodhound,
            display: "Text",
            templates: {
                suggestion: suggestionTemplate
            }
        };

        this.bloodhound = bloodhound;
        this.dataset = dataset;
    }

    private getDefaultSuggestionTemplate(item: IToken): string {
        return "<div><div class=\"suggestion\" style='font-weight: bold'>" + item.Text + "</div><div class=\"description\">" + item.Description + "</div></div>";
    }
}

interface ITypeaheadItem {
    Text: string;
    Description: string;
}

interface IToken {
    Id: number;
    Text: string;
    Description: string;
}

interface ITokenCharacteristic {
    Id: number;
    MorphologicalCharacteristic: string;
    Description: string;
    CanonicalFormList: Array<ICanonicalForm>;
}

interface ICanonicalForm {
    Id: number;
    Text: string;
    Description: string;
    Type: CanonicalFormTypeEnum;
    HyperCanonicalForm: IHyperCanonicalForm;
}

interface IHyperCanonicalForm {
    Id: number;
    Text: string;
    Description: string;
    Type: HyperCanonicalFormTypeEnum;
}

enum CanonicalFormTypeEnum {
    Lemma = 0,
    Stemma = 1,
    LemmaOld = 2,
    StemmaOld = 3,
}

enum HyperCanonicalFormTypeEnum {
    HyperLemma = 0,
    HyperStemma = 1,
}