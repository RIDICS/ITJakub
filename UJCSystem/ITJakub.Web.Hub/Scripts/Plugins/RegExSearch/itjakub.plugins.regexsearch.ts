class HtmlItemsFactory {

    public static createOption(label: string, value: string): HTMLOptionElement {
        var conditionOption = document.createElement("option");
        conditionOption.innerHTML = label;
        conditionOption.value = value;

        return conditionOption;
    }

    public static createButton(label: string): HTMLButtonElement {
        var button = document.createElement("button");
        button.type = "button";
        button.innerHTML = label;
        $(button).addClass("btn");
        $(button).addClass("btn-default");
        $(button).addClass("regexsearch-button");

        return button;
    }
}

class RegExSearch {
    container: HTMLDivElement;
    innerContainer: HTMLDivElement;
    regExConditions: Array<RegExConditionListItem>;
        
    constructor(container: HTMLDivElement) {
        this.container = container;
    }

    makeRegExSearch() {
        $(this.container).empty();
        this.regExConditions = [];

        var commandsDiv = document.createElement("div");

        var sentButton = HtmlItemsFactory.createButton("Vyhledat");
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
            data: JSON.stringify({ "json": json }),
            url: "/Dictionaries/Dictionaries/SearchCriteria", //TODO add getBaseUrl
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            success: (response) => {
            },
            error: (response : JQueryXHR) => {
                //$(this.container).empty();
                //$(this.container).append(response.responseText);
            }
        });

    }
}

class RegExConditionListItem {
    private html: HTMLDivElement;
    private parent: RegExSearch;
    private selectedSearchType: number;
    private innerConditionContainer: HTMLDivElement;
    private innerCondition: IRegExConditionBase;

    constructor(parent: RegExSearch) {
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

    getSearchType(): SearchTypeEnum {
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

        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Text", SearchTypeEnum.Fulltext.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Autor", SearchTypeEnum.Author.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Titul", SearchTypeEnum.Title.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Editor", SearchTypeEnum.Editor.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Období vzniku", SearchTypeEnum.Dating.toString()));

        this.selectedSearchType = SearchTypeEnum.Fulltext;

        $(searchDestinationSelect).change((eventData: Event) => {
            var oldSelectedSearchType = this.selectedSearchType;
            this.selectedSearchType = parseInt($(eventData.target).val());

            if (this.selectedSearchType !== oldSelectedSearchType) {
                this.changeConditionType(this.selectedSearchType, oldSelectedSearchType);
            }
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

    private changeConditionType(newSearchType : SearchTypeEnum, oldSearchType: SearchTypeEnum) {
        if ( !(this.innerCondition instanceof RegExWordConditionList) && (newSearchType === SearchTypeEnum.Author || newSearchType === SearchTypeEnum.Editor || newSearchType === SearchTypeEnum.Fulltext || newSearchType === SearchTypeEnum.Title)) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExWordConditionList(this);
            this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        }
        else if (!(this.innerCondition instanceof RegExDatingConditionList) && (newSearchType === SearchTypeEnum.Dating)) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExDatingConditionList(this);
            this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        }
        //else if (!(this.innerCondition instanceof RegExTokenDistanceConditionList) && (newSearchType === SearchTypeEnum.TokenDistance)) { //TODO
        //    $(this.innerConditionContainer).empty();
        //    this.innerCondition = new RegExTokenDistanceConditionList(this);
        //    this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        //}
    }

    private makeDefaultCondition() {
        $(this.innerConditionContainer).empty();
        this.innerCondition = new RegExWordConditionList(this);
        this.innerCondition.makeRegExCondition(this.innerConditionContainer);
    }

    getConditionValue(): ConditionResult {
        var conditionResult: ConditionResult = this.innerCondition.getConditionValue();
        conditionResult.searchType = this.getSearchType();
        conditionResult.conditionType = this.innerCondition.getConditionType();
        return conditionResult;
    }
}

interface IRegExConditionBase {
    parentRegExConditionListItem: RegExConditionListItem;

    makeRegExCondition(conditionContainerDiv: HTMLDivElement);

    getConditionValue(): ConditionResult;

    getConditionType(): ConditionTypeEnum;

}

class RegExWordConditionList implements IRegExConditionBase {
    parentRegExConditionListItem: RegExConditionListItem;
    private html: HTMLDivElement;
    private selectedWordFormType: string;
    private wordListContainerDiv: HTMLDivElement;
    private conditionInputArray: Array<RegExWordCondition>;

    constructor(parent: RegExConditionListItem) {
        this.parentRegExConditionListItem = parent;
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

        wordFormSelect.appendChild(HtmlItemsFactory.createOption("Lemma", this.wordFormType.Lemma));
        wordFormSelect.appendChild(HtmlItemsFactory.createOption("Hyperlemma - nové", this.wordFormType.HyperlemmaNew));
        wordFormSelect.appendChild(HtmlItemsFactory.createOption("Hyperlemma - staré", this.wordFormType.HyperlemmaOld));
        wordFormSelect.appendChild(HtmlItemsFactory.createOption("Stemma", this.wordFormType.Stemma));

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
            criteriaDescriptions.conditions.push(regExWordCondition.getConditionsValue());
        }
        return criteriaDescriptions;
    }

    getConditionType(): ConditionTypeEnum {
        return ConditionTypeEnum.WordList;
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

interface IRegExDatingConditionView {
    makeRangeView(container: HTMLDivElement);

    getLowerValue(): number;
    getHigherValue(): number;
}

class RegExDatingConditionRangePeriodView implements IRegExDatingConditionView {
    private minCenturyValue: number = 8;
    private maxCenturyValue: number = 21;

    private selectedCenturyLowerValue: number;
    private selectedCenturyHigherValue: number;
    private selectedPeriodLowerValue: number;
    private selectedPeriodHigherValue: number;
    private selectedDecadeLowerValue: number;
    private selectedDecadeHigherValue: number;

    private lowerValue: number;
    private higherValue: number;

    private periodEnabled: boolean;
    private decadeEnabled: boolean;

    private dateDisplayDiv: HTMLDivElement;

    public makeRangeView(container : HTMLDivElement) {
        var precisionInpuDiv = container;
        var centurySliderDiv: HTMLDivElement = window.document.createElement("div");
        $(centurySliderDiv).addClass("regex-dating-century-div regex-slider-div");

        var centuryCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        $(centuryCheckboxDiv).addClass("regex-dating-checkbox-div");

        var centuryNameSpan: HTMLSpanElement = window.document.createElement("span");
        centuryNameSpan.innerHTML = "Století";
        centuryCheckboxDiv.appendChild(centuryNameSpan);
        centurySliderDiv.appendChild(centuryCheckboxDiv);
        precisionInpuDiv.appendChild(centurySliderDiv);

        var centuryArray = new Array<DatingSliderValue>();
        for (var century = this.minCenturyValue; century <= this.maxCenturyValue; century++) {
            centuryArray.push(new DatingSliderValue(century.toString(), century * 100 - 100, century * 100 - 1)); //calculate century low and high values (i.e 18. century is 1700 - 1799)
        }

        var sliderCentury = this.makeSlider(centuryArray, ". století",(selectedValue: DatingSliderValue) => { this.centuryChanged(selectedValue) });
        $(sliderCentury).change();
        centurySliderDiv.appendChild(sliderCentury);

        var periodSliderDiv: HTMLDivElement = window.document.createElement("div");
        $(periodSliderDiv).addClass("regex-dating-period-div regex-slider-div");

        var periodCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        $(periodCheckboxDiv).addClass("regex-dating-checkbox-div");
        var periodValueCheckbox: HTMLInputElement = window.document.createElement("input");
        periodValueCheckbox.type = "checkbox";
        $(periodValueCheckbox).change((eventData: Event) => {
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget);
            if (currentTarget.checked) {
                $(eventData.target).parent().siblings(".slider").slider("option", "disabled", false);
                $(eventData.target).parent().siblings(".slider").find(".slider-tip").show();
                this.periodEnabled = true;

                $(eventData.target).parents(".regex-slider-div").siblings(".regex-slider-div").find(".regex-dating-checkbox-div").find("input").prop('checked', false).change();//uncheck other checboxes 
            } else {
                $(eventData.target).parent().siblings(".slider").slider("option", "disabled", true);
                $(eventData.target).parent().siblings(".slider").find(".slider-tip").hide();
                this.periodEnabled = false;
            }

            this.changedValue();
        });

        var periodNameSpan: HTMLSpanElement = window.document.createElement("span");
        periodNameSpan.innerHTML = "Přibližná doba";
        periodCheckboxDiv.appendChild(periodValueCheckbox);
        periodCheckboxDiv.appendChild(periodNameSpan);
        periodSliderDiv.appendChild(periodCheckboxDiv);
        precisionInpuDiv.appendChild(periodSliderDiv);

        var sliderPeriod = this.makeSlider(new Array<DatingSliderValue>(new DatingSliderValue("začátek", 0, -85), new DatingSliderValue("čtvrtina", 0, -75), new DatingSliderValue("třetina", 0, -66), new DatingSliderValue("polovina", 0, -50), new DatingSliderValue("konec", 85, 0)), "",(selectedValue: DatingSliderValue) => { this.periodChanged(selectedValue) });
        $(sliderPeriod).slider("option", "disabled", true);
        $(sliderPeriod).parent().siblings(".slider").find(".slider-tip").hide();
        $(sliderPeriod).change();
        periodSliderDiv.appendChild(sliderPeriod);

        var decadesSliderDiv: HTMLDivElement = window.document.createElement("div");
        $(decadesSliderDiv).addClass("regex-dating-decades-div regex-slider-div");

        var decadeCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        $(decadeCheckboxDiv).addClass("regex-dating-checkbox-div");

        var decadesCheckbox: HTMLInputElement = window.document.createElement("input");
        decadesCheckbox.type = "checkbox";
        $(decadesCheckbox).change((eventData: Event) => {
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget);
            if (currentTarget.checked) {
                $(eventData.target).parent().siblings(".slider").slider("option", "disabled", false);
                $(eventData.target).parent().siblings(".slider").find(".slider-tip").show();
                this.decadeEnabled = true;

                $(eventData.target).parents(".regex-slider-div").siblings(".regex-slider-div").find(".regex-dating-checkbox-div").find("input").prop('checked', false).change();//uncheck other checboxes 
            } else {
                $(eventData.target).parent().siblings(".slider").slider("option", "disabled", true);
                $(eventData.target).parent().siblings(".slider").find(".slider-tip").hide();
                this.decadeEnabled = false;
            }

            this.changedValue();
        });

        var decadesNameSpan: HTMLSpanElement = window.document.createElement("span");
        decadesNameSpan.innerHTML = "Léta";
        decadeCheckboxDiv.appendChild(decadesCheckbox);
        decadeCheckboxDiv.appendChild(decadesNameSpan);
        decadesSliderDiv.appendChild(decadeCheckboxDiv);
        precisionInpuDiv.appendChild(decadesSliderDiv);

        var decadesArray = new Array<DatingSliderValue>();
        for (var decades = 0; decades <= 90; decades += 10) {
            decadesArray.push(new DatingSliderValue(decades.toString(), decades, -(100 - (decades + 10)))); //calculate decades low and high values (i.e 20. decades of 18. century is 1720-1729)
        }
        var sliderDecades = this.makeSlider(decadesArray, ". léta",(selectedValue: DatingSliderValue) => { this.decadeChanged(selectedValue) });
        $(sliderDecades).slider("option", "disabled", true);
        $(sliderDecades).parent().siblings(".slider").find(".slider-tip").hide();
        $(sliderDecades).change();
        decadesSliderDiv.appendChild(sliderDecades);

        var datingDisplayedValueDiv = document.createElement('div');
        $(datingDisplayedValueDiv).addClass("regex-dating-condition-displayed-value");
        this.dateDisplayDiv = datingDisplayedValueDiv;
        precisionInpuDiv.appendChild(datingDisplayedValueDiv);

        this.changedValue();
    }

    private centuryChanged(sliderValue: DatingSliderValue) {
        this.selectedCenturyLowerValue = sliderValue.lowNumberValue;
        this.selectedCenturyHigherValue = sliderValue.highNumberValue;
        this.changedValue();
    }

    private periodChanged(sliderValue: DatingSliderValue) {
        this.selectedPeriodLowerValue = sliderValue.lowNumberValue;
        this.selectedPeriodHigherValue = sliderValue.highNumberValue;
        this.changedValue();
    }

    private decadeChanged(sliderValue: DatingSliderValue) {
        this.selectedDecadeLowerValue = sliderValue.lowNumberValue;
        this.selectedDecadeHigherValue = sliderValue.highNumberValue;
        this.changedValue();
    }

    private changedValue() {
        $(this.dateDisplayDiv).empty();
        var lower = this.selectedCenturyLowerValue;
        var higher = this.selectedCenturyHigherValue;

        if (this.periodEnabled) {
            lower += this.selectedPeriodLowerValue;
            higher += this.selectedPeriodHigherValue;
        }

        if (this.decadeEnabled) {
            lower += this.selectedDecadeLowerValue;
            higher += this.selectedDecadeHigherValue;
        }

        this.lowerValue = lower;
        this.higherValue = higher;
        $(this.dateDisplayDiv).html("(" + lower + "-" + higher + ")");
    }

    private makeSlider(valuesArray: Array<DatingSliderValue>, nameEnding: string, callbackFunction: (selectedValue: DatingSliderValue) => void) {
        var slider: HTMLDivElement = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: valuesArray.length - 1,
            value: 0,
            slide: (event, ui) => {
                $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html(valuesArray[ui.value].name + nameEnding);

            },
            change: (event: Event, ui: JQueryUI.SliderUIParams) => {
                callbackFunction(valuesArray[ui.value]);
            }
        });

        callbackFunction(valuesArray[0]); //default is first

        var sliderTooltip: HTMLDivElement = document.createElement('div');
        $(sliderTooltip).addClass('tooltip top slider-tip');
        var arrowTooltip: HTMLDivElement = document.createElement('div');
        $(arrowTooltip).addClass('tooltip-arrow');
        sliderTooltip.appendChild(arrowTooltip);

        var innerTooltip: HTMLDivElement = document.createElement('div');
        $(innerTooltip).addClass('tooltip-inner');
        $(innerTooltip).html(valuesArray[0].name + nameEnding);
        sliderTooltip.appendChild(innerTooltip);

        var sliderHandle = $(slider).find('.ui-slider-handle');
        $(sliderHandle).append(sliderTooltip);

        return slider;
    }

    getLowerValue(): number { return this.lowerValue; }

    getHigherValue(): number { return this.higherValue; }
}

class RegExDatingConditionRangeYearView implements IRegExDatingConditionView {
    private minValue: number = 800;
    private maxValue: number = 2100;
    private initValue: number = 800;

    private actualValue: number;

    makeRangeView(container: HTMLDivElement) {
        var precisionInpuDiv = container;
        var textInput : HTMLInputElement = document.createElement("input");
        textInput.type = "number";
        textInput.min = this.minValue.toString();
        textInput.max = this.maxValue.toString();
        textInput.value = this.initValue.toString();
        this.actualValue = this.initValue;

        // allows only digits input
        $(textInput).keyup((e: Event)=> {
            var value = $(e.target).val();
            value.replace(/[^0-9]/g, '');
            $(e.target).val(value);
            $(e.target).text(value);

            this.actualValue = parseInt(value);
        });

        var spanInput: HTMLSpanElement = document.createElement("span");
        $(spanInput).addClass("regex-dating-input-span");
        spanInput.innerHTML = "Rok:";
        
        precisionInpuDiv.appendChild(spanInput);
        precisionInpuDiv.appendChild(textInput);
    }


    getLowerValue(): number { return this.actualValue; }

    getHigherValue(): number { return this.actualValue; }
}

class RegExDatingConditionList implements IRegExConditionBase {
    parentRegExConditionListItem: RegExConditionListItem;
    private html: HTMLDivElement;
    private datingListContainerDiv: HTMLDivElement;
    private conditionInputArray: Array<RegExDatingCondition>;

    constructor(parent: RegExConditionListItem) {
        this.parentRegExConditionListItem = parent;
    }

    makeRegExCondition(conditionContainerDiv: HTMLDivElement) {
        this.datingListContainerDiv = document.createElement("div");
        $(this.datingListContainerDiv).addClass("regexsearch-condition-list-div");
        conditionContainerDiv.appendChild(this.datingListContainerDiv);
        this.resetDatingConditions();
    }

    getConditionValue(): DatingCriteriaListDescription {
        var criteriaDescriptions = new DatingCriteriaListDescription();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExDatingCondition = this.conditionInputArray[i];
            criteriaDescriptions.conditions.push(regExDatingCondition.getConditionValue());
        }
        return criteriaDescriptions;
    }

    getConditionType(): ConditionTypeEnum {
        return ConditionTypeEnum.DatingList;
    }

    resetDatingConditions() {
        $(this.datingListContainerDiv).empty();
        this.conditionInputArray = [];
        var newDatingCondition = new RegExDatingCondition(this);
        newDatingCondition.makeRegExCondition();
        newDatingCondition.setClickableDelimeter();
        this.conditionInputArray.push(newDatingCondition);
        this.datingListContainerDiv.appendChild(newDatingCondition.getHtml());
    }

    addDatingCondition() {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        var newDatingCondition = new RegExDatingCondition(this);
        newDatingCondition.makeRegExCondition();
        newDatingCondition.setClickableDelimeter();
        this.conditionInputArray.push(newDatingCondition);
        this.datingListContainerDiv.appendChild(newDatingCondition.getHtml());
    }

    removeDatingCondition(condition: RegExDatingCondition) {

        var index = this.conditionInputArray.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.conditionInputArray[index];
            this.datingListContainerDiv.removeChild(arrayItem.getHtml());
            this.conditionInputArray.splice(index, 1);
        }

        if (this.conditionInputArray.length === 1) {
            this.conditionInputArray[0].setClickableDelimeter();
        }

        if (this.conditionInputArray.length === 0) {
            this.resetDatingConditions();
        }
    }
}

class RegExDatingCondition  {
    private DATING_AROUND: number = 3; //const

    private datingPrecision: DatingPrecisionEnum;
    private datingRange: DatingRangeEnum;

    private precisionInpuDiv: HTMLDivElement;

    private firstDateView: IRegExDatingConditionView;
    private secondDateView: IRegExDatingConditionView;

    private parent: RegExDatingConditionList;
    private html: HTMLDivElement;

    private delimeterClass: string = "regexsearch-dating-or-delimiter";

    constructor(parent: RegExDatingConditionList) {
        this.parent = parent;
    }

    getHtml(): HTMLDivElement {
        return this.html;
    }

    private removeDelimeter() {
        $(this.html).find("."+this.delimeterClass).empty();
    }

    private hasDelimeter(): boolean {
        var isEmpty = $(this.html).find("."+this.delimeterClass).is(":empty");
        return !isEmpty;
    }

    setTextDelimeter() {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find("."+this.delimeterClass).append(textDelimeter);
    }

    setClickableDelimeter() {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find("."+this.delimeterClass).append(clickableDelimeter);
    }

    private createClickableDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = "+ Nebo";
        $(addWordSpan).click(() => {
            this.parent.addDatingCondition();
        });

        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass("regexsearch-or-delimiter");

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeDatingCondition(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    private createTextDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = "Nebo";
        $(delimeterDiv).addClass(this.delimeterClass);

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeDatingCondition(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    makeRegExCondition() {
        var datingConditionDiv = document.createElement('div');
        $(datingConditionDiv).addClass("regex-dating-condition");

        var datingDiv = document.createElement('div');
        $(datingDiv).addClass("regex-dating-condition-value-select");

        datingDiv.appendChild(this.makeTopSelectBoxes());

        var precisionInpuDiv: HTMLDivElement = window.document.createElement("div");
        $(precisionInpuDiv).addClass("regex-dating-precision-div");
        this.precisionInpuDiv = precisionInpuDiv;
        datingDiv.appendChild(precisionInpuDiv);

        this.changeViews();
        datingConditionDiv.appendChild(datingDiv);
        datingConditionDiv.appendChild(this.createTextDelimeter());
        this.html = datingConditionDiv;
    }

    public makeTopSelectBoxes() : HTMLDivElement {
        var datingFormDiv = document.createElement("div");
        $(datingFormDiv).addClass("regex-dating-condition-selects");

        var datingSelectDiv = document.createElement("div");
        $(datingSelectDiv).addClass("regex-dating-condition-select");

        var datingFormSpan = document.createElement("span");
        datingFormSpan.innerHTML = "Zadání rozmezí";
        $(datingFormSpan).addClass("regexsearch-upper-select-label");
        datingSelectDiv.appendChild(datingFormSpan);

        var datingFormSelect = document.createElement("select");
        $(datingFormSelect).addClass("regexsearch-select");
        datingSelectDiv.appendChild(datingFormSelect);

        datingFormSelect.appendChild(HtmlItemsFactory.createOption("Starší než", DatingRangeEnum.OlderThen.toString()));
        datingFormSelect.appendChild(HtmlItemsFactory.createOption("Mladší než", DatingRangeEnum.YoungerThen.toString()));
        datingFormSelect.appendChild(HtmlItemsFactory.createOption("Mezi", DatingRangeEnum.Between.toString()));
        datingFormSelect.appendChild(HtmlItemsFactory.createOption("Kolem", DatingRangeEnum.Around.toString()));

        this.datingRange = DatingRangeEnum.OlderThen;

        $(datingFormSelect).change((eventData: Event) => {
            var oldRange = this.datingRange;
            this.datingRange = parseInt($(eventData.target).val());

            if (oldRange !== this.datingRange) {
                this.changeViews();
            }
        });

        var precisionSelectDiv = document.createElement("div");
        $(precisionSelectDiv).addClass("regex-dating-condition-select");

        var precisionFormSpan = document.createElement("span");
        precisionFormSpan.innerHTML = "Zadání přesnosti";
        $(precisionFormSpan).addClass("regexsearch-upper-select-label");
        precisionSelectDiv.appendChild(precisionFormSpan);

        var precisionFormSelect = document.createElement("select");
        $(precisionFormSelect).addClass("regexsearch-select");
        precisionSelectDiv.appendChild(precisionFormSelect);

        precisionFormSelect.appendChild(HtmlItemsFactory.createOption("Období", DatingPrecisionEnum.Period.toString()));
        precisionFormSelect.appendChild(HtmlItemsFactory.createOption("Rok", DatingPrecisionEnum.Year.toString()));

        this.datingPrecision = DatingPrecisionEnum.Period;

        $(precisionFormSelect).change((eventData: Event) => {
            var oldPrecision = this.datingPrecision;
            this.datingPrecision = parseInt($(eventData.target).val());

            if (oldPrecision !== this.datingPrecision) {
                this.changeViews();
            }
        });
        precisionSelectDiv.appendChild(precisionFormSelect);

        datingFormDiv.appendChild(datingSelectDiv);
        datingFormDiv.appendChild(precisionSelectDiv);

        return datingFormDiv;
    }

    private changeViews() {
        $(this.precisionInpuDiv).empty();

        this.firstDateView = this.createInputRangeView();

        this.firstDateView.makeRangeView(this.precisionInpuDiv);

        if (this.datingRange === DatingRangeEnum.Between) {
            var delimeter = document.createElement("div");
            delimeter.innerHTML = "až";
            this.precisionInpuDiv.appendChild(delimeter);
            this.secondDateView = this.createInputRangeView();
            this.secondDateView.makeRangeView(this.precisionInpuDiv);
        } else {
            this.secondDateView = null;
        }
    }

    private createInputRangeView(): IRegExDatingConditionView{
        if (this.datingPrecision === DatingPrecisionEnum.Period) {
            return new RegExDatingConditionRangePeriodView();
        } else {
            return new RegExDatingConditionRangeYearView();
        }
    }

    getConditionValue(): DatingCriteriaDescription {
        var datingValue = new DatingCriteriaDescription();

        switch (this.datingRange) {
            case DatingRangeEnum.YoungerThen:
                datingValue.notAfter = this.firstDateView.getLowerValue();
                break;
            case DatingRangeEnum.OlderThen:
                datingValue.notBefore = this.firstDateView.getHigherValue();
                break;
            case DatingRangeEnum.Around:
                datingValue.notAfter = this.firstDateView.getHigherValue() + this.DATING_AROUND;
                datingValue.notBefore = this.firstDateView.getLowerValue() - this.DATING_AROUND;
                break;
            case DatingRangeEnum.Between:
                datingValue.notBefore = this.firstDateView.getLowerValue();
                datingValue.notAfter = this.secondDateView.getHigherValue();
                break;
            
            default:
                break;
        }
        return datingValue;
    }
}

class DatingSliderValue {
    name: string; //displayed name i.e 18 (century) or prelom
    lowNumberValue: number; // lower value i.e. 1700 or -10
    highNumberValue: number; // higher value i.e 1799 or +10

    constructor(name: string, lowNumberValue: number, highNumberValue: number) {
        this.name = name;
        this.lowNumberValue = lowNumberValue;
        this.highNumberValue = highNumberValue;
    }
}

class RegExWordCondition {
    private html: HTMLDivElement;
    private inputsArray: Array<RegExWordInput>;
    private hiddenWordInputSelects: Array<WordInputTypeEnum>;
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
        this.hiddenWordInputSelects = new Array<WordInputTypeEnum>();
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
        if (!(newInput.getConditionType() === WordInputTypeEnum.Contains)) {
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
            case WordInputTypeEnum.StartsWith:
                wordCriteriaDescription.startsWith = inputValue;
                break;
            case WordInputTypeEnum.Contains:
                wordCriteriaDescription.contains.push(inputValue);
                break;
            case WordInputTypeEnum.EndsWith:
                wordCriteriaDescription.endsWith = inputValue;
                break;
            default:
                break;
            }
        }
        return wordCriteriaDescription;
    }


    wordInputConditionChanged(wordInput: RegExWordInput, oldWordInputType: WordInputTypeEnum) {
        var newWordInputType = wordInput.getConditionType();

        if (typeof oldWordInputType !== "undefined") {
            this.wordInpuConditionRemoved(oldWordInputType);
        }

        if (!(newWordInputType === WordInputTypeEnum.Contains)) {
            for (var i = 0; i < this.inputsArray.length; i++) {
                if (this.inputsArray[i] === wordInput) continue;
                this.inputsArray[i].hideSelectCondition(newWordInputType);
            }

            this.hiddenWordInputSelects.push(newWordInputType);
        }
    }

    wordInpuConditionRemoved(wordInputType: WordInputTypeEnum) {

        if (!(wordInputType === WordInputTypeEnum.Contains)) {
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


class RegExWordInput {
    private html: HTMLDivElement;
    private editorDiv: HTMLDivElement;
    private conditionInput: HTMLInputElement;
    private conditionInputType: WordInputTypeEnum;
    private parentRegExWordCondition: RegExWordCondition;
    private regexButtonsDiv: HTMLDivElement;
    private conditionSelectbox: HTMLSelectElement;

    constructor(parent: RegExWordCondition) {
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

        conditionSelect.appendChild(HtmlItemsFactory.createOption("Začíná na", WordInputTypeEnum.StartsWith.toString()));
        //conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(HtmlItemsFactory.createOption("Obsahuje", WordInputTypeEnum.Contains.toString()));
        //conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(HtmlItemsFactory.createOption("Končí na", WordInputTypeEnum.EndsWith.toString()));
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

        var removeButton = HtmlItemsFactory.createButton("");
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

        var anythingButton = HtmlItemsFactory.createButton("Cokoliv");
        regexButtonsDiv.appendChild(anythingButton);
        $(anythingButton).addClass("regexsearch-editor-button");
        $(anythingButton).click(() => {
            this.conditionInput.value += "%";
        });

        var oneCharButton = HtmlItemsFactory.createButton("Jeden znak");
        regexButtonsDiv.appendChild(oneCharButton);
        $(oneCharButton).addClass("regexsearch-editor-button");
        $(oneCharButton).click(() => {
            this.conditionInput.value += "_";
        });

        this.regexButtonsDiv = regexButtonsDiv;
        $(this.regexButtonsDiv).hide();
        mainDiv.appendChild(regexButtonsDiv);

        this.html = mainDiv;

        $(this.conditionSelectbox).val(WordInputTypeEnum.Contains.toString());
        $(this.conditionSelectbox).change();
    }

    getConditionValue(): string {
        return this.conditionInput.value;
    }

    getConditionType(): WordInputTypeEnum {
        return this.conditionInputType;
    }

    showSelectCondition(wordInputType: WordInputTypeEnum) {
        $(this.conditionSelectbox).find(`option[value=${wordInputType.toString()}]`).show();
    }

    hideSelectCondition(wordInputType: WordInputTypeEnum) {
        $(this.conditionSelectbox).find(`option[value=${wordInputType.toString()}]`).hide();
    }


}

class ConditionResult {
    searchType: SearchTypeEnum;         //enum Author, Text, Editor etc.
    conditionType: ConditionTypeEnum;      //type of derived class ie WordList = WordList
}

class DatingCriteriaListDescription extends ConditionResult {
    conditions: Array<DatingCriteriaDescription>;

    constructor() {
        super();
        this.conditions = new Array<DatingCriteriaDescription>();
    }
}

class DatingCriteriaDescription  {
    notBefore: number;
    notAfter: number;
}

class WordsCriteriaListDescription extends ConditionResult {
    conditions: Array<WordCriteriaDescription>;

    constructor() {
        super();
        this.conditions = new Array<WordCriteriaDescription>();
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

class TokenDistanceCriteriaListDescription extends ConditionResult {
    conditions: Array<TokenDistanceCriteriaDescription>;

    constructor() {
        super();
        this.conditions = new Array<TokenDistanceCriteriaDescription>();
    }
}

class TokenDistanceCriteriaDescription {
    distance: number;
    first: WordCriteriaDescription;
    second: WordCriteriaDescription;
}

enum WordInputTypeEnum {
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
        [EnumMember] Fulltext = 4,
        [EnumMember] Heading = 5,
        [EnumMember] Sentence = 6,
        [EnumMember] Result = 7,
        [EnumMember] ResultRestriction = 8,
        [EnumMember] TokenDistance = 9,
 * 
 */
enum SearchTypeEnum {
    Author = 0,
    Title = 1,
    Editor = 2,
    Dating = 3,
    Fulltext = 4,
    Heading = 5,
    Sentence = 6,
    Result = 7,
    ResultRestriction = 8,
    TokenDistance = 9,
}

/*
 * ConditionTypeEnum must match with ConditionTypeEnum number values in C#
        WordList = 0,
        DatingList = 1,
        TokenDistanceList = 2,
 * 
 */
enum ConditionTypeEnum {
    WordList = 0,
    DatingList = 1,
    TokenDistanceList = 2
}

enum DatingPrecisionEnum {
    Year = 0,
    Period = 1
}

enum DatingRangeEnum {
    OlderThen = 0,
    YoungerThen = 1,
    Between = 2,
    Around = 3
}