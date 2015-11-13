class Derivation {
    private container: string;
    private searchBox: LemmatizationSearchBox;
    private idList: Array<number>;
    private tbody: HTMLTableSectionElement;

    constructor(container: string) {
        this.container = container;
        this.searchBox = new LemmatizationSearchBox("#mainSearchInput");
    }

    public make() {
        var createHyperOption = (value: HyperCanonicalFormTypeEnum): HTMLOptionElement => {
            var label = LemmatizationCanonicalForm.hyperTypeToString(value);
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
                this.loadData(<IHyperCanonicalForm>this.searchBox.getValue());
            }
        };
        
        $("#type-select")
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperLemma))
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperStemma));

        this.searchBox.setDataSet("HyperCanonicalForm", "type=0");
        this.searchBox.create(selectedChangedCallback);

        $("#type-select").on("change", (e) => {
            var value = $(e.target).val();
            this.searchBox.setDataSet("HyperCanonicalForm", "type=" + value);
            this.searchBox.create(selectedChangedCallback);
            this.searchBox.reload();
        });

        $("#loadButton").click(() => {
            var item = <IHyperCanonicalForm>this.searchBox.getValue();
            this.loadData(item);
        });
    }

    private loadData(value: IHyperCanonicalForm) {
        $("#descriptionContainer").text(value.Description);

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Lemmatization/GetCanonicalFormIdList",
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
            .addClass("lemmatization-table")
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
            url: getBaseUrl() + "Lemmatization/GetCanonicalFormDetail",
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
}

interface IInverseTokenCharacteristic {
    Id: number;
    MorphologicalCharacteristic: string;
    Description: string;
    Token: IToken;
}

interface IInverseCanonicalForm {
    Id: number;
    Text: string;
    Description: string;
    Type: CanonicalFormTypeEnum;
    CanonicalFormFor: Array<IInverseTokenCharacteristic>;
}