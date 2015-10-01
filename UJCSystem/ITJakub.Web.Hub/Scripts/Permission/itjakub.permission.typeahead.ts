
class ConcreteInstanceSearchBox {
    private inputField: string;
    private itemToPrintableConverter: (item: any) => IPrintableItem;
    private urlWithController: string;
    private options: Twitter.Typeahead.Options;
    private dataset: Twitter.Typeahead.Dataset;
    private bloodhound: Bloodhound<string>;
    private currentItem: any;

    constructor(inputFieldElement: string, controllerName: string, itemToPrintableConverter: (item: any) => IPrintableItem) {
        this.inputField = inputFieldElement;
        this.itemToPrintableConverter = itemToPrintableConverter;
        this.urlWithController = getBaseUrl() + controllerName;

        this.options = {
            hint: true,
            highlight: false,
            minLength: 1
        };
    }

    setValue(value: any): void {
        $(this.inputField).typeahead('val', value);
    }

    getValue(): any {
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
            remote: remoteOptions,
            limit: 10
        });

        var suggestionTemplate = (item) => { return this.getDefaultSuggestionTemplate(item) };

        var dataset: Twitter.Typeahead.Dataset = {
            name: name,
            source: bloodhound,
            displayKey: "Text",
            templates: {
                suggestion: suggestionTemplate
            }
        };

        this.bloodhound = bloodhound;
        this.dataset = dataset;
    }

    private getDefaultSuggestionTemplate(item: any): string {
        var printableItem = this.itemToPrintableConverter(item);
        return "<div><div class=\"suggestion\" style='font-weight: bold'>" + printableItem.Name + "</div><div class=\"description\">" + printableItem.Description + "</div></div>";
    }
}

