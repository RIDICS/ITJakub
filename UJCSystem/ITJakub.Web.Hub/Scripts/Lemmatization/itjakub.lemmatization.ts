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
        this.searchBox.create((suggestionsCount: number) => {
            console.log("suggestions: " + suggestionsCount);
        });


    }

    
}

class LemmatizationSearchBox {
    private inputField: string;
    private urlWithController: string;
    private options: Twitter.Typeahead.Options;
    private dataset: Twitter.Typeahead.Dataset;
    private bloodhound: Bloodhound<string>;

    constructor(inputFieldElement: string) {
        this.inputField = inputFieldElement;
        this.urlWithController = getBaseUrl() + "Lemmatization";

        this.options = {
            hint: true,
            highlight: true,
            minLength: 1
        };
    }
    
    value(value: any): void {
        $(this.inputField).typeahead('val', value);
    }

    create(selectionChangedCallback: (suggestionCount: number) => void): void {
        $(this.inputField).typeahead(this.options, this.dataset);
        $(this.inputField).bind("typeahead:render", <any>function (e, suggestions) {
            selectionChangedCallback(suggestions ? suggestions.length : 0);
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
        var prefetchUrl: string = this.urlWithController + "/GetTypeahead" + name;
        var remoteUrl: string = this.urlWithController + "/GetTypeahead" + name + "?query=%QUERY";

        if (parameterUrlString != null) {
            prefetchUrl += "?" + parameterUrlString;
            remoteUrl += "&" + parameterUrlString;
        }

        var remoteOptions: Bloodhound.RemoteOptions<string> = {
            url: remoteUrl,
            wildcard: "%QUERY"
        };

        var bloodhound: Bloodhound<string> = new Bloodhound({
            datumTokenizer: Bloodhound.tokenizers.whitespace,
            queryTokenizer: Bloodhound.tokenizers.whitespace,
            prefetch: prefetchUrl,
            remote: remoteOptions
        });

        var dataset: Twitter.Typeahead.Dataset = {
            name: name,
            limit: 10,
            source: bloodhound,
            templates: {
                suggestion: this.getSuggestionTemplate
            }
        };

        this.bloodhound = bloodhound;
        this.dataset = dataset;
    }

    private getSuggestionTemplate(item: ILemmatizationSearchBoxItem) {
        return "<div class=\"suggestion\">" + item.Text + "</div><div class=\"description\">" + item.Description + "</div>";
    }
}

interface ILemmatizationSearchBoxItem {
    Text: string;
    Description: string;
}