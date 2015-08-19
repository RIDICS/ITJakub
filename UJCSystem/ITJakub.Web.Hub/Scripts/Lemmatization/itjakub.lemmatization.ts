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
        this.searchBox.create((selectedExists: boolean) => {
            if (selectedExists || this.searchBox.getInputValue() === "") {
                $("#addNewTokenButton").addClass("hidden");
                $("#loadButton").removeClass("hidden");
            } else {
                $("#addNewTokenButton").removeClass("hidden");
                $("#loadButton").addClass("hidden");
            }
        });
        this.lemmatizationCharacteristic.init();

        $("#loadButton").click(() => {
            var tokenItem = this.searchBox.getValue();
            this.loadToken(tokenItem);
        });

        $("#addNewTokenButton").click(() => {
            this.showAddNewToken();
        });

        $("#addNewCharacteristic").click(() => {
            this.lemmatizationCharacteristic.show(this.currentTokenItem.Id);
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

    show(tokenId: number) {
        this.tokenId = tokenId;
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
            success: (newTokenId) => {
                $("#newTokenCharacteristic").modal("hide");

                //todo show new empty characteristic
            }
        });
    }
}

class LemmatizationCharacteristicTable {
    private container: HTMLDivElement;
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

        $(td1).text("Kanonická forma");
        $(td2).text("Typ");
        $(td3).text("Popis");
        $(headerTr)
            .append(td1)
            .append(td2)
            .append(td3);
        $(table).append(headerTr);

        for (var i = 0; i < this.item.CanonicalFormList.length; i++) {
            var canonicalFormItem = this.item.CanonicalFormList[i];
            var canonicalForm = new LemmatizationCanonicalForm(this.item.Id, canonicalFormItem, table);
            canonicalForm.make();
        }

        this.addRowWithNewLineCommand(table);
        $(tableDiv).append(table);

        $(this.container)
            .append(descriptionDiv)
            .append(morphologicalDiv)
            .append(tableDiv);
    }

    private addRowWithNewLineCommand(table: HTMLTableElement) {
        var tr = document.createElement("tr");
        var td1 = document.createElement("td");
        var td2 = document.createElement("td");
        var td3 = document.createElement("td");

        var newFormButton = document.createElement("button");
        $(newFormButton).text("+");

        $(td1).append(newFormButton);
        $(td2).text("");
        $(td3).text("");
        $(tr).append(td1)
            .append(td2)
            .append(td3);
        $(table).append(tr);
    }
}

class LemmatizationCanonicalForm {
    private tableContainer: HTMLTableElement;
    private canonicalForm: ICanonicalForm;
    private tokenId: number;

    constructor(tokenCharacteristicId: number, canonicalForm: ICanonicalForm, tableContainer: HTMLTableElement) {
        this.tableContainer = tableContainer;
        this.canonicalForm = canonicalForm;
        this.tokenId = tokenCharacteristicId;
    }

    make() {
        var tr = document.createElement("tr");
        var td1 = document.createElement("td");
        var td2 = document.createElement("td");
        var td3 = document.createElement("td");

        $(td1).text(this.canonicalForm.Text);
        $(td2).text(this.typeToString(this.canonicalForm.Type));
        $(td3).text(this.canonicalForm.Description);

        $(tr).append(td1)
            .append(td2)
            .append(td3);

        $(this.tableContainer).append(tr);
    }

    private typeToString(canonicalFormType: CanonicalFormTypeEnum): string {
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
}

class LemmatizationSearchBox {
    private inputField: string;
    private urlWithController: string;
    private options: Twitter.Typeahead.Options;
    private dataset: Twitter.Typeahead.Dataset;
    private bloodhound: Bloodhound<string>;
    private currentItem: IToken;

    constructor(inputFieldElement: string) {
        this.inputField = inputFieldElement;
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

    getValue(): IToken {
        return this.currentItem;
    }

    getInputValue(): string {
        return <any>($(this.inputField).typeahead("val"));
    }

    create(selectionChangedCallback: (selectedExists: boolean) => void): void {
        var self = this;
        $(this.inputField).typeahead(this.options, this.dataset);
        $(this.inputField).bind("typeahead:render", <any>function(e, ...datums) {
            var isEmpty = $(".tt-menu", e.target.parentNode).hasClass("tt-empty");
            if (isEmpty) {
                self.currentItem = null;
                selectionChangedCallback(false);
                return;
            }

            var currentText = self.getInputValue();
            var suggestionElements = $(".suggestion", e.target.parentNode);
            for (var i = 0; i < suggestionElements.length; i++) {
                if ($(suggestionElements[i]).text() === currentText) {
                    self.currentItem = datums[i];
                    selectionChangedCallback(true);
                    return;
                }
            }
            self.currentItem = null;
            selectionChangedCallback(false);
        });
        $(this.inputField).bind("typeahead:select", <any>function (e, datum) {
            self.currentItem = datum;
            selectionChangedCallback(true);
        });
        $(this.inputField).bind("typeahead:autocomplete", <any>function (e, datum) {
            self.currentItem = datum;
            selectionChangedCallback(true);
        });
    }

    destroy(): void {
        $(this.inputField).typeahead("destroy");
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

        var dataset: Twitter.Typeahead.Dataset = {
            name: name,
            limit: 10,
            source: bloodhound,
            display: "Text",
            templates: {
                suggestion: this.getSuggestionTemplate
            }
        };

        this.bloodhound = bloodhound;
        this.dataset = dataset;
    }

    private getSuggestionTemplate(item: IToken) {
        return "<div><div class=\"suggestion\" style='font-weight: bold'>" + item.Text + "</div><div class=\"description\">" + item.Description + "</div></div>";
    }
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