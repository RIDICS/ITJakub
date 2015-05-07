var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var RegExSearchBase = (function () {
    function RegExSearchBase() {
    }
    RegExSearchBase.prototype.createOption = function (label, value) {
        var conditionOption = document.createElement("option");
        conditionOption.innerHTML = label;
        conditionOption.value = value;
        return conditionOption;
    };
    RegExSearchBase.prototype.createButton = function (label) {
        var button = document.createElement("button");
        button.type = "button";
        button.innerHTML = label;
        $(button).addClass("btn");
        $(button).addClass("btn-default");
        $(button).addClass("regexsearch-button");
        return button;
    };
    return RegExSearchBase;
})();
var RegExSearch = (function (_super) {
    __extends(RegExSearch, _super);
    function RegExSearch(container) {
        _super.call(this);
        this.container = container;
    }
    RegExSearch.prototype.makeRegExSearch = function () {
        var _this = this;
        $(this.container).empty();
        this.regExConditions = [];
        var commandsDiv = document.createElement("div");
        var addConditionsButton = this.createButton("Přidat podmínku");
        commandsDiv.appendChild(addConditionsButton);
        $(addConditionsButton).click(function () {
            _this.addNewConditions();
        });
        var removeConditionsButton = this.createButton("Odebrat podmínku");
        commandsDiv.appendChild(removeConditionsButton);
        var firstInnerDiv = document.createElement("div");
        var newRegExConditions = new RegExConditions(firstInnerDiv);
        newRegExConditions.makeRegExCondition();
        var arrayItem = new RegExConditionsArrayItem(firstInnerDiv, newRegExConditions);
        this.regExConditions.push(arrayItem);
        $(this.container).append(commandsDiv);
        $(this.container).append(firstInnerDiv);
    };
    RegExSearch.prototype.addNewConditions = function () {
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
    };
    return RegExSearch;
})(RegExSearchBase);
var RegExConditionsArrayItem = (function () {
    function RegExConditionsArrayItem(htmlElement, regExConditions) {
        this.htmlElement = htmlElement;
        this.regExConditions = regExConditions;
    }
    return RegExConditionsArrayItem;
})();
var RegExConditions = (function (_super) {
    __extends(RegExConditions, _super);
    function RegExConditions(container) {
        _super.call(this);
        this.container = container;
    }
    RegExConditions.prototype.makeRegExCondition = function () {
        $(this.container).empty();
        var mainSearchDiv = document.createElement("div");
        var searchDestinationDiv = document.createElement("div");
        mainSearchDiv.appendChild(searchDestinationDiv);
        var searchDestinationSpan = document.createElement("span");
        searchDestinationSpan.innerHTML = "Zvolte oblast vyhledávání";
        searchDestinationDiv.appendChild(searchDestinationSpan);
        var searchDestinationSelect = document.createElement("select");
        searchDestinationDiv.appendChild(searchDestinationSelect);
        //TODO add options
        var wordFormDiv = document.createElement("div");
        mainSearchDiv.appendChild(wordFormDiv);
        var wordFormSpan = document.createElement("span");
        wordFormSpan.innerHTML = "Tvar slova:";
        wordFormDiv.appendChild(wordFormSpan);
        var wordFormSelect = document.createElement("select");
        wordFormDiv.appendChild(wordFormSelect);
        //TODO add options
        var conditionsDiv = document.createElement("div");
        mainSearchDiv.appendChild(conditionsDiv);
        var commandsDiv = document.createElement("div");
        mainSearchDiv.appendChild(commandsDiv);
        var addConditionButton = this.createButton("Přidat");
        commandsDiv.appendChild(addConditionButton);
        var removeConditionButton = this.createButton("Odebrat");
        commandsDiv.appendChild(removeConditionButton);
        $(this.container).append(mainSearchDiv);
    };
    return RegExConditions;
})(RegExSearchBase);
var RegExEditor = (function (_super) {
    __extends(RegExEditor, _super);
    function RegExEditor(container, searchBox) {
        _super.call(this);
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
        var _this = this;
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
        $(submitButton).click(function (event) {
            _this.searchBox.value = conditionInput.value;
            // TODO more logic - using conditionType
        });
        $(this.container).append(mainRegExDiv);
    };
    return RegExEditor;
})(RegExSearchBase);
//# sourceMappingURL=itjakub.plugins.regexsearch.js.map