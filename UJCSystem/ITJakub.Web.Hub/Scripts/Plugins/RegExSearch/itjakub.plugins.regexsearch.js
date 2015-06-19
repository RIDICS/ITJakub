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
    RegExCondition.prototype.getConditionString = function () {
        var conditionString = this.conditionInputArray[0].getConditionsValue();
        for (var i = 1; i < this.conditionInputArray.length; i++) {
            var regExInput = this.conditionInputArray[i];
            conditionString += "&" + regExInput.getConditionsValue(); //TODO change to AND
        }
        return conditionString;
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
        this.inputsArray = [];
        var newInput = new RegExWordInput(this);
        newInput.makeRegExInput();
        //newInput.removeDelimeter();
        this.inputsArray.push(newInput);
        this.inputsContainerDiv.appendChild(newInput.getHtml());
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
        //if (this.inputsArray[0].hasDelimeter) {
        //    this.inputsArray[0].removeDelimeter();
        //}
    };
    RegExWordCondition.prototype.getConditionsValue = function () {
        //for (var i = 0; i < LENGTH; i++) {
        //}
        //TODO implement
        return "";
    };
    return RegExWordCondition;
})(RegExSearchBase);
var RegExWordInput = (function (_super) {
    __extends(RegExWordInput, _super);
    function RegExWordInput(parent) {
        _super.call(this);
        this.conditionType = Object.freeze({
            StartsWith: "starts-with",
            Contains: "contains",
            EndsWith: "ends-with"
        });
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
        //var labelDiv = document.createElement("div");
        //labelDiv.innerHTML = "A zároveň";
        //$(labelDiv).addClass("regexsearch-input-and-delimiter");
        //mainDiv.appendChild(labelDiv);
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
        conditionSelect.appendChild(this.createOption("Začíná na", this.conditionType.StartsWith));
        //conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(this.createOption("Obsahuje", this.conditionType.Contains));
        //conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(this.createOption("Končí na", this.conditionType.EndsWith));
        //conditionSelect.appendChild(this.createOption("Nekončí na", this.conditionType.NotEndsWith));
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
        //$(regExButton).click(() => {
        //    if (!this.regExEditor || this.editorDiv.children.length === 0) {
        //        this.regExEditor = new RegExEditor(this.editorDiv, this.conditionInput);
        //        this.regExEditor.makeRegExEditor();
        //    } else if ($(this.editorDiv).hasClass("hidden")) {
        //        $(this.editorDiv).removeClass("hidden");
        //    } else {
        //        $(this.editorDiv).addClass("hidden");
        //    }
        //});
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
        this.html = mainDiv;
    };
    RegExWordInput.prototype.getConditionValue = function () {
        return this.conditionInput.value;
    };
    return RegExWordInput;
})(RegExSearchBase);
//class RegExEditor extends RegExSearchBase {
//    container: HTMLDivElement;
//    searchBox: HTMLInputElement;
//    constructor(container: HTMLDivElement, searchBox: HTMLInputElement) {
//        super();
//        this.container = container;
//        this.searchBox = searchBox;
//    }
//    private conditionType = Object.freeze({
//        StartsWith: "starts-with",
//        NotStartsWith: "not-starts-with",
//        Contains: "contains",
//        NotContains: "not-contains",
//        EndsWith: "ends-with",
//        NotEndsWith: "not-ends-with"
//    });
//    public makeRegExEditor() {
//        $(this.container).empty();
//        var mainRegExDiv: HTMLDivElement = document.createElement("div");
//        $(mainRegExDiv).addClass("content-container");
//        $(mainRegExDiv).addClass("regexsearch-editor-container");
//        var titleHeading: HTMLSpanElement = document.createElement("span");
//        titleHeading.innerHTML = "Editor regulárního výrazu";
//        $(titleHeading).addClass("regexsearch-editor-title");
//        mainRegExDiv.appendChild(titleHeading);
//        var editorDiv: HTMLDivElement = document.createElement("div");
//        mainRegExDiv.appendChild(editorDiv);
//        var conditionTitleDiv: HTMLDivElement = document.createElement("div");
//        conditionTitleDiv.innerHTML = "Podmínka";
//        editorDiv.appendChild(conditionTitleDiv);
//        var conditionTypeDiv: HTMLDivElement = document.createElement("div");
//        $(conditionTypeDiv).addClass("regexsearch-condition-type-div");
//        editorDiv.appendChild(conditionTypeDiv);
//        var conditionSelect: HTMLSelectElement = document.createElement("select");
//        $(conditionSelect).addClass("regexsearch-condition-select");
//        conditionTypeDiv.appendChild(conditionSelect);
//        conditionSelect.appendChild(this.createOption("Začíná na", this.conditionType.StartsWith));
//        conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
//        conditionSelect.appendChild(this.createOption("Obsahuje", this.conditionType.Contains));
//        conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
//        conditionSelect.appendChild(this.createOption("Končí na", this.conditionType.EndsWith));
//        conditionSelect.appendChild(this.createOption("Nekončí na", this.conditionType.NotEndsWith));
//        var conditionDiv: HTMLDivElement = document.createElement("div");
//        $(conditionDiv).addClass("regexsearch-condition-div");
//        editorDiv.appendChild(conditionDiv);
//        var conditionInputDiv: HTMLDivElement = document.createElement("div");
//        conditionDiv.appendChild(conditionInputDiv);
//        var conditionInput: HTMLInputElement = document.createElement("input");
//        conditionInput.type = "text";
//        $(conditionInput).addClass("regexsearch-input");
//        conditionInputDiv.appendChild(conditionInput);
//        var conditionButtonsDiv: HTMLDivElement = document.createElement("div");
//        conditionDiv.appendChild(conditionButtonsDiv);
//        var anythingButton: HTMLButtonElement = this.createButton("Cokoliv");
//        conditionButtonsDiv.appendChild(anythingButton);
//        $(anythingButton).addClass("regexsearch-editor-button");
//        $(anythingButton).click(() => {
//            conditionInput.value += ".*";
//        });
//        var orButton: HTMLButtonElement = this.createButton("Nebo");
//        conditionButtonsDiv.appendChild(orButton);
//        orButton.style.cssFloat = "right";
//        $(orButton).addClass("regexsearch-editor-button");
//        $(orButton).click(() => {
//            conditionInput.value += "|";
//        });
//        var commandButtonsDiv: HTMLDivElement = document.createElement("div");
//        $(commandButtonsDiv).addClass("regexsearch-command-buttons-div");
//        editorDiv.appendChild(commandButtonsDiv);
//        var stornoButton: HTMLButtonElement = this.createButton("Zrušit");
//        commandButtonsDiv.appendChild(stornoButton);
//        $(stornoButton).click(() => {
//            $(this.container).empty();
//        });
//        var submitButton: HTMLButtonElement = this.createButton("Dokončit");
//        commandButtonsDiv.appendChild(submitButton);
//        $(submitButton).click(() => {
//            var inputValue: string = conditionInput.value;
//            var outputValue: string;
//            switch (conditionSelect.value) {
//                case this.conditionType.StartsWith:
//                    outputValue = "(" + inputValue + ")(.*)";
//                    break;
//                case this.conditionType.NotStartsWith:
//                    outputValue = "(?!" + inputValue + ")(.*)";
//                    break;
//                case this.conditionType.Contains:
//                    outputValue = "(.*)(" + inputValue + ")(.*)";
//                    break;
//                case this.conditionType.NotContains:
//                    outputValue = "((?!" + inputValue + ").)*";
//                    break;
//                case this.conditionType.EndsWith:
//                    outputValue = "(.*)(" + inputValue + ")";
//                    break;
//                case this.conditionType.NotEndsWith:
//                    outputValue = "((?!" + inputValue + "$).)*";
//                    break;
//                default:
//                    outputValue = "";
//            }
//            this.searchBox.value = "^" + outputValue + "$";
//            $(this.container).addClass("hidden");
//        });
//        $(this.container).append(mainRegExDiv);
//    }
//}
//# sourceMappingURL=itjakub.plugins.regexsearch.js.map