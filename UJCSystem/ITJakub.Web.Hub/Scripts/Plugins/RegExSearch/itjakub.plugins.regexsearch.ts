class RegExSearchBase {
    protected createOption(label: string, value: string): HTMLOptionElement {
        var conditionOption = document.createElement("option");
        conditionOption.innerHTML = label;
        conditionOption.value = value;

        return conditionOption;
    }

    protected createButton(label: string): HTMLButtonElement {
        var button: HTMLButtonElement = document.createElement("button");
        button.type = "button";
        button.innerHTML = label;
        $(button).addClass("btn");
        $(button).addClass("btn-default");
        $(button).addClass("regexsearch-button");

        return button;
    }
} 

class RegExSearch extends RegExSearchBase {
    container: HTMLDivElement;
    innerContainer: HTMLDivElement;
    regExConditions: Array<RegExConditions>;

    constructor(container: HTMLDivElement) {
        super();
        this.container = container;
    }

    public makeRegExSearch() {
        $(this.container).empty();
        this.regExConditions = [];

        var commandsDiv: HTMLDivElement = document.createElement("div");

        var addConditionsButton: HTMLButtonElement = this.createButton("Přidat podmínku");
        commandsDiv.appendChild(addConditionsButton);
        $(addConditionsButton).click(() => {
            this.addNewConditions();
        });

        var removeConditionsButton: HTMLButtonElement = this.createButton("Odebrat podmínku");
        $(removeConditionsButton).click(() => {
            this.removeLastConditions();
        });
        commandsDiv.appendChild(removeConditionsButton);

        this.innerContainer = document.createElement("div");
        this.addNewConditions(false);
        
        var endDelimiter: HTMLDivElement = document.createElement("div");
        endDelimiter.innerHTML = "&nbsp;";
        $(endDelimiter).addClass("regexsearch-delimiter");

        $(this.container).append(commandsDiv);
        $(this.container).append(this.innerContainer);
        $(this.container).append(endDelimiter);
    }

    private addNewConditions(useDelimiter:boolean = true) {
        var newRegExConditions = new RegExConditions(this);
        newRegExConditions.makeRegExCondition();
        if (!useDelimiter) {
            newRegExConditions.removeDelimeter();
        }
        this.regExConditions.push(newRegExConditions);
        $(this.innerContainer).append(newRegExConditions.getHtml());
    }

    private removeLastConditions() {
        if (this.regExConditions.length <= 1)
            return;

        var arrayItem: RegExConditions = this.regExConditions.pop();
        this.innerContainer.removeChild(arrayItem.getHtml());
    }

    public getConditionsString(): string {
        var outputString = "";

        for (var i = 0; i < this.regExConditions.length; i++) {
            var regExConditions = this.regExConditions[i];
            outputString += "(?=.*(" + regExConditions.getConditionsString() + "))";
        }

        return "^" + outputString + ".*$";
    }
}

class RegExConditions extends RegExSearchBase {
    private html: HTMLDivElement;
    private conditionsContainerDiv: HTMLDivElement;
    private conditionsInputArray: Array<RegExInput>;
    private parent: RegExSearch;
    private selectedSearchType: string;
    private selectedWordFormType: string;

    constructor(parent: RegExSearch) {
        super();
        this.parent = parent;
    }

    public getHtml(): HTMLDivElement {
        return this.html;
    }

    public removeDelimeter() {
        $(this.html).find(".regexsearch-delimiter").remove();
    }

    public hasDelimeter(): boolean {
        var delimeter = $(this.html).find(".regexsearch-delimiter");
        return (typeof delimeter != 'undefined' && delimeter != null);
    }

    private wordFormType = {
        Lemma: "lemma",
        HyperlemmaNew: "hyperlemma-new",
        HyperlemmaOld: "hyperlemma-old",
        Stemma: "stemma"
    }

    private searchType = {
        Text: "text",
        Author: "author",
        Title: "title",
        Responsible: "responsible"
    }

    public makeRegExCondition() {

        var mainDiv = document.createElement("div");

        var andInfoDiv = document.createElement("div");
        $(andInfoDiv).addClass("regexsearch-delimiter");
        andInfoDiv.innerHTML = "A zároveň";
        mainDiv.appendChild(andInfoDiv);
  

        var conditionsDiv = document.createElement("div");
        $(conditionsDiv).addClass("regexsearch-condition-main-div");
        conditionsDiv.appendChild(mainDiv);

        
        var mainSearchDiv: HTMLDivElement = document.createElement("div");

        var searchDestinationDiv: HTMLDivElement = document.createElement("div");
        $(searchDestinationDiv).addClass("regexsearch-destination-div");
        mainSearchDiv.appendChild(searchDestinationDiv);

        var searchDestinationSpan: HTMLSpanElement = document.createElement("span");
        searchDestinationSpan.innerHTML = "Zvolte oblast vyhledávání";
        $(searchDestinationSpan).addClass("regexsearch-upper-select-label");
        searchDestinationDiv.appendChild(searchDestinationSpan);

        var searchDestinationSelect: HTMLSelectElement = document.createElement("select");
        $(searchDestinationSelect).addClass("regexsearch-select");
        searchDestinationDiv.appendChild(searchDestinationSelect);

        searchDestinationSelect.appendChild(this.createOption("Text", this.searchType.Text));
        searchDestinationSelect.appendChild(this.createOption("Autor", this.searchType.Author));
        searchDestinationSelect.appendChild(this.createOption("Titul", this.searchType.Title));
        searchDestinationSelect.appendChild(this.createOption("Editor", this.searchType.Responsible));

        this.selectedSearchType = this.searchType.Text;

        //$(searchDestinationSelect).change(() => { //TODO make change method
        //    this.selectedSearchType = 
        //});

        var wordFormDiv: HTMLDivElement = document.createElement("div");
        $(wordFormDiv).addClass("regexsearch-word-form-div");
        mainSearchDiv.appendChild(wordFormDiv);

        var wordFormSpan: HTMLSpanElement = document.createElement("span");
        wordFormSpan.innerHTML = "Tvar slova";
        $(wordFormSpan).addClass("regexsearch-upper-select-label");
        wordFormDiv.appendChild(wordFormSpan);

        var wordFormSelect: HTMLSelectElement = document.createElement("select");
        $(wordFormSelect).addClass("regexsearch-select");
        wordFormDiv.appendChild(wordFormSelect);

        wordFormSelect.appendChild(this.createOption("Lemma", this.wordFormType.Lemma));
        wordFormSelect.appendChild(this.createOption("Hyperlemma - nové", this.wordFormType.HyperlemmaNew));
        wordFormSelect.appendChild(this.createOption("Hyperlemma - staré", this.wordFormType.HyperlemmaOld));
        wordFormSelect.appendChild(this.createOption("Stemma", this.wordFormType.Stemma));

        this.selectedWordFormType = this.wordFormType.Lemma;

        this.conditionsContainerDiv = document.createElement("div");
        $(this.conditionsContainerDiv).addClass("regexsearch-condition-list-div");
        mainSearchDiv.appendChild(this.conditionsContainerDiv);

        var commandsDiv: HTMLDivElement = document.createElement("div");
        $(commandsDiv).addClass("regexsearch-conditions-commands");
        mainSearchDiv.appendChild(commandsDiv);

        var addConditionButton: HTMLButtonElement = this.createButton("Přidat výraz");
        $(addConditionButton).click(() => {
            this.addCondition();
        });
        commandsDiv.appendChild(addConditionButton);

        this.resetConditions();
        $(conditionsDiv).append(mainSearchDiv);
        this.html = conditionsDiv;
    }

    public resetConditions() {
        $(this.conditionsContainerDiv).empty();
        this.conditionsInputArray = [];
        var newRegExInput = new RegExInput(this);
        newRegExInput.makeRegExInput();
        newRegExInput.removeDelimeter();
        this.conditionsInputArray.push(newRegExInput);        
        this.conditionsContainerDiv.appendChild(newRegExInput.getHtml());
    }

    public addCondition() {
        var newRegExInput = new RegExInput(this);
        newRegExInput.makeRegExInput();
        this.conditionsInputArray.push(newRegExInput);
        this.conditionsContainerDiv.appendChild(newRegExInput.getHtml());
    }

    public removeCondition(condition: RegExInput) {

        var index = this.conditionsInputArray.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.conditionsInputArray[index];
            this.conditionsContainerDiv.removeChild(arrayItem.getHtml());
            this.conditionsInputArray.splice(index, 1);
        }

        if (this.conditionsInputArray.length === 0) {
            this.resetConditions();
        }

        if (this.conditionsInputArray[0].hasDelimeter) {
            this.conditionsInputArray[0].removeDelimeter();
        }
    }

    public getConditionsString(): string {
        var conditionsString = this.conditionsInputArray[0].getConditionValue();
        for (var i = 1; i < this.conditionsInputArray.length; i++) {
            var regExInput = this.conditionsInputArray[i];
            conditionsString += "|" + regExInput.getConditionValue();
        }

        return conditionsString;
    }
}

class RegExInput extends RegExSearchBase {
    private html: HTMLDivElement;
    private editorDiv: HTMLDivElement;
    private conditionInput: HTMLInputElement;
    private regExEditor: RegExEditor;
    private parentRegExConditions: RegExConditions;

    constructor(parent: RegExConditions) {
        super();
        this.parentRegExConditions = parent;
    }

    public getHtml(): HTMLDivElement {
        return this.html;
    }

    public removeDelimeter() {
        $(this.html).find(".regexsearch-or-delimiter").remove();
    }

    public hasDelimeter(): boolean {
        var delimeter = $(this.html).find(".regexsearch-or-delimiter");
        return (typeof delimeter != 'undefined' && delimeter != null) ;
    }

    public makeRegExInput() {
        var mainDiv: HTMLDivElement = document.createElement("div");

        var labelDiv = document.createElement("div");
        labelDiv.innerHTML = "Nebo";
        $(labelDiv).addClass("regexsearch-or-delimiter");
        mainDiv.appendChild(labelDiv);

        var lineDiv = document.createElement("div");
        this.editorDiv = document.createElement("div");

        this.conditionInput = document.createElement("input");
        this.conditionInput.type = "text";
        $(this.conditionInput).addClass("form-control");
        $(this.conditionInput).addClass("regexsearch-condition-input");
        lineDiv.appendChild(this.conditionInput);

        var regExButton: HTMLButtonElement = document.createElement("button");
        regExButton.innerText = "R";
        regExButton.type = "button";
        $(regExButton).addClass("btn");
        $(regExButton).addClass("regexsearch-condition-input-button");
        $(regExButton).click(() => {
            if (!this.regExEditor || this.editorDiv.children.length === 0) {
                this.regExEditor = new RegExEditor(this.editorDiv, this.conditionInput);
                this.regExEditor.makeRegExEditor();
            } else if ($(this.editorDiv).hasClass("hidden")) {
                $(this.editorDiv).removeClass("hidden");
            } else {
                $(this.editorDiv).addClass("hidden");
            }
        });
        lineDiv.appendChild(regExButton);

        var removeButton: HTMLButtonElement = this.createButton("");
        var removeGlyph: HTMLSpanElement = document.createElement("span");
        $(removeGlyph).addClass("glyphicon");
        $(removeGlyph).addClass("glyphicon-trash");
        removeButton.appendChild(removeGlyph);
        $(removeButton).click(() => {
            this.parentRegExConditions.removeCondition(this);
        });

        lineDiv.appendChild(removeButton);

        mainDiv.appendChild(lineDiv);
        mainDiv.appendChild(this.editorDiv);
        this.html = mainDiv;
    }

    public getConditionValue(): string {
        return this.conditionInput.value;
    }
}

class RegExEditor extends RegExSearchBase {
    container: HTMLDivElement;
    searchBox: HTMLInputElement;

    constructor(container: HTMLDivElement, searchBox: HTMLInputElement) {
        super();
        this.container = container;
        this.searchBox = searchBox;
    }

    private conditionType = Object.freeze({
        StartsWith: "starts-with",
        NotStartsWith: "not-starts-with",
        Contains: "contains",
        NotContains: "not-contains",
        EndsWith: "ends-with",
        NotEndsWith: "not-ends-with"
    });

    public makeRegExEditor() {
        $(this.container).empty();
        
        var mainRegExDiv: HTMLDivElement = document.createElement("div");
        $(mainRegExDiv).addClass("content-container");
        $(mainRegExDiv).addClass("regexsearch-editor-container");

        var titleHeading: HTMLSpanElement = document.createElement("span");
        titleHeading.innerHTML = "Editor regulárního výrazu";
        $(titleHeading).addClass("regexsearch-editor-title");
        mainRegExDiv.appendChild(titleHeading);

        var editorDiv: HTMLDivElement = document.createElement("div");
        mainRegExDiv.appendChild(editorDiv);

        var conditionTitleDiv: HTMLDivElement = document.createElement("div");
        conditionTitleDiv.innerHTML = "Podmínka";
        editorDiv.appendChild(conditionTitleDiv);


        var conditionTypeDiv: HTMLDivElement = document.createElement("div");
        $(conditionTypeDiv).addClass("regexsearch-condition-type-div");
        editorDiv.appendChild(conditionTypeDiv);

        var conditionSelect: HTMLSelectElement = document.createElement("select");
        $(conditionSelect).addClass("regexsearch-condition-select");
        conditionTypeDiv.appendChild(conditionSelect);

        conditionSelect.appendChild(this.createOption("Začíná na", this.conditionType.StartsWith));
        conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(this.createOption("Obsahuje", this.conditionType.Contains));
        conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(this.createOption("Končí na", this.conditionType.EndsWith));
        conditionSelect.appendChild(this.createOption("Nekončí na", this.conditionType.NotEndsWith));


        var conditionDiv: HTMLDivElement = document.createElement("div");
        $(conditionDiv).addClass("regexsearch-condition-div");
        editorDiv.appendChild(conditionDiv);

        var conditionInputDiv: HTMLDivElement = document.createElement("div");
        conditionDiv.appendChild(conditionInputDiv);

        var conditionInput: HTMLInputElement = document.createElement("input");
        conditionInput.type = "text";
        $(conditionInput).addClass("regexsearch-input");
        conditionInputDiv.appendChild(conditionInput);

        var conditionButtonsDiv: HTMLDivElement = document.createElement("div");
        conditionDiv.appendChild(conditionButtonsDiv);

        var anythingButton: HTMLButtonElement = this.createButton("Cokoliv");
        conditionButtonsDiv.appendChild(anythingButton);
        $(anythingButton).addClass("regexsearch-editor-button");
        $(anythingButton).click(() => {
            conditionInput.value += ".*";
        });

        var orButton: HTMLButtonElement = this.createButton("Nebo");
        conditionButtonsDiv.appendChild(orButton);
        orButton.style.cssFloat = "right";
        $(orButton).addClass("regexsearch-editor-button");
        $(orButton).click(() => {
            conditionInput.value += "|";
        });


        var commandButtonsDiv: HTMLDivElement = document.createElement("div");
        $(commandButtonsDiv).addClass("regexsearch-command-buttons-div");
        editorDiv.appendChild(commandButtonsDiv);

        var stornoButton: HTMLButtonElement = this.createButton("Zrušit");
        commandButtonsDiv.appendChild(stornoButton);
        $(stornoButton).click(() => {
            $(this.container).empty();
        });

        var submitButton: HTMLButtonElement = this.createButton("Dokončit");
        commandButtonsDiv.appendChild(submitButton);
        $(submitButton).click(() => {
            var inputValue: string = conditionInput.value;
            var outputValue: string;
            
            switch (conditionSelect.value) {
                case this.conditionType.StartsWith:
                    outputValue = "(" + inputValue + ")(.*)";
                    break;
                case this.conditionType.NotStartsWith:
                    outputValue = "(?!" + inputValue + ")(.*)";
                    break;
                case this.conditionType.Contains:
                    outputValue = "(.*)(" + inputValue + ")(.*)";
                    break;
                case this.conditionType.NotContains:
                    outputValue = "((?!" + inputValue + ").)*";
                    break;
                case this.conditionType.EndsWith:
                    outputValue = "(.*)(" + inputValue + ")";
                    break;
                case this.conditionType.NotEndsWith:
                    outputValue = "((?!" + inputValue + "$).)*";
                    break;
                default:
                    outputValue = "";
            }

            this.searchBox.value = "^" + outputValue + "$";

            $(this.container).addClass("hidden");
        });


        $(this.container).append(mainRegExDiv);
    }
}
