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
    regExConditions: Array<RegExCondition>;

    constructor(container: HTMLDivElement) {
        super();
        this.container = container;
    }

    public makeRegExSearch() {
        $(this.container).empty();
        this.regExConditions = [];

        var commandsDiv: HTMLDivElement = document.createElement("div");

        var sentButton: HTMLButtonElement = this.createButton("Vyhledat");
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

    public addNewCondition(useDelimiter: boolean = true) {
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
    }

    public removeLastCondition() {
        if (this.regExConditions.length <= 1)
            return;

        var arrayItem: RegExCondition = this.regExConditions.pop();
        this.innerContainer.removeChild(arrayItem.getHtml());
    }

    public removeCondition(condition: RegExCondition) {
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

    public getConditionsResultObject(): Object {
        var resultArray = new Array();

        for (var i = 0; i < this.regExConditions.length; i++) {
            var regExCondition: RegExCondition = this.regExConditions[i];
            resultArray.push(regExCondition.getConditionValue());

        }

        return resultArray;
    }

    public getConditionsResultJSON(): string {
        var jsonString = JSON.stringify(this.getConditionsResultObject());
        return jsonString;
    }

    public processSearch() {
        var json = this.getConditionsResultJSON();

        $.ajax({
            type: "POST",
            traditional: true,
            data: json,
            url: "/Dictionaries/Dictionaries/SearchCriteria", //TODO add getBaseUrl
            dataType: 'json',
            contentType: 'application/json',
            success: (response) => {
            },
            error: (response) => {
            }
        });

    }
}

class RegExCondition extends RegExSearchBase {
    private html: HTMLDivElement;
    private conditionContainerDiv: HTMLDivElement;
    private conditionInputArray: Array<RegExWordCondition>;
    private parent: RegExSearch;
    private selectedSearchType: number;
    private selectedWordFormType: string;

    constructor(parent: RegExSearch) {
        super();
        this.parent = parent;
    }

    public getHtml(): HTMLDivElement {
        return this.html;
    }

    public removeDelimeter() {
        $(this.html).find(".regexsearch-delimiter").empty();
    }

    private hasDelimeter(): boolean {
        var isEmpty = $(this.html).find(".regexsearch-delimiter").is(':empty');
        return !isEmpty;
    }

    public setTextDelimeter() {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-delimiter").append(textDelimeter);
    }

    public setClickableDelimeter() {
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

        var trashButton: HTMLButtonElement = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph: HTMLSpanElement = document.createElement("span");
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

        var trashButton: HTMLButtonElement = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph: HTMLSpanElement = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeCondition(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }



    public getWordFormType(): string {
        return this.selectedWordFormType;
    }

    public getSearchType(): number {
        return this.selectedSearchType;
    }

    private wordFormType = {
        Lemma: "lemma",
        HyperlemmaNew: "hyperlemma-new",
        HyperlemmaOld: "hyperlemma-old",
        Stemma: "stemma"
    }

    public makeRegExCondition() {

        var conditionsDiv = document.createElement("div");
        $(conditionsDiv).addClass("regexsearch-condition-main-div");
        
        var mainSearchDiv: HTMLDivElement = document.createElement("div");

        var searchDestinationDiv: HTMLDivElement = document.createElement("div");
        $(searchDestinationDiv).addClass("regexsearch-destination-div");
        mainSearchDiv.appendChild(searchDestinationDiv);

        var searchDestinationSpan: HTMLSpanElement = document.createElement("span");
        searchDestinationSpan.innerHTML = "Zvolte oblast vyhledávání";
        $(searchDestinationSpan).addClass("regexsearch-upper-select-label");
        searchDestinationDiv.appendChild(searchDestinationSpan);

        var searchDestinationSelect: HTMLSelectElement = document.createElement("select");
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

        var wordFormDiv: HTMLDivElement = document.createElement("div");
        $(wordFormDiv).addClass("regexsearch-word-form-div");
        //mainSearchDiv.appendChild(wordFormDiv); //TODO implement after it iss implemented on server side

        var wordFormSpan: HTMLSpanElement = document.createElement("span");
        wordFormSpan.innerHTML = "Tvar slova";
        $(wordFormSpan).addClass("regexsearch-upper-select-label");
        wordFormDiv.appendChild(wordFormSpan);

        var wordFormSelect: HTMLSelectElement = document.createElement("select");
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
    }

    public resetWords() {
        $(this.conditionContainerDiv).empty();
        this.conditionInputArray = [];
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExWordCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);        
        this.conditionContainerDiv.appendChild(newWordCondition.getHtml());
    }

    public addWord() {
        this.conditionInputArray[this.conditionInputArray.length-1].setTextDelimeter();
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExWordCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.conditionContainerDiv.appendChild(newWordCondition.getHtml());
    }

    public removeWord(condition: RegExWordCondition) {

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
    }

    public getConditionValue(): WordsCriteriaConditionDescription {
        var criteriaDescriptions = new WordsCriteriaConditionDescription();
        criteriaDescriptions.searchType = this.getSearchType();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExWordCondition = this.conditionInputArray[i];
            criteriaDescriptions.wordCriteriaDescription.push(regExWordCondition.getConditionsValue());
        }
        return criteriaDescriptions;
    }
}


class RegExWordCondition extends RegExSearchBase {
    private html: HTMLDivElement;
    private parentRegExCondition: RegExCondition;
    private inputsArray: Array<RegExWordInput>;
    private hiddenWordInputSelects: Array<WordInputType>;
    private inputsContainerDiv: HTMLDivElement;

    constructor(parent: RegExCondition) {
        super();
        this.parentRegExCondition = parent;
    }

    public getHtml(): HTMLDivElement {
        return this.html;
    }

    private removeDelimeter() {
        $(this.html).find(".regexsearch-or-delimiter").empty();
    }

    private hasDelimeter(): boolean {
        var isEmpty = $(this.html).find(".regexsearch-or-delimiter").is(':empty');
        return !isEmpty;
    }

    public setTextDelimeter() {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-or-delimiter").append(textDelimeter);
    }

    public setClickableDelimeter() {
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
            this.parentRegExCondition.addWord();
        });

        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass("regexsearch-or-delimiter");

        var trashButton: HTMLButtonElement = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph: HTMLSpanElement = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parentRegExCondition.removeWord(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    private createTextDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = "Nebo";
        $(delimeterDiv).addClass("regexsearch-or-delimiter");

        var trashButton: HTMLButtonElement = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph: HTMLSpanElement = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parentRegExCondition.removeWord(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    public makeRegExWordCondition() {
        var mainDiv: HTMLDivElement = document.createElement("div");
        $(mainDiv).addClass("reg-ex-word-condition");

        this.inputsContainerDiv = document.createElement("div");
        $(this.inputsContainerDiv).addClass("regexsearch-word-input-list-div");
        mainDiv.appendChild(this.inputsContainerDiv);

        var commandsDiv: HTMLDivElement = document.createElement("div");
        $(commandsDiv).addClass("regexsearch-conditions-commands");
        mainDiv.appendChild(commandsDiv);

        var addConditionButton: HTMLButtonElement = this.createButton("+");
        $(addConditionButton).addClass("regexsearch-add-input-button");
        $(addConditionButton).click(() => {
            this.addInput();
        });
        commandsDiv.appendChild(addConditionButton);
        mainDiv.appendChild(this.createTextDelimeter());
        this.resetInputs();
        this.html = mainDiv;
    }

    public resetInputs() {
        this.hiddenWordInputSelects = new Array<WordInputType>();
        $(this.inputsContainerDiv).empty();
        this.inputsArray = new Array<RegExWordInput>();
        this.addInput();
    }

    public addInput() {
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

    public removeInput(input: RegExWordInput) {
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

    public getConditionsValue(): WordCriteriaDescription {
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

        if (typeof oldWordInputType !== 'undefined') {
            this.wordInpuConditionRemoved(oldWordInputType);   
        }

        if (!(newWordInputType === WordInputType.Contains)) {
            for (var i = 0; i < this.inputsArray.length; i++) {
                if(this.inputsArray[i] === wordInput) continue;
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

    public getHtml(): HTMLDivElement {
        return this.html;
    }

    public hasDelimeter(): boolean {
        var delimeter = $(this.html).find(".regexsearch-input-and-delimiter");
        return (typeof delimeter != 'undefined' && delimeter != null) ;
    }

    public makeRegExInput() {
        var mainDiv: HTMLDivElement = document.createElement("div");
        $(mainDiv).addClass("reg-ex-word-input");

        var lineDiv = document.createElement("div");
        $(lineDiv).addClass("regex-word-input-textbox");

        var editorDiv = document.createElement("div");
        this.editorDiv = editorDiv;

        var conditionTitleDiv: HTMLDivElement = document.createElement("div");
        conditionTitleDiv.innerHTML = "Podmínka";
        editorDiv.appendChild(conditionTitleDiv);


        var conditionTypeDiv: HTMLDivElement = document.createElement("div");
        $(conditionTypeDiv).addClass("regexsearch-condition-type-div");
        editorDiv.appendChild(conditionTypeDiv);

        var conditionSelect: HTMLSelectElement = document.createElement("select");
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

        var regExButton: HTMLButtonElement = document.createElement("button");
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

        var removeButton: HTMLButtonElement = this.createButton("");
        var removeGlyph: HTMLSpanElement = document.createElement("span");
        $(removeGlyph).addClass("glyphicon");
        $(removeGlyph).addClass("glyphicon-trash");
        removeButton.appendChild(removeGlyph);
        $(removeButton).click(() => {
            this.parentRegExWordCondition.removeInput(this);
        });

        lineDiv.appendChild(removeButton);
        mainDiv.appendChild(this.editorDiv);
        mainDiv.appendChild(lineDiv);


        var regexButtonsDiv: HTMLDivElement = document.createElement("div");
        $(regexButtonsDiv).addClass("regexsearch-regex-buttons-div");

        var anythingButton: HTMLButtonElement = this.createButton("Cokoliv");
        regexButtonsDiv.appendChild(anythingButton);
        $(anythingButton).addClass("regexsearch-editor-button");
        $(anythingButton).click(() => {
            this.conditionInput.value += "%";
        });

        var oneCharButton: HTMLButtonElement = this.createButton("Jeden znak");
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

    public getConditionValue(): string {
        return this.conditionInput.value;
    }

    public getConditionType(): WordInputType {
        return this.conditionInputType;
    }

    showSelectCondition(wordInputType: WordInputType) {
        $(this.conditionSelectbox).find("option[value=" + (wordInputType.toString())+"]").show();
    }

    hideSelectCondition(wordInputType: WordInputType) {
        $(this.conditionSelectbox).find("option[value=" + (wordInputType.toString()) + "]").hide();
    }


}

class WordCriteriaDescription {
    public startsWith: string;
    public contains: Array<string>;
    public endsWith: string;

    constructor() {
        this.contains = new Array<string>();
    }
}


class WordsCriteriaConditionDescription {
    public searchType: number;
    public wordCriteriaDescription: Array<WordCriteriaDescription>;

    constructor() {
        this.wordCriteriaDescription = new Array<WordCriteriaDescription>();
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
