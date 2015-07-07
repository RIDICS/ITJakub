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
        var newRegExConditions = new RegExConditionListItem(this);
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
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
            },
            error: function (response) {
            }
        });
    };
    return RegExSearch;
})(RegExSearchBase);
var RegExConditionListItem = (function (_super) {
    __extends(RegExConditionListItem, _super);
    function RegExConditionListItem(parent) {
        _super.call(this);
        this.parent = parent;
    }
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
        searchDestinationSelect.appendChild(this.createOption("Text", 4 /* Text */.toString()));
        searchDestinationSelect.appendChild(this.createOption("Autor", 0 /* Author */.toString()));
        searchDestinationSelect.appendChild(this.createOption("Titul", 1 /* Title */.toString()));
        searchDestinationSelect.appendChild(this.createOption("Editor", 2 /* Responsible */.toString()));
        searchDestinationSelect.appendChild(this.createOption("Období vzniku", 3 /* Dating */.toString()));
        this.selectedSearchType = 4 /* Text */;
        $(searchDestinationSelect).change(function (eventData) {
            var oldSelectedSearchType = _this.selectedSearchType;
            _this.selectedSearchType = parseInt($(eventData.target).val());
            if (_this.selectedSearchType !== oldSelectedSearchType) {
                _this.changeConditionType(_this.selectedSearchType, oldSelectedSearchType);
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
    };
    RegExConditionListItem.prototype.changeConditionType = function (newSearchType, oldSearchType) {
        if (this.innerCondition instanceof RegExWordConditionList && newSearchType === 3 /* Dating */) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExDatingCondition(this);
            this.innerCondition.makeRegExCondition(this.innerConditionContainer);
        }
        else if (this.innerCondition instanceof RegExDatingCondition && newSearchType !== 3 /* Dating */) {
            $(this.innerConditionContainer).empty();
            this.innerCondition = new RegExWordConditionList(this);
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
        return conditionResult;
    };
    return RegExConditionListItem;
})(RegExSearchBase);
var RegExConditionBase = (function (_super) {
    __extends(RegExConditionBase, _super);
    function RegExConditionBase(parent) {
        _super.call(this);
        this.parentRegExConditionList = parent;
    }
    RegExConditionBase.prototype.makeRegExCondition = function (conditionContainerDiv) {
    };
    RegExConditionBase.prototype.getConditionValue = function () {
        return null;
    };
    return RegExConditionBase;
})(RegExSearchBase);
var RegExWordConditionList = (function (_super) {
    __extends(RegExWordConditionList, _super);
    function RegExWordConditionList(parent) {
        _super.call(this, parent);
        this.wordFormType = {
            Lemma: "lemma",
            HyperlemmaNew: "hyperlemma-new",
            HyperlemmaOld: "hyperlemma-old",
            Stemma: "stemma"
        };
    }
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
        wordFormSelect.appendChild(this.createOption("Lemma", this.wordFormType.Lemma));
        wordFormSelect.appendChild(this.createOption("Hyperlemma - nové", this.wordFormType.HyperlemmaNew));
        wordFormSelect.appendChild(this.createOption("Hyperlemma - staré", this.wordFormType.HyperlemmaOld));
        wordFormSelect.appendChild(this.createOption("Stemma", this.wordFormType.Stemma));
        this.selectedWordFormType = this.wordFormType.Lemma;
        $(wordFormSelect).change(function (eventData) {
            _this.selectedWordFormType = $(eventData.target).val();
        });
        this.wordListContainerDiv = document.createElement("div");
        $(this.wordListContainerDiv).addClass("regexsearch-condition-list-div");
        conditionContainerDiv.appendChild(this.wordListContainerDiv);
        this.resetWords();
    };
    RegExWordConditionList.prototype.getConditionValue = function () {
        var criteriaDescriptions = new WordsCriteriaListDescription();
        for (var i = 0; i < this.conditionInputArray.length; i++) {
            var regExWordCondition = this.conditionInputArray[i];
            criteriaDescriptions.wordCriteriaDescription.push(regExWordCondition.getConditionsValue());
        }
        return criteriaDescriptions;
    };
    RegExWordConditionList.prototype.resetWords = function () {
        $(this.wordListContainerDiv).empty();
        this.conditionInputArray = [];
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExWordCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.wordListContainerDiv.appendChild(newWordCondition.getHtml());
    };
    RegExWordConditionList.prototype.addWord = function () {
        this.conditionInputArray[this.conditionInputArray.length - 1].setTextDelimeter();
        var newWordCondition = new RegExWordCondition(this);
        newWordCondition.makeRegExWordCondition();
        newWordCondition.setClickableDelimeter();
        this.conditionInputArray.push(newWordCondition);
        this.wordListContainerDiv.appendChild(newWordCondition.getHtml());
    };
    RegExWordConditionList.prototype.removeWord = function (condition) {
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
    };
    return RegExWordConditionList;
})(RegExConditionBase);
var RegExDatingConditionRangePeriodView = (function () {
    function RegExDatingConditionRangePeriodView() {
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
        for (var century = 8; century <= 21; century++) {
            centuryArray.push(new DatingSliderValue(century.toString(), century * 100 - 100, century * 100 - 1)); //calculate century low and high values (i.e 18. century is 1700 - 1799)
        }
        var sliderCentury = this.makeSlider(centuryArray, ". století", function (selectedValue) {
            _this.centuryChanged(selectedValue);
        });
        centurySliderDiv.appendChild(sliderCentury);
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
            _this.refreshDisplayedDate();
        });
        var periodNameSpan = window.document.createElement("span");
        periodNameSpan.innerHTML = "Přibližná doba";
        periodCheckboxDiv.appendChild(periodValueCheckbox);
        periodCheckboxDiv.appendChild(periodNameSpan);
        periodSliderDiv.appendChild(periodCheckboxDiv);
        precisionInpuDiv.appendChild(periodSliderDiv);
        var sliderPeriod = this.makeSlider(new Array(new DatingSliderValue("začátek", 0, -85), new DatingSliderValue("čtvrtina", 0, -75), new DatingSliderValue("třetina", 0, -66), new DatingSliderValue("polovina", 0, -50), new DatingSliderValue("konec", 85, 0)), "", function (selectedValue) {
            _this.periodChanged(selectedValue);
        });
        $(sliderPeriod).slider("option", "disabled", true);
        $(sliderPeriod).parent().siblings(".slider").find(".slider-tip").hide();
        periodSliderDiv.appendChild(sliderPeriod);
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
            _this.refreshDisplayedDate();
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
        var sliderDecades = this.makeSlider(decadesArray, ". léta", function (selectedValue) {
            _this.decadeChanged(selectedValue);
        });
        $(sliderDecades).slider("option", "disabled", true);
        $(sliderDecades).parent().siblings(".slider").find(".slider-tip").hide();
        decadesSliderDiv.appendChild(sliderDecades);
        var datingDisplayedValueDiv = document.createElement('div');
        $(datingDisplayedValueDiv).addClass("regex-dating-condition-displayed-value");
        this.dateDisplayDiv = datingDisplayedValueDiv;
        precisionInpuDiv.appendChild(datingDisplayedValueDiv);
        this.refreshDisplayedDate();
    };
    RegExDatingConditionRangePeriodView.prototype.centuryChanged = function (sliderValue) {
        this.selectedCenturyLowerValue = sliderValue.lowNumberValue;
        this.selectedCenturyHigherValue = sliderValue.highNumberValue;
        this.refreshDisplayedDate();
    };
    RegExDatingConditionRangePeriodView.prototype.periodChanged = function (sliderValue) {
        this.selectedPeriodLowerValue = sliderValue.lowNumberValue;
        this.selectedPeriodHigherValue = sliderValue.highNumberValue;
        this.refreshDisplayedDate();
    };
    RegExDatingConditionRangePeriodView.prototype.decadeChanged = function (sliderValue) {
        this.selectedDecadeLowerValue = sliderValue.lowNumberValue;
        this.selectedDecadeHigherValue = sliderValue.highNumberValue;
        this.refreshDisplayedDate();
    };
    RegExDatingConditionRangePeriodView.prototype.refreshDisplayedDate = function () {
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
    return RegExDatingConditionRangePeriodView;
})();
var RegExDatingConditionRangeYearView = (function () {
    function RegExDatingConditionRangeYearView() {
    }
    RegExDatingConditionRangeYearView.prototype.makeRangeView = function (container) {
        var precisionInpuDiv = container;
        var textInput = document.createElement("input");
        textInput.type = "number";
        textInput.min = "800";
        textInput.max = "2100";
        textInput.value = "800";
        textInput.id = "800";
        // allows only digits input
        $(textInput).keyup(function (e) {
            var value = $(this).val();
            value.replace(/[^0-9]/g, '');
            $(this).val(value);
            $(this).text(value);
        });
        var spanInput = document.createElement("span");
        $(spanInput).addClass("regex-dating-input-span");
        spanInput.innerHTML = "Rok:";
        precisionInpuDiv.appendChild(spanInput);
        precisionInpuDiv.appendChild(textInput);
    };
    return RegExDatingConditionRangeYearView;
})();
var RegExDatingCondition = (function (_super) {
    __extends(RegExDatingCondition, _super);
    function RegExDatingCondition(parent) {
        _super.call(this, parent);
    }
    RegExDatingCondition.prototype.makeRegExCondition = function (conditionContainerDiv) {
        var datingDiv = document.createElement('div');
        $(datingDiv).addClass("regex-dating-condition");
        datingDiv.appendChild(this.makeTopSelectBoxes());
        var precisionInpuDiv = window.document.createElement("div");
        $(precisionInpuDiv).addClass("regex-dating-precision-div");
        this.precisionInpuDiv = precisionInpuDiv;
        datingDiv.appendChild(precisionInpuDiv);
        this.makePeriodInputRange();
        $(conditionContainerDiv).append(datingDiv);
    };
    RegExDatingCondition.prototype.makeYearInputRange = function () {
        new RegExDatingConditionRangeYearView().makeRangeView(this.precisionInpuDiv);
    };
    RegExDatingCondition.prototype.makePeriodInputRange = function () {
        new RegExDatingConditionRangePeriodView().makeRangeView(this.precisionInpuDiv);
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
        datingFormSelect.appendChild(this.createOption("Starší než", 0 /* OlderThen */.toString()));
        datingFormSelect.appendChild(this.createOption("Mladší než", 1 /* YoungerThen */.toString()));
        datingFormSelect.appendChild(this.createOption("Mezi", 2 /* Between */.toString()));
        datingFormSelect.appendChild(this.createOption("Kolem", 3 /* Around */.toString()));
        $(datingFormSelect).change(function (eventData) {
            _this.datingRange = parseInt($(eventData.target).val());
            if (_this.datingRange === 2 /* Between */ && _this.secondDateView === null) {
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
        precisionFormSelect.appendChild(this.createOption("Období", 1 /* Period */.toString()));
        precisionFormSelect.appendChild(this.createOption("Rok", 0 /* Year */.toString()));
        $(precisionFormSelect).change(function (eventData) {
            var oldPrecision = _this.datingPrecision;
            _this.datingPrecision = parseInt($(eventData.target).val());
            if (oldPrecision !== _this.datingPrecision) {
                $(_this.precisionInpuDiv).empty();
                _this.makeInputRangeView();
            }
        });
        precisionSelectDiv.appendChild(precisionFormSelect);
        datingFormDiv.appendChild(datingSelectDiv);
        datingFormDiv.appendChild(precisionSelectDiv);
        return datingFormDiv;
    };
    RegExDatingCondition.prototype.makeInputRangeView = function () {
        if (this.datingPrecision === 1 /* Period */) {
            this.makePeriodInputRange();
        }
        else {
            this.makeYearInputRange();
        }
    };
    RegExDatingCondition.prototype.getConditionValue = function () {
        return null; //TODO
    };
    return RegExDatingCondition;
})(RegExConditionBase);
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
        this.parent = parent;
    }
    RegExWordCondition.prototype.getHtml = function () {
        return this.html;
    };
    RegExWordCondition.prototype.removeDelimeter = function () {
        $(this.html).find(".regexsearch-or-delimiter").empty();
    };
    RegExWordCondition.prototype.hasDelimeter = function () {
        var isEmpty = $(this.html).find(".regexsearch-or-delimiter").is(":empty");
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
            _this.parent.addWord();
        });
        delimeterDiv.appendChild(addWordSpan);
        $(delimeterDiv).addClass("regexsearch-or-delimiter");
        var trashButton = document.createElement("button");
        $(trashButton).addClass("regexsearch-delimiter-remove-button");
        var removeGlyph = document.createElement("span");
        $(removeGlyph).addClass("glyphicon glyphicon-trash regex-clickable-text");
        trashButton.appendChild(removeGlyph);
        $(trashButton).click(function () {
            _this.parent.removeWord(_this);
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
            _this.parent.removeWord(_this);
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
        conditionSelect.appendChild(this.createOption("Začíná na", 0 /* StartsWith */.toString()));
        //conditionSelect.appendChild(this.createOption("Nezačíná na", this.conditionType.NotStartsWith));
        conditionSelect.appendChild(this.createOption("Obsahuje", 1 /* Contains */.toString()));
        //conditionSelect.appendChild(this.createOption("Neobsahuje", this.conditionType.NotContains));
        conditionSelect.appendChild(this.createOption("Končí na", 2 /* EndsWith */.toString()));
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
})(RegExSearchBase);
var ConditionResult = (function () {
    function ConditionResult() {
    }
    return ConditionResult;
})();
var WordsCriteriaListDescription = (function (_super) {
    __extends(WordsCriteriaListDescription, _super);
    function WordsCriteriaListDescription() {
        _super.call(this);
        this.wordCriteriaDescription = new Array();
    }
    return WordsCriteriaListDescription;
})(ConditionResult);
var WordCriteriaDescription = (function () {
    function WordCriteriaDescription() {
        this.contains = new Array();
    }
    return WordCriteriaDescription;
})();
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
        [EnumMember] Text = 4
 *
 */
var SearchTypeEnum;
(function (SearchTypeEnum) {
    SearchTypeEnum[SearchTypeEnum["Author"] = 0] = "Author";
    SearchTypeEnum[SearchTypeEnum["Title"] = 1] = "Title";
    SearchTypeEnum[SearchTypeEnum["Responsible"] = 2] = "Responsible";
    SearchTypeEnum[SearchTypeEnum["Dating"] = 3] = "Dating";
    SearchTypeEnum[SearchTypeEnum["Text"] = 4] = "Text";
})(SearchTypeEnum || (SearchTypeEnum = {}));
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