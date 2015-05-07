var RegExEditor = (function () {
    function RegExEditor(container, searchBox) {
        this.conditionType = {
            StartsWith: "starts-with",
            NotStartsWith: "not-starts-with",
            Contains: "contains",
            NotContains: "not-contains",
            EndsWith: "ends-with",
            NotEndsWith: "not-ends-with"
        };
        this.container = container;
        this.searchBox = searchBox;
    }
    RegExEditor.prototype.makeRegExEditor = function () {
        $(this.container).empty();
        var mainRegExDiv = document.createElement("div");
        $(mainRegExDiv).addClass("content-container");
        var titleHeading = document.createElement("h3");
        titleHeading.innerHTML = "Editor regulárního výrazu";
        mainRegExDiv.appendChild(titleHeading);
        var editorDiv = document.createElement("div");
        mainRegExDiv.appendChild(editorDiv);
        var conditionTitleDiv = document.createElement("div");
        conditionTitleDiv.innerHTML = "Podmínka";
        editorDiv.appendChild(conditionTitleDiv);
        var conditionTypeDiv = document.createElement("div");
        $(conditionTypeDiv).addClass("regexsearch-condition-type-div");
        editorDiv.appendChild(conditionTypeDiv);
        var conditionSelect = document.createElement("select");
        $(conditionSelect).addClass("regexsearch-condition-select");
        conditionTypeDiv.appendChild(conditionSelect);
        conditionSelect.appendChild(this.createOption("Začíná na", this.conditionType.StartsWith));
        conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(this.createOption("Obsahuje", this.conditionType.Contains));
        conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(this.createOption("Končí na", this.conditionType.EndsWith));
        conditionSelect.appendChild(this.createOption("Nekončí na", this.conditionType.NotEndsWith));
        var conditionDiv = document.createElement("div");
        $(conditionDiv).addClass("regexsearch-condition-div");
        editorDiv.appendChild(conditionDiv);
        var conditionInputDiv = document.createElement("div");
        conditionDiv.appendChild(conditionInputDiv);
        var conditionInput = document.createElement("input");
        conditionInput.type = "text";
        $(conditionInput).addClass("regexsearch-input");
        conditionInputDiv.appendChild(conditionInput);
        var conditionButtonsDiv = document.createElement("div");
        conditionDiv.appendChild(conditionButtonsDiv);
        var anythingButton = this.createButton("Cokoliv");
        conditionButtonsDiv.appendChild(anythingButton);
        $(anythingButton).addClass("regexsearch-input-button");
        $(anythingButton).click(function (event) {
            conditionInput.value += ".*";
        });
        var orButton = this.createButton("Nebo");
        conditionButtonsDiv.appendChild(orButton);
        orButton.style.cssFloat = "right";
        $(orButton).addClass("regexsearch-input-button");
        $(orButton).click(function (event) {
            conditionInput.value += "|";
        });
        var commandButtonsDiv = document.createElement("div");
        $(commandButtonsDiv).addClass("regexsearch-command-buttons-div");
        editorDiv.appendChild(commandButtonsDiv);
        var stornoButton = this.createButton("Zrušit");
        commandButtonsDiv.appendChild(stornoButton);
        var backButton = this.createButton("Zpět");
        commandButtonsDiv.appendChild(backButton);
        var nextButton = this.createButton("Další");
        commandButtonsDiv.appendChild(nextButton);
        var submitButton = this.createButton("Dokončit");
        submitButton.style.marginLeft = "10px";
        commandButtonsDiv.appendChild(submitButton);
        $(this.container).append(mainRegExDiv);
    };
    RegExEditor.prototype.createOption = function (label, value) {
        var conditionOption = document.createElement("option");
        conditionOption.innerHTML = label;
        conditionOption.value = value;
        return conditionOption;
    };
    RegExEditor.prototype.createButton = function (label) {
        var button = document.createElement("button");
        button.type = "button";
        button.innerHTML = label;
        $(button).addClass("btn");
        $(button).addClass("btn-default");
        $(button).addClass("regexsearch-button");
        return button;
    };
    return RegExEditor;
})();
//# sourceMappingURL=itjakub.plugins.regexsearch.js.map