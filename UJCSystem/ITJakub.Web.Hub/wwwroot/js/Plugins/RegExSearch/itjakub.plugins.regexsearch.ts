class HtmlItemsFactory {

    public static createOption(label: string, value: string, disabled?: boolean): JQuery {
        const optionEl = $(`<option value="${value}">${label}</option>`);
        if (disabled) {
            optionEl.prop("disabled", true);
        }
        return optionEl;
    }

    public static createOptionGroup(label: string): HTMLOptGroupElement {
        var conditionOption: HTMLOptGroupElement = document.createElement("optgroup");
        conditionOption.label = label;
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

class Search {
    private fulltextIsLimited = false;
    private numberOfFullTextConditions = 0;
    private speedAnimation: number = 200; //200=fast, 600=slow
    private advancedRegexEditor: RegExAdvancedSearchEditor;
    private favoriteQueryComponent: FavoriteQuery;
    private keyboardComponent: KeyboardComponent;

    private searchButton: HTMLButtonElement;
    private advancedButton: HTMLButtonElement;
    private searchInputTextbox: HTMLInputElement;
    private searchbarAdvancedEditorContainer: HTMLDivElement;
    private favoritesContainer: HTMLDivElement;

    private container: HTMLDivElement;

    private processSearchTextCallback: (text: string) => void;
    private processSearchJsonCallback: (json: string) => void;
    private overrideSetQueryCallback: (text: string) => void;

    private lastQuery: string;
    private lastQueryWasJson: boolean;

    private enabledOptions: Array<SearchTypeEnum>;

    private favoriteQueriesConfig: IModulInicializatorConfigurationSearchFavorites;



    constructor(container: HTMLDivElement, processSearchJsonCallback: (jsonData: string) => void, processSearchTextCallback: (text: string) => void, favoriteQueriesConfig: IModulInicializatorConfigurationSearchFavorites) {
        this.favoriteQueriesConfig = favoriteQueriesConfig;
        this.container = container;
        this.processSearchJsonCallback = processSearchJsonCallback;
        this.processSearchTextCallback = processSearchTextCallback;
    }

    public setOverrideQueryCallback(callback: (text: string) => void) {
        this.overrideSetQueryCallback = callback;

        if (this.favoriteQueryComponent) {
            this.favoriteQueryComponent.setOverrideQueryCallback(callback);
        }
    }

    limitFullTextSearchToOne() {
        this.fulltextIsLimited = true;
    }

    makeSearch(enabledOptions: Array<SearchTypeEnum>) {
        this.enabledOptions = enabledOptions;

        var searchAreaDiv = document.createElement("div");
        searchAreaDiv.classList.add("regex-search-div");

        var form: HTMLFormElement = document.createElement("form");
        form.setAttribute("role", "form");
        
        form.classList.add("form-horizontal");
        searchAreaDiv.appendChild(form);

        var formGroupDiv = document.createElement("div");
        formGroupDiv.classList.add("form-group");
        formGroupDiv.classList.add("searchbar");
        form.appendChild(formGroupDiv);

        var searchbarButtonsDiv = document.createElement("div");
        searchbarButtonsDiv.classList.add("searchbar-buttons");
        formGroupDiv.appendChild(searchbarButtonsDiv);
        
        var searchButton = document.createElement("button");
        searchButton.type = "button";
        searchButton.innerHTML = localization.translate("Search", "Home").value;
        searchButton.classList.add("btn");
        searchButton.classList.add("btn-default");
        searchButton.classList.add("searchbar-button");
        searchbarButtonsDiv.appendChild(searchButton);

        this.searchButton = searchButton;

        if (typeof this.processSearchJsonCallback !== "undefined" && this.processSearchJsonCallback != null) {
       
            var advancedButton = document.createElement("button");
            advancedButton.type = "button";
            advancedButton.innerHTML = localization.translate("Advanced", "PluginsJs").value;
            advancedButton.classList.add("btn");
            advancedButton.classList.add("btn-default");
            advancedButton.classList.add("searchbar-button");
            searchbarButtonsDiv.appendChild(advancedButton);

            this.advancedButton = advancedButton;

            var advancedButtonSpanCarrot = document.createElement("span");
            advancedButtonSpanCarrot.classList.add("glyphicon");
            advancedButtonSpanCarrot.classList.add("glyphicon-chevron-down");
            advancedButtonSpanCarrot.classList.add("regexsearch-button-glyph");
            advancedButton.appendChild(advancedButtonSpanCarrot);

            $(this.advancedButton).click(() => {
                $(this.advancedButton).css("visibility", "hidden");

                if ($(this.searchbarAdvancedEditorContainer).is(":hidden")) {       //show advanced search
                    var $searchInputTextbox = $(this.searchInputTextbox);
                    var textboxValue = $searchInputTextbox.val() as string;
                    if (this.isValidJson(textboxValue)) {
                        this.advancedRegexEditor.importJson(textboxValue);
                    }
                    $(this.searchbarAdvancedEditorContainer).slideDown(this.speedAnimation);
                    $searchInputTextbox.prop("disabled", true);
                    $searchInputTextbox.closest(".input_container").find(".keyboard-icon-img").addClass("disabled");
                    $searchInputTextbox.closest(".input_container").find(".regexsearch-input-button").prop("disabled", true);
                    $(this.searchButton).prop("disabled", true);

                    if (!this.favoriteQueryComponent.isHidden()) {
                        this.favoriteQueryComponent.hide();
                    }
                }
            });

        } 

        var searchbarInputDiv = document.createElement("div");
        searchbarInputDiv.classList.add("regex-searchbar-inputs");
        searchbarInputDiv.classList.add("input_container");
        formGroupDiv.appendChild(searchbarInputDiv);
        
        var searchbarInput: HTMLInputElement = document.createElement("input");
        searchbarInput.type = "text";
        searchbarInput.placeholder = localization.translate("Search...", "PluginsJs").value;
        searchbarInput.setAttribute("data-keyboard-id", "0");
        var searchbarInputClassList = searchbarInput.classList;
        searchbarInputClassList.add("form-control");
        searchbarInputClassList.add("searchbar-input");
        searchbarInputClassList.add("keyboard-input");
        searchbarInputDiv.appendChild(searchbarInput);

        var keyboardButton = document.createElement("button");
        keyboardButton.type = "button";
        keyboardButton.classList.add("btn", "regexsearch-input-button");
        keyboardButton.style.right = "45px";
        var keyboardIcon = document.createElement("div");
        keyboardIcon.classList.add("custom-glyphicon-keyboard");
        keyboardIcon.style.height = "100%";
        keyboardButton.appendChild(keyboardIcon);
        searchbarInputDiv.appendChild(keyboardButton);

        var favoriteButton = document.createElement("button");
        favoriteButton.type = "button";
        favoriteButton.classList.add("btn", "regexsearch-input-button");
        var favoriteIcon = document.createElement("span");
        favoriteIcon.classList.add("glyphicon", "glyphicon-star");
        favoriteIcon.style.fontSize = "110%";
        favoriteIcon.style.marginTop = "1px";
        favoriteButton.appendChild(favoriteIcon);
        searchbarInputDiv.appendChild(favoriteButton);
        
        this.searchInputTextbox = searchbarInput;

        $(this.searchInputTextbox).keypress((event: any) => {
            var keyCode = event.which || event.keyCode; 
            if (keyCode === 13) {     //13 = Enter
                $(this.searchButton).click();
                event.preventDefault(); 
                event.stopPropagation();
                return false;
            }
        });

        $(favoriteButton).click(() => {
            if (this.favoriteQueryComponent.isHidden()) {
                if (!$(this.searchbarAdvancedEditorContainer).is(":hidden")) {
                    this.closeAdvancedSearchEditor();
                }
                this.favoriteQueryComponent.show();
            } else {
                this.favoriteQueryComponent.hide();
            }
        });

        var searchbarAdvancedEditor = document.createElement("div");
        searchbarInputDiv.classList.add("regex-searchbar-advanced-editor");
        searchAreaDiv.appendChild(searchbarAdvancedEditor);

        var favoritesContainer = document.createElement("div");
        searchAreaDiv.appendChild(favoritesContainer);

        this.searchbarAdvancedEditorContainer = searchbarAdvancedEditor;
        this.favoritesContainer = favoritesContainer;

        if (typeof this.processSearchJsonCallback !== "undefined" && this.processSearchJsonCallback != null) {

            this.advancedRegexEditor = new RegExAdvancedSearchEditor(this.searchbarAdvancedEditorContainer, (json: string) => this.closeAdvancedSearchEditorWithImport(json), (json: string) => this.closeAdvancedSearchEditor());
            this.advancedRegexEditor.setEnabledOptions(enabledOptions);
            if (this.fulltextIsLimited) {
                this.advancedRegexEditor.limitFullTextOptions();
            }
            this.advancedRegexEditor.makeRegExSearch();
            $(this.searchbarAdvancedEditorContainer).hide();
        } else {
            searchbarInputDiv.classList.add("no-advanced");
        }

        this.favoriteQueryComponent = new FavoriteQuery($(this.favoritesContainer), $(this.searchInputTextbox), this.favoriteQueriesConfig.bookType, this.favoriteQueriesConfig.queryType);
        this.favoriteQueryComponent.setOverrideQueryCallback(this.overrideSetQueryCallback);
        
        $(this.container).append(searchAreaDiv);

        $(this.searchButton).click(() => {
            this.processSearch();
        });

        var keyboardComponent = KeyboardManager.getKeyboard("0");
        //keyboardComponent.registerInput(searchbarInput);
        keyboardComponent.registerButton(keyboardButton, searchbarInput, (text) => {
            if (typeof this.overrideSetQueryCallback === "function") {
                this.overrideSetQueryCallback(text);
            } else {
                $(searchbarInput).val(text);
            }
        });
    }

    closeAdvancedSearchEditorWithImport(jsonData: string) {
        this.writeTextToTextField(jsonData);
        this.closeAdvancedSearchEditor();
    }

    closeAdvancedSearchEditor() {
        $(this.searchbarAdvancedEditorContainer).slideUp(this.speedAnimation);      //hide advanced search
        var $searchInputTextbox=$(this.searchInputTextbox);
        $searchInputTextbox.prop("disabled", false);
        $searchInputTextbox.closest(".input_container").find(".keyboard-icon-img").removeClass("disabled");
        $searchInputTextbox.closest(".input_container").find(".regexsearch-input-button").prop("disabled", false);
        $(this.searchButton).prop("disabled", false);
        $(this.advancedButton).css("visibility", "visible");
        $searchInputTextbox.focus();
    }

    public writeTextToTextField(text: string) {
        if (typeof this.overrideSetQueryCallback === "function") {
            this.overrideSetQueryCallback(text);
            return;
        }

        $(this.searchInputTextbox).text(text);
        $(this.searchInputTextbox).val(text);
        $(this.searchInputTextbox).change();
    }

    public getTextFromTextField(): string {
        return $(this.searchInputTextbox).val() as string;
    }

    public processSearchQuery(query: string) {
        this.writeTextToTextField(query);
        this.processSearch();
    }

    private isValidJson(data: string): boolean {
        try {
            JSON.parse(data);
            return true;
        } catch (e) {
            return false;
        } 
    }

    processSearch() {
        var searchboxValue = $(this.searchInputTextbox).val() as string;
        this.lastQuery = searchboxValue;
        if (this.isValidJson(searchboxValue)) {
            this.lastQueryWasJson = true;
            var query = this.getFilteredQuery(searchboxValue, this.enabledOptions); //filter disabled options
            this.writeTextToTextField(query);
            this.processSearchJsonCallback(query);
        } else {
            this.lastQueryWasJson = false;
            this.processSearchTextCallback(searchboxValue);
        }
    }

    getLastQuery(): string {
        return this.lastQuery;
    }

    getLastQueryFiltered(enabledOptions: Array<SearchTypeEnum>): string {
        return this.getFilteredQuery(this.getLastQuery(), enabledOptions);
    }

    getFilteredQuery(query: string, enabledOptions: Array<SearchTypeEnum>):string {
        if (!this.isValidJson(query)) return query;
        var actualState = this.advancedRegexEditor.getConditionsResultJSON();
        this.advancedRegexEditor.importJson(query);
        var filteredQuery = this.advancedRegexEditor.getConditionsResultJSON(enabledOptions);
        this.advancedRegexEditor.importJson(actualState);
        return filteredQuery;
    }

    isLastQueryJson(): boolean {
        return this.lastQueryWasJson;
    }

    isLastQueryText(): boolean {
        return !this.lastQueryWasJson;
    }
}

class RegExAdvancedSearchEditor {
    private fulltextIsLimited = false;
    private regexDoneCallback: (jsonData: string) => void;
    private regexCancelledCallback: (jsonData: string) => void;
    private container: HTMLDivElement;
    private innerContainer: HTMLDivElement;
    private regExConditions: Array<RegExConditionListItem>;
    private enabledOptionsArray: Array<SearchTypeEnum>;

    constructor(container: HTMLDivElement, regexDoneCallback: (jsonData: string) => void, regexCancelledCallback: (jsonData: string) => void){
        this.regexDoneCallback = regexDoneCallback;
        this.regexCancelledCallback = regexCancelledCallback;
        this.container = container;
    }

    setEnabledOptions(enabledOptions: Array<SearchTypeEnum>) {
        //Set options which are allowed for the actual portal.
        const allowedOptions = JSON.parse(he.decode($("#allowedSearchOptions").data("options"))) as Array<IKeyValue<string, boolean>>;
        const filteredEnabledOptions = new Array<SearchTypeEnum>();
        for (let optionId of enabledOptions) {
            const optionName = SearchTypeEnum[optionId];

            for (let allowedOption of allowedOptions) {
                if (allowedOption.key === optionName) {
                    if (allowedOption.value) {
                        filteredEnabledOptions.push(optionId);
                    }
                    break;
                }
            }
        }

        this.enabledOptionsArray = filteredEnabledOptions;
    }

    limitFullTextOptions() {
        this.fulltextIsLimited = true;
    }

    makeRegExSearch() {
        $(this.container).empty();
        this.regExConditions = [];

        var commandsDiv = document.createElement("div");
        $(commandsDiv).addClass("regex-search-buttons-bottom-area");

        var sentButton = HtmlItemsFactory.createButton(localization.translate("Finish", "PluginsJs").value);
        $(sentButton).addClass("regex-search-button");
        commandsDiv.appendChild(sentButton);
        $(sentButton).click(() => {
            var json = this.getConditionsResultJSON();
            this.regexDoneCallback(json);
        });

        var cancelButton = HtmlItemsFactory.createButton(localization.translate("Cancel", "PluginsJs").value);
        $(cancelButton).addClass("regex-search-button");
        commandsDiv.appendChild(cancelButton);
        $(cancelButton).click(() => {
            var json = this.getConditionsResultJSON();
            this.regexCancelledCallback(json);
        });

        this.innerContainer = document.createElement("div");
        this.addNewCondition(true);
        $(this.container).append(this.innerContainer);
        $(this.container).append(commandsDiv);
    }

    importJson(json: string) {
        var jsonDataArray = JSON.parse(json);
        $(this.innerContainer).empty();
        this.regExConditions = new Array<RegExConditionListItem>();

        for (var i = 0; i < jsonDataArray.length; i++) {
            var conditionData = jsonDataArray[i];
            if (this.enabledOptionsArray && $.inArray(conditionData.searchType, this.enabledOptionsArray) >= 0) {
                this.addNewCondition();
                this.getLastCondition().importData(conditionData);
            }
        }
    }

    addNewCondition(useDelimiter: boolean = true) {
        var disableOptions = false;
        if (this.regExConditions.length > 0) {
            this.getLastCondition().setTextDelimeter();
            if (this.fulltextIsLimited) {
                this.regExConditions.forEach((condition) => {
                    switch (condition.getSearchType()) {
                    case SearchTypeEnum.Fulltext:
                    case SearchTypeEnum.TokenDistance:
                    case SearchTypeEnum.Sentence:
                    case SearchTypeEnum.Heading:
                        disableOptions = true;
                    } 
                });
            }
        }

        var newRegExConditions = new RegExConditionListItem(this);
        newRegExConditions.makeRegExCondition(this.enabledOptionsArray, disableOptions);
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
        if (index) {
            var arrayItem = this.regExConditions[index];
            $(arrayItem.getHtml()).fadeToggle("slow", "linear", () => {
                this.innerContainer.removeChild(arrayItem.getHtml());    
            });
            this.regExConditions.splice(index, 1);
        }

        if (this.regExConditions.length === 0) {
            this.addNewCondition(true);
        } else {
            this.getLastCondition().setClickableDelimeter();
        }
    }

    getConditionsResultObject(enabledOptions?: Array<SearchTypeEnum>): Object {
        var resultArray = new Array();
        for (let i = 0; i < this.regExConditions.length; i++) {
            const regExCondition = this.regExConditions[i];
            const regExConditionValue = regExCondition.getConditionValue();
            if (!enabledOptions || $.inArray(regExConditionValue.searchType, enabledOptions) >= 0) {
                resultArray.push(regExConditionValue);
            }
        }
        return resultArray;
    }

    getConditionsResultJSON(enabledOptions?: Array<SearchTypeEnum>): string {
        var jsonString = JSON.stringify(this.getConditionsResultObject(enabledOptions));
        return jsonString;
    }

    private getLastCondition(): RegExConditionListItem {
        if (!this.regExConditions || !this.regExConditions.length) return null;
        return this.regExConditions[this.regExConditions.length - 1];
    }
}

class RegExConditionListItem {
    private html: HTMLDivElement;
    private readonly parent: RegExAdvancedSearchEditor;
    private selectedSearchType: number;
    private innerConditionContainer: HTMLDivElement;
    private innerCondition: IRegExConditionListBase;
    private searchDestinationSelect: HTMLSelectElement;

    constructor(parent: RegExAdvancedSearchEditor) {
        this.parent = parent;
    }

    importData(conditionData: ConditionResult) {
        $(this.searchDestinationSelect).val(conditionData.searchType.toString());
        $(this.searchDestinationSelect).change();
        this.innerCondition.importData(conditionData);
    }

    getHtml(): HTMLDivElement {
        return this.html;
    }

    removeDelimeter() {
        $(this.html).find(".regexsearch-delimiter").empty();
    }

    private hasDelimeter(): boolean {
        const isEmpty = $(this.html).find(".regexsearch-delimiter").is(":empty");
        return !isEmpty;
    }

    setTextDelimeter() {
        const textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-delimiter").append(textDelimeter);
    }

    setClickableDelimeter() {
        const clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-delimiter").append(clickableDelimeter);
    }

    private createClickableDelimeter(): HTMLDivElement {
        const delimeterDiv = $("<div></div>");
        const addWordSpan = $("<span></span>");
        addWordSpan.addClass("regex-clickable-text");
        addWordSpan.text(localization.translate("+And", "PluginsJs").value);
        addWordSpan.click(() => {
            this.parent.addNewCondition();//TODO
        });

        delimeterDiv.append(addWordSpan);

        const trashButton = $("<button></button>");
        trashButton.addClass("regexsearch-delimiter-remove-button");
        const removeGlyph = $("<span></span>");
        removeGlyph.addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.append(removeGlyph);
        trashButton.click(() => {
            this.parent.removeCondition(this);
        });

        delimeterDiv.append(trashButton);

        return delimeterDiv[0] as Node as HTMLDivElement;
    }

    private createTextDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = localization.translate("And", "PluginsJs").value;

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

    private enableOptions(enabledOptions: Array<SearchTypeEnum>) {

        if (!enabledOptions) return; //TODO log error
        
        if (this.searchDestinationSelect) {
            for (let i = 0; i < enabledOptions.length; i++) {
                const enabled = enabledOptions[i];
                const option = $(this.searchDestinationSelect).find(`option[value = ${enabled.toString()}]`);
                option.show();
                option.removeClass("hidden");
            }

            const optGroups = $(this.searchDestinationSelect).find("optgroup");
            for (let j = 0; j < optGroups.length; j++) {
                const optGroup = optGroups[j];
                const visibleChilds = $(optGroup as Node as Element).children(":not(.hidden)");
                if (!visibleChilds.length) $(optGroup as Node as Element).hide();
            }
        }
    }

    makeRegExCondition(enabledOptions?: Array<SearchTypeEnum>, fullTextLimited?: boolean) {

        var conditionsDiv = document.createElement("div");
        $(conditionsDiv).addClass("regexsearch-condition-main-div");

        var mainSearchDiv = document.createElement("div");

        var searchDestinationDiv = document.createElement("div");
        $(searchDestinationDiv).addClass("regexsearch-destination-div");
        mainSearchDiv.appendChild(searchDestinationDiv);

        var searchDestinationSpan = document.createElement("span");
        searchDestinationSpan.innerHTML = localization.translate("ChooseSearchFiled", "PluginsJs").value;
        $(searchDestinationSpan).addClass("regexsearch-upper-select-label");
        searchDestinationDiv.appendChild(searchDestinationSpan);

        var searchDestinationSelect = document.createElement("select");
        const searchDestinationSelectEl = $(searchDestinationSelect);
        searchDestinationSelectEl.addClass("regexsearch-select");
        searchDestinationDiv.appendChild(searchDestinationSelect);

        var metadataOptGroupEl = $(HtmlItemsFactory.createOptionGroup(localization.translate("Metadata", "PluginsJs").value));
        searchDestinationSelectEl.append(metadataOptGroupEl);

        metadataOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("Title", "PluginsJs").value, SearchTypeEnum.Title.toString()));
        metadataOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("Author", "PluginsJs").value, SearchTypeEnum.Author.toString()));
        metadataOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("Editor", "PluginsJs").value, SearchTypeEnum.Editor.toString()));
        metadataOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("Dating", "PluginsJs").value, SearchTypeEnum.Dating.toString()));
        metadataOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("Term", "PluginsJs").value, SearchTypeEnum.Term.toString()));

        var textOptGroupEl = $(HtmlItemsFactory.createOptionGroup(localization.translate("Text", "PluginsJs").value));
        searchDestinationSelectEl.append(textOptGroupEl);

        textOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("Fulltext", "PluginsJs").value, SearchTypeEnum.Fulltext.toString(), fullTextLimited));
        textOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("TokenDistance", "PluginsJs").value, SearchTypeEnum.TokenDistance.toString(), fullTextLimited));
        textOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("InSentence", "PluginsJs").value, SearchTypeEnum.Sentence.toString(), fullTextLimited));
        textOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("InHeading", "PluginsJs").value, SearchTypeEnum.Heading.toString(), fullTextLimited));

        var headwordsOptGroupEl = $(HtmlItemsFactory.createOptionGroup(localization.translate("Headword", "PluginsJs").value));
        searchDestinationSelectEl.append(headwordsOptGroupEl);

        headwordsOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("TokenDistance", "PluginsJs").value, SearchTypeEnum.HeadwordDescriptionTokenDistance.toString()));
        headwordsOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("Headword", "PluginsJs").value, SearchTypeEnum.Headword.toString()));
        headwordsOptGroupEl.append(HtmlItemsFactory.createOption(localization.translate("HeadwordDescription", "PluginsJs").value, SearchTypeEnum.HeadwordDescription.toString()));

        searchDestinationSelectEl.change((eventData) => {
            var oldSelectedSearchType = this.selectedSearchType;
            this.selectedSearchType = parseInt($(eventData.target).val() as string);

            if (this.selectedSearchType !== oldSelectedSearchType) {
                this.changeConditionType(this.selectedSearchType, oldSelectedSearchType);
            }
        });

        this.searchDestinationSelect = searchDestinationSelectEl[0] as Node as HTMLSelectElement;

        var options = $(this.searchDestinationSelect).find("option");
        options.hide();
        options.addClass("hidden");
        

        this.enableOptions(enabledOptions);

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

        var defaultValue = $(this.searchDestinationSelect).find("option:not(.hidden)").first().val() as string;
        $(searchDestinationSelect).val(defaultValue);
        $(searchDestinationSelect).change();
    }

    private changeConditionType(newSearchType : SearchTypeEnum, oldSearchType: SearchTypeEnum) {
        if (!(this.innerCondition instanceof RegExWordConditionList) && (newSearchType === SearchTypeEnum.Author || newSearchType === SearchTypeEnum.Editor || newSearchType === SearchTypeEnum.Fulltext || newSearchType === SearchTypeEnum.Title || newSearchType === SearchTypeEnum.Heading || newSearchType === SearchTypeEnum.Sentence)) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExWordConditionList(this);
            this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        }
        else if (!(this.innerCondition instanceof RegExDatingConditionList) && (newSearchType === SearchTypeEnum.Dating)) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExDatingConditionList(this);
            this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        }
        else if (!(this.innerCondition instanceof RegExTokenDistanceConditionList) && (newSearchType === SearchTypeEnum.TokenDistance)) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExTokenDistanceConditionList(this);
            this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        }
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

interface IRegExConditionListBase {
    parentRegExConditionListItem: RegExConditionListItem;

    makeRegExCondition(conditionContainerDiv: HTMLDivElement);

    getConditionValue(): ConditionResult;

    getConditionType(): ConditionTypeEnum;

    resetItems();

    addItem();

    removeItem(item: IRegExConditionItemBase);

    importData(conditionData: ConditionResult);
    
    getLastItem(): IRegExConditionItemBase;
}

interface IRegExConditionItemBase {
    parent: IRegExConditionListBase;

    makeRegExItemCondition(); //creates html

    getHtml(): HTMLDivElement;

    removeDelimeter();

    hasDelimeter();
    
    setTextDelimeter();

    setClickableDelimeter();

    getConditionItemValue(): ConditionItemResult;

    importData(conditionData: ConditionItemResult);
}

class RegExWordConditionList implements IRegExConditionListBase {
    parentRegExConditionListItem: RegExConditionListItem;
    private html: HTMLDivElement;
    private selectedWordFormType: string;
    private wordListContainerDiv: HTMLDivElement;
    private conditionInputArray: Array<IRegExConditionItemBase>;

    constructor(parent: RegExConditionListItem) {
        this.parentRegExConditionListItem = parent;
    }

    importData(conditionsArray: WordsCriteriaListDescription) {
        this.resetItems();
        if (!conditionsArray.conditions.length) return;
        this.getLastItem().importData(conditionsArray.conditions[0]);
        for (let i = 1; i < conditionsArray.conditions.length; i++) {
            this.addItem();
            this.getLastItem().importData(conditionsArray.conditions[i]);
        }
    }

    getLastItem(): IRegExConditionItemBase {
        if (!this.conditionInputArray.length) return null;
        return this.conditionInputArray[this.conditionInputArray.length - 1];
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
        var wordFormDivEl = $(document.createElement("div"));
        wordFormDivEl.addClass("regexsearch-word-form-div");
        //wordListContainerDiv.appendChild(wordFormDiv); //TODO implement after it iss implemented on server side

        var wordFormSpanEl = $(document.createElement("span"));
        wordFormSpanEl.text(localization.translate("WordForm", "PluginsJs").value);
        wordFormSpanEl.addClass("regexsearch-upper-select-label");
        wordFormDivEl.append(wordFormSpanEl);

        const wordFormSelectEl = $(document.createElement("select"));
        wordFormSelectEl.addClass("regexsearch-select");
        wordFormDivEl.append(wordFormSelectEl);

        wordFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("Lemma", "PluginsJs").value, this.wordFormType.Lemma));
        wordFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("HyperlemmaNew", "PluginsJs").value, this.wordFormType.HyperlemmaNew));
        wordFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("HyperlemmaOld", "PluginsJs").value, this.wordFormType.HyperlemmaOld));
        wordFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("Stemma", "PluginsJs").value, this.wordFormType.Stemma));

        this.selectedWordFormType = this.wordFormType.Lemma;

        wordFormSelectEl.change((eventData) => {
            this.selectedWordFormType = $(eventData.target as HTMLElement).val() as string;
        });

        this.wordListContainerDiv = document.createElement("div");
        $(this.wordListContainerDiv).addClass("regexsearch-condition-list-div");
        conditionContainerDiv.appendChild(this.wordListContainerDiv);

        this.resetItems();
    }

    getConditionValue(): WordsCriteriaListDescription {
        var criteriaDescriptions: ConditionResult  = new WordsCriteriaListDescription();
        for (let i = 0; i < this.conditionInputArray.length; i++) {
            var regExWordCondition = this.conditionInputArray[i];
            criteriaDescriptions.conditions.push(regExWordCondition.getConditionItemValue());
        }
        return criteriaDescriptions;
    }

    getConditionType(): ConditionTypeEnum {
        return ConditionTypeEnum.WordList;
    }

    resetItems() {
        $(this.wordListContainerDiv).empty();
        this.conditionInputArray = [];
        const newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExItemCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.wordListContainerDiv.appendChild(newWordCondition.getHtml());
    }

    addItem() {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        const newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExItemCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.wordListContainerDiv.appendChild(newWordCondition.getHtml());
    }

    removeItem(item: IRegExConditionItemBase) {

        var index = this.conditionInputArray.indexOf(item, 0);
        if (index != undefined) {
            var arrayItem = this.conditionInputArray[index];
            $(arrayItem.getHtml()).fadeToggle("slow", "linear", () => {
                this.wordListContainerDiv.removeChild(arrayItem.getHtml());    
            });
            this.conditionInputArray.splice(index, 1);
        }

        if (this.conditionInputArray.length === 1) {
            this.conditionInputArray[0].setClickableDelimeter();
        }

        if (!this.conditionInputArray.length) {
            this.resetItems();
        }
    }
}

interface IRegExDatingConditionView {
    makeRangeView(container: HTMLDivElement);

    getLowerValue(): number;
    getHigherValue(): number;

    setValues(lower?: number, higher?: number);

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


    private centurySliderValues: Array<DatingSliderValue>;
    private periodSliderValues: Array<DatingSliderValue>;
    private decadeSliderValues: Array<DatingSliderValue>;

    private lowerValue: number;
    private higherValue: number;

    private periodEnabled: boolean;
    private decadeEnabled: boolean;

    private dateDisplayDiv: HTMLDivElement;

    private centurySlider: HTMLDivElement;
    private periodSlider: HTMLDivElement;
    private decadesSlider: HTMLDivElement;

    public makeRangeView(container : HTMLDivElement) {
        var precisionInputDiv = container;
        var centurySliderDiv: HTMLDivElement = window.document.createElement("div");
        $(centurySliderDiv).addClass("regex-dating-century-div regex-slider-div");

        var centuryCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        $(centuryCheckboxDiv).addClass("regex-dating-checkbox-div");

        var centuryNameSpan: HTMLSpanElement = window.document.createElement("span");
        centuryNameSpan.innerHTML = localization.translate("Century", "PluginsJs").value;
        centuryCheckboxDiv.appendChild(centuryNameSpan);
        centurySliderDiv.appendChild(centuryCheckboxDiv);
        precisionInputDiv.appendChild(centurySliderDiv);

        var centuryArray = new Array<DatingSliderValue>();
        for (var century = this.minCenturyValue; century <= this.maxCenturyValue; century++) {
            centuryArray.push(new DatingSliderValue(century.toString(), century * 100 - 100, century * 100 - 1)); //calculate century low and high values (i.e 18. century is 1700 - 1799)
        }

        this.centurySliderValues = centuryArray;

        var sliderCentury = this.makeSlider(centuryArray, localization.translate(".Century", "PluginsJs").value,(selectedValue: DatingSliderValue) => { this.centuryChanged(selectedValue) });
        $(sliderCentury).change();
        centurySliderDiv.appendChild(sliderCentury);

        this.centurySlider = sliderCentury;

        var periodSliderDiv: HTMLDivElement = window.document.createElement("div");
        $(periodSliderDiv).addClass("regex-dating-period-div regex-slider-div");

        var periodCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        $(periodCheckboxDiv).addClass("regex-dating-checkbox-div");
        var periodValueCheckbox: HTMLInputElement = window.document.createElement("input");
        periodValueCheckbox.type = "checkbox";
        $(periodValueCheckbox).change((eventData) => {
            var currentTarget: HTMLInputElement = eventData.currentTarget as HTMLInputElement;
            const targetEl = $(eventData.target as HTMLElement);
            if (currentTarget.checked) {
                targetEl.parent().siblings(".slider").slider("option", "disabled", false);
                targetEl.parent().siblings(".slider").find(".slider-tip").show();
                this.periodEnabled = true;

                targetEl.parents(".regex-slider-div").siblings(".regex-slider-div").find(".regex-dating-checkbox-div").find("input").prop('checked', false).change();//uncheck other checboxes 
            } else {
                targetEl.parent().siblings(".slider").slider("option", "disabled", true);
                targetEl.parent().siblings(".slider").find(".slider-tip").hide();
                this.periodEnabled = false;
            }

            this.changedValue();
        });

        var periodNameSpan: HTMLSpanElement = window.document.createElement("span");
        periodNameSpan.innerHTML = localization.translate("ApproxTime", "PluginsJs").value;
        periodCheckboxDiv.appendChild(periodValueCheckbox);
        periodCheckboxDiv.appendChild(periodNameSpan);
        periodSliderDiv.appendChild(periodCheckboxDiv);
        precisionInputDiv.appendChild(periodSliderDiv);

        this.periodSliderValues = new Array<DatingSliderValue>(
            new DatingSliderValue(localization.translate("Start", "PluginsJs").value, 0, -85),
            new DatingSliderValue(localization.translate("Quarter", "PluginsJs").value, 0, -75),
            new DatingSliderValue(localization.translate("Third", "PluginsJs").value, 0, -66),
            new DatingSliderValue(localization.translate("Half", "PluginsJs").value, 0, -50),
            new DatingSliderValue("3. třetina", 66, 0), // TODO add localization
            new DatingSliderValue("4. čtvrtina", 75, 0), // TODO add localization
            new DatingSliderValue(localization.translate("end", "PluginsJs").value, 85, 0));

        var sliderPeriod = this.makeSlider(this.periodSliderValues, "",(selectedValue: DatingSliderValue) => { this.periodChanged(selectedValue) });
        $(sliderPeriod).slider("option", "disabled", true);
        $(sliderPeriod).parent().siblings(".slider").find(".slider-tip").hide();
        $(sliderPeriod).change();
        periodSliderDiv.appendChild(sliderPeriod);

        this.periodSlider = sliderPeriod;

        var decadesSliderDiv: HTMLDivElement = window.document.createElement("div");
        $(decadesSliderDiv).addClass("regex-dating-decades-div regex-slider-div");

        var decadeCheckboxDiv: HTMLDivElement = window.document.createElement("div");
        $(decadeCheckboxDiv).addClass("regex-dating-checkbox-div");

        var decadesCheckbox: HTMLInputElement = window.document.createElement("input");
        decadesCheckbox.type = "checkbox";
        $(decadesCheckbox).change((eventData) => {
            var currentTarget: HTMLInputElement = <HTMLInputElement>(eventData.currentTarget);
            const targetEl = $(eventData.target as HTMLElement);
            if (currentTarget.checked) {
                targetEl.parent().siblings(".slider").slider("option", "disabled", false);
                targetEl.parent().siblings(".slider").find(".slider-tip").show();
                this.decadeEnabled = true;

                targetEl.parents(".regex-slider-div").siblings(".regex-slider-div").find(".regex-dating-checkbox-div").find("input").prop('checked', false).change();//uncheck other checboxes 
            } else {
                targetEl.parent().siblings(".slider").slider("option", "disabled", true);
                targetEl.parent().siblings(".slider").find(".slider-tip").hide();
                this.decadeEnabled = false;
            }

            this.changedValue();
        });

        var decadesNameSpan: HTMLSpanElement = window.document.createElement("span");
        decadesNameSpan.innerHTML = localization.translate("Decades", "PluginsJs").value;
        decadeCheckboxDiv.appendChild(decadesCheckbox);
        decadeCheckboxDiv.appendChild(decadesNameSpan);
        decadesSliderDiv.appendChild(decadeCheckboxDiv);
        precisionInputDiv.appendChild(decadesSliderDiv);

        var decadesArray = new Array<DatingSliderValue>();
        for (var decades = 0; decades <= 90; decades += 10) {
            decadesArray.push(new DatingSliderValue(decades.toString(), decades, -(100 - (decades + 10)))); //calculate decades low and high values (i.e 20. decades of 18. century is 1720-1729)
        }

        this.decadeSliderValues = decadesArray;

        var sliderDecades = this.makeSlider(decadesArray, localization.translate(".Decades", "PluginsJs").value,(selectedValue: DatingSliderValue) => { this.decadeChanged(selectedValue) });
        $(sliderDecades).slider("option", "disabled", true);
        $(sliderDecades).parent().siblings(".slider").find(".slider-tip").hide();
        $(sliderDecades).change();
        decadesSliderDiv.appendChild(sliderDecades);

        this.decadesSlider = sliderDecades;

        var datingDisplayedValueDiv = document.createElement('div');
        $(datingDisplayedValueDiv).addClass("regex-dating-condition-displayed-value");
        this.dateDisplayDiv = datingDisplayedValueDiv;
        precisionInputDiv.appendChild(datingDisplayedValueDiv);

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
                $(event.target as HTMLElement).find('.ui-slider-handle').find('.tooltip-inner').html(valuesArray[ui.value].name + nameEnding);

            },
            change: (event: Event, ui: JQueryUI.SliderUIParams) => {
                callbackFunction(valuesArray[ui.value]);
                $(event.target as HTMLElement).find('.ui-slider-handle').find('.tooltip-inner').html(valuesArray[ui.value].name + nameEnding);
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

    setValues(lower?: number, higher?: number) {
        var century: number = lower !== null ? Math.floor(lower/100) : Math.floor(higher/100);
        var centuryIndex: number = 0;
        for (var i = 0; i < this.centurySliderValues.length; i++) {
            var centurySliderValue = this.centurySliderValues[i];
            if (Math.floor(centurySliderValue.lowNumberValue/100) === century) {
                $(this.centurySlider).slider("value", i);
                centuryIndex = i;
                break;
            }
        }

        var importedCentury = this.centurySliderValues[centuryIndex];

        var periodIndex = -1;
        for (var i = 0; i < this.periodSliderValues.length; i++) {
            var periodSliderValue = this.periodSliderValues[i];
            if ((lower === null || (periodSliderValue.lowNumberValue + importedCentury.lowNumberValue) === lower) && (higher === null || (periodSliderValue.highNumberValue + importedCentury.highNumberValue) === higher)) {
                $(this.periodSlider).slider("value", i);
                periodIndex = i;
                break;
            }
        }

        if (periodIndex < 0) {
            for (var i = 0; i < this.decadeSliderValues.length; i++) {
                var decadeSliderValue = this.decadeSliderValues[i];
                if ((lower === null || (decadeSliderValue.lowNumberValue + importedCentury.lowNumberValue) === lower) && (higher === null || (decadeSliderValue.highNumberValue + importedCentury.highNumberValue) === higher)) {
                    $(this.decadesSlider).slider("value", i);
                    break;
                }
            }
        }
    }
}

class RegExDatingConditionRangeYearView implements IRegExDatingConditionView {
    private minValue: number = 800;
    private maxValue: number = 2100;
    private initValue: number = 800;

    private actualValue: number;

    private valueInput: HTMLInputElement;

    makeRangeView(container: HTMLDivElement) {
        var precisionInpuDiv = container;
        var textInput : HTMLInputElement = document.createElement("input");
        textInput.type = "number";
        textInput.min = this.minValue.toString();
        textInput.max = this.maxValue.toString();
        textInput.value = this.initValue.toString();
        this.actualValue = this.initValue;

        // allows only digits input
        $(textInput).keyup((e) => {
            const targetEl = $(e.target as HTMLElement);
            var value = targetEl.val() as string;
            value.replace(/[^0-9]/g, '');
            targetEl.val(value);
            targetEl.text(value);

            this.actualValue = parseInt(value);
        });

        $(textInput).change((e) => {
            var value = $(e.target as HTMLElement).val() as string;
            this.actualValue = parseInt(value);
        });

        this.valueInput = textInput;

        var spanInput: HTMLSpanElement = document.createElement("span");
        $(spanInput).addClass("regex-dating-input-span");
        spanInput.innerHTML = localization.translate("Year:", "PluginsJs").value;
        
        precisionInpuDiv.appendChild(spanInput);
        precisionInpuDiv.appendChild(textInput);
    }


    getLowerValue(): number { return this.actualValue; }

    getHigherValue(): number { return this.actualValue; }

    private setValue(value: number) {
        $(this.valueInput).val(value.toString());
        $(this.valueInput).text(value.toString());
        $(this.valueInput).change();
    }

    setValues(lower?: number, higher?: number) {
        if (lower !== null) {
            this.setValue(lower);
        }
        else {
            this.setValue(higher);
        }
    }
}

class RegExDatingConditionList implements IRegExConditionListBase {
    parentRegExConditionListItem: RegExConditionListItem;
    private html: HTMLDivElement;
    private datingListContainerDiv: HTMLDivElement;
    private conditionInputArray: Array<RegExDatingCondition>;

    constructor(parent: RegExConditionListItem) {
        this.parentRegExConditionListItem = parent;
    }

    importData(conditionsArray: DatingCriteriaListDescription) {
        this.resetItems();
        if (!conditionsArray.conditions.length) return;
        this.getLastItem().importData(conditionsArray.conditions[0]);
        for (let i = 1; i < conditionsArray.conditions.length; i++) {
            this.addItem();
            this.getLastItem().importData(conditionsArray.conditions[i]);
        }
    }

    getLastItem(): IRegExConditionItemBase {
        if (!this.conditionInputArray.length) return null;
        return this.conditionInputArray[this.conditionInputArray.length - 1];
    }

    makeRegExCondition(conditionContainerDiv: HTMLDivElement) {
        this.datingListContainerDiv = document.createElement("div");
        $(this.datingListContainerDiv).addClass("regexsearch-condition-list-div");
        conditionContainerDiv.appendChild(this.datingListContainerDiv);
        this.resetItems();
    }

    getConditionValue(): DatingCriteriaListDescription {
        var criteriaDescriptions = new DatingCriteriaListDescription();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExDatingCondition = this.conditionInputArray[i];
            criteriaDescriptions.conditions.push(regExDatingCondition.getConditionItemValue());
        }
        return criteriaDescriptions;
    }

    getConditionType(): ConditionTypeEnum {
        return ConditionTypeEnum.DatingList;
    }

    resetItems() {
        $(this.datingListContainerDiv).empty();
        this.conditionInputArray = [];
        var newDatingCondition = new RegExDatingCondition(this);
        newDatingCondition.makeRegExItemCondition();
        newDatingCondition.setClickableDelimeter();
        this.conditionInputArray.push(newDatingCondition);
        this.datingListContainerDiv.appendChild(newDatingCondition.getHtml());
    }

    addItem() {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        var newDatingCondition = new RegExDatingCondition(this);
        newDatingCondition.makeRegExItemCondition();
        newDatingCondition.setClickableDelimeter();
        this.conditionInputArray.push(newDatingCondition);
        this.datingListContainerDiv.appendChild(newDatingCondition.getHtml());
    }

    removeItem(condition: RegExDatingCondition) {

        var index = this.conditionInputArray.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.conditionInputArray[index];
            $(arrayItem.getHtml()).fadeToggle("slow", "linear",() => {
                this.datingListContainerDiv.removeChild(arrayItem.getHtml());    
            });
            this.conditionInputArray.splice(index, 1);
        }

        if (this.conditionInputArray.length === 1) {
            this.conditionInputArray[0].setClickableDelimeter();
        }

        if (this.conditionInputArray.length === 0) {
            this.resetItems();
        }
    }
}

class RegExDatingCondition implements IRegExConditionItemBase{
    private DATING_AROUND: number = 3; //const

    private datingPrecision: DatingPrecisionEnum;
    private datingRange: DatingRangeEnum;

    private precisionInputDiv: HTMLDivElement;

    private datingRangeSelect: HTMLSelectElement;
    private datingPrecisionSelect: HTMLSelectElement;

    private firstDateView: IRegExDatingConditionView;
    private secondDateView: IRegExDatingConditionView;

    parent: RegExDatingConditionList;
    private html: HTMLDivElement;

    private delimeterClass: string = "regexsearch-dating-or-delimiter";

    constructor(parent: RegExDatingConditionList) {
        this.parent = parent;
    }

    importData(conditionData: DatingCriteriaDescription) {
        $(this.datingRangeSelect).val(conditionData.datingRangeEnum.toString());
        $(this.datingRangeSelect).change();
        $(this.datingPrecisionSelect).val(conditionData.datingPrecision.toString());
        $(this.datingPrecisionSelect).change();
        

        switch (conditionData.datingRangeEnum) {
            case DatingRangeEnum.YoungerThen:
                this.firstDateView.setValues(conditionData.notAfter, null);
                break;
            case DatingRangeEnum.OlderThen:
                this.firstDateView.setValues(null, conditionData.notBefore);
                break;
            case DatingRangeEnum.Around:
                var lower = conditionData.notBefore + this.DATING_AROUND;
                var higher = conditionData.notAfter - this.DATING_AROUND;
                this.firstDateView.setValues(lower,higher);
                break;
            case DatingRangeEnum.Between:
                this.firstDateView.setValues(conditionData.notBefore, null);
                this.secondDateView.setValues(null, conditionData.notAfter);
                break;

            default:
                break;
        }
    }

    getHtml(): HTMLDivElement {
        return this.html;
    }

    removeDelimeter() {
        $(this.html).find("."+this.delimeterClass).empty();
    }

    hasDelimeter(): boolean {
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
        addWordSpan.innerHTML = localization.translate("+Or", "PluginsJs").value;
        $(addWordSpan).click(() => {
            this.parent.addItem();
        });

        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass(this.delimeterClass);

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeItem(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    private createTextDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = localization.translate("Or", "PluginsJs").value;
        $(delimeterDiv).addClass(this.delimeterClass);

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeItem(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    makeRegExItemCondition() {
        var datingConditionDiv = document.createElement('div');
        $(datingConditionDiv).addClass("regex-dating-condition");

        var datingDiv = document.createElement('div');
        $(datingDiv).addClass("regex-dating-condition-value-select");

        datingDiv.appendChild(this.makeTopSelectBoxes());

        var precisionInpuDiv: HTMLDivElement = window.document.createElement("div");
        $(precisionInpuDiv).addClass("regex-dating-precision-div");
        this.precisionInputDiv = precisionInpuDiv;
        datingDiv.appendChild(precisionInpuDiv);

        this.changeViews();
        datingConditionDiv.appendChild(datingDiv);
        datingConditionDiv.appendChild(this.createTextDelimeter());
        this.html = datingConditionDiv;
    }

    public makeTopSelectBoxes() : HTMLDivElement {
        var datingFormDivEl = $(document.createElement("div"));
        datingFormDivEl.addClass("regex-dating-condition-selects");

        var datingSelectDivEl = $(document.createElement("div"));
        datingSelectDivEl.addClass("regex-dating-condition-select");

        var datingFormSpanEl = $(document.createElement("span"));
        datingFormSpanEl.text(localization.translate("DatingSelect", "PluginsJs").value);
        datingFormSpanEl.addClass("regexsearch-upper-select-label");
        datingSelectDivEl.append(datingFormSpanEl);

        var datingFormSelectEl = $(document.createElement("select"));
        datingFormSelectEl.addClass("regexsearch-select");
        datingSelectDivEl.append(datingFormSelectEl);

        datingFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("OlderThan", "PluginsJs").value, DatingRangeEnum.OlderThen.toString()));
        datingFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("YoungerThan", "PluginsJs").value, DatingRangeEnum.YoungerThen.toString()));
        datingFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("Between", "PluginsJs").value, DatingRangeEnum.Between.toString()));
        datingFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("Around", "PluginsJs").value, DatingRangeEnum.Around.toString()));

        this.datingRange = DatingRangeEnum.OlderThen;

        datingFormSelectEl.change((eventData) => {
            var oldRange = this.datingRange;
            this.datingRange = parseInt($(eventData.target as HTMLElement).val() as string);

            if (oldRange !== this.datingRange) {
                this.changeViews();
            }
        });

        this.datingRangeSelect = datingFormSelectEl[0] as Node as HTMLSelectElement;

        var precisionSelectDivEl = $(document.createElement("div"));
        precisionSelectDivEl.addClass("regex-dating-condition-select");

        var precisionFormSpanEl = $(document.createElement("span"));
        precisionFormSpanEl.text(localization.translate("PrecisionSelect", "PluginsJs").value);
        precisionFormSpanEl.addClass("regexsearch-upper-select-label");
        precisionSelectDivEl.append(precisionFormSpanEl);

        var precisionFormSelectEl = $(document.createElement("select"));
        precisionFormSelectEl.addClass("regexsearch-select");
        precisionSelectDivEl.append(precisionFormSelectEl);

        precisionFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("Period", "PluginsJs").value, DatingPrecisionEnum.Period.toString()));
        precisionFormSelectEl.append(HtmlItemsFactory.createOption(localization.translate("Year", "PluginsJs").value, DatingPrecisionEnum.Year.toString()));

        this.datingPrecision = DatingPrecisionEnum.Period;

        precisionFormSelectEl.change((eventData) => {
            var oldPrecision = this.datingPrecision;
            this.datingPrecision = parseInt($(eventData.target as HTMLElement).val() as string);

            if (oldPrecision !== this.datingPrecision) {
                this.changeViews();
            }
        });

        this.datingPrecisionSelect = precisionFormSelectEl[0] as Node as HTMLSelectElement;

        precisionSelectDivEl.append(precisionFormSelectEl);

        datingFormDivEl.append(datingSelectDivEl);
        datingFormDivEl.append(precisionSelectDivEl);

        return datingFormDivEl[0] as Node as HTMLDivElement;
    }

    private changeViews() {
        var oldFirstDataView = this.firstDateView;
        $(this.precisionInputDiv).empty();
        this.firstDateView = this.createInputRangeView();

        this.firstDateView.makeRangeView(this.precisionInputDiv);
        if (typeof oldFirstDataView !== 'undefined' && oldFirstDataView !== null) {
            this.firstDateView.setValues(oldFirstDataView.getLowerValue(), oldFirstDataView.getHigherValue());
        }
        

        if (this.datingRange === DatingRangeEnum.Between) {
            var delimeter = document.createElement("div");
            delimeter.innerHTML = localization.translate("Till", "PluginsJs").value;
            this.precisionInputDiv.appendChild(delimeter);

            var oldSecondView = this.secondDateView;
            this.secondDateView = this.createInputRangeView();
            this.secondDateView.makeRangeView(this.precisionInputDiv);
            if (typeof oldSecondView !== 'undefined' && oldSecondView !== null) {
                this.secondDateView.setValues(oldSecondView.getLowerValue(), oldSecondView.getHigherValue());
            }

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

    getConditionItemValue(): DatingCriteriaDescription {
        var datingValue = new DatingCriteriaDescription();
        datingValue.datingRangeEnum = this.datingRange;
        datingValue.datingPrecision = this.datingPrecision;

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

class RegExWordCondition implements IRegExConditionItemBase{
    private html: HTMLDivElement;
    private inputsArray: Array<RegExWordInput>;
    private hiddenWordInputSelects: Array<WordInputTypeEnum>;
    private inputsContainerDiv: HTMLDivElement;
    parent: IRegExConditionListBase;

    private delimeterClass: string = "regexsearch-or-delimiter";

    constructor(parent?: IRegExConditionListBase) {
        this.parent = parent;
    }

    importData(conditionData: WordCriteriaDescription) {
        this.resetInputs();
        if (conditionData.startsWith) {
            this.getLastInput().importData(conditionData.startsWith, WordInputTypeEnum.StartsWith);
            this.addInput();
        }
        if (conditionData.contains.length) {
            for (var i = 0; i < conditionData.contains.length; i++) {
                this.getLastInput().importData(conditionData.contains[i], WordInputTypeEnum.Contains);
                this.addInput();
            }
        }
        if (conditionData.endsWith) {
            this.getLastInput().importData(conditionData.endsWith, WordInputTypeEnum.EndsWith);
            this.addInput();
        }
        if (conditionData.exactMatch) {
            this.getLastInput().importData(conditionData.exactMatch, WordInputTypeEnum.ExactMatch);
            this.addInput();
        }
        this.removeInput(this.getLastInput());
    }

    private getLastInput(): RegExWordInput {
        if (!this.inputsArray.length) return null;
        return this.inputsArray[this.inputsArray.length - 1];
    }

    getHtml(): HTMLDivElement {
        return this.html;
    }

    removeDelimeter() {
        $(this.html).find(`.${this.delimeterClass}`).empty();
    }

    hasDelimeter(): boolean {
        const isEmpty = $(this.html).find("." + this.delimeterClass).is(":empty");
        return !isEmpty;
    }

    setTextDelimeter() {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(`.${this.delimeterClass}`).append(textDelimeter);
    }

    setClickableDelimeter() {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(`.${this.delimeterClass}`).append(clickableDelimeter);
    }

    private createClickableDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = localization.translate("+Or", "PluginsJs").value;
        $(addWordSpan).click(() => {
            this.parent.addItem();
        });

        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass(this.delimeterClass);

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeItem(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    private createTextDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = localization.translate("Or", "PluginsJs").value;
        $(delimeterDiv).addClass(this.delimeterClass);

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeItem(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    makeRegExItemCondition() {
        var mainDiv = $("<div></div>");
        mainDiv.addClass("reg-ex-word-condition");

        const inputsContainerEl = $("<div></div>");
        inputsContainerEl.addClass("regexsearch-word-input-list-div");
        this.inputsContainerDiv = inputsContainerEl[0] as Node as HTMLDivElement;
        mainDiv.append(inputsContainerEl);

        var commandsDiv = $("<div></div>");
        commandsDiv.addClass("regexsearch-conditions-commands");
        mainDiv.append(commandsDiv);

        var addConditionButton = $("<button></button>");
        addConditionButton.prop("type", "button");
        addConditionButton.text("+");
        addConditionButton.addClass("btn btn-default regexsearch-button regexsearch-add-input-button");
        addConditionButton.click(() => {
            this.addInput();
        });
        commandsDiv.append(addConditionButton[0] as Node as HTMLElement);
        mainDiv.append(this.createTextDelimeter());
        this.resetInputs();
        this.html = mainDiv[0] as Node as HTMLDivElement;
    }

    resetInputs() {
        this.hiddenWordInputSelects = new Array<WordInputTypeEnum>();
        $(this.inputsContainerDiv).empty();
        this.inputsArray = new Array<RegExWordInput>();
        this.addInput();
        $(this.inputsContainerDiv).find("select").trigger("change");
    }

    addInput() {
        const newInput = new RegExWordInput(this);
        newInput.makeRegExInput();
        for (let i = 0; i < this.hiddenWordInputSelects.length; i++) {
            newInput.hideSelectCondition(this.hiddenWordInputSelects[i]);
        }
        if (!(newInput.getConditionType() === WordInputTypeEnum.Contains)) {
            this.hiddenWordInputSelects.push(newInput.getConditionType());
        }
        if (!(newInput.getConditionType() === WordInputTypeEnum.ExactMatch)) {
            this.hiddenWordInputSelects.push(WordInputTypeEnum.ExactMatch);
        }
        this.inputsArray.push(newInput);
        this.inputsContainerDiv.appendChild(newInput.getHtml());
    }

    removeInput(input: RegExWordInput) {
        this.wordInpuConditionRemoved(input.getConditionType());
        const index = this.inputsArray.indexOf(input, 0);
        if (index >= 0) {
            var arrayItem = this.inputsArray[index];
            this.inputsContainerDiv.removeChild(arrayItem.getHtml());
            this.inputsArray.splice(index, 1);
        }

        if (this.inputsArray.length === 0) {
            this.resetInputs();
        }
    }

    getConditionItemValue(): WordCriteriaDescription {
        var wordCriteriaDescription = new WordCriteriaDescription();
        for (let i = 0; i < this.inputsArray.length; i++) {
            const wordInput = this.inputsArray[i];
            const inputValue = wordInput.getConditionValue();
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
            case WordInputTypeEnum.ExactMatch:
                wordCriteriaDescription.exactMatch = inputValue;
                break;
            default:
                break;
            }
        }
        return wordCriteriaDescription;
    }


    wordInputConditionChanged(wordInput: RegExWordInput, oldWordInputType: WordInputTypeEnum) {
        var newWordInputType = wordInput.getConditionType();

        if (oldWordInputType) {
            this.wordInpuConditionRemoved(oldWordInputType);
        }

        if (!(newWordInputType === WordInputTypeEnum.Contains)) {
            for (let i = 0; i < this.inputsArray.length; i++) {
                if (this.inputsArray[i] === wordInput) continue;
                this.inputsArray[i].hideSelectCondition(newWordInputType);
            }

            this.hiddenWordInputSelects.push(newWordInputType);
        }
    }

    wordInpuConditionRemoved(wordInputType: WordInputTypeEnum) {

        if (!(wordInputType === WordInputTypeEnum.Contains)) {
            for (let i = 0; i < this.inputsArray.length; i++) {
                this.inputsArray[i].showSelectCondition(wordInputType);
            }
        }

        const index = this.hiddenWordInputSelects.indexOf(wordInputType, 0);
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
    private readonly parentRegExWordCondition: RegExWordCondition;
    private regexButtonsDiv: HTMLDivElement;
    private conditionSelectbox: HTMLSelectElement;

    constructor(parent: RegExWordCondition) {
        this.parentRegExWordCondition = parent;
    }

    public importData(data: string, wordInputType: WordInputTypeEnum) {
        $(this.conditionSelectbox).val(wordInputType.toString());
        $(this.conditionSelectbox).change();

        $(this.conditionInput).val(data);
        $(this.conditionInput).text(data);
        $(this.conditionInput).change();
    }

    getHtml(): HTMLDivElement {
        return this.html;
    }

    hasDelimeter(): boolean {
        const delimeter = $(this.html).find(".regexsearch-input-and-delimiter");
        return (delimeter ? true : false);
    }

    makeRegExInput() {
        var mainDiv = document.createElement("div");
        mainDiv.classList.add("reg-ex-word-input");

        var lineDiv = document.createElement("div");
        lineDiv.classList.add("regex-word-input-textbox");

        var editorDiv = document.createElement("div");
        this.editorDiv = editorDiv;

        var conditionTitleDiv = document.createElement("div");
        conditionTitleDiv.innerHTML = localization.translate("Constraint", "PluginsJs").value;
        editorDiv.appendChild(conditionTitleDiv);
        
        var conditionTypeDivEl = $(document.createElement("div"));
        conditionTypeDivEl.addClass("regexsearch-condition-type-div");
        editorDiv.appendChild(conditionTypeDivEl[0]);

        var conditionSelectEl = $(document.createElement("select"));
        conditionSelectEl.addClass("regexsearch-condition-select");
        conditionTypeDivEl.append(conditionSelectEl);

        conditionSelectEl.append(HtmlItemsFactory.createOption(localization.translate("StartsWith", "PluginsJs").value, WordInputTypeEnum.StartsWith.toString()));
        //conditionSelectEl.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelectEl.append(HtmlItemsFactory.createOption(localization.translate("Contains", "PluginsJs").value, WordInputTypeEnum.Contains.toString()));
        //conditionSelectEl.append(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelectEl.append(HtmlItemsFactory.createOption(localization.translate("EndsWith", "PluginsJs").value, WordInputTypeEnum.EndsWith.toString()));
        //conditionSelectEl.append(this.createOption("Nekončí na", this.conditionType.NotEndsWith));
        conditionSelectEl.append(HtmlItemsFactory.createOption("Přesně shoduje", WordInputTypeEnum.ExactMatch.toString())); // TODO add localization

        conditionSelectEl.change((eventData) => {
            var oldConditonType = this.conditionInputType;
            const selectEl = $(eventData.target as HTMLElement);
            this.conditionInputType = parseInt(selectEl.val() as string);
            if (this.conditionInputType === WordInputTypeEnum.ExactMatch) {
                const regexWordConditionEl = selectEl.parents(".reg-ex-word-condition");
                const wordInputEl = selectEl.parents(".reg-ex-word-input");
                const otherWordInputs = wordInputEl.siblings(".reg-ex-word-input");
                if (otherWordInputs) {
                    otherWordInputs.find(".regexsearch-condition-input").prop("disabled", true);
                    otherWordInputs.find(".regexsearch-condition-input-button").prop("disabled", true);
                }
                const regexAddInputButton = regexWordConditionEl.find(".regexsearch-add-input-button");
                regexAddInputButton.prop("disabled", true);
            } else {
                const regexWordConditionEl = selectEl.parents(".reg-ex-word-condition");
                const wordInputEl = selectEl.parents(".reg-ex-word-input");
                const otherWordInputs = wordInputEl.siblings(".reg-ex-word-input");
                if (otherWordInputs) {
                    otherWordInputs.find(".regexsearch-condition-input").prop("disabled", false);
                    otherWordInputs.find(".regexsearch-condition-input-button").prop("disabled", false);
                }
                const regexAddInputButton = regexWordConditionEl.find(".regexsearch-add-input-button");
                regexAddInputButton.prop("disabled", false);
            }
            this.parentRegExWordCondition.wordInputConditionChanged(this, oldConditonType);
        });

        this.conditionSelectbox = conditionSelectEl[0] as Node as HTMLSelectElement;

        this.conditionInput = document.createElement("input");
        this.conditionInput.type = "text";
        this.conditionInput.classList.add("form-control");
        this.conditionInput.classList.add("regexsearch-condition-input");
        this.conditionInput.classList.add("keyboard-input");
        this.conditionInput.setAttribute("data-keyboard-id", "0");
        
        lineDiv.appendChild(this.conditionInput);
        
        var keyboardButton = $(document.createElement("button"));
        keyboardButton.attr("type", "button");
        keyboardButton.addClass("btn");
        keyboardButton.addClass("regexsearch-condition-input-button");
        var keyboardIcon = $(document.createElement("div"));
        keyboardIcon.addClass("custom-glyphicon-keyboard");
        keyboardIcon.css("height", "100%");
        keyboardButton.append(keyboardIcon);
        lineDiv.appendChild(keyboardButton[0]);

        var keyboardComponent = KeyboardManager.getKeyboard("0");
        keyboardComponent.registerButton(keyboardButton[0] as Node as HTMLButtonElement, this.conditionInput, null);

        var regExButton = $("<button></button>");
        regExButton.text("R");
        regExButton.attr("type", "button");
        regExButton.addClass("btn regexsearch-condition-input-button");
        regExButton.click(() => {
            if ($(this.regexButtonsDiv).is(":hidden")) {
                $(this.regexButtonsDiv).slideDown("fast");
            } else {
                $(this.regexButtonsDiv).slideUp("fast");
            }
        });
        lineDiv.appendChild(regExButton[0]);

        var removeButton = HtmlItemsFactory.createButton("");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon");
        $(removeGlyph).addClass("glyphicon-trash");
        removeButton.appendChild(removeGlyph);
        $(removeButton).css("margin-left", "3px");
        $(removeButton).click((event) => {
            const eventTargetEl = $(event.target as Node as Element);
            const conditionList = eventTargetEl.parents(".reg-ex-word-condition");

            this.parentRegExWordCondition.removeInput(this);

            const regexWordInputEls = conditionList.find(".reg-ex-word-input");
            var numberOfExactMatchSeachTypes = 0;
            regexWordInputEls.each((index, element) => {
                const wordInputEl = $(element as Node as Element);
                const selectEl = wordInputEl.find("select");
                const searchType: WordInputTypeEnum = parseInt(selectEl.val() as string);
                if (searchType === WordInputTypeEnum.ExactMatch) {
                    numberOfExactMatchSeachTypes++;
                }
            });
            if (numberOfExactMatchSeachTypes === 0) {
                const plusButtonEl = conditionList.find(".regexsearch-add-input-button");
                plusButtonEl.prop("disabled", false);
                regexWordInputEls.find(".regexsearch-condition-input").prop("disabled", false);
                regexWordInputEls.find(".regexsearch-condition-input-button").prop("disabled", false);
            }
        });

        lineDiv.appendChild(removeButton);
        mainDiv.appendChild(this.editorDiv);
        mainDiv.appendChild(lineDiv);


        var regexButtonsDiv = document.createElement("div");
        $(regexButtonsDiv).addClass("regexsearch-regex-buttons-div");

        var anythingButton = HtmlItemsFactory.createButton(localization.translate("Anything", "PluginsJs").value);
        regexButtonsDiv.appendChild(anythingButton);
        $(anythingButton).addClass("regexsearch-editor-button");
        $(anythingButton).click(() => {
            this.conditionInput.value += "%";
        });

        var oneCharButton = HtmlItemsFactory.createButton(localization.translate("OneCharacter", "PluginsJs").value);
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

class RegExTokenDistanceConditionList implements IRegExConditionListBase {
    parentRegExConditionListItem: RegExConditionListItem;
    private tokenDistanceListContainerDiv: HTMLDivElement;
    private conditionInputArray: Array<RegExTokenDistanceCondition>;

    constructor(parent: RegExConditionListItem) {
        this.parentRegExConditionListItem = parent;
    }
    
    importData(conditionsArray: TokenDistanceCriteriaListDescription) {
        this.resetItems();
        if (!conditionsArray.conditions.length) return;
        this.getLastItem().importData(conditionsArray.conditions[0]);
        for (var i = 1; i < conditionsArray.conditions.length; i++) {
            this.addItem();
            this.getLastItem().importData(conditionsArray.conditions[i]);
        }
    }

    getLastItem(): IRegExConditionItemBase {
        if (!this.conditionInputArray.length) return null;
        return this.conditionInputArray[this.conditionInputArray.length - 1];
    }

    public makeRegExCondition(conditionContainerDiv: HTMLDivElement) {
        this.tokenDistanceListContainerDiv = document.createElement("div");
        $(this.tokenDistanceListContainerDiv).addClass("regexsearch-condition-list-div");
        conditionContainerDiv.appendChild(this.tokenDistanceListContainerDiv);

        this.resetItems();
    }

    getConditionValue(): TokenDistanceCriteriaListDescription {
        var criteriaDescriptions = new TokenDistanceCriteriaListDescription();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExTokenDistanceCondition = this.conditionInputArray[i];
            criteriaDescriptions.conditions.push(regExTokenDistanceCondition.getConditionItemValue());
        }
        return criteriaDescriptions;
    }

    getConditionType(): ConditionTypeEnum {
        return ConditionTypeEnum.TokenDistanceList;
    }

    resetItems() {
        $(this.tokenDistanceListContainerDiv).empty();
        this.conditionInputArray = [];
        const newTokenDistanceCondition = new RegExTokenDistanceCondition(this);
        newTokenDistanceCondition.makeRegExItemCondition();
        newTokenDistanceCondition.setClickableDelimeter();
        this.conditionInputArray.push(newTokenDistanceCondition);
        this.tokenDistanceListContainerDiv.appendChild(newTokenDistanceCondition.getHtml());
    }

    addItem() {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        var newTokenDistanceCondition = new RegExTokenDistanceCondition(this);
        newTokenDistanceCondition.makeRegExItemCondition();
        newTokenDistanceCondition.setClickableDelimeter();
        this.conditionInputArray.push(newTokenDistanceCondition);
        this.tokenDistanceListContainerDiv.appendChild(newTokenDistanceCondition.getHtml());
    }

    removeItem(condition: RegExTokenDistanceCondition) {

        const index = this.conditionInputArray.indexOf(condition, 0);
        if (!index) {
            var arrayItem = this.conditionInputArray[index];
            $(arrayItem.getHtml()).fadeToggle("slow", "linear",() => {
                this.tokenDistanceListContainerDiv.removeChild(arrayItem.getHtml());
            });
            this.conditionInputArray.splice(index, 1);
        }

        if (this.conditionInputArray.length === 1) {
            this.conditionInputArray[0].setClickableDelimeter();
        }

        if (this.conditionInputArray.length === 0) {
            this.resetItems();
        }
    }
}

class RegExTokenDistanceCondition implements IRegExConditionItemBase {
    parent: IRegExConditionListBase;

    private minTokenDistanceValue: number = 1;
    private maxTokenDistanceValue: number = 100;
    private initTokenDistanceValue: number = 1;

    private actualTokenDistanceValue: number;

    private html: HTMLDivElement;
    private tokenDistanceInput: HTMLInputElement;

    private firstToken: RegExWordCondition;
    private secondToken: RegExWordCondition;
    private tokenDistance: number;

    private delimeterClass: string = "regexsearch-token-distance-or-delimiter";

    constructor(parent: RegExTokenDistanceConditionList) {
        this.parent = parent;
    }

    importData(conditionData: TokenDistanceCriteriaDescription) {
        this.firstToken.importData(conditionData.first);
        this.secondToken.importData(conditionData.second);
        $(this.tokenDistanceInput).val(conditionData.distance.toString());
        $(this.tokenDistanceInput).keyup();
        $(this.tokenDistanceInput).change();
    }

    getHtml(): HTMLDivElement {
        return this.html;
    }

    removeDelimeter() {
        $(this.html).find(`.${this.delimeterClass}`).empty();
    }

    hasDelimeter(): boolean {
        const isEmpty = $(this.html).find(`.${this.delimeterClass}`).is(":empty");
        return !isEmpty;
    }

    setTextDelimeter() {
        const textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(`.${this.delimeterClass}`).append(textDelimeter);
    }

    setClickableDelimeter() {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(`.${this.delimeterClass}`).append(clickableDelimeter);
    }

    private createClickableDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = localization.translate("+Or", "PluginsJs").value;
        $(addWordSpan).click(() => {
            this.parent.addItem();
        });

        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass(this.delimeterClass);

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeItem(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    private createTextDelimeter(): HTMLDivElement {
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = localization.translate("Or", "PluginsJs").value;
        $(delimeterDiv).addClass(this.delimeterClass);

        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(() => {
            this.parent.removeItem(this);
        });

        delimeterDiv.appendChild(trashButton);

        return delimeterDiv;
    }

    makeRegExItemCondition() {
        var mainDiv = document.createElement("div");
        $(mainDiv).addClass("regexsearch-token-distance-condition");

        this.firstToken = new RegExWordCondition();
        this.firstToken.makeRegExItemCondition();
        this.firstToken.removeDelimeter();

        this.secondToken = new RegExWordCondition();
        this.secondToken.makeRegExItemCondition();
        this.secondToken.removeDelimeter();
        
        mainDiv.appendChild(this.firstToken.getHtml());

        var inputTextDiv = document.createElement("div");
        $(inputTextDiv).addClass("regexsearch-token-distance-condition-input-div");

        var inputTextSpan = document.createElement("span");
        $(inputTextSpan).addClass("regexsearch-token-distance-condition-input-text");
        inputTextSpan.innerHTML = localization.translate("Distance:", "PluginsJs").value;
        inputTextDiv.appendChild(inputTextSpan);

        var tokenDistanceInput = document.createElement("input");
        tokenDistanceInput.type = "number";
        tokenDistanceInput.min = this.minTokenDistanceValue.toString();
        tokenDistanceInput.max = this.maxTokenDistanceValue.toString();
        tokenDistanceInput.value = this.initTokenDistanceValue.toString();
        this.actualTokenDistanceValue = this.initTokenDistanceValue;
        $(tokenDistanceInput).addClass("form-control");
        $(tokenDistanceInput).addClass("regexsearch-condition-input");
        inputTextDiv.appendChild(tokenDistanceInput);

        $(tokenDistanceInput).keyup((e) => {
            const targetEl = $(e.target as Node as Element);
            var value = targetEl.val() as string;
            value.replace(/[^0-9]/g, '');
            targetEl.val(value);
            targetEl.text(value);

            this.actualTokenDistanceValue = parseInt(value);
        });

        $(tokenDistanceInput).change((e) => {
            var value = $(e.target as Node as Element).val() as string;
            this.actualTokenDistanceValue = parseInt(value);
        });

        this.tokenDistanceInput = tokenDistanceInput;

        mainDiv.appendChild(inputTextDiv);

        mainDiv.appendChild(this.secondToken.getHtml());
        mainDiv.appendChild(this.createTextDelimeter());

        this.html = mainDiv;
    }

    getConditionItemValue(): TokenDistanceCriteriaDescription {
        var tokenDistanceCriteriaDescription = new TokenDistanceCriteriaDescription();
        tokenDistanceCriteriaDescription.first = this.firstToken.getConditionItemValue();
        tokenDistanceCriteriaDescription.second = this.secondToken.getConditionItemValue();
        tokenDistanceCriteriaDescription.distance = this.actualTokenDistanceValue;
        return tokenDistanceCriteriaDescription;
    }
}


//  Classes for storing data


class ConditionResult {
    searchType: SearchTypeEnum;         //enum Author, Text, Editor etc.
    conditionType: ConditionTypeEnum;      //type of derived class ie WordList = WordList
    conditions: Array<ConditionItemResult>;
}

class ConditionItemResult {
    
}

class DatingCriteriaListDescription extends ConditionResult {
    constructor() {
        super();
        this.conditions = new Array<DatingCriteriaDescription>();
    }
}

class DatingCriteriaDescription extends ConditionItemResult{
    notBefore: number;
    notAfter: number;
    datingPrecision: DatingPrecisionEnum;
    datingRangeEnum: DatingRangeEnum;
}

class WordsCriteriaListDescription extends ConditionResult {
    constructor() {
        super();
        this.conditions = new Array<WordCriteriaDescription>();
    }
}

class WordCriteriaDescription extends ConditionItemResult{
    startsWith: string;
    contains: Array<string>;
    endsWith: string;
    exactMatch: string;

    constructor() {
        super();
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

class TokenDistanceCriteriaDescription extends ConditionItemResult{
    distance: number;
    first: WordCriteriaDescription;
    second: WordCriteriaDescription;
}

enum WordInputTypeEnum {
    StartsWith = 0,
    Contains = 1,
    EndsWith = 2,
    ExactMatch = 3
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
    Headword = 10,
    HeadwordDescription = 11,
    HeadwordDescriptionTokenDistance = 12,
    SelectedCategory = 13,
    Term = 14
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