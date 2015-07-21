var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var HtmlItemsFactory = (function () {
    function HtmlItemsFactory() {
    }
    HtmlItemsFactory.createOption = function (label, value) {
        var conditionOption = document.createElement("option");
        conditionOption.innerHTML = label;
        conditionOption.value = value;
        return conditionOption;
    };
    HtmlItemsFactory.createButton = function (label) {
        var button = document.createElement("button");
        button.type = "button";
        button.innerHTML = label;
        $(button).addClass("btn");
        $(button).addClass("btn-default");
        $(button).addClass("regexsearch-button");
        return button;
    };
    return HtmlItemsFactory;
})();
var Search = (function () {
    function Search(container, jsonSearchUrl, textSearchUrl, processSearchResultCallback) {
        this.speedAnimation = 200; //200=fast, 600=slow
        this.container = container;
        this.jsonSearchUrl = jsonSearchUrl;
        this.textSearchUrl = textSearchUrl;
        this.searchResultCallback = processSearchResultCallback;
    }
    Search.prototype.makeSearch = function (disabledOptions) {
        var _this = this;
        var searchAreaDiv = document.createElement("div");
        $(searchAreaDiv).addClass("regex-search-div");
        var form = document.createElement("form");
        $(form).attr("role", "form");
        $(form).addClass("form-horizontal");
        $(searchAreaDiv).append(form);
        var formGroupDiv = document.createElement("div");
        $(formGroupDiv).addClass("form-group searchbar");
        $(form).append(formGroupDiv);
        var searchbarButtonsDiv = document.createElement("div");
        $(searchbarButtonsDiv).addClass("searchbar-buttons");
        $(formGroupDiv).append(searchbarButtonsDiv);
        var searchButton = document.createElement("button");
        searchButton.type = "button";
        searchButton.innerHTML = "Vyhledat";
        $(searchButton).addClass("btn btn-default searchbar-button");
        $(searchbarButtonsDiv).append(searchButton);
        this.searchButton = searchButton;
        var advancedButton = document.createElement("button");
        advancedButton.type = "button";
        advancedButton.innerHTML = "Pokročilé";
        $(advancedButton).addClass("btn btn-default searchbar-button");
        $(searchbarButtonsDiv).append(advancedButton);
        this.advancedButton = advancedButton;
        var advancedButtonSpanCarrot = document.createElement("span");
        $(advancedButtonSpanCarrot).addClass("glyphicon glyphicon-chevron-down regexsearch-button-glyph");
        $(advancedButton).append(advancedButtonSpanCarrot);
        var searchbarInputDiv = document.createElement("div");
        $(searchbarInputDiv).addClass("regex-searchbar-inputs");
        $(formGroupDiv).append(searchbarInputDiv);
        var searchbarInput = document.createElement("input");
        searchbarInput.type = "text";
        searchbarInput.placeholder = "Hledat...";
        $(searchbarInput).addClass("form-control searchbar-input");
        $(searchbarInputDiv).append(searchbarInput);
        this.searchInputTextbox = searchbarInput;
        var searchbarAdvancedEditor = document.createElement("div");
        $(searchbarInputDiv).addClass("regex-searchbar-advanced-editor");
        $(searchAreaDiv).append(searchbarAdvancedEditor);
        this.searchbarAdvancedEditorContainer = searchbarAdvancedEditor;
        $(this.container).append(searchAreaDiv);
        $(this.searchButton).click(function (event) {
            _this.processSearch();
        });
        $(this.advancedButton).click(function () {
            $(_this.advancedButton).css("visibility", "hidden");
            if (_this.searchbarAdvancedEditorContainer.children.length === 0) {
                _this.advancedRegexEditor = new RegExAdvancedSearchEditor(_this.searchbarAdvancedEditorContainer, function (json) { return _this.closeAdvancedSearchEditor(json); });
                _this.advancedRegexEditor.setDisabledOptions(disabledOptions);
                _this.advancedRegexEditor.makeRegExSearch();
                $(_this.searchbarAdvancedEditorContainer).hide();
                $(_this.searchbarAdvancedEditorContainer).slideDown(_this.speedAnimation);
            }
            else if ($(_this.searchbarAdvancedEditorContainer).is(":hidden")) {
                var textboxValue = $(_this.searchInputTextbox).val();
                if (_this.isValidJson(textboxValue)) {
                    _this.advancedRegexEditor.importJson(textboxValue);
                }
                $(_this.searchbarAdvancedEditorContainer).slideDown(_this.speedAnimation);
                $(_this.searchInputTextbox).prop('disabled', true);
                $(_this.searchButton).prop('disabled', true);
            }
        });
    };
    Search.prototype.closeAdvancedSearchEditor = function (jsonData) {
        this.importJsonToTextField(jsonData);
        $(this.searchbarAdvancedEditorContainer).slideUp(this.speedAnimation); //hide advanced search
        $(this.searchInputTextbox).prop('disabled', false);
        $(this.searchButton).prop('disabled', false);
        $(this.advancedButton).css("visibility", "visible");
    };
    Search.prototype.importJsonToTextField = function (json) {
        $(this.searchInputTextbox).text(json);
        $(this.searchInputTextbox).val(json);
    };
    Search.prototype.importJsonToAdvancedSearch = function (json) {
    };
    Search.prototype.isValidJson = function (data) {
        try {
            JSON.parse(data);
            return true;
        }
        catch (e) {
            return false;
        }
    };
    Search.prototype.processSearch = function () {
        var searchboxValue = $(this.searchInputTextbox).val();
        if (this.isValidJson(searchboxValue)) {
            this.processSearchJson(searchboxValue);
        }
        else {
            this.processSearchText(searchboxValue);
        }
    };
    Search.prototype.processSearchJson = function (json) {
        var _this = this;
        $.ajax({
            type: "POST",
            traditional: true,
            data: JSON.stringify({ "json": json }),
            url: this.jsonSearchUrl,
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                _this.searchResultCallback(response);
            },
            error: function (response) {
            }
        });
    };
    Search.prototype.processSearchText = function (text) {
        var _this = this;
        $.ajax({
            type: "POST",
            traditional: true,
            data: JSON.stringify({ "text": text }),
            url: this.textSearchUrl,
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            success: function (response) {
                _this.searchResultCallback(response);
            },
            error: function (response) {
            }
        });
    };
    return Search;
})();
var RegExAdvancedSearchEditor = (function () {
    function RegExAdvancedSearchEditor(container, regexDoneCallback) {
        this.regexDoneCallback = regexDoneCallback;
        this.container = container;
    }
    RegExAdvancedSearchEditor.prototype.setDisabledOptions = function (disabledOptions) {
        this.disabledOptionsArray = disabledOptions;
    };
    RegExAdvancedSearchEditor.prototype.makeRegExSearch = function () {
        var _this = this;
        $(this.container).empty();
        this.regExConditions = [];
        var commandsDiv = document.createElement("div");
        var sentButton = HtmlItemsFactory.createButton("Dokončit");
        $(sentButton).addClass("regex-search-button");
        commandsDiv.appendChild(sentButton);
        $(sentButton).click(function () {
            var json = _this.getConditionsResultJSON();
            _this.regexDoneCallback(json);
        });
        this.innerContainer = document.createElement("div");
        this.addNewCondition(true);
        $(this.container).append(this.innerContainer);
        $(this.container).append(commandsDiv);
    };
    RegExAdvancedSearchEditor.prototype.importJson = function (json) {
        var jsonDataArray = JSON.parse(json);
        $(this.innerContainer).empty();
        this.regExConditions = new Array();
        for (var i = 0; i < jsonDataArray.length; i++) {
            var conditionData = jsonDataArray[i];
            this.addNewCondition();
            this.getLastCondition().importData(conditionData);
        }
    };
    RegExAdvancedSearchEditor.prototype.addNewCondition = function (useDelimiter) {
        if (useDelimiter === void 0) { useDelimiter = true; }
        if (this.regExConditions.length > 0) {
            this.getLastCondition().setTextDelimeter();
        }
        var newRegExConditions = new RegExConditionListItem(this);
        newRegExConditions.disbaleOptions(this.disabledOptionsArray);
        newRegExConditions.makeRegExCondition();
        newRegExConditions.setClickableDelimeter();
        if (!useDelimiter) {
            newRegExConditions.removeDelimeter();
        }
        this.regExConditions.push(newRegExConditions);
        $(this.innerContainer).append(newRegExConditions.getHtml());
    };
    RegExAdvancedSearchEditor.prototype.removeLastCondition = function () {
        if (this.regExConditions.length <= 1)
            return;
        var arrayItem = this.regExConditions.pop();
        this.innerContainer.removeChild(arrayItem.getHtml());
    };
    RegExAdvancedSearchEditor.prototype.removeCondition = function (condition) {
        var _this = this;
        var index = this.regExConditions.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.regExConditions[index];
            $(arrayItem.getHtml()).fadeToggle("slow", "linear", function () {
                _this.innerContainer.removeChild(arrayItem.getHtml());
            });
            this.regExConditions.splice(index, 1);
        }
        if (this.regExConditions.length === 0) {
            this.addNewCondition(true);
        }
        else {
            this.getLastCondition().setClickableDelimeter();
        }
    };
    RegExAdvancedSearchEditor.prototype.getConditionsResultObject = function () {
        var resultArray = new Array();
        for (var i = 0; i < this.regExConditions.length; i++) {
            var regExCondition = this.regExConditions[i];
            resultArray.push(regExCondition.getConditionValue());
        }
        return resultArray;
    };
    RegExAdvancedSearchEditor.prototype.getConditionsResultJSON = function () {
        var jsonString = JSON.stringify(this.getConditionsResultObject());
        return jsonString;
    };
    RegExAdvancedSearchEditor.prototype.getLastCondition = function () {
        if (this.regExConditions === null || this.regExConditions.length === 0)
            return null;
        return this.regExConditions[this.regExConditions.length - 1];
    };
    return RegExAdvancedSearchEditor;
})();
var RegExConditionListItem = (function () {
    function RegExConditionListItem(parent) {
        this.parent = parent;
    }
    RegExConditionListItem.prototype.importData = function (conditionData) {
        $(this.searchDestinationSelect).val(conditionData.searchType.toString());
        $(this.searchDestinationSelect).change();
        this.innerCondition.importData(conditionData);
        //TODO implement disabled options
    };
    RegExConditionListItem.prototype.getHtml = function () {
        return this.html;
    };
    RegExConditionListItem.prototype.removeDelimeter = function () {
        $(this.html).find(".regexsearch-delimiter").empty();
    };
    RegExConditionListItem.prototype.hasDelimeter = function () {
        var isEmpty = $(this.html).find(".regexsearch-delimiter").is(":empty");
        return !isEmpty;
    };
    RegExConditionListItem.prototype.setTextDelimeter = function () {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-delimiter").append(textDelimeter);
    };
    RegExConditionListItem.prototype.setClickableDelimeter = function () {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find(".regexsearch-delimiter").append(clickableDelimeter);
    };
    RegExConditionListItem.prototype.createClickableDelimeter = function () {
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
    RegExConditionListItem.prototype.createTextDelimeter = function () {
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
    RegExConditionListItem.prototype.getSearchType = function () {
        return this.selectedSearchType;
    };
    RegExConditionListItem.prototype.disbaleOptions = function (disabledOptions) {
        if (typeof this.searchDestinationSelect !== "undefined" || this.searchDestinationSelect !== null) {
            for (var i = 0; i < disabledOptions.length; i++) {
                var disabled = disabledOptions[i];
                $(this.searchDestinationSelect).find("option[value = " + disabled.toString() + "]").hide();
            }
        }
        this.disabledOptions = disabledOptions;
    };
    RegExConditionListItem.prototype.makeRegExCondition = function () {
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
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Text", 4 /* Fulltext */.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("X tokenů od sebe", 9 /* TokenDistance */.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Ve větě", 6 /* Sentence */.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("V nadpisu", 5 /* Heading */.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Autor", 0 /* Author */.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Titul", 1 /* Title */.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Editor", 2 /* Editor */.toString()));
        searchDestinationSelect.appendChild(HtmlItemsFactory.createOption("Období vzniku", 3 /* Dating */.toString()));
        this.selectedSearchType = 4 /* Fulltext */;
        $(searchDestinationSelect).change(function (eventData) {
            var oldSelectedSearchType = _this.selectedSearchType;
            _this.selectedSearchType = parseInt($(eventData.target).val());
            if (_this.selectedSearchType !== oldSelectedSearchType) {
                _this.changeConditionType(_this.selectedSearchType, oldSelectedSearchType);
            }
        });
        this.searchDestinationSelect = searchDestinationSelect;
        this.disbaleOptions(this.disabledOptions);
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
    };
    RegExConditionListItem.prototype.changeConditionType = function (newSearchType, oldSearchType) {
        if (!(this.innerCondition instanceof RegExWordConditionList) && (newSearchType === 0 /* Author */ || newSearchType === 2 /* Editor */ || newSearchType === 4 /* Fulltext */ || newSearchType === 1 /* Title */ || newSearchType === 5 /* Heading */ || newSearchType === 6 /* Sentence */)) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExWordConditionList(this);
            this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        }
        else if (!(this.innerCondition instanceof RegExDatingConditionList) && (newSearchType === 3 /* Dating */)) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExDatingConditionList(this);
            this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        }
        else if (!(this.innerCondition instanceof RegExTokenDistanceConditionList) && (newSearchType === 9 /* TokenDistance */)) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExTokenDistanceConditionList(this);
            this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        }
    };
    RegExConditionListItem.prototype.makeDefaultCondition = function () {
        $(this.innerConditionContainer).empty();
        this.innerCondition = new RegExWordConditionList(this);
        this.innerCondition.makeRegExCondition(this.innerConditionContainer);
    };
    RegExConditionListItem.prototype.getConditionValue = function () {
        var conditionResult = this.innerCondition.getConditionValue();
        conditionResult.searchType = this.getSearchType();
        conditionResult.conditionType = this.innerCondition.getConditionType();
        return conditionResult;
    };
    return RegExConditionListItem;
})();
var RegExWordConditionList = (function () {
    function RegExWordConditionList(parent) {
        this.wordFormType = {
            Lemma: "lemma",
            HyperlemmaNew: "hyperlemma-new",
            HyperlemmaOld: "hyperlemma-old",
            Stemma: "stemma"
        };
        this.parentRegExConditionListItem = parent;
    }
    RegExWordConditionList.prototype.importData = function (conditionsArray) {
        this.resetItems();
        if (conditionsArray.conditions.length === 0)
            return;
        this.getLastItem().importData(conditionsArray.conditions[0]);
        for (var i = 1; i < conditionsArray.conditions.length; i++) {
            this.addItem();
            this.getLastItem().importData(conditionsArray.conditions[i]);
        }
    };
    RegExWordConditionList.prototype.getLastItem = function () {
        if (this.conditionInputArray.length === 0)
            return null;
        return this.conditionInputArray[this.conditionInputArray.length - 1];
    };
    RegExWordConditionList.prototype.getWordFormType = function () {
        return this.selectedWordFormType;
    };
    RegExWordConditionList.prototype.makeRegExCondition = function (conditionContainerDiv) {
        var _this = this;
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
        $(wordFormSelect).change(function (eventData) {
            _this.selectedWordFormType = $(eventData.target).val();
        });
        this.wordListContainerDiv = document.createElement("div");
        $(this.wordListContainerDiv).addClass("regexsearch-condition-list-div");
        conditionContainerDiv.appendChild(this.wordListContainerDiv);
        this.resetItems();
    };
    RegExWordConditionList.prototype.getConditionValue = function () {
        var criteriaDescriptions = new WordsCriteriaListDescription();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExWordCondition = this.conditionInputArray[i];
            criteriaDescriptions.conditions.push(regExWordCondition.getConditionItemValue());
        }
        return criteriaDescriptions;
    };
    RegExWordConditionList.prototype.getConditionType = function () {
        return 0 /* WordList */;
    };
    RegExWordConditionList.prototype.resetItems = function () {
        $(this.wordListContainerDiv).empty();
        this.conditionInputArray = [];
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExItemCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.wordListContainerDiv.appendChild(newWordCondition.getHtml());
    };
    RegExWordConditionList.prototype.addItem = function () {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExItemCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.wordListContainerDiv.appendChild(newWordCondition.getHtml());
    };
    RegExWordConditionList.prototype.removeItem = function (item) {
        var _this = this;
        var index = this.conditionInputArray.indexOf(item, 0);
        if (index != undefined) {
            var arrayItem = this.conditionInputArray[index];
            $(arrayItem.getHtml()).fadeToggle("slow", "linear", function () {
                _this.wordListContainerDiv.removeChild(arrayItem.getHtml());
            });
            this.conditionInputArray.splice(index, 1);
        }
        if (this.conditionInputArray.length === 1) {
            this.conditionInputArray[0].setClickableDelimeter();
        }
        if (this.conditionInputArray.length === 0) {
            this.resetItems();
        }
    };
    return RegExWordConditionList;
})();
var RegExDatingConditionRangePeriodView = (function () {
    function RegExDatingConditionRangePeriodView() {
        this.minCenturyValue = 8;
        this.maxCenturyValue = 21;
    }
    RegExDatingConditionRangePeriodView.prototype.makeRangeView = function (container) {
        var _this = this;
        var precisionInpuDiv = container;
        var centurySliderDiv = window.document.createElement("div");
        $(centurySliderDiv).addClass("regex-dating-century-div regex-slider-div");
        var centuryCheckboxDiv = window.document.createElement("div");
        $(centuryCheckboxDiv).addClass("regex-dating-checkbox-div");
        var centuryNameSpan = window.document.createElement("span");
        centuryNameSpan.innerHTML = "Století";
        centuryCheckboxDiv.appendChild(centuryNameSpan);
        centurySliderDiv.appendChild(centuryCheckboxDiv);
        precisionInpuDiv.appendChild(centurySliderDiv);
        var centuryArray = new Array();
        for (var century = this.minCenturyValue; century <= this.maxCenturyValue; century++) {
            centuryArray.push(new DatingSliderValue(century.toString(), century * 100 - 100, century * 100 - 1)); //calculate century low and high values (i.e 18. century is 1700 - 1799)
        }
        this.centurySliderValues = centuryArray;
        var sliderCentury = this.makeSlider(centuryArray, ". století", function (selectedValue) {
            _this.centuryChanged(selectedValue);
        });
        $(sliderCentury).change();
        centurySliderDiv.appendChild(sliderCentury);
        this.centurySlider = sliderCentury;
        var periodSliderDiv = window.document.createElement("div");
        $(periodSliderDiv).addClass("regex-dating-period-div regex-slider-div");
        var periodCheckboxDiv = window.document.createElement("div");
        $(periodCheckboxDiv).addClass("regex-dating-checkbox-div");
        var periodValueCheckbox = window.document.createElement("input");
        periodValueCheckbox.type = "checkbox";
        $(periodValueCheckbox).change(function (eventData) {
            var currentTarget = (eventData.currentTarget);
            if (currentTarget.checked) {
                $(eventData.target).parent().siblings(".slider").slider("option", "disabled", false);
                $(eventData.target).parent().siblings(".slider").find(".slider-tip").show();
                _this.periodEnabled = true;
                $(eventData.target).parents(".regex-slider-div").siblings(".regex-slider-div").find(".regex-dating-checkbox-div").find("input").prop('checked', false).change(); //uncheck other checboxes 
            }
            else {
                $(eventData.target).parent().siblings(".slider").slider("option", "disabled", true);
                $(eventData.target).parent().siblings(".slider").find(".slider-tip").hide();
                _this.periodEnabled = false;
            }
            _this.changedValue();
        });
        var periodNameSpan = window.document.createElement("span");
        periodNameSpan.innerHTML = "Přibližná doba";
        periodCheckboxDiv.appendChild(periodValueCheckbox);
        periodCheckboxDiv.appendChild(periodNameSpan);
        periodSliderDiv.appendChild(periodCheckboxDiv);
        precisionInpuDiv.appendChild(periodSliderDiv);
        this.periodSliderValues = new Array(new DatingSliderValue("začátek", 0, -85), new DatingSliderValue("čtvrtina", 0, -75), new DatingSliderValue("třetina", 0, -66), new DatingSliderValue("polovina", 0, -50), new DatingSliderValue("konec", 85, 0));
        var sliderPeriod = this.makeSlider(this.periodSliderValues, "", function (selectedValue) {
            _this.periodChanged(selectedValue);
        });
        $(sliderPeriod).slider("option", "disabled", true);
        $(sliderPeriod).parent().siblings(".slider").find(".slider-tip").hide();
        $(sliderPeriod).change();
        periodSliderDiv.appendChild(sliderPeriod);
        this.periodSlider = sliderPeriod;
        var decadesSliderDiv = window.document.createElement("div");
        $(decadesSliderDiv).addClass("regex-dating-decades-div regex-slider-div");
        var decadeCheckboxDiv = window.document.createElement("div");
        $(decadeCheckboxDiv).addClass("regex-dating-checkbox-div");
        var decadesCheckbox = window.document.createElement("input");
        decadesCheckbox.type = "checkbox";
        $(decadesCheckbox).change(function (eventData) {
            var currentTarget = (eventData.currentTarget);
            if (currentTarget.checked) {
                $(eventData.target).parent().siblings(".slider").slider("option", "disabled", false);
                $(eventData.target).parent().siblings(".slider").find(".slider-tip").show();
                _this.decadeEnabled = true;
                $(eventData.target).parents(".regex-slider-div").siblings(".regex-slider-div").find(".regex-dating-checkbox-div").find("input").prop('checked', false).change(); //uncheck other checboxes 
            }
            else {
                $(eventData.target).parent().siblings(".slider").slider("option", "disabled", true);
                $(eventData.target).parent().siblings(".slider").find(".slider-tip").hide();
                _this.decadeEnabled = false;
            }
            _this.changedValue();
        });
        var decadesNameSpan = window.document.createElement("span");
        decadesNameSpan.innerHTML = "Léta";
        decadeCheckboxDiv.appendChild(decadesCheckbox);
        decadeCheckboxDiv.appendChild(decadesNameSpan);
        decadesSliderDiv.appendChild(decadeCheckboxDiv);
        precisionInpuDiv.appendChild(decadesSliderDiv);
        var decadesArray = new Array();
        for (var decades = 0; decades <= 90; decades += 10) {
            decadesArray.push(new DatingSliderValue(decades.toString(), decades, -(100 - (decades + 10)))); //calculate decades low and high values (i.e 20. decades of 18. century is 1720-1729)
        }
        this.decadeSliderValues = decadesArray;
        var sliderDecades = this.makeSlider(decadesArray, ". léta", function (selectedValue) {
            _this.decadeChanged(selectedValue);
        });
        $(sliderDecades).slider("option", "disabled", true);
        $(sliderDecades).parent().siblings(".slider").find(".slider-tip").hide();
        $(sliderDecades).change();
        decadesSliderDiv.appendChild(sliderDecades);
        this.decadesSlider = sliderDecades;
        var datingDisplayedValueDiv = document.createElement('div');
        $(datingDisplayedValueDiv).addClass("regex-dating-condition-displayed-value");
        this.dateDisplayDiv = datingDisplayedValueDiv;
        precisionInpuDiv.appendChild(datingDisplayedValueDiv);
        this.changedValue();
    };
    RegExDatingConditionRangePeriodView.prototype.centuryChanged = function (sliderValue) {
        this.selectedCenturyLowerValue = sliderValue.lowNumberValue;
        this.selectedCenturyHigherValue = sliderValue.highNumberValue;
        this.changedValue();
    };
    RegExDatingConditionRangePeriodView.prototype.periodChanged = function (sliderValue) {
        this.selectedPeriodLowerValue = sliderValue.lowNumberValue;
        this.selectedPeriodHigherValue = sliderValue.highNumberValue;
        this.changedValue();
    };
    RegExDatingConditionRangePeriodView.prototype.decadeChanged = function (sliderValue) {
        this.selectedDecadeLowerValue = sliderValue.lowNumberValue;
        this.selectedDecadeHigherValue = sliderValue.highNumberValue;
        this.changedValue();
    };
    RegExDatingConditionRangePeriodView.prototype.changedValue = function () {
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
    };
    RegExDatingConditionRangePeriodView.prototype.makeSlider = function (valuesArray, nameEnding, callbackFunction) {
        var slider = document.createElement('div');
        $(slider).addClass('slider');
        $(slider).slider({
            min: 0,
            max: valuesArray.length - 1,
            value: 0,
            slide: function (event, ui) {
                $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html(valuesArray[ui.value].name + nameEnding);
            },
            change: function (event, ui) {
                callbackFunction(valuesArray[ui.value]);
                $(event.target).find('.ui-slider-handle').find('.tooltip-inner').html(valuesArray[ui.value].name + nameEnding);
            }
        });
        callbackFunction(valuesArray[0]); //default is first
        var sliderTooltip = document.createElement('div');
        $(sliderTooltip).addClass('tooltip top slider-tip');
        var arrowTooltip = document.createElement('div');
        $(arrowTooltip).addClass('tooltip-arrow');
        sliderTooltip.appendChild(arrowTooltip);
        var innerTooltip = document.createElement('div');
        $(innerTooltip).addClass('tooltip-inner');
        $(innerTooltip).html(valuesArray[0].name + nameEnding);
        sliderTooltip.appendChild(innerTooltip);
        var sliderHandle = $(slider).find('.ui-slider-handle');
        $(sliderHandle).append(sliderTooltip);
        return slider;
    };
    RegExDatingConditionRangePeriodView.prototype.getLowerValue = function () {
        return this.lowerValue;
    };
    RegExDatingConditionRangePeriodView.prototype.getHigherValue = function () {
        return this.higherValue;
    };
    RegExDatingConditionRangePeriodView.prototype.setValues = function (lower, higher) {
        var century = lower !== null ? Math.floor(lower / 100) : Math.floor(higher / 100);
        var centuryIndex = 0;
        for (var i = 0; i < this.centurySliderValues.length; i++) {
            var centurySliderValue = this.centurySliderValues[i];
            if (Math.floor(centurySliderValue.lowNumberValue / 100) === century) {
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
    };
    return RegExDatingConditionRangePeriodView;
})();
var RegExDatingConditionRangeYearView = (function () {
    function RegExDatingConditionRangeYearView() {
        this.minValue = 800;
        this.maxValue = 2100;
        this.initValue = 800;
    }
    RegExDatingConditionRangeYearView.prototype.makeRangeView = function (container) {
        var _this = this;
        var precisionInpuDiv = container;
        var textInput = document.createElement("input");
        textInput.type = "number";
        textInput.min = this.minValue.toString();
        textInput.max = this.maxValue.toString();
        textInput.value = this.initValue.toString();
        this.actualValue = this.initValue;
        // allows only digits input
        $(textInput).keyup(function (e) {
            var value = $(e.target).val();
            value.replace(/[^0-9]/g, '');
            $(e.target).val(value);
            $(e.target).text(value);
            _this.actualValue = parseInt(value);
        });
        $(textInput).change(function (e) {
            var value = $(e.target).val();
            _this.actualValue = parseInt(value);
        });
        this.valueInput = textInput;
        var spanInput = document.createElement("span");
        $(spanInput).addClass("regex-dating-input-span");
        spanInput.innerHTML = "Rok:";
        precisionInpuDiv.appendChild(spanInput);
        precisionInpuDiv.appendChild(textInput);
    };
    RegExDatingConditionRangeYearView.prototype.getLowerValue = function () {
        return this.actualValue;
    };
    RegExDatingConditionRangeYearView.prototype.getHigherValue = function () {
        return this.actualValue;
    };
    RegExDatingConditionRangeYearView.prototype.setValue = function (value) {
        $(this.valueInput).val(value.toString());
        $(this.valueInput).text(value.toString());
        $(this.valueInput).change();
    };
    RegExDatingConditionRangeYearView.prototype.setValues = function (lower, higher) {
        if (lower !== null) {
            this.setValue(lower);
        }
        else {
            this.setValue(higher);
        }
    };
    return RegExDatingConditionRangeYearView;
})();
var RegExDatingConditionList = (function () {
    function RegExDatingConditionList(parent) {
        this.parentRegExConditionListItem = parent;
    }
    RegExDatingConditionList.prototype.importData = function (conditionsArray) {
        this.resetItems();
        if (conditionsArray.conditions.length === 0)
            return;
        this.getLastItem().importData(conditionsArray.conditions[0]);
        for (var i = 1; i < conditionsArray.conditions.length; i++) {
            this.addItem();
            this.getLastItem().importData(conditionsArray.conditions[i]);
        }
    };
    RegExDatingConditionList.prototype.getLastItem = function () {
        if (this.conditionInputArray.length === 0)
            return null;
        return this.conditionInputArray[this.conditionInputArray.length - 1];
    };
    RegExDatingConditionList.prototype.makeRegExCondition = function (conditionContainerDiv) {
        this.datingListContainerDiv = document.createElement("div");
        $(this.datingListContainerDiv).addClass("regexsearch-condition-list-div");
        conditionContainerDiv.appendChild(this.datingListContainerDiv);
        this.resetItems();
    };
    RegExDatingConditionList.prototype.getConditionValue = function () {
        var criteriaDescriptions = new DatingCriteriaListDescription();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExDatingCondition = this.conditionInputArray[i];
            criteriaDescriptions.conditions.push(regExDatingCondition.getConditionItemValue());
        }
        return criteriaDescriptions;
    };
    RegExDatingConditionList.prototype.getConditionType = function () {
        return 1 /* DatingList */;
    };
    RegExDatingConditionList.prototype.resetItems = function () {
        $(this.datingListContainerDiv).empty();
        this.conditionInputArray = [];
        var newDatingCondition = new RegExDatingCondition(this);
        newDatingCondition.makeRegExItemCondition();
        newDatingCondition.setClickableDelimeter();
        this.conditionInputArray.push(newDatingCondition);
        this.datingListContainerDiv.appendChild(newDatingCondition.getHtml());
    };
    RegExDatingConditionList.prototype.addItem = function () {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        var newDatingCondition = new RegExDatingCondition(this);
        newDatingCondition.makeRegExItemCondition();
        newDatingCondition.setClickableDelimeter();
        this.conditionInputArray.push(newDatingCondition);
        this.datingListContainerDiv.appendChild(newDatingCondition.getHtml());
    };
    RegExDatingConditionList.prototype.removeItem = function (condition) {
        var _this = this;
        var index = this.conditionInputArray.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.conditionInputArray[index];
            $(arrayItem.getHtml()).fadeToggle("slow", "linear", function () {
                _this.datingListContainerDiv.removeChild(arrayItem.getHtml());
            });
            this.conditionInputArray.splice(index, 1);
        }
        if (this.conditionInputArray.length === 1) {
            this.conditionInputArray[0].setClickableDelimeter();
        }
        if (this.conditionInputArray.length === 0) {
            this.resetItems();
        }
    };
    return RegExDatingConditionList;
})();
var RegExDatingCondition = (function () {
    function RegExDatingCondition(parent) {
        this.DATING_AROUND = 3; //const
        this.delimeterClass = "regexsearch-dating-or-delimiter";
        this.parent = parent;
    }
    RegExDatingCondition.prototype.importData = function (conditionData) {
        $(this.datingRangeSelect).val(conditionData.datingRangeEnum.toString());
        $(this.datingRangeSelect).change();
        $(this.datingPrecisionSelect).val(conditionData.datingPrecision.toString());
        $(this.datingPrecisionSelect).change();
        switch (conditionData.datingRangeEnum) {
            case 1 /* YoungerThen */:
                this.firstDateView.setValues(conditionData.notAfter, null);
                break;
            case 0 /* OlderThen */:
                this.firstDateView.setValues(null, conditionData.notBefore);
                break;
            case 3 /* Around */:
                var lower = conditionData.notBefore + this.DATING_AROUND;
                var higher = conditionData.notAfter - this.DATING_AROUND;
                this.firstDateView.setValues(lower, higher);
                break;
            case 2 /* Between */:
                this.firstDateView.setValues(conditionData.notBefore, null);
                this.secondDateView.setValues(null, conditionData.notAfter);
                break;
            default:
                break;
        }
    };
    RegExDatingCondition.prototype.getHtml = function () {
        return this.html;
    };
    RegExDatingCondition.prototype.removeDelimeter = function () {
        $(this.html).find("." + this.delimeterClass).empty();
    };
    RegExDatingCondition.prototype.hasDelimeter = function () {
        var isEmpty = $(this.html).find("." + this.delimeterClass).is(":empty");
        return !isEmpty;
    };
    RegExDatingCondition.prototype.setTextDelimeter = function () {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find("." + this.delimeterClass).append(textDelimeter);
    };
    RegExDatingCondition.prototype.setClickableDelimeter = function () {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find("." + this.delimeterClass).append(clickableDelimeter);
    };
    RegExDatingCondition.prototype.createClickableDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = "+ Nebo";
        $(addWordSpan).click(function () {
            _this.parent.addItem();
        });
        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass(this.delimeterClass);
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeItem(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
    };
    RegExDatingCondition.prototype.createTextDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = "Nebo";
        $(delimeterDiv).addClass(this.delimeterClass);
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeItem(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
    };
    RegExDatingCondition.prototype.makeRegExItemCondition = function () {
        var datingConditionDiv = document.createElement('div');
        $(datingConditionDiv).addClass("regex-dating-condition");
        var datingDiv = document.createElement('div');
        $(datingDiv).addClass("regex-dating-condition-value-select");
        datingDiv.appendChild(this.makeTopSelectBoxes());
        var precisionInpuDiv = window.document.createElement("div");
        $(precisionInpuDiv).addClass("regex-dating-precision-div");
        this.precisionInputDiv = precisionInpuDiv;
        datingDiv.appendChild(precisionInpuDiv);
        this.changeViews();
        datingConditionDiv.appendChild(datingDiv);
        datingConditionDiv.appendChild(this.createTextDelimeter());
        this.html = datingConditionDiv;
    };
    RegExDatingCondition.prototype.makeTopSelectBoxes = function () {
        var _this = this;
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
        datingFormSelect.appendChild(HtmlItemsFactory.createOption("Starší než", 0 /* OlderThen */.toString()));
        datingFormSelect.appendChild(HtmlItemsFactory.createOption("Mladší než", 1 /* YoungerThen */.toString()));
        datingFormSelect.appendChild(HtmlItemsFactory.createOption("Mezi", 2 /* Between */.toString()));
        datingFormSelect.appendChild(HtmlItemsFactory.createOption("Kolem", 3 /* Around */.toString()));
        this.datingRange = 0 /* OlderThen */;
        $(datingFormSelect).change(function (eventData) {
            var oldRange = _this.datingRange;
            _this.datingRange = parseInt($(eventData.target).val());
            if (oldRange !== _this.datingRange) {
                _this.changeViews();
            }
        });
        this.datingRangeSelect = datingFormSelect;
        var precisionSelectDiv = document.createElement("div");
        $(precisionSelectDiv).addClass("regex-dating-condition-select");
        var precisionFormSpan = document.createElement("span");
        precisionFormSpan.innerHTML = "Zadání přesnosti";
        $(precisionFormSpan).addClass("regexsearch-upper-select-label");
        precisionSelectDiv.appendChild(precisionFormSpan);
        var precisionFormSelect = document.createElement("select");
        $(precisionFormSelect).addClass("regexsearch-select");
        precisionSelectDiv.appendChild(precisionFormSelect);
        precisionFormSelect.appendChild(HtmlItemsFactory.createOption("Období", 1 /* Period */.toString()));
        precisionFormSelect.appendChild(HtmlItemsFactory.createOption("Rok", 0 /* Year */.toString()));
        this.datingPrecision = 1 /* Period */;
        $(precisionFormSelect).change(function (eventData) {
            var oldPrecision = _this.datingPrecision;
            _this.datingPrecision = parseInt($(eventData.target).val());
            if (oldPrecision !== _this.datingPrecision) {
                _this.changeViews();
            }
        });
        this.datingPrecisionSelect = precisionFormSelect;
        precisionSelectDiv.appendChild(precisionFormSelect);
        datingFormDiv.appendChild(datingSelectDiv);
        datingFormDiv.appendChild(precisionSelectDiv);
        return datingFormDiv;
    };
    RegExDatingCondition.prototype.changeViews = function () {
        var oldFirstDataView = this.firstDateView;
        $(this.precisionInputDiv).empty();
        this.firstDateView = this.createInputRangeView();
        this.firstDateView.makeRangeView(this.precisionInputDiv);
        if (typeof oldFirstDataView !== 'undefined' && oldFirstDataView !== null) {
            this.firstDateView.setValues(oldFirstDataView.getLowerValue(), oldFirstDataView.getHigherValue());
        }
        if (this.datingRange === 2 /* Between */) {
            var delimeter = document.createElement("div");
            delimeter.innerHTML = "až";
            this.precisionInputDiv.appendChild(delimeter);
            var oldSecondView = this.secondDateView;
            this.secondDateView = this.createInputRangeView();
            this.secondDateView.makeRangeView(this.precisionInputDiv);
            if (typeof oldSecondView !== 'undefined' && oldSecondView !== null) {
                this.secondDateView.setValues(oldSecondView.getLowerValue(), oldSecondView.getHigherValue());
            }
        }
        else {
            this.secondDateView = null;
        }
    };
    RegExDatingCondition.prototype.createInputRangeView = function () {
        if (this.datingPrecision === 1 /* Period */) {
            return new RegExDatingConditionRangePeriodView();
        }
        else {
            return new RegExDatingConditionRangeYearView();
        }
    };
    RegExDatingCondition.prototype.getConditionItemValue = function () {
        var datingValue = new DatingCriteriaDescription();
        datingValue.datingRangeEnum = this.datingRange;
        datingValue.datingPrecision = this.datingPrecision;
        switch (this.datingRange) {
            case 1 /* YoungerThen */:
                datingValue.notAfter = this.firstDateView.getLowerValue();
                break;
            case 0 /* OlderThen */:
                datingValue.notBefore = this.firstDateView.getHigherValue();
                break;
            case 3 /* Around */:
                datingValue.notAfter = this.firstDateView.getHigherValue() + this.DATING_AROUND;
                datingValue.notBefore = this.firstDateView.getLowerValue() - this.DATING_AROUND;
                break;
            case 2 /* Between */:
                datingValue.notBefore = this.firstDateView.getLowerValue();
                datingValue.notAfter = this.secondDateView.getHigherValue();
                break;
            default:
                break;
        }
        return datingValue;
    };
    return RegExDatingCondition;
})();
var DatingSliderValue = (function () {
    function DatingSliderValue(name, lowNumberValue, highNumberValue) {
        this.name = name;
        this.lowNumberValue = lowNumberValue;
        this.highNumberValue = highNumberValue;
    }
    return DatingSliderValue;
})();
var RegExWordCondition = (function () {
    function RegExWordCondition(parent) {
        this.delimeterClass = "regexsearch-or-delimiter";
        this.parent = parent;
    }
    RegExWordCondition.prototype.importData = function (conditionData) {
        this.resetInputs();
        if (typeof conditionData.startsWith !== "undefined" && conditionData.startsWith !== "") {
            this.getLastInput().importData(conditionData.startsWith, 0 /* StartsWith */);
            this.addInput();
        }
        if (typeof conditionData.contains !== "undefined" && conditionData.contains.length > 0) {
            for (var i = 0; i < conditionData.contains.length; i++) {
                this.getLastInput().importData(conditionData.contains[i], 1 /* Contains */);
                this.addInput();
            }
        }
        if (typeof conditionData.endsWith !== "undefined" && conditionData.endsWith !== "") {
            this.getLastInput().importData(conditionData.endsWith, 2 /* EndsWith */);
            this.addInput();
        }
        this.removeInput(this.getLastInput());
    };
    RegExWordCondition.prototype.getLastInput = function () {
        if (this.inputsArray.length === 0)
            return null;
        return this.inputsArray[this.inputsArray.length - 1];
    };
    RegExWordCondition.prototype.getHtml = function () {
        return this.html;
    };
    RegExWordCondition.prototype.removeDelimeter = function () {
        $(this.html).find("." + this.delimeterClass).empty();
    };
    RegExWordCondition.prototype.hasDelimeter = function () {
        var isEmpty = $(this.html).find("." + this.delimeterClass).is(":empty");
        return !isEmpty;
    };
    RegExWordCondition.prototype.setTextDelimeter = function () {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find("." + this.delimeterClass).append(textDelimeter);
    };
    RegExWordCondition.prototype.setClickableDelimeter = function () {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find("." + this.delimeterClass).append(clickableDelimeter);
    };
    RegExWordCondition.prototype.createClickableDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = "+ Nebo";
        $(addWordSpan).click(function () {
            _this.parent.addItem();
        });
        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass(this.delimeterClass);
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeItem(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
    };
    RegExWordCondition.prototype.createTextDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = "Nebo";
        $(delimeterDiv).addClass(this.delimeterClass);
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeItem(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
    };
    RegExWordCondition.prototype.makeRegExItemCondition = function () {
        var _this = this;
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
        $(addConditionButton).click(function () {
            _this.addInput();
        });
        commandsDiv.appendChild(addConditionButton);
        mainDiv.appendChild(this.createTextDelimeter());
        this.resetInputs();
        this.html = mainDiv;
    };
    RegExWordCondition.prototype.resetInputs = function () {
        this.hiddenWordInputSelects = new Array();
        $(this.inputsContainerDiv).empty();
        this.inputsArray = new Array();
        this.addInput();
    };
    RegExWordCondition.prototype.addInput = function () {
        var newInput = new RegExWordInput(this);
        newInput.makeRegExInput();
        for (var i = 0; i < this.hiddenWordInputSelects.length; i++) {
            newInput.hideSelectCondition(this.hiddenWordInputSelects[i]);
        }
        if (!(newInput.getConditionType() === 1 /* Contains */)) {
            this.hiddenWordInputSelects.push(newInput.getConditionType());
        }
        this.inputsArray.push(newInput);
        this.inputsContainerDiv.appendChild(newInput.getHtml());
    };
    RegExWordCondition.prototype.removeInput = function (input) {
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
    };
    RegExWordCondition.prototype.getConditionItemValue = function () {
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
    RegExWordCondition.prototype.wordInputConditionChanged = function (wordInput, oldWordInputType) {
        var newWordInputType = wordInput.getConditionType();
        if (typeof oldWordInputType !== "undefined") {
            this.wordInpuConditionRemoved(oldWordInputType);
        }
        if (!(newWordInputType === 1 /* Contains */)) {
            for (var i = 0; i < this.inputsArray.length; i++) {
                if (this.inputsArray[i] === wordInput)
                    continue;
                this.inputsArray[i].hideSelectCondition(newWordInputType);
            }
            this.hiddenWordInputSelects.push(newWordInputType);
        }
    };
    RegExWordCondition.prototype.wordInpuConditionRemoved = function (wordInputType) {
        if (!(wordInputType === 1 /* Contains */)) {
            for (var i = 0; i < this.inputsArray.length; i++) {
                this.inputsArray[i].showSelectCondition(wordInputType);
            }
        }
        var index = this.hiddenWordInputSelects.indexOf(wordInputType, 0);
        if (index >= 0) {
            this.hiddenWordInputSelects.splice(index, 1);
        }
    };
    return RegExWordCondition;
})();
var RegExWordInput = (function () {
    function RegExWordInput(parent) {
        this.parentRegExWordCondition = parent;
    }
    RegExWordInput.prototype.importData = function (data, wordInputType) {
        $(this.conditionSelectbox).val(wordInputType.toString());
        $(this.conditionSelectbox).change();
        $(this.conditionInput).val(data);
        $(this.conditionInput).text(data);
        $(this.conditionInput).change();
    };
    RegExWordInput.prototype.getHtml = function () {
        return this.html;
    };
    RegExWordInput.prototype.hasDelimeter = function () {
        var delimeter = $(this.html).find(".regexsearch-input-and-delimiter");
        return (typeof delimeter != "undefined" && delimeter != null);
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
        conditionSelect.appendChild(HtmlItemsFactory.createOption("Začíná na", 0 /* StartsWith */.toString()));
        //conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(HtmlItemsFactory.createOption("Obsahuje", 1 /* Contains */.toString()));
        //conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(HtmlItemsFactory.createOption("Končí na", 2 /* EndsWith */.toString()));
        //conditionSelect.appendChild(this.createOption("Nekončí na", this.conditionType.NotEndsWith));
        $(conditionSelect).change(function (eventData) {
            var oldConditonType = _this.conditionInputType;
            _this.conditionInputType = parseInt($(eventData.target).val());
            _this.parentRegExWordCondition.wordInputConditionChanged(_this, oldConditonType);
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
        $(regExButton).click(function () {
            if ($(_this.regexButtonsDiv).is(":hidden")) {
                $(_this.regexButtonsDiv).slideDown("fast");
            }
            else {
                $(_this.regexButtonsDiv).slideUp("fast");
            }
        });
        lineDiv.appendChild(regExButton);
        var removeButton = HtmlItemsFactory.createButton("");
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
        var anythingButton = HtmlItemsFactory.createButton("Cokoliv");
        regexButtonsDiv.appendChild(anythingButton);
        $(anythingButton).addClass("regexsearch-editor-button");
        $(anythingButton).click(function () {
            _this.conditionInput.value += "%";
        });
        var oneCharButton = HtmlItemsFactory.createButton("Jeden znak");
        regexButtonsDiv.appendChild(oneCharButton);
        $(oneCharButton).addClass("regexsearch-editor-button");
        $(oneCharButton).click(function () {
            _this.conditionInput.value += "_";
        });
        this.regexButtonsDiv = regexButtonsDiv;
        $(this.regexButtonsDiv).hide();
        mainDiv.appendChild(regexButtonsDiv);
        this.html = mainDiv;
        $(this.conditionSelectbox).val(1 /* Contains */.toString());
        $(this.conditionSelectbox).change();
    };
    RegExWordInput.prototype.getConditionValue = function () {
        return this.conditionInput.value;
    };
    RegExWordInput.prototype.getConditionType = function () {
        return this.conditionInputType;
    };
    RegExWordInput.prototype.showSelectCondition = function (wordInputType) {
        $(this.conditionSelectbox).find("option[value=" + wordInputType.toString() + "]").show();
    };
    RegExWordInput.prototype.hideSelectCondition = function (wordInputType) {
        $(this.conditionSelectbox).find("option[value=" + wordInputType.toString() + "]").hide();
    };
    return RegExWordInput;
})();
var RegExTokenDistanceConditionList = (function () {
    function RegExTokenDistanceConditionList(parent) {
        this.parentRegExConditionListItem = parent;
    }
    RegExTokenDistanceConditionList.prototype.importData = function (conditionsArray) {
        this.resetItems();
        if (conditionsArray.conditions.length === 0)
            return;
        this.getLastItem().importData(conditionsArray.conditions[0]);
        for (var i = 1; i < conditionsArray.conditions.length; i++) {
            this.addItem();
            this.getLastItem().importData(conditionsArray.conditions[i]);
        }
    };
    RegExTokenDistanceConditionList.prototype.getLastItem = function () {
        if (this.conditionInputArray.length === 0)
            return null;
        return this.conditionInputArray[this.conditionInputArray.length - 1];
    };
    RegExTokenDistanceConditionList.prototype.makeRegExCondition = function (conditionContainerDiv) {
        this.tokenDistanceListContainerDiv = document.createElement("div");
        $(this.tokenDistanceListContainerDiv).addClass("regexsearch-condition-list-div");
        conditionContainerDiv.appendChild(this.tokenDistanceListContainerDiv);
        this.resetItems();
    };
    RegExTokenDistanceConditionList.prototype.getConditionValue = function () {
        var criteriaDescriptions = new TokenDistanceCriteriaListDescription();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExTokenDistanceCondition = this.conditionInputArray[i];
            criteriaDescriptions.conditions.push(regExTokenDistanceCondition.getConditionItemValue());
        }
        return criteriaDescriptions;
    };
    RegExTokenDistanceConditionList.prototype.getConditionType = function () {
        return 2 /* TokenDistanceList */;
    };
    RegExTokenDistanceConditionList.prototype.resetItems = function () {
        $(this.tokenDistanceListContainerDiv).empty();
        this.conditionInputArray = [];
        var newTokenDistanceCondition = new RegExTokenDistanceCondition(this);
        newTokenDistanceCondition.makeRegExItemCondition();
        newTokenDistanceCondition.setClickableDelimeter();
        this.conditionInputArray.push(newTokenDistanceCondition);
        this.tokenDistanceListContainerDiv.appendChild(newTokenDistanceCondition.getHtml());
    };
    RegExTokenDistanceConditionList.prototype.addItem = function () {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        var newTokenDistanceCondition = new RegExTokenDistanceCondition(this);
        newTokenDistanceCondition.makeRegExItemCondition();
        newTokenDistanceCondition.setClickableDelimeter();
        this.conditionInputArray.push(newTokenDistanceCondition);
        this.tokenDistanceListContainerDiv.appendChild(newTokenDistanceCondition.getHtml());
    };
    RegExTokenDistanceConditionList.prototype.removeItem = function (condition) {
        var _this = this;
        var index = this.conditionInputArray.indexOf(condition, 0);
        if (index != undefined) {
            var arrayItem = this.conditionInputArray[index];
            $(arrayItem.getHtml()).fadeToggle("slow", "linear", function () {
                _this.tokenDistanceListContainerDiv.removeChild(arrayItem.getHtml());
            });
            this.conditionInputArray.splice(index, 1);
        }
        if (this.conditionInputArray.length === 1) {
            this.conditionInputArray[0].setClickableDelimeter();
        }
        if (this.conditionInputArray.length === 0) {
            this.resetItems();
        }
    };
    return RegExTokenDistanceConditionList;
})();
var RegExTokenDistanceCondition = (function () {
    function RegExTokenDistanceCondition(parent) {
        this.minTokenDistanceValue = 1;
        this.maxTokenDistanceValue = 100;
        this.initTokenDistanceValue = 1;
        this.delimeterClass = "regexsearch-token-distance-or-delimiter";
        this.parent = parent;
    }
    RegExTokenDistanceCondition.prototype.importData = function (conditionData) {
        this.firstToken.importData(conditionData.first);
        this.secondToken.importData(conditionData.second);
        $(this.tokenDistanceInput).val(conditionData.distance.toString());
        $(this.tokenDistanceInput).keyup();
        $(this.tokenDistanceInput).change();
    };
    RegExTokenDistanceCondition.prototype.getHtml = function () {
        return this.html;
    };
    RegExTokenDistanceCondition.prototype.removeDelimeter = function () {
        $(this.html).find("." + this.delimeterClass).empty();
    };
    RegExTokenDistanceCondition.prototype.hasDelimeter = function () {
        var isEmpty = $(this.html).find("." + this.delimeterClass).is(":empty");
        return !isEmpty;
    };
    RegExTokenDistanceCondition.prototype.setTextDelimeter = function () {
        var textDelimeter = this.createTextDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find("." + this.delimeterClass).append(textDelimeter);
    };
    RegExTokenDistanceCondition.prototype.setClickableDelimeter = function () {
        var clickableDelimeter = this.createClickableDelimeter();
        if (this.hasDelimeter()) {
            this.removeDelimeter();
        }
        $(this.html).find("." + this.delimeterClass).append(clickableDelimeter);
    };
    RegExTokenDistanceCondition.prototype.createClickableDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        var addWordSpan = document.createElement("span");
        $(addWordSpan).addClass("regex-clickable-text");
        addWordSpan.innerHTML = "+ Nebo";
        $(addWordSpan).click(function () {
            _this.parent.addItem();
        });
        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass(this.delimeterClass);
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeItem(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
    };
    RegExTokenDistanceCondition.prototype.createTextDelimeter = function () {
        var _this = this;
        var delimeterDiv = document.createElement("div");
        delimeterDiv.innerHTML = "Nebo";
        $(delimeterDiv).addClass(this.delimeterClass);
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeItem(_this);
        });
        delimeterDiv.appendChild(trashButton);
        return delimeterDiv;
    };
    RegExTokenDistanceCondition.prototype.makeRegExItemCondition = function () {
        var _this = this;
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
        inputTextSpan.innerHTML = "Vzdálenost: ";
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
        $(tokenDistanceInput).keyup(function (e) {
            var value = $(e.target).val();
            value.replace(/[^0-9]/g, '');
            $(e.target).val(value);
            $(e.target).text(value);
            _this.actualTokenDistanceValue = parseInt(value);
        });
        $(tokenDistanceInput).change(function (e) {
            var value = $(e.target).val();
            _this.actualTokenDistanceValue = parseInt(value);
        });
        this.tokenDistanceInput = tokenDistanceInput;
        mainDiv.appendChild(inputTextDiv);
        mainDiv.appendChild(this.secondToken.getHtml());
        mainDiv.appendChild(this.createTextDelimeter());
        this.html = mainDiv;
    };
    RegExTokenDistanceCondition.prototype.getConditionItemValue = function () {
        var tokenDistanceCriteriaDescription = new TokenDistanceCriteriaDescription();
        tokenDistanceCriteriaDescription.first = this.firstToken.getConditionItemValue();
        tokenDistanceCriteriaDescription.second = this.secondToken.getConditionItemValue();
        tokenDistanceCriteriaDescription.distance = this.actualTokenDistanceValue;
        return tokenDistanceCriteriaDescription;
    };
    return RegExTokenDistanceCondition;
})();
//  Classes for storing data
var ConditionResult = (function () {
    function ConditionResult() {
    }
    return ConditionResult;
})();
var ConditionItemResult = (function () {
    function ConditionItemResult() {
    }
    return ConditionItemResult;
})();
var DatingCriteriaListDescription = (function (_super) {
    __extends(DatingCriteriaListDescription, _super);
    function DatingCriteriaListDescription() {
        _super.call(this);
        this.conditions = new Array();
    }
    return DatingCriteriaListDescription;
})(ConditionResult);
var DatingCriteriaDescription = (function (_super) {
    __extends(DatingCriteriaDescription, _super);
    function DatingCriteriaDescription() {
        _super.apply(this, arguments);
    }
    return DatingCriteriaDescription;
})(ConditionItemResult);
var WordsCriteriaListDescription = (function (_super) {
    __extends(WordsCriteriaListDescription, _super);
    function WordsCriteriaListDescription() {
        _super.call(this);
        this.conditions = new Array();
    }
    return WordsCriteriaListDescription;
})(ConditionResult);
var WordCriteriaDescription = (function (_super) {
    __extends(WordCriteriaDescription, _super);
    function WordCriteriaDescription() {
        _super.call(this);
        this.contains = new Array();
    }
    return WordCriteriaDescription;
})(ConditionItemResult);
var TokenDistanceCriteriaListDescription = (function (_super) {
    __extends(TokenDistanceCriteriaListDescription, _super);
    function TokenDistanceCriteriaListDescription() {
        _super.call(this);
        this.conditions = new Array();
    }
    return TokenDistanceCriteriaListDescription;
})(ConditionResult);
var TokenDistanceCriteriaDescription = (function (_super) {
    __extends(TokenDistanceCriteriaDescription, _super);
    function TokenDistanceCriteriaDescription() {
        _super.apply(this, arguments);
    }
    return TokenDistanceCriteriaDescription;
})(ConditionItemResult);
var WordInputTypeEnum;
(function (WordInputTypeEnum) {
    WordInputTypeEnum[WordInputTypeEnum["StartsWith"] = 0] = "StartsWith";
    WordInputTypeEnum[WordInputTypeEnum["Contains"] = 1] = "Contains";
    WordInputTypeEnum[WordInputTypeEnum["EndsWith"] = 2] = "EndsWith";
})(WordInputTypeEnum || (WordInputTypeEnum = {}));
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
var SearchTypeEnum;
(function (SearchTypeEnum) {
    SearchTypeEnum[SearchTypeEnum["Author"] = 0] = "Author";
    SearchTypeEnum[SearchTypeEnum["Title"] = 1] = "Title";
    SearchTypeEnum[SearchTypeEnum["Editor"] = 2] = "Editor";
    SearchTypeEnum[SearchTypeEnum["Dating"] = 3] = "Dating";
    SearchTypeEnum[SearchTypeEnum["Fulltext"] = 4] = "Fulltext";
    SearchTypeEnum[SearchTypeEnum["Heading"] = 5] = "Heading";
    SearchTypeEnum[SearchTypeEnum["Sentence"] = 6] = "Sentence";
    SearchTypeEnum[SearchTypeEnum["Result"] = 7] = "Result";
    SearchTypeEnum[SearchTypeEnum["ResultRestriction"] = 8] = "ResultRestriction";
    SearchTypeEnum[SearchTypeEnum["TokenDistance"] = 9] = "TokenDistance";
})(SearchTypeEnum || (SearchTypeEnum = {}));
/*
 * ConditionTypeEnum must match with ConditionTypeEnum number values in C#
        WordList = 0,
        DatingList = 1,
        TokenDistanceList = 2,
 *
 */
var ConditionTypeEnum;
(function (ConditionTypeEnum) {
    ConditionTypeEnum[ConditionTypeEnum["WordList"] = 0] = "WordList";
    ConditionTypeEnum[ConditionTypeEnum["DatingList"] = 1] = "DatingList";
    ConditionTypeEnum[ConditionTypeEnum["TokenDistanceList"] = 2] = "TokenDistanceList";
})(ConditionTypeEnum || (ConditionTypeEnum = {}));
var DatingPrecisionEnum;
(function (DatingPrecisionEnum) {
    DatingPrecisionEnum[DatingPrecisionEnum["Year"] = 0] = "Year";
    DatingPrecisionEnum[DatingPrecisionEnum["Period"] = 1] = "Period";
})(DatingPrecisionEnum || (DatingPrecisionEnum = {}));
var DatingRangeEnum;
(function (DatingRangeEnum) {
    DatingRangeEnum[DatingRangeEnum["OlderThen"] = 0] = "OlderThen";
    DatingRangeEnum[DatingRangeEnum["YoungerThen"] = 1] = "YoungerThen";
    DatingRangeEnum[DatingRangeEnum["Between"] = 2] = "Between";
    DatingRangeEnum[DatingRangeEnum["Around"] = 3] = "Around";
})(DatingRangeEnum || (DatingRangeEnum = {}));
//# sourceMappingURL=itjakub.plugins.regexsearch.js.map