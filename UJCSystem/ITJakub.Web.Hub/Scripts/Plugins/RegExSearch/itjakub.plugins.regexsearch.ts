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
        commandsDiv.appendChild(removeConditionsButton);


        var firstInnerDiv: HTMLDivElement = document.createElement("div");

        var newRegExConditions = new RegExConditions(firstInnerDiv);
        newRegExConditions.makeRegExCondition();

        var arrayItem = new RegExConditionsArrayItem(firstInnerDiv, newRegExConditions);
        this.regExConditions.push(arrayItem);


        $(this.container).append(commandsDiv);
        $(this.container).append(firstInnerDiv);
    }

    private addNewConditions() {
        var mainDiv = document.createElement("div");

        var andInfoDiv = document.createElement("div");
        andInfoDiv.innerHTML = "A zároveň";
        mainDiv.appendChild(andInfoDiv);

        var conditionsDiv = document.createElement("div");
        mainDiv.appendChild(conditionsDiv);

        var newRegExConditions = new RegExConditions(conditionsDiv);
        newRegExConditions.makeRegExCondition();

        var arrayItem = new RegExConditionsArrayItem(mainDiv, newRegExConditions);
        this.regExConditions.push(arrayItem);


        $(this.innerContainer).append(mainDiv);
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

class RegExConditions extends RegExSearchBase {
    container: HTMLDivElement;

    constructor(container: HTMLDivElement) {
        super();
        this.container = container;
    }

    public makeRegExCondition() {
        $(this.container).empty();

        var mainSearchDiv: HTMLDivElement = document.createElement("div");

        var searchDestinationDiv: HTMLDivElement = document.createElement("div");
        mainSearchDiv.appendChild(searchDestinationDiv);

        var searchDestinationSpan: HTMLSpanElement = document.createElement("span");
        searchDestinationSpan.innerHTML = "Zvolte oblast vyhledávání";
        searchDestinationDiv.appendChild(searchDestinationSpan);

        var searchDestinationSelect: HTMLSelectElement = document.createElement("select");
        searchDestinationDiv.appendChild(searchDestinationSelect);

        //TODO add options

        var wordFormDiv: HTMLDivElement = document.createElement("div");
        mainSearchDiv.appendChild(wordFormDiv);

        var wordFormSpan: HTMLSpanElement = document.createElement("span");
        wordFormSpan.innerHTML = "Tvar slova:";
        wordFormDiv.appendChild(wordFormSpan);

        var wordFormSelect: HTMLSelectElement = document.createElement("select");
        wordFormDiv.appendChild(wordFormSelect);

        //TODO add options

        var conditionsDiv: HTMLDivElement = document.createElement("div");
        mainSearchDiv.appendChild(conditionsDiv);

        var commandsDiv: HTMLDivElement = document.createElement("div");
        mainSearchDiv.appendChild(commandsDiv);

        var addConditionButton: HTMLButtonElement = this.createButton("Přidat");
        commandsDiv.appendChild(addConditionButton);

        var removeConditionButton: HTMLButtonElement = this.createButton("Odebrat");
        commandsDiv.appendChild(removeConditionButton);


        $(this.container).append(mainSearchDiv);
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

    private conditionType = {
        StartsWith: "starts-with",
        NotStartsWith: "not-starts-with",
        Contains: "contains",
        NotContains: "not-contains",
        EndsWith: "ends-with",
        NotEndsWith: "not-ends-with"
    };

    public makeRegExEditor() {
        $(this.container).empty();
        
        var mainRegExDiv: HTMLDivElement = document.createElement("div");
        $(mainRegExDiv).addClass("content-container");

        var titleHeading: HTMLHeadingElement = document.createElement("h3");
        titleHeading.innerHTML = "Editor regulárního výrazu";
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
        $(anythingButton).addClass("regexsearch-input-button");
        $(anythingButton).click(event => {
            conditionInput.value += ".*";
        });

        var orButton: HTMLButtonElement = this.createButton("Nebo");
        conditionButtonsDiv.appendChild(orButton);
        orButton.style.cssFloat = "right";
        $(orButton).addClass("regexsearch-input-button");
        $(orButton).click(event => {
            conditionInput.value += "|";
        });


        var commandButtonsDiv: HTMLDivElement = document.createElement("div");
        $(commandButtonsDiv).addClass("regexsearch-command-buttons-div");
        editorDiv.appendChild(commandButtonsDiv);

        var stornoButton: HTMLButtonElement = this.createButton("Zrušit");
        commandButtonsDiv.appendChild(stornoButton);

        var backButton: HTMLButtonElement = this.createButton("Zpět");
        commandButtonsDiv.appendChild(backButton);

        var nextButton: HTMLButtonElement = this.createButton("Další");
        commandButtonsDiv.appendChild(nextButton);

        var submitButton: HTMLButtonElement = this.createButton("Dokončit");
        submitButton.style.marginLeft = "10px";
        commandButtonsDiv.appendChild(submitButton);
        $(submitButton).click(event => {
            this.searchBox.value = conditionInput.value;
            // TODO more logic - using conditionType
        });


        $(this.container).append(mainRegExDiv);
    }
}
