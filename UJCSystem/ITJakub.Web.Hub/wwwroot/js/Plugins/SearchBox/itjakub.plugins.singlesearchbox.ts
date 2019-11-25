///<reference path="../../../js/typings-custom/typeahead-custom_.d.ts" />

class TypeaheadSearchBoxBase<T> {
    private displayPath: (item: T) => string;
    private suggestionTemplate: (item: T) => string;
    private inputField: string;
    private urlWithController: string;
    private options: Twitter.Typeahead.Options;
    protected datasets: Array<Twitter.Typeahead.Dataset<T>>;
    protected bloodhounds: Array<Bloodhound<T>>;
    private currentItem: T;

    constructor(inputFieldElement: string, controllerName: string, displayPath: (item: T) => string, suggestionTemplate: (item: T) => string = null, menuEl?: JQuery) {
        this.inputField = inputFieldElement;
        this.suggestionTemplate = suggestionTemplate;
        this.displayPath = displayPath;
        this.urlWithController = getBaseUrl() + controllerName;

        this.options = {
            hint: true,
            highlight: false,
            minLength: 1,
            menu: menuEl
        };
    }

    setValue(value: any): void {
        $(this.inputField).typeahead('val', value);
    }

    getValue(): T {
        return this.currentItem;
    }

    getInputValue(): string {
        return <any>$(this.inputField).typeahead("val");
    }

    create(selectionChangedCallback: (selectedExists: boolean, selectConfirmed: boolean) => void): void {
        var self = this;
        $(this.inputField).typeahead(this.options, this.datasets);
        $(this.inputField).bind("typeahead:render", <any>function (e, ...datums: T[]) {
            if (datums.length > 0) {
                var currentText = self.getInputValue();
                for (var i = 0; i < datums.length; i++) {
                    var text = self.displayPath(datums[i]);
                    if (text === currentText) {
                        self.currentItem = datums[i];
                        selectionChangedCallback(true, false);
                        return;
                    }
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
        //$(this.inputField).change(function () {
        //    if (!$(this).val()) {
        //        self.currentItem = null;
        //        selectionChangedCallback(false, false);
        //    }
        //});
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
        if (!this.bloodhounds)
            return;

        for (var i = 0; i < this.bloodhounds.length; i++) {
            var bloodhound = this.bloodhounds[i];
            bloodhound.clear();
            bloodhound.clearPrefetchCache();
            bloodhound.clearRemoteCache();
        }
    }

    protected createDataSet(name: string, groupHeader: string, parameterUrlString: string): ITuple<Bloodhound<T>, Twitter.Typeahead.Dataset<T>> {
        var remoteUrl: string = this.urlWithController + "/GetTypeahead" + name + "?query=%QUERY";

        if (parameterUrlString != null) {
            remoteUrl += "&" + parameterUrlString;
        }

        var remoteOptions: Bloodhound.RemoteOptions<T> = {
            url: remoteUrl,
            wildcard: "%QUERY"
        };

        var bloodhound: Bloodhound<T> = new Bloodhound({
            datumTokenizer: this.datumTokenizer,
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            remote: remoteOptions
        });

        var dataset: Twitter.Typeahead.Dataset<T> = {
            name: name,
            source: bloodhound,
            display: this.displayPath,
            limit: 10,
            templates: {}
        };
        if (this.suggestionTemplate) {
            dataset.templates.suggestion = this.suggestionTemplate;
        }
        if (groupHeader) {
            dataset.templates.header = `<div class="tt-suggestions-header">${groupHeader}</div>`;
        }

        return {
            item1: bloodhound,
            item2: dataset
        };
    }

    private datumTokenizer(datum: T): string[] {
        var text = this.displayPath(datum);
        return Bloodhound.tokenizers.whitespace(text);
    }

    public static getDefaultSuggestionTemplate(name: string, description: string) {
        return `<div><div class="suggestion" style='font-weight: bold'>${escapeHtmlChars(name)}</div><div class="description">${escapeHtmlChars(description)}</div></div>`;
    }
}

class SingleSetTypeaheadSearchBox<T> extends TypeaheadSearchBoxBase<T> {
    setDataSet(name: string, parameterUrlString: string = null): void {
        this.clearCache();
        this.destroy();

        var result = this.createDataSet(name, null, parameterUrlString);

        this.bloodhounds = [result.item1];
        this.datasets = [result.item2];
    }
}

class MultiSetTypeaheadSearchBox<T> extends TypeaheadSearchBoxBase<T> {
    clearDataSets(): void {
        this.clearCache();
        this.destroy();
        this.bloodhounds = null;
        this.datasets = null;
    }

    addDataSet(name: string, groupHeader: string, parameterUrlString: string = null): void {
        var result = this.createDataSet(name, groupHeader, parameterUrlString);

        if (!this.bloodhounds) this.bloodhounds = [];
        if (!this.datasets) this.datasets = [];

        this.bloodhounds.push(result.item1);
        this.datasets.push(result.item2);
    }

    public static getDefaultSuggestionTemplateMulti(name: string, description: string) {
        return `<div style="padding-left: 1.5rem;">${TypeaheadSearchBoxBase.getDefaultSuggestionTemplate(name, description)}</div>`;
    }
}