class NewFavoriteDialog {
    private allowMultipleLabels: boolean;
    private favoriteManager: FavoriteManager;
    private container: HTMLDivElement;
    private onSaveCallback: (data: INewFavoriteItemData) => void;
    private labelColorInput: ColorInput;
    private activeTabClass: string;
    
    constructor(favoriteManager: FavoriteManager, allowMultipleLabels: boolean) {
        this.allowMultipleLabels = allowMultipleLabels;
        this.favoriteManager = favoriteManager;
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
        $(".modal-title", this.container).text("Přiřadit štítky k vybrané položce");

        var saveIcon = document.createElement("span");
        var saveTitle = document.createElement("span");
        $(saveIcon)
            .addClass("glyphicon")
            .addClass("glyphicon-star");
        $(saveTitle)
            .text("Potvrdit přiřazení štítků");

        $(".close-button", this.container)
            .addClass("hidden");

        $(".save-button", this.container)
            .empty()
            .append(saveIcon)
            .append(saveTitle)
            .click(this.onSaveButtonClick.bind(this));
    }

    public show(itemName: string) {
        var queryParameters = {
            itemName: itemName
        };
        var queryString = $.param(queryParameters);
        var url = getBaseUrl() + "Favorite/NewFavorite?" + queryString;

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
    }

    private finishInnerInitialization() {
        $(".modal-body", this.container).removeClass("loading");
        $(".no-label-info", this.container).hide();

        if (!this.allowMultipleLabels) {
            $("[name=favorite-label]", this.container)
                .attr("type", "radio")
                .parent().css("padding-left", "27px");
        }

        $("[name=favorite-label]", this.container).change(event => {
            var checkbox = <HTMLInputElement>event.target;
            var checkboxJQuery = $(checkbox);
            var labelId = checkboxJQuery.val();

            if (!this.allowMultipleLabels) {
                $(".favorite-selected-label-info", this.container).empty();
            }
            if (!checkbox.checked) {
                $(".favorite-selected-label-info [data-id=" + labelId + "]", this.container).remove();
                this.checkSelectedItems();
                return;
            }

            var selectedLabelName = checkboxJQuery.data("name");
            var selectedLabelColor = checkboxJQuery.data("color");
            var color = new HexColor(selectedLabelColor);
            var fontColor = FavoriteHelper.getDefaultFontColor(color);
            var borderColor = FavoriteHelper.getDefaultBorderColor(color);

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

        $("[name=favorite-label]:checked", this.container).trigger("change");

        $(".favorite-select-label-item", this.container).each((index, element) => {
            var backgroundColor = $("input", element).data("color");
            var color = new HexColor(backgroundColor);
            var fontColor = FavoriteHelper.getDefaultFontColor(color);
            var borderColor = FavoriteHelper.getDefaultBorderColor(color);

            $(element)
                .css("color", fontColor)
                .css("border-color", borderColor);
        });

        $(".nav-tabs a", this.container).click((event) => {
            $(".nav-tabs li, .tab-pane").removeClass("active");
            var navLinkJQuery = $(event.currentTarget);
            var tabClass = navLinkJQuery.data("tab-class");
            this.activeTabClass = tabClass;

            navLinkJQuery.closest("li").addClass("active");
            $("." + tabClass, this.container).addClass("active");
        });

        $(".favorite-label-filter", this.container).on("change keyup paste", (event) => {
            var filter = $(event.currentTarget).val().toLocaleLowerCase();
            var isAnyVisible = false;
            $(".favorite-select-label .radio").each((index, element) => {
                var name = String($("input", element).data("name")).toLocaleLowerCase();
                if (name.indexOf(filter) !== -1) {
                    $(element).show();
                    isAnyVisible = true;
                } else {
                    $(element).hide();
                }
            });

            if (isAnyVisible) {
                $(".no-label-info").hide();
            } else {
                $(".no-label-info").show();
            }
        });

        this.labelColorInput = new ColorInput($("#favorite-label-color"), $("#favorite-label-color-button"));
        this.labelColorInput.make();
    }

    private checkSelectedItems() {
        var labelsJQuery = $(".favorite-selected-label-info .label", this.container);
        if (labelsJQuery.length === 0) {
            var emptyLabel = document.createElement("span");
            $(emptyLabel)
                .addClass("label")
                .addClass("label-default")
                .text("Žádný štítek");
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
        var itemName = $("#favorite-name-2").val();
        var labelName = $("#favorite-label-name").val();
        var color = this.labelColorInput.getValue();

        var error = "";
        if (!labelName) {
            error = "Nebylo zadáno jméno.";
        }
        if (!FavoriteHelper.isValidHexColor(color)) {
            error += " Nesprávný formát barvy (požadovaný formát: #000000).";
        }
        if (error.length > 0) {
            this.showError(error);
            return;
        }

        this.favoriteManager.createFavoriteLabel(labelName, color, (id, error) => {
            if (error) {
                this.showError("Chyba při vytváření nového štítku");
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
        var itemName: string = $("#favorite-name").val();
        var labels = new Array<INewFavoriteItemDataLabel>();

        $("[name=favorite-label]:checked", this.container).each((index, element) => {
            var elementJQuery = $(element);
            var label: INewFavoriteItemDataLabel = {
                labelId: elementJQuery.val(),
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

    constructor(inputElement: JQuery, buttonElement: JQuery) {
        this.buttonElement = buttonElement;
        this.inputElement = inputElement;
    }

    public make() {
        var elements = $(this.inputElement).add(this.buttonElement);
        elements.colorpickerplus();
        elements.on("changeColor", (event, color) => {
            if (color == null) {
                color = "#FFFFFF";
            }

            this.setValue(color);
        });

        this.inputElement.change(() => this.updateBackground());

        // hack fix input type recognition in Color Picker Plus
        this.buttonElement.data("colorpickerplus").hasInput = () => false;

        // disable saving custom colors:
        $(".colorpickerplus-container button").remove();
        if (window.localStorage) {
            window.localStorage.removeItem("colorpickerplus_custom_colors");
        }
    }

    public setValue(value: string) {
        this.inputElement.val(value);
        this.buttonElement.data("cpp-color", value);
        this.updateBackground();
    }

    public getValue(): string {
        return this.inputElement.val();
    }

    private updateBackground() {
        var value = this.inputElement.val();
        if (value.length !== 7) {
            value = "#FFFFFF";
        }

        this.inputElement.css("background-color", value);
        this.inputElement.css("color", FavoriteHelper.getFontColor(value));
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