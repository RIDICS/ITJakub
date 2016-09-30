class NewFavoriteDialog {
    private favoriteManager: FavoriteManager;
    private container: HTMLDivElement;
    private onSaveCallback: (data: INewFavoriteItemData) => void;
    private selectedLabelId: number;
    private selectedLabelName: string;
    private selectedLabelColor: string;
    
    constructor(favoriteManager: FavoriteManager) {
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
        $(".modal-title", this.container).text("Přidat novou oblíbenou položku");
        $(".save-button").click(this.onSaveButtonClick.bind(this));
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

        $("[name=favorite-label]", this.container).change(event => {
            var checkbox = <HTMLInputElement>event.target;
            var checkboxJQuery = $(checkbox);
            var labelId = checkboxJQuery.val();

            if (!checkbox.checked) {
                $(".favorite-selected-label-info [data-id=" + labelId + "]", this.container).remove();
                this.checkSelectedItems();
                return;
            }

            this.selectedLabelId = labelId;
            this.selectedLabelName = checkboxJQuery.data("name");
            this.selectedLabelColor = checkboxJQuery.data("color");
            var fontColor = FavoriteHelper.getFontColor(this.selectedLabelColor);
            var borderColor = FavoriteHelper.getDefaultBorderColor(new HexColor(this.selectedLabelColor));

            var newLabelSpan = document.createElement("span");
            $(newLabelSpan)
                .attr("data-id", labelId)
                .text(this.selectedLabelName)
                .addClass("label")
                .css("background-color", this.selectedLabelColor)
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

            navLinkJQuery.closest("li").addClass("active");
            $("." + tabClass, this.container).addClass("active");
        });

        $(".favorite-label-filter", this.container).on("change keyup paste", (event) => {
            var filter = $(event.currentTarget).val().toLocaleLowerCase();
            var isAnyVisible = false;
            $(".favorite-select-label .radio").each((index, element) => {
                var name = <string>$("input", element).data("name").toLocaleLowerCase();
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

    private onSaveButtonClick() {
        $(".saving-icon", this.container).removeClass("hidden");
        $(".error", this.container).addClass("hidden");
        
        if (this.onSaveCallback) {
            var resultData = this.getData();
            this.onSaveCallback(resultData);
        }
    }

    private getData(): INewFavoriteItemData {
        var itemName: string = $("#favorite-name").val();
        
        var resultData: INewFavoriteItemData = {
            itemName: itemName,
            labelId: this.selectedLabelId,
            labelName: this.selectedLabelName,
            labelColor: this.selectedLabelColor
        };
        return resultData;
    }
}

interface INewFavoriteItemData {
    labelId: number;
    itemName: string;
    labelName: string;
    labelColor: string;
}