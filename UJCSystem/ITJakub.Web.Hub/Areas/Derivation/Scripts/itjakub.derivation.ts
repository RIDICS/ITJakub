class Derivation {
    private container: string;
    private searchBox: DerivationSearchBox;
    private idList: Array<number>;
    private tbody: HTMLTableSectionElement;

    constructor(container: string) {
        this.container = container;
        this.searchBox = new DerivationSearchBox("#mainSearchInput");
    }

    public make() {
        var createHyperOption = (value: DHyperCanonicalFormTypeEnum): HTMLOptionElement => {
            var label = this.hyperTypeToString(value);
            var element = document.createElement("option");
            $(element).attr("value", value);
            $(element).text(label);
            return element;
        };
        
        var selectedChangedCallback = (selectedExists: boolean, selectionConfirmed: boolean) => {
            if (selectedExists || this.searchBox.getInputValue() === "") {
                $("#loadButton").removeClass("hidden");
            } else {
                $("#loadButton").addClass("hidden");
            }

            if (selectionConfirmed) {
                this.loadData(<IDHyperCanonicalForm>this.searchBox.getValue());
            }
        };
        
        $("#type-select")
            .append(createHyperOption(DHyperCanonicalFormTypeEnum.HyperLemma))
            .append(createHyperOption(DHyperCanonicalFormTypeEnum.HyperStemma));

        this.searchBox.setDataSet("HyperCanonicalForm", "type=0");
        this.searchBox.create(selectedChangedCallback);

        $("#type-select").on("change", (e) => {
            var value = $(e.target).val();
            this.searchBox.setDataSet("HyperCanonicalForm", "type=" + value);
            this.searchBox.create(selectedChangedCallback);
            this.searchBox.reload();
        });

        $("#loadButton").click(() => {
            var item = <IDHyperCanonicalForm>this.searchBox.getValue();
            this.loadData(item);
        });
    }

    private loadData(value: IDHyperCanonicalForm) {
        $("#descriptionContainer").text(value.Description);

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Derivation/Derivation/GetCanonicalFormIdList",
            data: {
                hyperCanonicalFormId: value.Id
            },
            dataType: "json",
            contentType: "application/json",
            success: (idList) => {
                this.idList = idList;
                this.initTable();
            }
        });
    }

    private initTable() {
        $(this.container).empty();
        
        var table = document.createElement("table");
        var thead = document.createElement("thead");
        var headerTr = document.createElement("tr");
        var th1 = document.createElement("th");
        var th2 = document.createElement("th");
        var th3 = document.createElement("th");
        var tbody = document.createElement("tbody");
        this.tbody = tbody;
        
        $(th1).addClass("column-canonical-form")
            .text("Kanonická forma");
        $(th2).addClass("column-canonical-form")
            .text("Morfologická charakteristika");
        $(th3).addClass("column-canonical-form")
            .text("Token");
        $(headerTr)
            .append(th1)
            .append(th2)
            .append(th3);
        $(thead).append(headerTr);
        $(table)
            .addClass("derivation-table")
            .append(thead)
            .append(tbody);

        for (var i = 0; i < this.idList.length; i++) {
            var tr = document.createElement("tr");
            var td = document.createElement("td");
            $(td).addClass("column-canonical-form")
                .addClass("loading")
                .attr("colspan", 3);
            $(tr).addClass("lazy-loading")
                .append(td)
                .attr("data-id", this.idList[i])
                .bind("appearing", event => {
                    this.onTableRowAppear(event);
                });
            
            $(this.tbody).append(tr);
        }

        $(this.container).append(table);
    }

    private onTableRowAppear(event: JQueryEventObject) {
        var tr = event.target;
        var id = $(tr).data("id");
        $(tr).unbind("appearing")
            .removeClass("lazy-loading");
        this.loadCanonicalForm(id);
    }

    private loadCanonicalForm(id: number) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Derivation/Derivation/GetCanonicalFormDetail",
            data: {
                canonicalFormId: id
            },
            dataType: "json",
            contentType: "application/json",
            success: (canonicalForm: IInverseCanonicalForm) => {
                this.processCanonicalForm(canonicalForm);
            }
        });
    }

    private processCanonicalForm(canonicalForm: IInverseCanonicalForm) {
        var rows = 1;
        var tr = $("tr[data-id=\"" + canonicalForm.Id + "\"]", this.tbody);
        var td1 = document.createElement("td");
        $(td1).text(canonicalForm.Text);
        tr.empty();
        tr.append(td1);

        var characteristicCount = canonicalForm.CanonicalFormFor.length;
        if (characteristicCount > 1)
            rows = characteristicCount;
        for (var i = 0; i < characteristicCount; i++) {
            var characteristic = canonicalForm.CanonicalFormFor[i];
            if (i > 0) {
                var lastTr = tr;
                tr = $(document.createElement("tr"));
                lastTr.after(tr);
            }

            var td2 = document.createElement("td");
            $(td2).text(characteristic.MorphologicalCharacteristic);
            tr.append(td2);

            var td3 = document.createElement("td");
            $(td3).text(characteristic.Token.Text);
            tr.append(td3);
        }

        $(td1).attr("rowspan", rows);
    }

    private hyperTypeToString(hyperCanonicalForm: DHyperCanonicalFormTypeEnum): string {
        switch (hyperCanonicalForm) {
            case DHyperCanonicalFormTypeEnum.HyperLemma:
                return "Hyperlemma";
            case DHyperCanonicalFormTypeEnum.HyperStemma:
                return "Hyperstemma";
            default:
                return "";
        }
    }
}

class DerivationSearchBox {
    private inputField: string;
    private suggestionTemplate: (item: any) => string;
    private urlWithController: string;
    private options: Twitter.Typeahead.Options;
    private dataset: Twitter.Typeahead.Dataset;
    private bloodhound: Bloodhound<string>;
    private currentItem: IDTypeaheadItem;

    constructor(inputFieldElement: string, suggestionTemplate: (item: any) => string = null) {
        this.inputField = inputFieldElement;
        this.suggestionTemplate = suggestionTemplate;
        this.urlWithController = getBaseUrl() + "Derivation/Derivation";

        this.options = {
            hint: true,
            highlight: false,
            minLength: 1
        };
    }

    setValue(value: any): void {
        $(this.inputField).typeahead('val', value);
    }

    getValue(): IDTypeaheadItem {
        return this.currentItem;
    }

    getInputValue(): string {
        return <any>($(this.inputField).typeahead("val"));
    }

    create(selectionChangedCallback: (selectedExists: boolean, selectConfirmed: boolean) => void): void {
        var self = this;
        $(this.inputField).typeahead(this.options, this.dataset);
        $(this.inputField).bind("typeahead:render", <any>function (e, ...datums) {
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

    private getDefaultSuggestionTemplate(item: IDToken): string {
        return "<div><div class=\"suggestion\" style='font-weight: bold'>" + item.Text + "</div><div class=\"description\">" + item.Description + "</div></div>";
    }
}

interface IDTypeaheadItem {
    Text: string;
    Description: string;
}

interface IDToken {
    Id: number;
    Text: string;
    Description: string;
}

interface IDHyperCanonicalForm {
    Id: number;
    Text: string;
    Description: string;
    Type: DHyperCanonicalFormTypeEnum;
}

interface IInverseTokenCharacteristic {
    Id: number;
    MorphologicalCharacteristic: string;
    Description: string;
    Token: IDToken;
}

interface IInverseCanonicalForm {
    Id: number;
    Text: string;
    Description: string;
    Type: DCanonicalFormTypeEnum;
    CanonicalFormFor: Array<IInverseTokenCharacteristic>;
}

enum DCanonicalFormTypeEnum {
    Lemma = 0,
    Stemma = 1,
    LemmaOld = 2,
    StemmaOld = 3,
}

enum DHyperCanonicalFormTypeEnum {
    HyperLemma = 0,
    HyperStemma = 1,
}