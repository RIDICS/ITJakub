class Derivation {
    private container: string;
    private searchBox: SingleSetTypeaheadSearchBox<IHyperCanonicalForm>;
    private idList: Array<number>;
    private tbody: HTMLTableSectionElement;

    constructor(container: string) {
        this.container = container;
        this.searchBox = new SingleSetTypeaheadSearchBox<IHyperCanonicalForm>("#mainSearchInput",
            "Derivation/Derivation",
            (item) => item.text,
            (item) => SingleSetTypeaheadSearchBox.getDefaultSuggestionTemplate(item.text, item.description));
    }

    public make() {
        var createHyperOption = (value: HyperCanonicalFormTypeEnum): HTMLOptionElement => {
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
                this.loadData(this.searchBox.getValue());
            }
        };
        
        $("#type-select")
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperLemma))
            .append(createHyperOption(HyperCanonicalFormTypeEnum.HyperStemma));

        this.searchBox.setDataSet("HyperCanonicalForm", "type=0");
        this.searchBox.create(selectedChangedCallback);

        $("#type-select").on("change", (e) => {
            var value = $(e.target as Node as HTMLElement).val() as string;
            this.searchBox.setDataSet("HyperCanonicalForm", "type=" + value);
            this.searchBox.create(selectedChangedCallback);
            this.searchBox.reload();
        });

        $("#loadButton").click(() => {
            var item = this.searchBox.getValue();
            this.loadData(item);
        });
    }

    private loadData(value: IHyperCanonicalForm) {
        $("#descriptionContainer").text(value.description);

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Derivation/Derivation/GetCanonicalFormIdList",
            data: {
                hyperCanonicalFormId: value.id
            } as JQuery.PlainObject,
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
                    this.onTableRowAppear(event.target);
                });
            
            $(this.tbody).append(tr);
        }

        $(this.container).append(table);
    }

    private onTableRowAppear(targetElement: HTMLElement) {
        var tr = targetElement;
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
            } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success: (canonicalForm: IInverseCanonicalForm) => {
                this.processCanonicalForm(canonicalForm);
            }
        });
    }

    private processCanonicalForm(canonicalForm: IInverseCanonicalForm) {
        var rows = 1;
        var tr = $("tr[data-id=\"" + canonicalForm.id + "\"]", this.tbody);
        var td1 = document.createElement("td");
        $(td1).text(canonicalForm.text);
        tr.empty();
        tr.append(td1);

        var characteristicCount = canonicalForm.canonicalFormFor.length;
        if (characteristicCount > 1)
            rows = characteristicCount;
        for (var i = 0; i < characteristicCount; i++) {
            var characteristic = canonicalForm.canonicalFormFor[i];
            if (i > 0) {
                var lastTr = tr;
                tr = $(document.createElement("tr"));
                lastTr.after(tr);
            }

            var td2 = document.createElement("td");
            $(td2).text(characteristic.morphologicalCharacteristic);
            tr.append(td2);

            var td3 = document.createElement("td");
            $(td3).text(characteristic.token.text);
            tr.append(td3);
        }

        $(td1).attr("rowspan", rows);
    }

    private hyperTypeToString(hyperCanonicalForm: HyperCanonicalFormTypeEnum): string {
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
