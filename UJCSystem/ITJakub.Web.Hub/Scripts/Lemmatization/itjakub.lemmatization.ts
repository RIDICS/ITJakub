$(document).ready(() => {
    var lemmatization = new Lemmatization("#mainContainer");
    lemmatization.make();
});

class Lemmatization {
    private mainContainer: string;
    private searchBox: LemmatizationSearchBox;

    constructor(mainContainer: string) {
        this.mainContainer = mainContainer;
        this.searchBox = new LemmatizationSearchBox("#mainSearchInput");
    }

    public make() {
        $(this.mainContainer).empty();
        this.searchBox.setDataSet("Token");
        this.searchBox.create((selectedExists: boolean) => {
            if (selectedExists)
                $("#addNewTokenButton").addClass("hidden");
            else
                $("#addNewTokenButton").removeClass("hidden");
        });

        $("#loadButton").click(() => {
            this.loadToken(this.searchBox.getValue());
        });

        $("#addNewTokenButton").click(() => {
            $("#newTokenDialog").modal("show");
        });

        $("#addNewCharacteristic").click(() => {
            $("#newTokenCharacteristic").modal("show");
        });
    }

    private loadToken(item: ILemmatizationSearchBoxItem) {
        $("#specificToken").text(item.Text);
        $("#specificTokenDescription").text(item.Description);
    }
}

class LemmatizationSearchBox {
    private inputField: string;
    private urlWithController: string;
    private options: Twitter.Typeahead.Options;
    private dataset: Twitter.Typeahead.Dataset;
    private bloodhound: Bloodhound<string>;
    private currentItem: ILemmatizationSearchBoxItem;

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

    getValue(): ILemmatizationSearchBoxItem {
        return this.currentItem;
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

            var currentText = $(".tt-input", e.target.parentNode).val();
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

    clearAndDestroy(): void {
        if (this.bloodhound) {
            this.bloodhound.clear();
            this.bloodhound.clearPrefetchCache();
            this.bloodhound.clearRemoteCache();
        }

        this.dataset = null;
        this.bloodhound = null;
        this.destroy();
    }

    setDataSet(name: string, parameterUrlString: string = null): void {
        this.clearAndDestroy();
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

    private getSuggestionTemplate(item: ILemmatizationSearchBoxItem) {
        return "<div><div class=\"suggestion\" style='font-weight: bold'>" + item.Text + "</div><div class=\"description\">" + item.Description + "</div></div>";
    }
}

interface ILemmatizationSearchBoxItem {
    Id: number;
    Text: string;
    Description: string;
}