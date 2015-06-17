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
            _this.addNewCondition();
        });
        var removeConditionsButton = this.createButton("Odebrat podmínku");
        $(removeConditionsButton).click(function () {
            _this.removeLastCondition();
        });
        commandsDiv.appendChild(removeConditionsButton);
        var sentButton = this.createButton("Vyhledat");
        commandsDiv.appendChild(sentButton);
        $(sentButton).click(function () {
            _this.processSearch();
        });
        this.innerContainer = document.createElement("div");
        this.addNewCondition(true);
        //var endDelimiter: HTMLDivElement = document.createElement("div");
        //endDelimiter.innerHTML = "&nbsp;";
        //$(endDelimiter).addClass("regexsearch-delimiter");
        $(this.container).append(commandsDiv);
        $(this.container).append(this.innerContainer);
        //$(this.container).append(endDelimiter);
    };
    RegExSearch.prototype.addNewCondition = function (useDelimiter) {
        if (useDelimiter === void 0) { useDelimiter = true; }
        var newRegExConditions = new RegExCondition(this);
        newRegExConditions.makeRegExCondition();
        if (!useDelimiter) {
            newRegExConditions.removeDelimeter();
        }
        this.regExConditions.push(newRegExConditions);
        $(this.innerContainer).append(newRegExConditions.getHtml());
    };
    RegExSearch.prototype.removeLastCondition = function () {
        if (this.regExConditions.length <= 1)
            return;
        var arrayItem = this.regExConditions.pop();
        this.innerContainer.removeChild(arrayItem.getHtml());
    };
    RegExSearch.prototype.removeCondition = function (condition) {
        var index = this.regExConditions.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.regExConditions[index];
            this.innerContainer.removeChild(arrayItem.getHtml());
            this.regExConditions.splice(index, 1);
        }
        if (this.regExConditions.length === 0) {
            this.addNewCondition(true);
        }
        //if (this.regExConditions[0].hasDelimeter) { //TODO change last delimeter
        //    this.regExConditions[0].removeDelimeter();
        //}
    };
    RegExSearch.prototype.getConditionsResultString = function () {
        var outputString = "";
        for (var i = 0; i < this.regExConditions.length; i++) {
            var regExConditions = this.regExConditions[i];
            outputString += "(?=.*(" + regExConditions.getConditionString() + "))";
        }
        return "^" + outputString + ".*$";
    };
    RegExSearch.prototype.getConditionsResultObject = function () {
        var resultObject = new Object();
        for (var i = 0; i < this.regExConditions.length; i++) {
            var regExCondition = this.regExConditions[i];
            var resultArrayForType = resultObject[regExCondition.getSearchType()];
            if (typeof resultArrayForType == 'undefined' || resultArrayForType == null) {
                resultArrayForType = new Array();
                resultObject[regExCondition.getSearchType()] = resultArrayForType;
            }
            resultArrayForType.push(regExCondition.getConditionString());
        }
        return resultObject;
    };
    RegExSearch.prototype.getConditionsResultJSON = function () {
        var jsonString = JSON.stringify(this.getConditionsResultObject());
        return jsonString;
    };
    RegExSearch.prototype.processSearch = function () {
        var json = this.getConditionsResultJSON();
        $.ajax({
            type: "POST",
            traditional: true,
            data: json,
            url: "/Dictionaries/Dictionaries/SearchCriteria",
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
            },
            error: function (response) {
            }
        });
    };
    return RegExSearch;
})(RegExSearchBase);
var RegExCondition = (function (_super) {
    __extends(RegExCondition, _super);
    function RegExCondition(parent) {
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
    RegExCondition.prototype.getHtml = function () {
        return this.html;
    };
    RegExCondition.prototype.removeDelimeter = function () {
        $(this.html).find(".regexsearch-delimiter").remove();
    };
    RegExCondition.prototype.hasDelimeter = function () {
        var delimeter = $(this.html).find(".regexsearch-delimiter");
        return (typeof delimeter != 'undefined' && delimeter != null);
    };
    RegExCondition.prototype.getWordFormType = function () {
        return this.selectedWordFormType;
    };
    RegExCondition.prototype.getSearchType = function () {
        return this.selectedSearchType;
    };
    RegExCondition.prototype.makeRegExCondition = function () {
        var _this = this;
        var conditionsDiv = document.createElement("div");
        $(conditionsDiv).addClass("regexsearch-condition-main-div");
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
        this.conditionContainerDiv = document.createElement("div");
        $(this.conditionContainerDiv).addClass("regexsearch-condition-list-div");
        mainSearchDiv.appendChild(this.conditionContainerDiv);
        var commandsDiv = document.createElement("div");
        $(commandsDiv).addClass("regexsearch-conditions-commands");
        mainSearchDiv.appendChild(commandsDiv);
        var addConditionButton = this.createButton("Přidat výraz");
        $(addConditionButton).click(function () {
            _this.addCondition();
        });
        commandsDiv.appendChild(addConditionButton);
        this.resetCondition();
        var mainDiv = document.createElement("div");
        var andInfoDiv = document.createElement("div");
        $(andInfoDiv).addClass("regexsearch-delimiter");
        andInfoDiv.innerHTML = "A zároveň";
        mainDiv.appendChild(andInfoDiv);
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon");
        $(removeGlyph).addClass("glyphicon-trash");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeCondition(_this);
        });
        andInfoDiv.appendChild(trashButton);
        $(conditionsDiv).append(mainSearchDiv);
        $(conditionsDiv).append(mainDiv);
        this.html = conditionsDiv;
    };
    RegExCondition.prototype.resetCondition = function () {
        $(this.conditionContainerDiv).empty();
        this.conditionInputArray = [];
        var newRegExInput = new RegExInput(this);
        newRegExInput.makeRegExInput();
        newRegExInput.removeDelimeter();
        this.conditionInputArray.push(newRegExInput);
        this.conditionContainerDiv.appendChild(newRegExInput.getHtml());
    };
    RegExCondition.prototype.addCondition = function () {
        var newRegExInput = new RegExInput(this);
        newRegExInput.makeRegExInput();
        this.conditionInputArray.push(newRegExInput);
        this.conditionContainerDiv.appendChild(newRegExInput.getHtml());
    };
    RegExCondition.prototype.removeCondition = function (condition) {
        var index = this.conditionInputArray.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.conditionInputArray[index];
            this.conditionContainerDiv.removeChild(arrayItem.getHtml());
            this.conditionInputArray.splice(index, 1);
        }
        if (this.conditionInputArray.length === 0) {
            this.resetCondition();
        }
        if (this.conditionInputArray[0].hasDelimeter) {
            this.conditionInputArray[0].removeDelimeter();
        }
    };
    RegExCondition.prototype.getConditionString = function () {
        var conditionString = this.conditionInputArray[0].getConditionValue();
        for (var i = 1; i < this.conditionInputArray.length; i++) {
            var regExInput = this.conditionInputArray[i];
            conditionString += "|" + regExInput.getConditionValue();
        }
        return conditionString;
    };
    return RegExCondition;
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