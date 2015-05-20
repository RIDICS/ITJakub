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
    regExConditions: Array<RegExConditionsArrayItem>;

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
        var mainDiv = document.createElement("div");

        if (useDelimiter)
        {
            var andInfoDiv = document.createElement("div");
            $(andInfoDiv).addClass("regexsearch-delimiter");
            andInfoDiv.innerHTML = "A zároveň";
            mainDiv.appendChild(andInfoDiv);
        }

        var conditionsDiv = document.createElement("div");
        $(conditionsDiv).addClass("regexsearch-condition-main-div");
        mainDiv.appendChild(conditionsDiv);

        var newRegExConditions = new RegExConditions(conditionsDiv);
        newRegExConditions.makeRegExCondition();

        var arrayItem = new RegExConditionsArrayItem(mainDiv, newRegExConditions);
        this.regExConditions.push(arrayItem);


        $(this.innerContainer).append(mainDiv);
    }

    private removeLastConditions() {
        if (this.regExConditions.length <= 1)
            return;

        var arrayItem: RegExConditionsArrayItem = this.regExConditions.pop();
        this.innerContainer.removeChild(arrayItem.htmlElement);
    }

    public getConditionsString(): string {
        var outputString = "";

        for (var i = 0; i < this.regExConditions.length; i++) {
            var regExConditions = this.regExConditions[i].regExConditions;
            outputString += "(?=.*(" + regExConditions.getConditionsString() + "))";
        }

        return "^" + outputString + ".*$";
    }
}

class RegExConditionsArrayItem {
    htmlElement: HTMLDivElement;
    regExConditions: RegExConditions;

    constructor(htmlElement: HTMLDivElement, regExConditions: RegExConditions) {
        this.htmlElement = htmlElement;
        this.regExConditions = regExConditions;
    }
}

class RegExInputArrayItem {
    htmlElement: HTMLDivElement;
    regExInput: RegExInput;

    constructor(htmlElement: HTMLDivElement, regExInput: RegExInput) {
        this.htmlElement = htmlElement;
        this.regExInput = regExInput;
    }
}

class RegExConditions extends RegExSearchBase {
    container: HTMLDivElement;
    conditionsContainerDiv: HTMLDivElement;
    conditionsInputArray: Array<RegExInputArrayItem>;

    constructor(container: HTMLDivElement) {
        super();
        this.container = container;
    }

    private wordFormType = {
        Lemma: "lemma",
        HyperlemmaNew: "hyperlemma-new",
        HyperlemmaOld: "hyperlemma-old",
        Stemma: "stemma"
    }

    public makeRegExCondition() {
        $(this.container).empty();

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

        searchDestinationSelect.appendChild(this.createOption("Fulltext", "fulltext"));

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

        this.conditionsContainerDiv = document.createElement("div");
        $(this.conditionsContainerDiv).addClass("regexsearch-condition-list-div");
        mainSearchDiv.appendChild(this.conditionsContainerDiv);

        var commandsDiv: HTMLDivElement = document.createElement("div");
        $(commandsDiv).addClass("regexsearch-conditions-commands");
        mainSearchDiv.appendChild(commandsDiv);

        var addConditionButton: HTMLButtonElement = this.createButton("Přidat");
        $(addConditionButton).click(() => {
            this.addConditions();
        });
        commandsDiv.appendChild(addConditionButton);

        var removeConditionButton: HTMLButtonElement = this.createButton("Odebrat");
        $(removeConditionButton).click(() => {
            this.removeSelectedConditions();
        });
        commandsDiv.appendChild(removeConditionButton);


        this.resetConditions();
        $(this.container).append(mainSearchDiv);
    }

    private resetConditions() {
        $(this.conditionsContainerDiv).empty();
        this.conditionsInputArray = [];

        var lineDiv = document.createElement("div");

        var newRegExInput = new RegExInput(lineDiv);
        newRegExInput.makeRegExInput();
        
        var arrayItem = new RegExInputArrayItem(lineDiv, newRegExInput);
        this.conditionsInputArray.push(arrayItem);
        
        this.conditionsContainerDiv.appendChild(lineDiv);
    }

    private addConditions() {
        var mainDiv: HTMLDivElement = document.createElement("div");

        var labelDiv = document.createElement("div");
        labelDiv.innerHTML = "Nebo";
        $(labelDiv).addClass("regexsearch-or-delimiter");
        mainDiv.appendChild(labelDiv);

        var lineDiv = document.createElement("div");
        mainDiv.appendChild(lineDiv);

        var newRegExInput = new RegExInput(lineDiv);
        newRegExInput.makeRegExInput();
        
        var arrayItem = new RegExInputArrayItem(mainDiv, newRegExInput);
        this.conditionsInputArray.push(arrayItem);
        
        
        this.conditionsContainerDiv.appendChild(mainDiv);
    }

    private removeSelectedConditions() {
        var uncheckedItems: Array<RegExInputArrayItem> = [];
        var arrayItem: RegExInputArrayItem;

        for (var i = 0; i < this.conditionsInputArray.length; i++) {
            arrayItem = this.conditionsInputArray[i];
            if (arrayItem.regExInput.isChecked()) {
                this.conditionsContainerDiv.removeChild(arrayItem.htmlElement);
            } else {
                uncheckedItems.push(arrayItem);
            }
        }

        // Remove "Or" label from first condition field
        if (this.conditionsInputArray[0].regExInput.isChecked() && uncheckedItems.length > 0) {
            uncheckedItems[0].htmlElement.removeChild(uncheckedItems[0].htmlElement.firstChild);
        }

        this.conditionsInputArray = uncheckedItems;

        if (this.conditionsInputArray.length === 0) {
            this.resetConditions();
        }
    }

    public getConditionsString(): string {
        var conditionsString = this.conditionsInputArray[0].regExInput.getConditionValue();
        for (var i = 1; i < this.conditionsInputArray.length; i++) {
            var regExInput = this.conditionsInputArray[i].regExInput;
            conditionsString += "|" + regExInput.getConditionValue();
        }

        return conditionsString;
    }
}

class RegExInput extends RegExSearchBase {
    private container: HTMLDivElement;
    private editorDiv: HTMLDivElement;
    private checkBox: HTMLInputElement;
    private conditionInput: HTMLInputElement;
    private regExEditor: RegExEditor;

    constructor(container: HTMLDivElement) {
        super();
        this.container = container;
    }

    public makeRegExInput() {
        $(this.container).empty();

        var lineDiv: HTMLDivElement = document.createElement("div");
        this.editorDiv = document.createElement("div");

        this.checkBox = document.createElement("input");
        this.checkBox.type = "checkbox";
        $(this.checkBox).addClass("regexsearch-checkbox");
        lineDiv.appendChild(this.checkBox);

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

        $(this.container).append(lineDiv);
        $(this.container).append(this.editorDiv);
    }

    public isChecked(): boolean {
        return this.checkBox.checked;
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

        //var backButton: HTMLButtonElement = this.createButton("Zpět");
        //commandButtonsDiv.appendChild(backButton);

        //var nextButton: HTMLButtonElement = this.createButton("Další");
        //commandButtonsDiv.appendChild(nextButton);

        var submitButton: HTMLButtonElement = this.createButton("Dokončit");
        //submitButton.style.marginLeft = "25px";
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
