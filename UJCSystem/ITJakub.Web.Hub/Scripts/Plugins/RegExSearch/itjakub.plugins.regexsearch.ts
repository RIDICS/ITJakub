class RegExEditor {
    container: HTMLDivElement;
    searchBox: HTMLElement;

    constructor(container: HTMLDivElement, searchBox: HTMLElement) {
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
        editorDiv.appendChild(conditionTypeDiv);

        var conditionSelect: HTMLSelectElement = document.createElement("select");
        conditionTypeDiv.appendChild(conditionSelect);

        conditionSelect.appendChild(this.createOption("Začíná na", this.conditionType.StartsWith));
        conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(this.createOption("Obsahuje", this.conditionType.Contains));
        conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(this.createOption("Končí na", this.conditionType.EndsWith));
        conditionSelect.appendChild(this.createOption("Nekončí na", this.conditionType.NotEndsWith));


        var conditionDiv: HTMLDivElement = document.createElement("div");
        editorDiv.appendChild(conditionDiv);

        var conditionInputDiv: HTMLDivElement = document.createElement("div");
        conditionDiv.appendChild(conditionInputDiv);

        var conditionInput: HTMLInputElement = document.createElement("input");
        conditionInput.type = "text";
        conditionInputDiv.appendChild(conditionInput);

        var conditionButtonsDiv: HTMLDivElement = document.createElement("div");
        conditionDiv.appendChild(conditionButtonsDiv);

        var anythingButton: HTMLButtonElement = this.createButton("Cokoliv");
        conditionButtonsDiv.appendChild(anythingButton);

        var orButton: HTMLButtonElement = this.createButton("Nebo");
        conditionButtonsDiv.appendChild(orButton);


        var commandButtonsDiv: HTMLDivElement = document.createElement("div");
        editorDiv.appendChild(commandButtonsDiv);

        var stornoButton: HTMLButtonElement = this.createButton("Zrušit");
        commandButtonsDiv.appendChild(stornoButton);

        var backButton: HTMLButtonElement = this.createButton("Zpět");
        commandButtonsDiv.appendChild(backButton);

        var nextButton: HTMLButtonElement = this.createButton("Další");
        commandButtonsDiv.appendChild(nextButton);

        var submitButton: HTMLButtonElement = this.createButton("Dokončit");
        commandButtonsDiv.appendChild(submitButton);


        $(this.container).append(mainRegExDiv);
    }

    private createOption(label: string, value: string): HTMLOptionElement {
        var conditionOption = document.createElement("option");
        conditionOption.innerHTML = label;
        conditionOption.value = value;

        return conditionOption;
    }

    private createButton(label: string): HTMLButtonElement {
        var button: HTMLButtonElement = document.createElement("button");
        button.innerHTML = label;
        $(button).addClass("btn");
        $(button).addClass("btn-default");
        $(button).addClass("style-button");

        return button;
    }
} 