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
        $(removeConditionsButton).click(function () {
            _this.removeLastConditions();
        });
        commandsDiv.appendChild(removeConditionsButton);
        this.innerContainer = document.createElement("div");
        this.addNewConditions(false);
        var endDelimiter = document.createElement("div");
        endDelimiter.innerHTML = "&nbsp;";
        $(endDelimiter).addClass("regexsearch-delimiter");
        $(this.container).append(commandsDiv);
        $(this.container).append(this.innerContainer);
        $(this.container).append(endDelimiter);
    };
    RegExSearch.prototype.addNewConditions = function (useDelimiter) {
        if (useDelimiter === void 0) { useDelimiter = true; }
        var mainDiv = document.createElement("div");
        if (useDelimiter) {
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
    };
    RegExSearch.prototype.removeLastConditions = function () {
        if (this.regExConditions.length <= 1)
            return;
        var arrayItem = this.regExConditions.pop();
        this.innerContainer.removeChild(arrayItem.htmlElement);
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
var RegExInputArrayItem = (function () {
    function RegExInputArrayItem(htmlElement, regExInput) {
        this.htmlElement = htmlElement;
        this.regExInput = regExInput;
    }
    return RegExInputArrayItem;
})();
var RegExConditions = (function (_super) {
    __extends(RegExConditions, _super);
    function RegExConditions(container) {
        _super.call(this);
        this.wordFormType = {
            Lemma: "lemma",
            HyperlemmaNew: "hyperlemma-new",
            HyperlemmaOld: "hyperlemma-old",
            Stemma: "stemma"
        };
        this.container = container;
    }
    RegExConditions.prototype.makeRegExCondition = function () {
        var _this = this;
        $(this.container).empty();
        var mainSearchDiv = document.createElement("div");
        var searchDestinationDiv = document.createElement("div");
        $(searchDestinationDiv).addClass("regexsearch-destination-div");
        mainSearchDiv.appendChild(searchDestinationDiv);
        var searchDestinationSpan = document.createElement("span");
        searchDestinationSpan.innerHTML = "Zvolte oblast vyhledávání";
        $(searchDestinationSpan).addClass("regexsearch-upper-select-label");
        searchDestinationDiv.appendChild(searchDestinationSpan);
        var searchDestinationSelect = document.createElement("select");
        $(searchDestinationSelect).addClass("regexsearch-select");
        searchDestinationDiv.appendChild(searchDestinationSelect);
        searchDestinationSelect.appendChild(this.createOption("Fulltext", "fulltext"));
        var wordFormDiv = document.createElement("div");
        $(wordFormDiv).addClass("regexsearch-word-form-div");
        mainSearchDiv.appendChild(wordFormDiv);
        var wordFormSpan = document.createElement("span");
        wordFormSpan.innerHTML = "Tvar slova";
        $(wordFormSpan).addClass("regexsearch-upper-select-label");
        wordFormDiv.appendChild(wordFormSpan);
        var wordFormSelect = document.createElement("select");
        $(wordFormSelect).addClass("regexsearch-select");
        wordFormDiv.appendChild(wordFormSelect);
        wordFormSelect.appendChild(this.createOption("Lemma", this.wordFormType.Lemma));
        wordFormSelect.appendChild(this.createOption("Hyperlemma - nové", this.wordFormType.HyperlemmaNew));
        wordFormSelect.appendChild(this.createOption("Hyperlemma - staré", this.wordFormType.HyperlemmaOld));
        wordFormSelect.appendChild(this.createOption("Stemma", this.wordFormType.Stemma));
        this.conditionsContainerDiv = document.createElement("div");
        $(this.conditionsContainerDiv).addClass("regexsearch-condition-list-div");
        mainSearchDiv.appendChild(this.conditionsContainerDiv);
        var commandsDiv = document.createElement("div");
        $(commandsDiv).addClass("regexsearch-conditions-commands");
        mainSearchDiv.appendChild(commandsDiv);
        var addConditionButton = this.createButton("Přidat");
        $(addConditionButton).click(function () {
            _this.addConditions();
        });
        commandsDiv.appendChild(addConditionButton);
        var removeConditionButton = this.createButton("Odebrat");
        $(removeConditionButton).click(function () {
            _this.removeSelectedConditions();
        });
        commandsDiv.appendChild(removeConditionButton);
        this.resetConditions();
        $(this.container).append(mainSearchDiv);
    };
    RegExConditions.prototype.resetConditions = function () {
        $(this.conditionsContainerDiv).empty();
        this.conditionsInputArray = [];
        var lineDiv = document.createElement("div");
        var newRegExInput = new RegExInput(lineDiv);
        newRegExInput.makeRegExInput();
        var arrayItem = new RegExInputArrayItem(lineDiv, newRegExInput);
        this.conditionsInputArray.push(arrayItem);
        this.conditionsContainerDiv.appendChild(lineDiv);
    };
    RegExConditions.prototype.addConditions = function () {
        var mainDiv = document.createElement("div");
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
    };
    RegExConditions.prototype.removeSelectedConditions = function () {
        var uncheckedItems = [];
        var arrayItem;
        for (var i = 0; i < this.conditionsInputArray.length; i++) {
            arrayItem = this.conditionsInputArray[i];
            if (arrayItem.regExInput.isChecked()) {
                this.conditionsContainerDiv.removeChild(arrayItem.htmlElement);
            }
            else {
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
    };
    return RegExConditions;
})(RegExSearchBase);
var RegExInput = (function (_super) {
    __extends(RegExInput, _super);
    function RegExInput(container) {
        _super.call(this);
        this.container = container;
    }
    RegExInput.prototype.makeRegExInput = function () {
        var _this = this;
        $(this.container).empty();
        var lineDiv = document.createElement("div");
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
        var regExButton = document.createElement("button");
        regExButton.innerText = "R";
        regExButton.type = "button";
        $(regExButton).addClass("btn");
        $(regExButton).addClass("regexsearch-condition-input-button");
        $(regExButton).click(function () {
            if (!_this.regExEditor || _this.editorDiv.children.length === 0) {
                _this.regExEditor = new RegExEditor(_this.editorDiv, _this.conditionInput);
                _this.regExEditor.makeRegExEditor();
            }
            else {
                _this.regExEditor = null;
                $(_this.editorDiv).empty();
            }
        });
        lineDiv.appendChild(regExButton);
        $(this.container).append(lineDiv);
        $(this.container).append(this.editorDiv);
    };
    RegExInput.prototype.isChecked = function () {
        return this.checkBox.checked;
    };
    RegExInput.prototype.getConditionValue = function () {
        return this.conditionInput.value;
    };
    return RegExInput;
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
        $(mainRegExDiv).addClass("regexsearch-editor-container");
        var titleHeading = document.createElement("span");
        titleHeading.innerHTML = "Editor regulárního výrazu";
        $(titleHeading).addClass("regexsearch-editor-title");
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
        $(anythingButton).addClass("regexsearch-editor-button");
        $(anythingButton).click(function () {
            conditionInput.value += ".*";
        });
        var orButton = this.createButton("Nebo");
        conditionButtonsDiv.appendChild(orButton);
        orButton.style.cssFloat = "right";
        $(orButton).addClass("regexsearch-editor-button");
        $(orButton).click(function () {
            conditionInput.value += "|";
        });
        var commandButtonsDiv = document.createElement("div");
        $(commandButtonsDiv).addClass("regexsearch-command-buttons-div");
        editorDiv.appendChild(commandButtonsDiv);
        var stornoButton = this.createButton("Zrušit");
        commandButtonsDiv.appendChild(stornoButton);
        $(stornoButton).click(function () {
            $(_this.container).empty();
        });
        var backButton = this.createButton("Zpět");
        commandButtonsDiv.appendChild(backButton);
        var nextButton = this.createButton("Další");
        commandButtonsDiv.appendChild(nextButton);
        var submitButton = this.createButton("Dokončit");
        submitButton.style.marginLeft = "25px";
        commandButtonsDiv.appendChild(submitButton);
        $(submitButton).click(function () {
            _this.searchBox.value = conditionInput.value;
            // TODO more logic - using conditionType
            $(_this.container).empty();
        });
        $(this.container).append(mainRegExDiv);
    };
    return RegExEditor;
})(RegExSearchBase);
//# sourceMappingURL=itjakub.plugins.regexsearch.js.map