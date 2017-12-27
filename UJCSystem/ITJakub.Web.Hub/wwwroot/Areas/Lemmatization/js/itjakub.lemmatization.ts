function initLemmatization(tokenId: string) {
    var canEdit = isUserInRole(RoleEnum.EditLemmatization);
    var lemmatization = new Lemmatization("#mainContainer", canEdit);
    lemmatization.make();

    var inputElement = <HTMLInputElement>$("#mainSearchInput").get(0);
    var keyboardButton = <HTMLButtonElement>$("#keyboard-button").get(0);
    var keyboardComponent = KeyboardManager.getKeyboard("0");
    keyboardComponent.registerButton(keyboardButton, inputElement, (newQuery) => lemmatization.setMainSearchBoxValue(newQuery));

    if (tokenId)
        lemmatization.load(Number(tokenId));
}

class Lemmatization {
    private canEdit: boolean;
    private mainContainer: string;
    private searchBox: SingleSetTypeaheadSearchBox<IToken>;
    private lemmatizationCharacteristic: LemmatizationCharacteristicEditor;
    private currentTokenItem: IToken;

    constructor(mainContainer: string, canEdit: boolean) {
        this.canEdit = canEdit;
        this.mainContainer = mainContainer;
        this.searchBox = new SingleSetTypeaheadSearchBox<IToken>("#mainSearchInput",
            "Lemmatization/Lemmatization",
            (item) => item.text,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.text, item.description));
        this.lemmatizationCharacteristic = new LemmatizationCharacteristicEditor();
    }

    public setMainSearchBoxValue(value: string) {
        this.searchBox.setValue(value);
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
                this.loadToken(this.searchBox.getValue());
            }
        });
        this.lemmatizationCharacteristic.init();
        LemmatizationCanonicalForm.init();

        $("#loadButton").click(() => {
            var tokenItem = this.searchBox.getValue();
            this.loadToken(tokenItem);
        });

        $("#addNewTokenButton").click(() => {
            this.showAddNewToken();
        });

        $("#addNewCharacteristic").click(() => {
            this.lemmatizationCharacteristic.show(this.currentTokenItem.id, (newTokenCharacteristic) => {
                this.addNewCharacteristic(newTokenCharacteristic);
            });
        });

        $("#save-token").click(() => {
            this.saveNewToken();
        });

        $("#showEditToken").click(() => {
            $("#edit-token-text").val(this.currentTokenItem.text);
            $("#edit-token-description").val(this.currentTokenItem.description);
            $("#editTokenDialog").modal({
                show: true,
                backdrop: "static"
            });
        });

        $("#save-edited-token").click(() => {
            this.saveEditedToken();
        });

        if (!this.canEdit) {
            $("#addNewTokenButton").remove();
            $("#showEditToken").remove();
            $("#addNewCharacteristic").remove();

            $("#newTokenDialog").remove();
            $("#editTokenDialog").remove();
            $("#newTokenCharacteristic").remove();

            $("#newCanonicalFormDialog").remove();
            $("#editCanonicalFormDialog").remove();
            $("#newHyperCanonicalFormDialog").remove();
            $("#editHyperCanonicalFormDialog").remove();
        }
    }

    public load(tokenId: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/GetToken",
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
        if (tokenItem == null) {
            return;
        }

        $(".content").removeClass("hidden");
        $("#specificToken").text(tokenItem.text);
        $("#specificTokenDescription").text(tokenItem.description);

        this.loadTokenCharacteristic();
    }
    
    private loadTokenCharacteristic() {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/GetTokenCharacteristic",
            data: {
                tokenId: this.currentTokenItem.id
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
            var characteristicTable = new LemmatizationCharacteristicTable(this.lemmatizationCharacteristic, characteristicItem, containerDiv, this.canEdit);
            characteristicTable.make(this.loadTokenCharacteristic.bind(this));

            $(this.mainContainer).append(containerDiv);
        }
    }

    private addNewCharacteristic(item: ITokenCharacteristic) {
        var containerDiv = document.createElement("div");
        var characteristicTable = new LemmatizationCharacteristicTable(this.lemmatizationCharacteristic, item, containerDiv, this.canEdit);
        characteristicTable.make(this.loadTokenCharacteristic.bind(this));

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
        var token = $("#new-token").val() as string;
        var description = $("#new-token-description").val() as string;

        if (!token) {
            return;
        }

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/CreateToken",
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
                    id: newTokenId,
                    text: token,
                    description: description
                };
                this.loadToken(tokenItem);
            }
        });
    }

    private saveEditedToken() {
        var description = $("#edit-token-description").val() as string;

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/EditToken",
            data: JSON.stringify({
                tokenId: this.currentTokenItem.id,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                $("#editTokenDialog").modal("hide");
                $("#specificTokenDescription").text(description);
                this.currentTokenItem.description = description;
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
            var value = selects.filter("#description-" + i).val() as string;
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
        var characteristic = tokenCharacteristic.morphologicalCharacteristic;
        $("#new-token-text-description").val(tokenCharacteristic.description);
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
        var description = $("#new-token-text-description").val() as string;
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/AddTokenCharacteristic",
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
                    id: newTokenCharacteristicId,
                    description: description,
                    morphologicalCharacteristic: this.currentValue,
                    canonicalFormList: []
                }

                this.itemSavedCallback(newTokenCharacteristic);
            }
        });
    }

    private saveEdit() {
        var description = $("#new-token-text-description").val() as string;
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/EditTokenCharacteristic",
            data: JSON.stringify({
                tokenCharacteristicId: this.tokenCharacteristic.id,
                morphologicalCharacteristic: this.currentValue,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                $("#newTokenCharacteristic").modal("hide");

                this.tokenCharacteristic.description = description;
                this.tokenCharacteristic.morphologicalCharacteristic = this.currentValue;
                
                this.itemSavedCallback(null);
            }
        });
    }
}

class LemmatizationCharacteristicTable {
    private canEdit: boolean;
    private characteristicEditor: LemmatizationCharacteristicEditor;
    private container: HTMLDivElement;
    private tbody: HTMLTableSectionElement;
    private item: ITokenCharacteristic;
    private descriptionDiv: HTMLDivElement;
    private morphologicalSpan: HTMLSpanElement;
    private reloadCallback: () => void;
    
    constructor(characteristicEditor: LemmatizationCharacteristicEditor, item: ITokenCharacteristic, container: HTMLDivElement, canEdit: boolean) {
        this.canEdit = canEdit;
        this.characteristicEditor = characteristicEditor;
        this.container = container;
        this.item = item;
    }

    make(reloadCallback: () => void) {
        this.reloadCallback = reloadCallback;
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
        var deleteCharacteristicButton = document.createElement("button");
        var delimeter = document.createElement("hr");

        $(editCharacteristicButton)
            .addClass("lemmatization-edit")
            .addClass("lemmatization-big-left-space")
            .text("Upravit")
            .click(() => {
                this.characteristicEditor.showEdit(this.item, this.updateUiAfterSave.bind(this));
            });
        $(deleteCharacteristicButton)
            .addClass("lemmatization-edit")
            .text("Smazat")
            .click(() => {
                this.delete();
            });
        $(editCharacteristicDiv)
            .append(editCharacteristicButton)
            .append(deleteCharacteristicButton);

        $(morphologicalLabelDiv)
            .addClass("lemmatization-label")
            .addClass("lemmatization-big-label-width")
            .text("Morfologická charakteristika:");
        $(morphologicalContentSpan)
            .addClass("lemmatization-label-content-big")
            .text(this.item.morphologicalCharacteristic);
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
            .text(this.item.description);
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

        for (var i = 0; i < this.item.canonicalFormList.length; i++) {
            var canonicalFormItem = this.item.canonicalFormList[i];
            var canonicalForm = new LemmatizationCanonicalForm(this.item.id, tbody, this.canEdit, canonicalFormItem);
            canonicalForm.make(this.addNewEmptyRow.bind(this), this.reloadCallback);
        }
        this.addNewEmptyRow();

        $(tableDiv).append(table);

        if (!this.canEdit) {
            editCharacteristicButton.remove();
            deleteCharacteristicButton.remove();
            th1.remove();
        }

        $(this.container)
            .append(morphologicalDiv)
            .append(descriptionDiv)
            .append(editCharacteristicDiv)
            .append(tableDiv)
            .append(delimeter);

        this.descriptionDiv = descriptionContentDiv;
        this.morphologicalSpan = morphologicalContentSpan;
    }

    private delete() {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/DeleteTokenCharacteristic",
            data: JSON.stringify({
                tokenCharacteristicId: this.item.id
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                $(this.container).empty();
            }
        });
    }

    private addNewEmptyRow() {
        if (!this.canEdit)
            return;

        var canonicalForm = new LemmatizationCanonicalForm(this.item.id, this.tbody, this.canEdit);
        canonicalForm.make(this.addNewEmptyRow.bind(this), this.reloadCallback);
    }

    private updateUiAfterSave() {
        $(this.descriptionDiv).text(this.item.description);
        $(this.morphologicalSpan).text(this.item.morphologicalCharacteristic);
    }
}

class LemmatizationCanonicalForm {
    private static searchBox: SingleSetTypeaheadSearchBox<ICanonicalForm>;
    private static hyperSearchBox: SingleSetTypeaheadSearchBox<IHyperCanonicalForm>;
    private newCanonicalFormCreatedCallback: (form: ICanonicalForm) => void;
    private reloadCallback: () => void;
    private canEdit: boolean;
    private tableBody: HTMLTableSectionElement;
    private canonicalForm: ICanonicalForm;
    private tokenCharacteristicId: number;
    private containerCanonicalForm: HTMLDivElement;
    private containerType: HTMLDivElement;
    private containerDescription: HTMLDivElement;
    private editButton: HTMLButtonElement;
    private deleteButton: HTMLButtonElement;
    private hyperLemmatization: LemmatizationHyperCanonicalForm;
    private tableRow1: HTMLTableRowElement;
    private tableRow2: HTMLTableRowElement;

    constructor(tokenCharacteristicId: number, tableBody: HTMLTableSectionElement, canEdit: boolean, canonicalForm: ICanonicalForm = null) {
        this.tableBody = tableBody;
        this.canEdit = canEdit;
        this.canonicalForm = canonicalForm;
        this.tokenCharacteristicId = tokenCharacteristicId;
    }

    make(newCanonicalFormCreatedCallback: (form: ICanonicalForm) => void, reloadCallback: () => void) {
        this.newCanonicalFormCreatedCallback = newCanonicalFormCreatedCallback;
        this.reloadCallback = reloadCallback;
        var tr = document.createElement("tr");
        var td1 = document.createElement("td");
        var td2 = document.createElement("td");
        var td3 = document.createElement("td");
        var td4 = document.createElement("td");
        var containerCanonicalForm = document.createElement("div");
        var containerType = document.createElement("div");
        var containerDescription = document.createElement("div");
        var editButton = document.createElement("button");
        var deleteButton = document.createElement("button");
        var editButtonContent = document.createElement("span");
        var deleteButtonContent = document.createElement("span");
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

        $(deleteButtonContent)
            .addClass("glyphicon")
            .addClass("glyphicon-trash");
        $(deleteButton).append(deleteButtonContent);
        $(deleteButton).click(() => {
            this.remove();
        });

        if (this.canonicalForm) {
            $(containerCanonicalForm).text(this.canonicalForm.text);
            $(containerType).text(LemmatizationCanonicalForm.typeToString(this.canonicalForm.type));
            $(containerDescription).text(this.canonicalForm.description);
        } else {
            $(deleteButton).addClass("hidden");
        }


        $(td1).addClass("column-commands")
            .append(deleteButton)
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
            if (this.canonicalForm.hyperCanonicalForm) {
                var hyperCanonicalForm = this.canonicalForm.hyperCanonicalForm;

                $(containerHyperForm).text(hyperCanonicalForm.text);
                $(containerHyperType).text(LemmatizationCanonicalForm.hyperTypeToString(hyperCanonicalForm.type));
                $(containerHyperDescription).text(hyperCanonicalForm.description);
                $(editHyperButton).removeClass("hidden");
            }
            $(setHyperButton).removeClass("hidden");
        }

        $(editHyperButton).click(() => {
            this.showEditHyperDialog();
        });
        $(setHyperButton).click(() => {
            if (this.canonicalForm.hyperCanonicalForm)
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

        if (!this.canEdit) {
            td1.remove();
            hyperTd1.remove();
        }

        $(this.tableBody).append(tr);
        $(this.tableBody).append(hyperTr);

        this.containerCanonicalForm = containerCanonicalForm;
        this.containerType = containerType;
        this.containerDescription = containerDescription;
        this.editButton = editButton;
        this.deleteButton = deleteButton;

        this.hyperLemmatization = new LemmatizationHyperCanonicalForm();
        this.hyperLemmatization.containerName = containerHyperForm;
        this.hyperLemmatization.containerType = containerHyperType;
        this.hyperLemmatization.containerDescription = containerHyperDescription;
        this.hyperLemmatization.editButton = editHyperButton;
        this.hyperLemmatization.setButton = setHyperButton;

        this.tableRow1 = tr;
        this.tableRow2 = hyperTr;
    }

    private remove() {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/RemoveCanonicalForm",
            data: JSON.stringify({
                tokenCharacteristicId: this.tokenCharacteristicId,
                canonicalFormId: this.canonicalForm.id
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                $(this.tableRow2).remove();
                $(this.tableRow1).remove();
            }
        });
    }

    private removeHyperItem() {
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/RemoveHyperCanonicalForm",
            data: JSON.stringify({
                canonicalFormId: this.canonicalForm.id
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                this.canonicalForm.hyperCanonicalForm = null;

                var item = this.hyperLemmatization;
                $(item.editButton).addClass("hidden");
                $(item.containerDescription).text("");
                $(item.containerName).text("");
                $(item.containerType).text("");

                $("#newHyperCanonicalFormDialog").modal("hide");
                
                this.reloadCallback();
            }
        });
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

        $("#edit-form-text").val(this.canonicalForm.text);
        $("#edit-form-type").val(String(this.canonicalForm.type));
        $("#edit-form-description").val(this.canonicalForm.description);
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
        $("#remove-hyper").off("click");
        $("#remove-hyper").click(() => {
            this.removeHyperItem();
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

        $("#edit-hyper-text").val(this.canonicalForm.hyperCanonicalForm.text);
        $("#edit-hyper-type").val(String(this.canonicalForm.hyperCanonicalForm.type));
        $("#edit-hyper-description").val(this.canonicalForm.hyperCanonicalForm.description);
    }

    private createItem() {
        var name = $("#new-form").val() as string;
        var formType = Number($("#new-form-type").val());
        var description = $("#new-form-description").val() as string;

        if ($("#tab-create-new").hasClass("active")) {
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/Lemmatization/CreateCanonicalForm",
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
            var currentItem: ICanonicalForm = (searchBox.getValue());

            if (!currentItem)
                return; //TODO show error

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/Lemmatization/AddCanonicalForm",
                data: JSON.stringify({
                    tokenCharacteristicId: this.tokenCharacteristicId,
                    canonicalFormId: currentItem.id
                }),
                dataType: "json",
                contentType: "application/json",
                success: () => {
                    this.updateAfterItemAssign(currentItem);
                }
            });
        }
    }

    private createHyperItem() {
        var name = $("#new-hyper").val() as string;
        var formType = Number($("#new-hyper-type").val());
        var description = $("#new-hyper-description").val() as string;

        if ($("#tab2-create-new").hasClass("active")) {
            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/Lemmatization/CreateHyperCanonicalForm",
                data: JSON.stringify({
                    canonicalFormId: this.canonicalForm.id,
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
            var currentItem: IHyperCanonicalForm = (searchBox.getValue());

            if (!currentItem)
                return; //TODO show error

            $.ajax({
                type: "POST",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/Lemmatization/SetHyperCanonicalForm",
                data: JSON.stringify({
                    canonicalFormId: this.canonicalForm.id,
                    hyperCanonicalFormId: currentItem.id
                }),
                dataType: "json",
                contentType: "application/json",
                success: () => {
                    this.updateAfterHyperItemCreation(currentItem.id, currentItem.text, currentItem.type, currentItem.description);
                }
            });
        }
    }

    private editItem() {
        var text = $("#edit-form-text").val() as string;
        var formType = Number($("#edit-form-type").val());
        var description = $("#edit-form-description").val() as string;

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/EditCanonicalForm",
            data: JSON.stringify({
                canonicalFormId: this.canonicalForm.id,
                text: text,
                type: formType,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                this.updateAfterItemEdit(text, formType, description);
                this.reloadCallback();
            }
        });
    }

    private editHyperItem() {
        var text = $("#edit-hyper-text").val() as string;
        var formType = Number($("#edit-hyper-type").val());
        var description = $("#edit-hyper-description").val() as string;

        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/Lemmatization/EditHyperCanonicalForm",
            data: JSON.stringify({
                hyperCanonicalFormId: this.canonicalForm.hyperCanonicalForm.id,
                text: text,
                type: formType,
                description: description
            }),
            dataType: "json",
            contentType: "application/json",
            success: () => {
                this.updateAfterHyperItemEdit(text, formType, description);
                this.reloadCallback();
            }
        });
    }

    private updateAfterItemEdit(text: string, formType: CanonicalFormTypeEnum, description: string) {
        this.canonicalForm.text = text;
        this.canonicalForm.type = formType;
        this.canonicalForm.description = description;

        $(this.containerCanonicalForm).text(this.canonicalForm.text);
        $(this.containerDescription).text(this.canonicalForm.description);
        $(this.containerType).text(LemmatizationCanonicalForm.typeToString(this.canonicalForm.type));
        $("#editCanonicalFormDialog").modal("hide");
    }

    private updateAfterHyperItemEdit(text: string, formType: HyperCanonicalFormTypeEnum, description: string) {
        var hyperCanonicalForm = this.canonicalForm.hyperCanonicalForm;
        hyperCanonicalForm.text = text;
        hyperCanonicalForm.type = formType;
        hyperCanonicalForm.description = description;

        $(this.hyperLemmatization.containerName).text(hyperCanonicalForm.text);
        $(this.hyperLemmatization.containerDescription).text(hyperCanonicalForm.description);
        $(this.hyperLemmatization.containerType).text(LemmatizationCanonicalForm.hyperTypeToString(hyperCanonicalForm.type));
        $("#editHyperCanonicalFormDialog").modal("hide");
    }

    private updateAfterItemAssign(canonicalForm: ICanonicalForm) {
        this.canonicalForm = canonicalForm;

        $(this.containerCanonicalForm).text(this.canonicalForm.text);
        $(this.containerDescription).text(this.canonicalForm.description);
        $(this.containerType).text(LemmatizationCanonicalForm.typeToString(this.canonicalForm.type));
        $(this.editButton).children()
            .removeClass("glyphicon-plus")
            .addClass("glyphicon-pencil");
        $(this.deleteButton).removeClass("hidden");
        $(this.hyperLemmatization.setButton).removeClass("hidden");

        if (canonicalForm.hyperCanonicalForm != null) {
            var hyperCanonicalForm = this.canonicalForm.hyperCanonicalForm;
            $(this.hyperLemmatization.containerName).text(hyperCanonicalForm.text);
            $(this.hyperLemmatization.containerDescription).text(hyperCanonicalForm.description);
            $(this.hyperLemmatization.containerType).text(LemmatizationCanonicalForm.hyperTypeToString(hyperCanonicalForm.type));
            $(this.hyperLemmatization.editButton).removeClass("hidden");
        }

        this.newCanonicalFormCreatedCallback(this.canonicalForm);
        $("#newCanonicalFormDialog").modal("hide");
    }

    private updateAfterItemCreation(newId: number, name: string, formType: CanonicalFormTypeEnum, description: string) {
        var canonicalForm: ICanonicalForm = {
            id: newId,
            text: name,
            type: formType,
            description: description,
            hyperCanonicalForm: null
        };

        this.updateAfterItemAssign(canonicalForm);
    }

    private updateAfterHyperItemCreation(newId: number, name: string, formType: HyperCanonicalFormTypeEnum, description: string) {
        this.canonicalForm.hyperCanonicalForm = {
            id: newId,
            text: name,
            type: formType,
            description: description
        }

        var hyperCanonicalForm = this.canonicalForm.hyperCanonicalForm;
        $(this.hyperLemmatization.containerName).text(hyperCanonicalForm.text);
        $(this.hyperLemmatization.containerDescription).text(hyperCanonicalForm.description);
        $(this.hyperLemmatization.containerType).text(LemmatizationCanonicalForm.hyperTypeToString(hyperCanonicalForm.type));
        $(this.hyperLemmatization.editButton).removeClass("hidden");

        $("#newHyperCanonicalFormDialog").modal("hide");
        this.reloadCallback();
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

        var searchBox = new SingleSetTypeaheadSearchBox<ICanonicalForm>("#new-form-existing-input",
            "Lemmatization/Lemmatization",
            (item) => item.text,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.text, item.description));
        var hyperSearchBox = new SingleSetTypeaheadSearchBox<IHyperCanonicalForm>("#new-hyper-existing-input",
            "Lemmatization/Lemmatization",
            (item) => item.text,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.text, item.description));

        var selectedChangedCallback = (selectedExist, selectionConfirmed) => {
            var currentItem = searchBox.getValue();
            var description = currentItem ? currentItem.description : "";
            $("#new-form-existing-description").val(description);
        };
        var hyperSelectedChangedCallback = (selectedExist, selectionConfirmed) => {
            var currentItem = hyperSearchBox.getValue();
            var description = currentItem ? currentItem.description : "";
            $("#new-hyper-existing-description").val(description);
        };

        searchBox.setDataSet("CanonicalForm", "type=0");
        searchBox.create(selectedChangedCallback);
        hyperSearchBox.setDataSet("HyperCanonicalForm", "type=0");
        hyperSearchBox.create(hyperSelectedChangedCallback);

        $("#new-form-existing-type").on("change", (e) => {
            var value = $(e.target).val() as string;
            searchBox.setDataSet("CanonicalForm", "type=" + value);
            searchBox.create(selectedChangedCallback);
            searchBox.reload();
        });
        $("#new-hyper-existing-type").on("change", (e) => {
            var value = $(e.target).val() as string;
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
