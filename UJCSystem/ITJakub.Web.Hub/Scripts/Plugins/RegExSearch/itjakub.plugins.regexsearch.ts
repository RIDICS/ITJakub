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
        //for (var i = 0; i < this.regExConditions.length; i++) {
        //    var regExCondition: RegExCondition = this.regExConditions[i];
        //    var resultArrayForType: Array<Object> = resultObject[regExCondition.getSearchType()];
        //    if (typeof resultArrayForType == 'undefined' || resultArrayForType == null) {
        //        resultArrayForType = new Array();
        //        resultObject[regExCondition.getSearchType()] = resultArrayForType;
        //    }
        //    resultArrayForType.push(regExCondition.getConditionValue());
        //}

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

    
/*
 * CriteriaKey C# Enum values must match with searchType number values
        [EnumMember] Author = 0,
        [EnumMember] Title = 1,
        [EnumMember] Editor = 2,
        [EnumMember] Dating = 3,
        [EnumMember] Text = 4
 * 
 */
    private searchType = {
        Author: 0,
        Title: 1,
        Responsible: 2,
        Dating: 3,
        Text: 4
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

        searchDestinationSelect.appendChild(this.createOption("Text", this.searchType.Text.toString()));
        searchDestinationSelect.appendChild(this.createOption("Autor", this.searchType.Author.toString()));
        searchDestinationSelect.appendChild(this.createOption("Titul", this.searchType.Title.toString()));
        searchDestinationSelect.appendChild(this.createOption("Editor", this.searchType.Responsible.toString()));

        this.selectedSearchType = this.searchType.Text;

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
        $(this.inputsContainerDiv).empty();
        this.inputsArray = new Array<RegExWordInput>();
        this.addInput();
    }

    public addInput() {
        var newInput = new RegExWordInput(this);
        newInput.makeRegExInput();
        this.inputsArray.push(newInput);
        this.inputsContainerDiv.appendChild(newInput.getHtml());
    }

    public removeInput(input: RegExWordInput) {
        var index = this.inputsArray.indexOf(input, 0);
        if (index != undefined) {
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
                case WordInputType.startsWith:
                    wordCriteriaDescription.startsWith = inputValue;
                    break;
                case WordInputType.contains:
                    wordCriteriaDescription.contains.push(inputValue);
                    break;
                case WordInputType.endsWith:
                    wordCriteriaDescription.endsWith = inputValue;
                    break;
                default:
                    break;
            }
        }
        return wordCriteriaDescription;
    }

}

class RegExWordInput extends RegExSearchBase {
    private html: HTMLDivElement;
    private editorDiv: HTMLDivElement;
    private conditionInput: HTMLInputElement;
    private conditionInputType: string;
    private parentRegExWordCondition: RegExWordCondition;

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

        conditionSelect.appendChild(this.createOption("Začíná na", WordInputType.startsWith));
        //conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(this.createOption("Obsahuje", WordInputType.contains));
        //conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(this.createOption("Končí na", WordInputType.endsWith));
        //conditionSelect.appendChild(this.createOption("Nekončí na", this.conditionType.NotEndsWith));

        this.conditionInputType = WordInputType.startsWith;

        $(conditionSelect).change((eventData: Event) => {
            this.conditionInputType = $(eventData.target).val();
        });

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
        
        this.html = mainDiv;
    }

    public getConditionValue(): string {
        return this.conditionInput.value;
    }

    public getConditionType(): string {
        return this.conditionInputType;
    }
}

class WordInputType {
    public static startsWith = "starts";
    public static contains = "contains";
    public static endsWith = "ends"; 
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

