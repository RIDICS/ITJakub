class RegExSearchBase {

    protected createOption(label: string, value: string): HTMLOptionElement {
        var conditionOption = document.createElement("option");
        conditionOption.innerHTML = label;
        conditionOption.value = value;

        return conditionOption;
    }

    protected createButton(label: string): HTMLButtonElement {
        var button = document.createElement("button");
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
    regExConditions: Array<RegExConditionListItem>;

    constructor(container: HTMLDivElement) {
        super();
        this.container = container;
    }

    makeRegExSearch() {
        $(this.container).empty();
        this.regExConditions = [];

        var commandsDiv = document.createElement("div");

        var sentButton = this.createButton("Vyhledat");
        $(sentButton).addClass("regex-search-button");
        commandsDiv.appendChild(sentButton);
        $(sentButton).click(() => {
            this.processSearch();
        });

        this.innerContainer = document.createElement("div");
        this.addNewCondition(true);
        $(this.container).append(this.innerContainer);
        $(this.container).append(commandsDiv);
    }

    addNewCondition(useDelimiter: boolean = true) {
        if (this.regExConditions.length > 0) {
            this.regExConditions[this.regExConditions.length - 1].setTextDelimeter();
        }
        var newRegExConditions = new RegExConditionListItem(this);
        newRegExConditions.makeRegExCondition();
        newRegExConditions.setClickableDelimeter();
        if (!useDelimiter) {
            newRegExConditions.removeDelimeter();
        }
        this.regExConditions.push(newRegExConditions);
        $(this.innerContainer).append(newRegExConditions.getHtml());
    }

    removeLastCondition() {
        if (this.regExConditions.length <= 1)
            return;

        var arrayItem = this.regExConditions.pop();
        this.innerContainer.removeChild(arrayItem.getHtml());
    }

    removeCondition(condition: RegExConditionListItem) {
        var index = this.regExConditions.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.regExConditions[index];
            this.innerContainer.removeChild(arrayItem.getHtml());
            this.regExConditions.splice(index, 1);
        }

        if (this.regExConditions.length === 0) {
            this.addNewCondition(true);
        } else {
            this.regExConditions[this.regExConditions.length - 1].setClickableDelimeter();
        }
    }

    getConditionsResultObject(): Object {
        var resultArray = new Array();

        for (var i = 0; i < this.regExConditions.length; i++) {
            var regExCondition = this.regExConditions[i];
            resultArray.push(regExCondition.getConditionValue());

        }

        return resultArray;
    }

    getConditionsResultJSON(): string {
        var jsonString = JSON.stringify(this.getConditionsResultObject());
        return jsonString;
    }

    processSearch() {
        var json = this.getConditionsResultJSON();

        $.ajax({
            type: "POST",
            traditional: true,
            data: json,
            url: "/Dictionaries/Dictionaries/SearchCriteria", //TODO add getBaseUrl
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
            },
            error: (response) => {
            }
        });

    }
}

class RegExConditionListItem extends RegExSearchBase {
    private html: HTMLDivElement;
    private parent: RegExSearch;
    private selectedSearchType: number;
    private innerConditionContainer: HTMLDivElement;
    private innerCondition: RegExConditionBase;

    constructor(parent: RegExSearch) {
        super();
        this.parent = parent;
    }

    getHtml(): HTMLDivElement {
        return this.html;
    }

    removeDelimeter() {
        $(this.html).find(".regexsearch-delimiter").empty();
    }

    private hasDelimeter(): boolean {
        var isEmpty = $(this.html).find(".regexsearch-delimiter").is(":empty");
        return !isEmpty;
    }

    setTextDelimeter() {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-delimiter").append(textDelimeter);
    }

    setClickableDelimeter() {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-delimiter").append(clickableDelimeter);
    }

    private createClickableDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = "+ A zároveň";
        $(addWordSpan).click(() => {
            this.parent.addNewCondition();
        });

        delimeterDiv.appendChild(addWordSpan);

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeCondition(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    private createTextDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = "A zároveň";

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeCondition(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    getSearchType(): number {
        return this.selectedSearchType;
    }

    makeRegExCondition() {
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

        searchDestinationSelect.appendChild(this.createOption("Text", SearchType.Text.toString()));
        searchDestinationSelect.appendChild(this.createOption("Autor", SearchType.Author.toString()));
        searchDestinationSelect.appendChild(this.createOption("Titul", SearchType.Title.toString()));
        searchDestinationSelect.appendChild(this.createOption("Editor", SearchType.Responsible.toString()));

        this.selectedSearchType = SearchType.Text;

        $(searchDestinationSelect).change((eventData: Event) => {
            this.selectedSearchType = parseInt($(eventData.target).val());
        });

        $(conditionsDiv).append(mainSearchDiv);

        this.innerConditionContainer = document.createElement("div");
        $(this.innerConditionContainer).addClass("regex-inner-conditon-container");
        this.makeDefaultCondition();

        $(conditionsDiv).append(this.innerConditionContainer);

        var delimeterDiv = document.createElement("div");
        $(delimeterDiv).addClass("regexsearch-delimiter");
        $(conditionsDiv).append(delimeterDiv);
        this.setClickableDelimeter();

        this.html = conditionsDiv;
    }

    private makeDefaultCondition() {
        $(this.innerConditionContainer).empty();
        this.innerCondition = new RegExWordConditionList(this);
        this.innerCondition.makeRegExCondition(this.innerConditionContainer);
    }

    getConditionValue(): ConditionResult {
        var conditionResult: ConditionResult = this.innerCondition.getConditionValue();
        conditionResult.searchType = this.getSearchType();
        return conditionResult;
    }
}

interface IRegExConditionBase {
    makeRegExCondition(conditionContainerDiv: HTMLDivElement);

    getConditionValue(): ConditionResult;
}

class RegExConditionBase extends RegExSearchBase implements IRegExConditionBase {
    protected parentRegExConditionList: RegExConditionListItem;

    constructor(parent: RegExConditionListItem) {
        super();
        this.parentRegExConditionList = parent;
    }

    makeRegExCondition(conditionContainerDiv: HTMLDivElement) { }

    getConditionValue() : ConditionResult { return null; }
}

class RegExWordConditionList extends RegExConditionBase {
    private html: HTMLDivElement;
    private selectedWordFormType: string;
    private wordListContainerDiv: HTMLDivElement;
    private conditionInputArray: Array<RegExWordCondition>;

    constructor(parent: RegExConditionListItem) {
        super(parent);
    }

    getWordFormType(): string {
        return this.selectedWordFormType;
    }

    private wordFormType = {
        Lemma: "lemma",
        HyperlemmaNew: "hyperlemma-new",
        HyperlemmaOld: "hyperlemma-old",
        Stemma: "stemma"
    };

    public makeRegExCondition(conditionContainerDiv: HTMLDivElement) {
        var wordFormDiv = document.createElement("div");
        $(wordFormDiv).addClass("regexsearch-word-form-div");
        //wordListContainerDiv.appendChild(wordFormDiv); //TODO implement after it iss implemented on server side

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

        $(wordFormSelect).change((eventData: Event) => {
            this.selectedWordFormType = $(eventData.target).val();
        });

        this.wordListContainerDiv = document.createElement("div");
        $(this.wordListContainerDiv).addClass("regexsearch-condition-list-div");
        conditionContainerDiv.appendChild(this.wordListContainerDiv);

        this.resetWords();
    }

    getConditionValue(): WordsCriteriaListDescription {
        var criteriaDescriptions = new WordsCriteriaListDescription();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExWordCondition = this.conditionInputArray[i];
            criteriaDescriptions.wordCriteriaDescription.push(regExWordCondition.getConditionsValue());
        }
        return criteriaDescriptions;
    }

    resetWords() {
        $(this.wordListContainerDiv).empty();
        this.conditionInputArray = [];
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExWordCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.wordListContainerDiv.appendChild(newWordCondition.getHtml());
    }

    addWord() {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExWordCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.wordListContainerDiv.appendChild(newWordCondition.getHtml());
    }

    removeWord(condition: RegExWordCondition) {

        var index = this.conditionInputArray.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.conditionInputArray[index];
            this.wordListContainerDiv.removeChild(arrayItem.getHtml());
            this.conditionInputArray.splice(index, 1);
        }

        if (this.conditionInputArray.length === 1) {
            this.conditionInputArray[0].setClickableDelimeter();
        }

        if (this.conditionInputArray.length === 0) {
            this.resetWords();
        }
    }
}


class RegExDatingCondition extends RegExConditionBase {

    constructor(parent: RegExConditionListItem) {
        super(parent);
    }
}

class RegExWordCondition {
    private html: HTMLDivElement;
    private inputsArray: Array<RegExWordInput>;
    private hiddenWordInputSelects: Array<WordInputType>;
    private inputsContainerDiv: HTMLDivElement;
    private parent: RegExWordConditionList;

    constructor(parent: RegExWordConditionList) {
        this.parent = parent;
    }

    getHtml(): HTMLDivElement {
        return this.html;
    }

    private removeDelimeter() {
        $(this.html).find(".regexsearch-or-delimiter").empty();
    }

    private hasDelimeter(): boolean {
        var isEmpty = $(this.html).find(".regexsearch-or-delimiter").is(":empty");
        return !isEmpty;
    }

    setTextDelimeter() {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-or-delimiter").append(textDelimeter);
    }

    setClickableDelimeter() {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-or-delimiter").append(clickableDelimeter);
    }

    private createClickableDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = "+ Nebo";
        $(addWordSpan).click(() => {
            this.parent.addWord();
        });

        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass("regexsearch-or-delimiter");

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeWord(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    private createTextDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = "Nebo";
        $(delimeterDiv).addClass("regexsearch-or-delimiter");

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeWord(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    makeRegExWordCondition() {
        var mainDiv = document.createElement("div");
        $(mainDiv).addClass("reg-ex-word-condition");

        this.inputsContainerDiv = document.createElement("div");
        $(this.inputsContainerDiv).addClass("regexsearch-word-input-list-div");
        mainDiv.appendChild(this.inputsContainerDiv);

        var commandsDiv = document.createElement("div");
        $(commandsDiv).addClass("regexsearch-conditions-commands");
        mainDiv.appendChild(commandsDiv);

        var addConditionButton = document.createElement("button");
        addConditionButton.type = "button";
        addConditionButton.innerHTML = "+";
        $(addConditionButton).addClass("btn");
        $(addConditionButton).addClass("btn-default");
        $(addConditionButton).addClass("regexsearch-button");
        $(addConditionButton).addClass("regexsearch-add-input-button");
        $(addConditionButton).click(() => {
            this.addInput();
        });
        commandsDiv.appendChild(addConditionButton);
        mainDiv.appendChild(this.createTextDelimeter());
        this.resetInputs();
        this.html = mainDiv;
    }

    resetInputs() {
        this.hiddenWordInputSelects = new Array<WordInputType>();
        $(this.inputsContainerDiv).empty();
        this.inputsArray = new Array<RegExWordInput>();
        this.addInput();
    }

    addInput() {
        var newInput = new RegExWordInput(this);
        newInput.makeRegExInput();
        for (var i = 0; i < this.hiddenWordInputSelects.length; i++) {
            newInput.hideSelectCondition(this.hiddenWordInputSelects[i]);
        }
        if (!(newInput.getConditionType() === WordInputType.Contains)) {
            this.hiddenWordInputSelects.push(newInput.getConditionType());
        }

        this.inputsArray.push(newInput);
        this.inputsContainerDiv.appendChild(newInput.getHtml());
    }

    removeInput(input: RegExWordInput) {
        this.wordInpuConditionRemoved(input.getConditionType());
        var index = this.inputsArray.indexOf(input, 0);
        if (index >= 0) {
            var arrayItem = this.inputsArray[index];
            this.inputsContainerDiv.removeChild(arrayItem.getHtml());
            this.inputsArray.splice(index, 1);
        }

        if (this.inputsArray.length === 0) {
            this.resetInputs();
        }
    }

    getConditionsValue(): WordCriteriaDescription {
        var wordCriteriaDescription = new WordCriteriaDescription();
        for (var i = 0; i < this.inputsArray.length; i++) {
            var wordInput = this.inputsArray[i];
            var inputValue = wordInput.getConditionValue();
            switch (wordInput.getConditionType()) {
            case WordInputType.StartsWith:
                wordCriteriaDescription.startsWith = inputValue;
                break;
            case WordInputType.Contains:
                wordCriteriaDescription.contains.push(inputValue);
                break;
            case WordInputType.EndsWith:
                wordCriteriaDescription.endsWith = inputValue;
                break;
            default:
                break;
            }
        }
        return wordCriteriaDescription;
    }


    wordInputConditionChanged(wordInput: RegExWordInput, oldWordInputType: WordInputType) {
        var newWordInputType = wordInput.getConditionType();

        if (typeof oldWordInputType !== "undefined") {
            this.wordInpuConditionRemoved(oldWordInputType);
        }

        if (!(newWordInputType === WordInputType.Contains)) {
            for (var i = 0; i < this.inputsArray.length; i++) {
                if (this.inputsArray[i] === wordInput) continue;
                this.inputsArray[i].hideSelectCondition(newWordInputType);
            }

            this.hiddenWordInputSelects.push(newWordInputType);
        }
    }

    wordInpuConditionRemoved(wordInputType: WordInputType) {

        if (!(wordInputType === WordInputType.Contains)) {
            for (var i = 0; i < this.inputsArray.length; i++) {
                this.inputsArray[i].showSelectCondition(wordInputType);
            }
        }

        var index = this.hiddenWordInputSelects.indexOf(wordInputType, 0);
        if (index >= 0) {
            this.hiddenWordInputSelects.splice(index, 1);
        }
    }
}


class RegExWordInput extends RegExSearchBase {
    private html: HTMLDivElement;
    private editorDiv: HTMLDivElement;
    private conditionInput: HTMLInputElement;
    private conditionInputType: WordInputType;
    private parentRegExWordCondition: RegExWordCondition;
    private regexButtonsDiv: HTMLDivElement;
    private conditionSelectbox: HTMLSelectElement;

    constructor(parent: RegExWordCondition) {
        super();
        this.parentRegExWordCondition = parent;
    }

    getHtml(): HTMLDivElement {
        return this.html;
    }

    hasDelimeter(): boolean {
        var delimeter = $(this.html).find(".regexsearch-input-and-delimiter");
        return (typeof delimeter != "undefined" && delimeter != null);
    }

    makeRegExInput() {
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

        conditionSelect.appendChild(this.createOption("Začíná na", WordInputType.StartsWith.toString()));
        //conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(this.createOption("Obsahuje", WordInputType.Contains.toString()));
        //conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(this.createOption("Končí na", WordInputType.EndsWith.toString()));
        //conditionSelect.appendChild(this.createOption("Nekončí na", this.conditionType.NotEndsWith));


        $(conditionSelect).change((eventData: Event) => {
            var oldConditonType = this.conditionInputType;
            this.conditionInputType = parseInt($(eventData.target).val());
            this.parentRegExWordCondition.wordInputConditionChanged(this, oldConditonType);
        });

        this.conditionSelectbox = conditionSelect;

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
        $(regExButton).click(() => {
            if ($(this.regexButtonsDiv).is(":hidden")) {
                $(this.regexButtonsDiv).show();
            } else {
                $(this.regexButtonsDiv).hide();
            }
        });
        lineDiv.appendChild(regExButton);

        var removeButton = this.createButton("");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon");
        $(removeGlyph).addClass("glyphicon-trash");
        removeButton.appendChild(removeGlyph);
        $(removeButton).click(() => {
            this.parentRegExWordCondition.removeInput(this);
        });

        lineDiv.appendChild(removeButton);
        mainDiv.appendChild(this.editorDiv);
        mainDiv.appendChild(lineDiv);


        var regexButtonsDiv = document.createElement("div");
        $(regexButtonsDiv).addClass("regexsearch-regex-buttons-div");

        var anythingButton = this.createButton("Cokoliv");
        regexButtonsDiv.appendChild(anythingButton);
        $(anythingButton).addClass("regexsearch-editor-button");
        $(anythingButton).click(() => {
            this.conditionInput.value += "%";
        });

        var oneCharButton = this.createButton("Jeden znak");
        regexButtonsDiv.appendChild(oneCharButton);
        $(oneCharButton).addClass("regexsearch-editor-button");
        $(oneCharButton).click(() => {
            this.conditionInput.value += "_";
        });

        this.regexButtonsDiv = regexButtonsDiv;
        $(this.regexButtonsDiv).hide();
        mainDiv.appendChild(regexButtonsDiv);

        this.html = mainDiv;

        $(this.conditionSelectbox).val(WordInputType.Contains.toString());
        $(this.conditionSelectbox).change();
    }

    getConditionValue(): string {
        return this.conditionInput.value;
    }

    getConditionType(): WordInputType {
        return this.conditionInputType;
    }

    showSelectCondition(wordInputType: WordInputType) {
        $(this.conditionSelectbox).find(`option[value=${wordInputType.toString()}]`).show();
    }

    hideSelectCondition(wordInputType: WordInputType) {
        $(this.conditionSelectbox).find(`option[value=${wordInputType.toString()}]`).hide();
    }


}

class ConditionResult {
    searchType: number;   
}


class WordsCriteriaListDescription extends ConditionResult {
    wordCriteriaDescription: Array<WordCriteriaDescription>;

    constructor() {
        super();
        this.wordCriteriaDescription = new Array<WordCriteriaDescription>();
    }
}

class WordCriteriaDescription {
    startsWith: string;
    contains: Array<string>;
    endsWith: string;

    constructor() {
        this.contains = new Array<string>();
    }
}

enum WordInputType {
    StartsWith = 0,
    Contains = 1,
    EndsWith = 2
}

/*
 * CriteriaKey C# Enum values must match with searchType number values
        [EnumMember] Author = 0,
        [EnumMember] Title = 1,
        [EnumMember] Editor = 2,
        [EnumMember] Dating = 3,
        [EnumMember] Text = 4
 * 
 */
enum SearchType {
    Author = 0,
    Title = 1,
    Responsible = 2,
    Dating = 3,
    Text = 4
}