$(document).ready(() => {
    var lemmatization = new Lemmatization("#mainContainer");
    lemmatization.make();
});

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
            var characteristicTable = new LemmatizationCharacteristicTable(characteristicItem, containerDiv);
            characteristicTable.make();

            $(this.mainContainer).append(containerDiv);
        }
    }

    private addNewCharacteristic(item: ITokenCharacteristic) {
        var containerDiv = document.createElement("div");
        var characteristicTable = new LemmatizationCharacteristicTable(item, containerDiv);
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
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/CreateToken",
            data: {
                token: token,
                description: description
            },
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
}

class LemmatizationCharacteristicEditor {
    private currentValue: string;
    private tokenId: number;
    private itemSavedCallback: (newTokenCharacteristic: ITokenCharacteristic) => void;

    init() {
        $("#newTokenCharacteristic select").on("change", () => {
            this.updateTag();
        });

        $("#saveCharacteristic").click(() => {
            this.save();
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

    getValue(): string {
        return this.currentValue;
    }

    show(tokenId: number, itemSavedCallback: (newTokenCharacteristic: ITokenCharacteristic) => void ) {
        this.tokenId = tokenId;
        this.itemSavedCallback = itemSavedCallback;
        this.clear();
        $("#newTokenCharacteristic").modal({
            show: true,
            backdrop: "static"
        });
    }

    private save() {
        var description = $("#new-token-text-description").val();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/AddTokenCharacteristic",
            data: {
                tokenId: this.tokenId,
                morphologicalCharacteristic: this.currentValue,
                description: description
            },
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
}

class LemmatizationCharacteristicTable {
    private container: HTMLDivElement;
    private table: HTMLTableElement;
    private item: ITokenCharacteristic;
    
    constructor(item: ITokenCharacteristic, container: HTMLDivElement) {
        this.container = container;
        this.item = item;
    }

    make() {
        $(this.container).empty();

        var morphologicalDiv = document.createElement("div");
        var descriptionDiv = document.createElement("div");
        var tableDiv = document.createElement("div");

        $(morphologicalDiv)
            .addClass("lemmatization-morphologic")
            .text(this.item.MorphologicalCharacteristic);

        $(descriptionDiv)
            .addClass("lemmatization-characteristic-description")
            .text(this.item.Description);

        $(tableDiv)
            .addClass("lemmatization-characteristic");

        var table = document.createElement("table");
        var headerTr = document.createElement("tr");
        var td1 = document.createElement("td");
        var td2 = document.createElement("td");
        var td3 = document.createElement("td");
        var td4 = document.createElement("td");
        this.table = table;

        $(td1).text("");
        $(td2).text("Kanonická forma");
        $(td3).text("Typ");
        $(td4).text("Popis");
        $(headerTr)
            .append(td1)
            .append(td2)
            .append(td3)
            .append(td4);
        $(table).append(headerTr);

        for (var i = 0; i < this.item.CanonicalFormList.length; i++) {
            var canonicalFormItem = this.item.CanonicalFormList[i];
            var canonicalForm = new LemmatizationCanonicalForm(this.item.Id, table, canonicalFormItem);
            canonicalForm.make(this.addNewEmptyRow.bind(this));
        }
        this.addNewEmptyRow();

        $(tableDiv).append(table);

        $(this.container)
            .append(descriptionDiv)
            .append(morphologicalDiv)
            .append(tableDiv);
    }

    private addNewEmptyRow() {
        var canonicalForm = new LemmatizationCanonicalForm(this.item.Id, this.table);
        canonicalForm.make(this.addNewEmptyRow.bind(this));
    }
}

class LemmatizationCanonicalForm {
    private static searchBox: LemmatizationSearchBox;
    private static hyperSearchBox: LemmatizationSearchBox;
    private newCanonicalFormCreatedCallback: (form: ICanonicalForm) => void;
    private tableContainer: HTMLTableElement;
    private canonicalForm: ICanonicalForm;
    private tokenCharacteristicId: number;
    private containerCanonicalForm: HTMLDivElement;
    private containerType: HTMLDivElement;
    private containerDescription: HTMLDivElement;
    private editButton: HTMLButtonElement;
    private hyperCanonicalForm: LemmatizationHyperCanonicalForm;

    constructor(tokenCharacteristicId: number, tableContainer: HTMLTableElement, canonicalForm: ICanonicalForm = null) {
        this.tableContainer = tableContainer;
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
        $(editButton).text(this.canonicalForm ? "E" : "+");
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


        $(td1).append(editButton);
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
        $(editHyperButton).text("+");

        if (this.canonicalForm) {
            if (this.canonicalForm.HyperCanonicalForm) {
                var hyperCanonicalForm = this.canonicalForm.HyperCanonicalForm;

                $(hyperTd2).text(hyperCanonicalForm.Text);
                $(hyperTd3).text(LemmatizationCanonicalForm.hyperTypeToString(hyperCanonicalForm.Type));
                $(hyperTd4).text(hyperCanonicalForm.Description);
                $(editHyperButton).text("E");
            }
        } else {
            $(editHyperButton).addClass("hidden");
        }

        $(editHyperButton).click(() => {
            if (this.canonicalForm.HyperCanonicalForm)
                this.showEditHyperDialog();
            else
                this.showCreateHyperDialog();
        });

        $(hyperTd1).append(editHyperButton);
        $(hyperTd2).append(containerHyperForm);
        $(hyperTd3).append(containerHyperType);
        $(hyperTd4).append(containerHyperDescription);
        $(hyperTr).addClass("hyper-canonical-form-row")
            .append(hyperTd1)
            .append(hyperTd2)
            .append(hyperTd3)
            .append(hyperTd4);

        $(this.tableContainer).append(tr);
        $(this.tableContainer).append(hyperTr);

        this.containerCanonicalForm = containerCanonicalForm;
        this.containerType = containerType;
        this.containerDescription = containerDescription;
        this.editButton = editButton;

        this.hyperCanonicalForm = new LemmatizationHyperCanonicalForm();
        this.hyperCanonicalForm.containerName = containerHyperForm;
        this.hyperCanonicalForm.containerType = containerHyperType;
        this.hyperCanonicalForm.containerDescription = containerHyperDescription;
        this.hyperCanonicalForm.editButton = editHyperButton;
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
            alert("todo save edited");
        });

        $("#edit-form-text").val(this.canonicalForm.Text);
        $("#edit-form-type").val(String(this.canonicalForm.Type));
        $("#edit-form-description").val(this.canonicalForm.Description);
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
                type: "GET",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/CreateCanonicalForm",
                data: {
                    tokenCharacteristicId: this.tokenCharacteristicId,
                    text: name,
                    type: formType,
                    description: description
                },
                dataType: "json",
                contentType: "application/json",
                success: (newCanonicalFormId) => {
                    this.updateUiAfterItemCreation(newCanonicalFormId, name, formType, description);
                }
            });
        } else {
            var searchBox = LemmatizationCanonicalForm.searchBox;
            var currentItem: ICanonicalForm = <ICanonicalForm>(searchBox.getValue());

            if (!currentItem)
                return; //TODO show error

            $.ajax({
                type: "GET",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/AddCanonicalForm",
                data: {
                    tokenCharacteristicId: this.tokenCharacteristicId,
                    canonicalFormId: currentItem.Id
                },
                dataType: "json",
                contentType: "application/json",
                success: () => {
                    this.updateUiAfterItemCreation(currentItem.Id, currentItem.Text, currentItem.Type, currentItem.Description);
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
                type: "GET",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/CreateHyperCanonicalForm",
                data: {
                    canonicalFormId: this.canonicalForm.Id,
                    text: name,
                    type: formType,
                    description: description
                },
                dataType: "json",
                contentType: "application/json",
                success: (newHyperCanonicalFormId) => {
                    this.updateUiAfterHyperItemCreation(newHyperCanonicalFormId, name, formType, description);
                }
            });
        } else {
            throw "Prepare typeahead searchbox first!";
            var searchBox = LemmatizationCanonicalForm.searchBox;
            var currentItem: ICanonicalForm = <ICanonicalForm>(searchBox.getValue());

            if (!currentItem)
                return; //TODO show error

            $.ajax({
                type: "GET",
                traditional: true,
                url: getBaseUrl() + "Lemmatization/SetHyperCanonicalForm",
                data: {
                    tokenCharacteristicId: this.tokenCharacteristicId,
                    canonicalFormId: currentItem.Id
                },
                dataType: "json",
                contentType: "application/json",
                success: () => {
                 //   this.updateUiAfterHyperItemCreation(currentItem.Id, currentItem.Text, currentItem.Type, currentItem.Description);
                }
            });
        }
    }

    private updateUiAfterItemCreation(newId: number, name: string, formType: CanonicalFormTypeEnum, description: string) {
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
        $(this.editButton).text("E");
        $(this.hyperCanonicalForm.editButton).removeClass("hidden");

        this.newCanonicalFormCreatedCallback(this.canonicalForm);
        $("#newCanonicalFormDialog").modal("hide");
    }

    private updateUiAfterHyperItemCreation(newId: number, name: string, formType: HyperCanonicalFormTypeEnum, description: string) {
        this.canonicalForm.HyperCanonicalForm = {
            Id: newId,
            Text: name,
            Type: formType,
            Description: description
        }

        var hyperCanonicalForm = this.canonicalForm.HyperCanonicalForm;
        $(this.hyperCanonicalForm.containerName).text(hyperCanonicalForm.Text);
        $(this.hyperCanonicalForm.containerDescription).text(hyperCanonicalForm.Description);
        $(this.hyperCanonicalForm.containerType).text(LemmatizationCanonicalForm.hyperTypeToString(hyperCanonicalForm.Type));
        $(this.hyperCanonicalForm.editButton).text("E");

        $("#newHyperCanonicalFormDialog").modal("hide");
    }

    private updateItem() {
        //$.ajax({
        //    type: "GET",
        //    traditional: true,
        //    url: getBaseUrl() + "Lemmatization/AddTokenCharacteristic",
        //    data: {
        //        tokenId: this.tokenId,
        //        morphologicalCharacteristic: this.currentValue,
        //        description: description
        //    },
        //    dataType: "json",
        //    contentType: "application/json",
        //    success: (newTokenId) => {
        //        $("#newTokenCharacteristic").modal("hide");

        //        //todo show new empty characteristic
        //    }
        //});
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
            remote: remoteOptions
        });

        var suggestionTemplate = this.suggestionTemplate ? this.suggestionTemplate : this.getDefaultSuggestionTemplate;
        var dataset: Twitter.Typeahead.Dataset = {
            name: name,
            limit: 10,
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