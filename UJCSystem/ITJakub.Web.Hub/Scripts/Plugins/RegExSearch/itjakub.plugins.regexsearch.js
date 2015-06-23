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
        var sentButton = this.createButton("Vyhledat");
        $(sentButton).addClass("regex-search-button");
        commandsDiv.appendChild(sentButton);
        $(sentButton).click(function () {
            _this.processSearch();
        });
        this.innerContainer = document.createElement("div");
        this.addNewCondition(true);
        $(this.container).append(this.innerContainer);
        $(this.container).append(commandsDiv);
    };
    RegExSearch.prototype.addNewCondition = function (useDelimiter) {
        if (useDelimiter === void 0) { useDelimiter = true; }
        if (this.regExConditions.length > 0) {
            this.regExConditions[this.regExConditions.length - 1].setTextDelimeter();
        }
        var newRegExConditions = new RegExCondition(this);
        newRegExConditions.makeRegExCondition();
        newRegExConditions.setClickableDelimeter();
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
        else {
            this.regExConditions[this.regExConditions.length - 1].setClickableDelimeter();
        }
    };
    RegExSearch.prototype.getConditionsResultObject = function () {
        var resultArray = new Array();
        for (var i = 0; i < this.regExConditions.length; i++) {
            var regExCondition = this.regExConditions[i];
            resultArray.push(regExCondition.getConditionValue());
        }
        return resultArray;
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
        this.parent = parent;
    }
    RegExCondition.prototype.getHtml = function () {
        return this.html;
    };
    RegExCondition.prototype.removeDelimeter = function () {
        $(this.html).find(".regexsearch-delimiter").empty();
    };
    RegExCondition.prototype.hasDelimeter = function () {
        var isEmpty = $(this.html).find(".regexsearch-delimiter").is(':empty');
        return !isEmpty;
    };
    RegExCondition.prototype.setTextDelimeter = function () {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-delimiter").append(textDelimeter);
    };
    RegExCondition.prototype.setClickableDelimeter = function () {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-delimiter").append(clickableDelimeter);
    };
    RegExCondition.prototype.createClickableDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = "+ A zároveň";
        $(addWordSpan).click(function () {
            _this.parent.addNewCondition();
        });
        delimeterDiv.appendChild(addWordSpan);
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeCondition(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
    };
    RegExCondition.prototype.createTextDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = "A zároveň";
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeCondition(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
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
        searchDestinationSelect.appendChild(this.createOption("Text", 4 /* Text */.toString()));
        searchDestinationSelect.appendChild(this.createOption("Autor", 0 /* Author */.toString()));
        searchDestinationSelect.appendChild(this.createOption("Titul", 1 /* Title */.toString()));
        searchDestinationSelect.appendChild(this.createOption("Editor", 2 /* Responsible */.toString()));
        this.selectedSearchType = 4 /* Text */;
        $(searchDestinationSelect).change(function (eventData) {
            _this.selectedSearchType = parseInt($(eventData.target).val());
        });
        var wordFormDiv = document.createElement("div");
        $(wordFormDiv).addClass("regexsearch-word-form-div");
        //mainSearchDiv.appendChild(wordFormDiv); //TODO implement after it iss implemented on server side
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
        $(conditionsDiv).append(mainSearchDiv);
        var delimeterDiv = document.createElement("div");
        $(delimeterDiv).addClass("regexsearch-delimiter");
        $(conditionsDiv).append(delimeterDiv);
        this.resetWords();
        this.html = conditionsDiv;
        this.setClickableDelimeter();
    };
    RegExCondition.prototype.resetWords = function () {
        $(this.conditionContainerDiv).empty();
        this.conditionInputArray = [];
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExWordCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.conditionContainerDiv.appendChild(newWordCondition.getHtml());
    };
    RegExCondition.prototype.addWord = function () {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExWordCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.conditionContainerDiv.appendChild(newWordCondition.getHtml());
    };
    RegExCondition.prototype.removeWord = function (condition) {
        var index = this.conditionInputArray.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.conditionInputArray[index];
            this.conditionContainerDiv.removeChild(arrayItem.getHtml());
            this.conditionInputArray.splice(index, 1);
        }
        if (this.conditionInputArray.length === 1) {
            this.conditionInputArray[0].setClickableDelimeter();
        }
        if (this.conditionInputArray.length === 0) {
            this.resetWords();
        }
    };
    RegExCondition.prototype.getConditionValue = function () {
        var criteriaDescriptions = new WordsCriteriaConditionDescription();
        criteriaDescriptions.searchType = this.getSearchType();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExWordCondition = this.conditionInputArray[i];
            criteriaDescriptions.wordCriteriaDescription.push(regExWordCondition.getConditionsValue());
        }
        return criteriaDescriptions;
    };
    return RegExCondition;
})(RegExSearchBase);
var RegExWordCondition = (function (_super) {
    __extends(RegExWordCondition, _super);
    function RegExWordCondition(parent) {
        _super.call(this);
        this.parentRegExCondition = parent;
    }
    RegExWordCondition.prototype.getHtml = function () {
        return this.html;
    };
    RegExWordCondition.prototype.removeDelimeter = function () {
        $(this.html).find(".regexsearch-or-delimiter").empty();
    };
    RegExWordCondition.prototype.hasDelimeter = function () {
        var isEmpty = $(this.html).find(".regexsearch-or-delimiter").is(':empty');
        return !isEmpty;
    };
    RegExWordCondition.prototype.setTextDelimeter = function () {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-or-delimiter").append(textDelimeter);
    };
    RegExWordCondition.prototype.setClickableDelimeter = function () {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-or-delimiter").append(clickableDelimeter);
    };
    RegExWordCondition.prototype.createClickableDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = "+ Nebo";
        $(addWordSpan).click(function () {
            _this.parentRegExCondition.addWord();
        });
        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass("regexsearch-or-delimiter");
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parentRegExCondition.removeWord(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
    };
    RegExWordCondition.prototype.createTextDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = "Nebo";
        $(delimeterDiv).addClass("regexsearch-or-delimiter");
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parentRegExCondition.removeWord(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
    };
    RegExWordCondition.prototype.makeRegExWordCondition = function () {
        var _this = this;
        var mainDiv = document.createElement("div");
        $(mainDiv).addClass("reg-ex-word-condition");
        this.inputsContainerDiv = document.createElement("div");
        $(this.inputsContainerDiv).addClass("regexsearch-word-input-list-div");
        mainDiv.appendChild(this.inputsContainerDiv);
        var commandsDiv = document.createElement("div");
        $(commandsDiv).addClass("regexsearch-conditions-commands");
        mainDiv.appendChild(commandsDiv);
        var addConditionButton = this.createButton("+");
        $(addConditionButton).addClass("regexsearch-add-input-button");
        $(addConditionButton).click(function () {
            _this.addInput();
        });
        commandsDiv.appendChild(addConditionButton);
        mainDiv.appendChild(this.createTextDelimeter());
        this.resetInputs();
        this.html = mainDiv;
    };
    RegExWordCondition.prototype.resetInputs = function () {
        $(this.inputsContainerDiv).empty();
        this.inputsArray = new Array();
        this.addInput();
    };
    RegExWordCondition.prototype.addInput = function () {
        var newInput = new RegExWordInput(this);
        newInput.makeRegExInput();
        this.inputsArray.push(newInput);
        this.inputsContainerDiv.appendChild(newInput.getHtml());
    };
    RegExWordCondition.prototype.removeInput = function (input) {
        var index = this.inputsArray.indexOf(input, 0);
        if (index != undefined) {
            var arrayItem = this.inputsArray[index];
            this.inputsContainerDiv.removeChild(arrayItem.getHtml());
            this.inputsArray.splice(index, 1);
        }
        if (this.inputsArray.length === 0) {
            this.resetInputs();
        }
    };
    RegExWordCondition.prototype.getConditionsValue = function () {
        var wordCriteriaDescription = new WordCriteriaDescription();
        for (var i = 0; i < this.inputsArray.length; i++) {
            var wordInput = this.inputsArray[i];
            var inputValue = wordInput.getConditionValue();
            switch (wordInput.getConditionType()) {
                case 0 /* StartsWith */:
                    wordCriteriaDescription.startsWith = inputValue;
                    break;
                case 1 /* Contains */:
                    wordCriteriaDescription.contains.push(inputValue);
                    break;
                case 2 /* EndsWith */:
                    wordCriteriaDescription.endsWith = inputValue;
                    break;
                default:
                    break;
            }
        }
        return wordCriteriaDescription;
    };
    RegExWordCondition.prototype.wordInputConditionChanged = function (wordInput) {
        var condition = wordInput.getConditionType();
    };
    return RegExWordCondition;
})(RegExSearchBase);
var RegExWordInput = (function (_super) {
    __extends(RegExWordInput, _super);
    function RegExWordInput(parent) {
        _super.call(this);
        this.parentRegExWordCondition = parent;
    }
    RegExWordInput.prototype.getHtml = function () {
        return this.html;
    };
    RegExWordInput.prototype.hasDelimeter = function () {
        var delimeter = $(this.html).find(".regexsearch-input-and-delimiter");
        return (typeof delimeter != 'undefined' && delimeter != null);
    };
    RegExWordInput.prototype.makeRegExInput = function () {
        var _this = this;
        var mainDiv = document.createElement("div");
        $(mainDiv).addClass("reg-ex-word-input");
        var lineDiv = document.createElement("div");
        $(lineDiv).addClass("regex-word-input-textbox");
        var editorDiv = document.createElement("div");
        this.editorDiv = editorDiv;
        var conditionTitleDiv = document.createElement("div");
        conditionTitleDiv.innerHTML = "Podmínka";
        editorDiv.appendChild(conditionTitleDiv);
        var conditionTypeDiv = document.createElement("div");
        $(conditionTypeDiv).addClass("regexsearch-condition-type-div");
        editorDiv.appendChild(conditionTypeDiv);
        var conditionSelect = document.createElement("select");
        $(conditionSelect).addClass("regexsearch-condition-select");
        conditionTypeDiv.appendChild(conditionSelect);
        conditionSelect.appendChild(this.createOption("Začíná na", 0 /* StartsWith */.toString()));
        //conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(this.createOption("Obsahuje", 1 /* Contains */.toString()));
        //conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(this.createOption("Končí na", 2 /* EndsWith */.toString()));
        //conditionSelect.appendChild(this.createOption("Nekončí na", this.conditionType.NotEndsWith));
        this.conditionInputType = 0 /* StartsWith */;
        $(conditionSelect).change(function (eventData) {
            _this.conditionInputType = parseInt($(eventData.target).val());
            _this.parentRegExWordCondition.wordInputConditionChanged(_this);
        });
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
            if ($(_this.regexButtonsDiv).is(":hidden")) {
                $(_this.regexButtonsDiv).show();
            }
            else {
                $(_this.regexButtonsDiv).hide();
            }
        });
        lineDiv.appendChild(regExButton);
        var removeButton = this.createButton("");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon");
        $(removeGlyph).addClass("glyphicon-trash");
        removeButton.appendChild(removeGlyph);
        $(removeButton).click(function () {
            _this.parentRegExWordCondition.removeInput(_this);
        });
        lineDiv.appendChild(removeButton);
        mainDiv.appendChild(this.editorDiv);
        mainDiv.appendChild(lineDiv);
        var regexButtonsDiv = document.createElement("div");
        $(regexButtonsDiv).addClass("regexsearch-regex-buttons-div");
        var anythingButton = this.createButton("Cokoliv");
        regexButtonsDiv.appendChild(anythingButton);
        $(anythingButton).addClass("regexsearch-editor-button");
        $(anythingButton).click(function () {
            _this.conditionInput.value += "%";
        });
        var oneCharButton = this.createButton("Jeden znak");
        regexButtonsDiv.appendChild(oneCharButton);
        $(oneCharButton).addClass("regexsearch-editor-button");
        $(oneCharButton).click(function () {
            _this.conditionInput.value += "_";
        });
        this.regexButtonsDiv = regexButtonsDiv;
        $(this.regexButtonsDiv).hide();
        mainDiv.appendChild(regexButtonsDiv);
        this.html = mainDiv;
    };
    RegExWordInput.prototype.getConditionValue = function () {
        return this.conditionInput.value;
    };
    RegExWordInput.prototype.getConditionType = function () {
        return this.conditionInputType;
    };
    return RegExWordInput;
})(RegExSearchBase);
var WordCriteriaDescription = (function () {
    function WordCriteriaDescription() {
        this.contains = new Array();
    }
    return WordCriteriaDescription;
})();
var WordsCriteriaConditionDescription = (function () {
    function WordsCriteriaConditionDescription() {
        this.wordCriteriaDescription = new Array();
    }
    return WordsCriteriaConditionDescription;
})();
var WordInputType;
(function (WordInputType) {
    WordInputType[WordInputType["StartsWith"] = 0] = "StartsWith";
    WordInputType[WordInputType["Contains"] = 1] = "Contains";
    WordInputType[WordInputType["EndsWith"] = 2] = "EndsWith";
})(WordInputType || (WordInputType = {}));
/*
 * CriteriaKey C# Enum values must match with searchType number values
        [EnumMember] Author = 0,
        [EnumMember] Title = 1,
        [EnumMember] Editor = 2,
        [EnumMember] Dating = 3,
        [EnumMember] Text = 4
 *
 */
var SearchType;
(function (SearchType) {
    SearchType[SearchType["Author"] = 0] = "Author";
    SearchType[SearchType["Title"] = 1] = "Title";
    SearchType[SearchType["Responsible"] = 2] = "Responsible";
    SearchType[SearchType["Dating"] = 3] = "Dating";
    SearchType[SearchType["Text"] = 4] = "Text";
})(SearchType || (SearchType = {}));
//# sourceMappingURL=itjakub.plugins.regexsearch.js.map