class NewFavoriteDialog {
    private static increaseBackgroundColorPercent = 80;
    private allowMultipleLabels: boolean;
    private favoriteManager: FavoriteManager;
    private container: HTMLDivElement;
    private onSaveCallback: (data: INewFavoriteItemData) => void;
    private labelColorInput: ColorInput;
    private activeTabClass: string;
    private saveTitle: HTMLSpanElement;
    private selectedRadioButton: HTMLInputElement;
    private isInitialized;
    private pendingShow: boolean|string;

	private localizationScope = "FavoriteJs";
    
    constructor(favoriteManager: FavoriteManager, allowMultipleLabels: boolean) {
        this.allowMultipleLabels = allowMultipleLabels;
        this.favoriteManager = favoriteManager;
        this.isInitialized = false;
        this.pendingShow = false;
    }

    public setSaveCallback(value: (data: INewFavoriteItemData) => void) {
        this.onSaveCallback = value;
    }

    public make() {
        this.container = document.createElement("div");
        $("body").append(this.container);

        var url = getBaseUrl() + "Favorite/Dialog";
        $(this.container).load(url, null, (responseTxt, statusTxt, xhr) => {
            if (statusTxt === "success") {
                this.finishInitialization();
            }
        });
    }

    private finishInitialization() {
        var dialogHeading = this.allowMultipleLabels
            ? localization.translate("AttachNewTags", this.localizationScope).value
            : localization.translate("AttachNewTag", this.localizationScope).value;
        $(".modal-title", this.container).text(dialogHeading);

        var saveIcon = document.createElement("span");
        var saveTitle = document.createElement("span");
        $(saveIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-star");
        this.saveTitle = saveTitle;

        $(".close-button", this.container)
            .addClass("hidden");

        $(".save-button", this.container)
            .empty()
            .append(saveIcon)
            .append(saveTitle)
            .click(this.onSaveButtonClick.bind(this));

        this.isInitialized = true;
        if (this.pendingShow !== false) {
            this.show(<string>this.pendingShow);
        }
    }

    public show(itemName: string) {
        if (!this.isInitialized) {
            this.pendingShow = itemName;
            return;
        }
        var queryParameters = {
            itemName: itemName
        };
        var queryString = $.param(queryParameters);
        var url = getBaseUrl() + "Favorite/NewFavorite?" + queryString;

        $(this.saveTitle)
            .text(localization.translate("Confirm", this.localizationScope).value);
        $(".modal-body", this.container)
            .addClass("loading")
            .empty();
        $(".modal-body", this.container).load(url, null, (responseTxt, statusTxt, xhr) => {
            if (statusTxt === "success") {
                this.finishInnerInitialization();
            }
        });

        $(".error", this.container)
            .addClass("hidden")
            .text("");
        $(".saving-icon", this.container)
            .addClass("hidden");

        $(".modal", this.container).modal({
            show: true,
            backdrop: "static"
        });
        this.pendingShow = false;
    }

    private finishInnerInitialization() {
        $(".modal-body", this.container).removeClass("loading");
        $(".no-label-info", this.container).hide();

        if (!this.allowMultipleLabels) {
            $("[name=favorite-label]", this.container)
                .attr("type", "radio")
                .parent().css("padding-left", "27px");

            var defaultLabel = $("[data-isdefault=true]", this.container);
            if (defaultLabel.length > 0) {
                var radioButton = defaultLabel[0] as Node as HTMLInputElement;
                radioButton.checked = true;
                this.selectedRadioButton = radioButton;
            } else {
                this.selectedRadioButton = null;
            }
        }

        $("[name=favorite-label]", this.container).change(event => {
            var checkbox = event.target as Node as HTMLInputElement;
            var checkboxJQuery = $(checkbox);
            var labelId = checkboxJQuery.val() as string;

            if (!this.allowMultipleLabels) {
                var $radioButton = $(this.selectedRadioButton);
                let backgroundColor = $radioButton.data("color");
                let color = new HexColor(backgroundColor);
                this.updateCheckboxColor($(this.selectedRadioButton), this.selectedRadioButton.checked, color);
                $(".favorite-selected-label-info", this.container).empty();

                this.selectedRadioButton = checkbox;
            }

            var selectedLabelName = checkboxJQuery.data("name");
            var selectedLabelColor = checkboxJQuery.data("color");
            var color = new HexColor(selectedLabelColor);
            var fontColor = FavoriteHelper.getDefaultFontColor(color);
            var borderColor = FavoriteHelper.getDefaultBorderColor(color);

            if (!checkbox.checked) {
                $(".favorite-selected-label-info [data-id=" + labelId + "]", this.container).remove();
                this.updateCheckboxColor(checkboxJQuery, checkbox.checked, color);
                this.checkSelectedItems();
                return;
            }
            
            this.updateCheckboxColor(checkboxJQuery, checkbox.checked, color);

            var newLabelSpan = document.createElement("span");
            $(newLabelSpan)
                .attr("data-id", labelId)
                .text(selectedLabelName)
                .addClass("label")
                .css("background-color", selectedLabelColor)
                .css("color", fontColor)
                .css("border-color", borderColor);

            $(".favorite-selected-label-info", this.container)
                .append(newLabelSpan);
            this.checkSelectedItems();
        });

        $(".favorite-select-label-item", this.container).each((index, element) => {
            var backgroundColor = $("input", element as Node as Element).data("color");
            var color = new HexColor(backgroundColor);
            var borderColor = FavoriteHelper.getDefaultBorderColor(color);
            var inactiveBackground = color.getIncreasedHexColor(NewFavoriteDialog.increaseBackgroundColorPercent);
            var inactiveBorder = new HexColor(borderColor).getIncreasedHexColor(NewFavoriteDialog.increaseBackgroundColorPercent);

            $(element as Node as Element)
                .css("color", FavoriteHelper.getInactiveFontColor())
                .css("border-color", inactiveBorder)
                .css("background-color", inactiveBackground);
        });

        $("[name=favorite-label]:checked", this.container).trigger("change");

        this.setActiveTab("tab-favorite-label-assign");
        $(".nav-tabs a", this.container).click((event) => {
            $(".nav-tabs li, .tab-pane").removeClass("active");
            var navLinkJQuery = $(event.currentTarget as Node as Element);
            var tabClass = navLinkJQuery.data("tab-class");
            
            this.setActiveTab(tabClass);
        });

        $(".favorite-label-filter", this.container).on("change keyup paste", (event) => {
            var filter = ($(event.currentTarget as Node as Element).val() as string).toLocaleLowerCase();
            var isAnyVisible = false;
            $(".favorite-select-label .radio").each((index, element) => {
                var name = String($("input", element as Node as Element).data("name")).toLocaleLowerCase();
                if (name.indexOf(filter) !== -1) {
                    $(element as Node as Element).show();
                    isAnyVisible = true;
                } else {
                    $(element as Node as Element).hide();
                }
            });

            if (isAnyVisible) {
                $(".no-label-info").hide();
            } else {
                $(".no-label-info").show();
            }
        });

        $(".favorite-label-name", this.container).change(this.updateLabelPreview.bind(this));

        var $favoriteLabelColor = $(".favorite-label-color", this.container);
        var $favoriteLabelColorButton = $(".favorite-label-color-button", this.container);
        this.labelColorInput = new ColorInput($favoriteLabelColor, $favoriteLabelColorButton);
        this.labelColorInput.make();
        this.labelColorInput.setColorChangedCallback(this.updateLabelPreview.bind(this));
    }

    private setActiveTab(tabClass: string) {
        this.activeTabClass = tabClass;

        $("." + tabClass, this.container).addClass("active");
        $(`[data-tab-class=${tabClass}]`, this.container).closest("li").addClass("active");

        var newSaveTitle: string;
        switch (tabClass) {
            case "tab-favorite-label-assign":
                newSaveTitle = this.allowMultipleLabels ? localization.translate("ConfirmTagsAttachment", this.localizationScope).value : 
                                                          localization.translate("ConfirmTagAttachment", this.localizationScope).value;
                break;
            case "tab-favorite-label-create":
                newSaveTitle = localization.translate("CreateAndAttachTag", this.localizationScope).value;
                break;
            default:
                newSaveTitle = localization.translate("Save", this.localizationScope).value;
        }
        $(this.saveTitle).text(newSaveTitle);
    }

    private updateCheckboxColor(checkBoxJQuery: JQuery, isChecked: boolean, color: HexColor) {
        var fontColor: string;
        var borderColor: string;
        var backgroundColor: string;

        if (isChecked) {
            fontColor = FavoriteHelper.getDefaultFontColor(color);
            borderColor = FavoriteHelper.getDefaultBorderColor(color);
            backgroundColor = color.getColor();
        } else {
            fontColor = FavoriteHelper.getInactiveFontColor();
            backgroundColor = color.getIncreasedHexColor(NewFavoriteDialog.increaseBackgroundColorPercent);
            borderColor = FavoriteHelper.getDefaultBorderColor(color);
            borderColor = new HexColor(borderColor).getIncreasedHexColor(NewFavoriteDialog.increaseBackgroundColorPercent);
        }

        checkBoxJQuery.closest("label")
            .css("color", fontColor)
            .css("border-color", borderColor)
            .css("background-color", backgroundColor);
    }

    private updateLabelPreview() {
        var labelName = $(".favorite-label-name", this.container).val() as string;
        var hexColor = $(".favorite-label-color", this.container).val() as string;
        var color = new HexColor(hexColor);

        var $labelPreview = $(".label-preview", this.container);
        $labelPreview.text(labelName);

        if (color.isValidHexColor()) {
            $labelPreview
                .css("background-color", hexColor)
                .css("border-color", FavoriteHelper.getDefaultBorderColor(color))
                .css("color", FavoriteHelper.getDefaultFontColor(color));
        } else {
            $labelPreview
                .css("background-color", "#FFFFFF")
                .css("border-color", "#000000")
                .css("color", "#000000");
        }
    }

    private checkSelectedItems() {
        var labelsJQuery = $(".favorite-selected-label-info .label", this.container);
        if (labelsJQuery.length === 0) {
            var emptyLabel = document.createElement("span");
            $(emptyLabel)
                .addClass("label")
                .addClass("label-default")
                .text(localization.translate("NoTag", this.localizationScope).value);
            $(".favorite-selected-label-info", this.container)
                .append(emptyLabel);
        } else {
            var emptyLabelJQuery = $(".favorite-selected-label-info .label-default");
            var isEmptyLabelExists = emptyLabelJQuery.length > 0;
            var isAnyLabel = $("[name=favorite-label]:checked", this.container).length > 0;

            if (isAnyLabel && isEmptyLabelExists) {
                emptyLabelJQuery.remove();
            }
        }
    }

    public hide() {
        $(".modal", this.container).modal("hide");
    }

    public showError(text: string) {
        $(".saving-icon", this.container).addClass("hidden");
        $(".error", this.container)
            .removeClass("hidden")
            .text(text);
    }

    private createFavoriteLabel() {
        var itemName = $(".favorite-name-2", this.container).val() as string;
        var labelName = $(".favorite-label-name", this.container).val() as string;
        var color = this.labelColorInput.getValue();

        var error = "";
        if (!labelName) {
            error = localization.translate("MissingName", this.localizationScope).value;
        }
        if (!FavoriteHelper.isValidHexColor(color)) {
            error += localization.translate("IncorrectColorFormat", this.localizationScope).value;
        }
        if (error.length > 0) {
            this.showError(error);
            return;
        }

        this.favoriteManager.createFavoriteLabel(labelName, color, (id, error) => {
            if (error) {
                this.showError(localization.translate("CreateNewTagError", this.localizationScope).value);
                return;
            }

            if (this.onSaveCallback) {
                var resultLabel: INewFavoriteItemDataLabel = {
                    labelId: id,
                    labelName: labelName,
                    labelColor: color
                };
                var resultData: INewFavoriteItemData = {
                    itemName: itemName,
                    labels: [resultLabel]
                };
                this.onSaveCallback(resultData);
            }
        });
    }

    private onSaveButtonClick() {
        $(".saving-icon", this.container).removeClass("hidden");
        $(".error", this.container).addClass("hidden");

        if (this.activeTabClass === "tab-favorite-label-create") {
            this.createFavoriteLabel();
            return;
        }

        if (this.onSaveCallback) {
            var resultData = this.getData();
            this.onSaveCallback(resultData);
        }
    }

    private getData(): INewFavoriteItemData {
        var itemName: string = $(".favorite-name", this.container).val() as string;
        var labels = new Array<INewFavoriteItemDataLabel>();

        $("[name=favorite-label]:checked", this.container).each((index, element) => {
            var elementJQuery = $(element as Node as Element);
            var label: INewFavoriteItemDataLabel = {
                labelId: parseInt(elementJQuery.val() as string),
                labelName: elementJQuery.data("name"),
                labelColor: elementJQuery.data("color")
            };
            labels.push(label);
        });

        var resultData: INewFavoriteItemData = {
            itemName: itemName,
            labels: labels
        };
        return resultData;
    }
}

class ColorInput {
    private buttonElement: JQuery;
    private inputElement: JQuery;
    private colorChangedCallback: (color: string) => void;

    constructor(inputElement: JQuery, buttonElement: JQuery) {
        this.buttonElement = buttonElement;
        this.inputElement = inputElement;
    }

    public make() {
        var elements = $(this.inputElement).add(this.buttonElement);
        elements.colorpickerplus({
            storeCustomColors: false
        });
        elements.on("changeColor", (event, color) => {
            if (color == null) {
                color = "#FFFFFF";
            }

            this.setValue(color);
        });

        this.inputElement.change(() => this.setValue(this.inputElement.val() as string));
    }

    public setValue(value: string) {
        this.inputElement.val(value);
        this.buttonElement.data("cpp-color", value);
        this.updateBackground();

        if (this.colorChangedCallback) {
            this.colorChangedCallback(value);
        }
    }

    public getValue(): string {
        return this.inputElement.val() as string;
    }

    private updateBackground() {
        var value = this.inputElement.val() as string;
        if (value.length !== 7) {
            value = "#FFFFFF";
        }

        this.inputElement.css("background-color", value);
        this.inputElement.css("color", FavoriteHelper.getFontColor(value));
    }

    public setColorChangedCallback(colorChangedCallback: (color: string) => void) {
        this.colorChangedCallback = colorChangedCallback;
    }
}

class NewFavoriteDialogProvider {
    private static dialogs = new DictionaryWrapper<NewFavoriteDialog>();

    constructor() {
        throw Error("Could not initialize static class");
    }

    public static getInstance(allowMultipleLabels: boolean): NewFavoriteDialog {
        var key = Number(allowMultipleLabels);
        var dialog = this.dialogs.get(key);
        if (!dialog) {
            dialog = new NewFavoriteDialog(new FavoriteManager(), true);
            dialog.make();
            this.dialogs.add(key, dialog);
        }
        return dialog;
    }
}

interface INewFavoriteItemData {
    itemName: string;
    labels: INewFavoriteItemDataLabel[];
}

interface INewFavoriteItemDataLabel {
    labelId: number;
    labelName: string;
    labelColor: string;
}