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
        var newRegExConditions = new RegExConditions(this);
        newRegExConditions.makeRegExCondition();
        if (!useDelimiter) {
            newRegExConditions.removeDelimeter();
        }
        this.regExConditions.push(newRegExConditions);
        $(this.innerContainer).append(newRegExConditions.getHtml());
    };
    RegExSearch.prototype.removeLastConditions = function () {
        if (this.regExConditions.length <= 1)
            return;
        var arrayItem = this.regExConditions.pop();
        this.innerContainer.removeChild(arrayItem.getHtml());
    };
    RegExSearch.prototype.getConditionsString = function () {
        var outputString = "";
        for (var i = 0; i < this.regExConditions.length; i++) {
            var regExConditions = this.regExConditions[i];
            outputString += "(?=.*(" + regExConditions.getConditionsString() + "))";
        }
        return "^" + outputString + ".*$";
    };
    return RegExSearch;
})(RegExSearchBase);
var RegExConditions = (function (_super) {
    __extends(RegExConditions, _super);
    function RegExConditions(parent) {
        _super.call(this);
        this.wordFormType = {
            Lemma: "lemma",
            HyperlemmaNew: "hyperlemma-new",
            HyperlemmaOld: "hyperlemma-old",
            Stemma: "stemma"
        };
        this.searchType = {
            Text: "text",
            Author: "author",
            Title: "title",
            Responsible: "responsible"
        };
        this.parent = parent;
    }
    RegExConditions.prototype.getHtml = function () {
        return this.html;
    };
    RegExConditions.prototype.removeDelimeter = function () {
        $(this.html).find(".regexsearch-delimiter").remove();
    };
    RegExConditions.prototype.hasDelimeter = function () {
        var delimeter = $(this.html).find(".regexsearch-delimiter");
        return (typeof delimeter != 'undefined' && delimeter != null);
    };
    RegExConditions.prototype.makeRegExCondition = function () {
        var _this = this;
        var mainDiv = document.createElement("div");
        var andInfoDiv = document.createElement("div");
        $(andInfoDiv).addClass("regexsearch-delimiter");
        andInfoDiv.innerHTML = "A zároveň";
        mainDiv.appendChild(andInfoDiv);
        var conditionsDiv = document.createElement("div");
        $(conditionsDiv).addClass("regexsearch-condition-main-div");
        conditionsDiv.appendChild(mainDiv);
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
        searchDestinationSelect.appendChild(this.createOption("Text", this.searchType.Text));
        searchDestinationSelect.appendChild(this.createOption("Autor", this.searchType.Author));
        searchDestinationSelect.appendChild(this.createOption("Titul", this.searchType.Title));
        searchDestinationSelect.appendChild(this.createOption("Editor", this.searchType.Responsible));
        this.selectedSearchType = this.searchType.Text;
        $(searchDestinationSelect).change(function (eventData) {
            _this.selectedSearchType = $(eventData.target).val();
        });
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
        this.selectedWordFormType = this.wordFormType.Lemma;
        $(wordFormSelect).change(function (eventData) {
            _this.selectedWordFormType = $(eventData.target).val();
        });
        this.conditionsContainerDiv = document.createElement("div");
        $(this.conditionsContainerDiv).addClass("regexsearch-condition-list-div");
        mainSearchDiv.appendChild(this.conditionsContainerDiv);
        var commandsDiv = document.createElement("div");
        $(commandsDiv).addClass("regexsearch-conditions-commands");
        mainSearchDiv.appendChild(commandsDiv);
        var addConditionButton = this.createButton("Přidat výraz");
        $(addConditionButton).click(function () {
            _this.addCondition();
        });
        commandsDiv.appendChild(addConditionButton);
        this.resetConditions();
        $(conditionsDiv).append(mainSearchDiv);
        this.html = conditionsDiv;
    };
    RegExConditions.prototype.resetConditions = function () {
        $(this.conditionsContainerDiv).empty();
        this.conditionsInputArray = [];
        var newRegExInput = new RegExInput(this);
        newRegExInput.makeRegExInput();
        newRegExInput.removeDelimeter();
        this.conditionsInputArray.push(newRegExInput);
        this.conditionsContainerDiv.appendChild(newRegExInput.getHtml());
    };
    RegExConditions.prototype.addCondition = function () {
        var newRegExInput = new RegExInput(this);
        newRegExInput.makeRegExInput();
        this.conditionsInputArray.push(newRegExInput);
        this.conditionsContainerDiv.appendChild(newRegExInput.getHtml());
    };
    RegExConditions.prototype.removeCondition = function (condition) {
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
    };
    RegExConditions.prototype.getConditionsString = function () {
        var conditionsString = this.conditionsInputArray[0].getConditionValue();
        for (var i = 1; i < this.conditionsInputArray.length; i++) {
            var regExInput = this.conditionsInputArray[i];
            conditionsString += "|" + regExInput.getConditionValue();
        }
        return conditionsString;
    };
    return RegExConditions;
})(RegExSearchBase);
var RegExInput = (function (_super) {
    __extends(RegExInput, _super);
    function RegExInput(parent) {
        _super.call(this);
        this.parentRegExConditions = parent;
    }
    RegExInput.prototype.getHtml = function () {
        return this.html;
    };
    RegExInput.prototype.removeDelimeter = function () {
        $(this.html).find(".regexsearch-or-delimiter").remove();
    };
    RegExInput.prototype.hasDelimeter = function () {
        var delimeter = $(this.html).find(".regexsearch-or-delimiter");
        return (typeof delimeter != 'undefined' && delimeter != null);
    };
    RegExInput.prototype.makeRegExInput = function () {
        var _this = this;
        var mainDiv = document.createElement("div");
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
            else if ($(_this.editorDiv).hasClass("hidden")) {
                $(_this.editorDiv).removeClass("hidden");
            }
            else {
                $(_this.editorDiv).addClass("hidden");
            }
        });
        lineDiv.appendChild(regExButton);
        var removeButton = this.createButton("");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon");
        $(removeGlyph).addClass("glyphicon-trash");
        removeButton.appendChild(removeGlyph);
        $(removeButton).click(function () {
            _this.parentRegExConditions.removeCondition(_this);
        });
        lineDiv.appendChild(removeButton);
        mainDiv.appendChild(lineDiv);
        mainDiv.appendChild(this.editorDiv);
        this.html = mainDiv;
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
        this.conditionType = Object.freeze({
            StartsWith: "starts-with",
            NotStartsWith: "not-starts-with",
            Contains: "contains",
            NotContains: "not-contains",
            EndsWith: "ends-with",
            NotEndsWith: "not-ends-with"
        });
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
        var submitButton = this.createButton("Dokončit");
        commandButtonsDiv.appendChild(submitButton);
        $(submitButton).click(function () {
            var inputValue = conditionInput.value;
            var outputValue;
            switch (conditionSelect.value) {
                case _this.conditionType.StartsWith:
                    outputValue = "(" + inputValue + ")(.*)";
                    break;
                case _this.conditionType.NotStartsWith:
                    outputValue = "(?!" + inputValue + ")(.*)";
                    break;
                case _this.conditionType.Contains:
                    outputValue = "(.*)(" + inputValue + ")(.*)";
                    break;
                case _this.conditionType.NotContains:
                    outputValue = "((?!" + inputValue + ").)*";
                    break;
                case _this.conditionType.EndsWith:
                    outputValue = "(.*)(" + inputValue + ")";
                    break;
                case _this.conditionType.NotEndsWith:
                    outputValue = "((?!" + inputValue + "$).)*";
                    break;
                default:
                    outputValue = "";
            }
            _this.searchBox.value = "^" + outputValue + "$";
            $(_this.container).addClass("hidden");
        });
        $(this.container).append(mainRegExDiv);
    };
    return RegExEditor;
})(RegExSearchBase);
//# sourceMappingURL=itjakub.plugins.regexsearch.js.map