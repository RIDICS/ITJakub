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

        $(".modal", this.container).modal({
            show: true,
            backdrop: "static"
        });
    }

    private finishInnerInitialization() {
        $(".modal-body", this.container).removeClass("loading");

        $("[name=favorite-label]", this.container).change(event => {
            var checkbox = <HTMLInputElement>event.target;
            if (!checkbox.checked)
                return;

            this.selectedLabelId = $(checkbox).val();
            this.selectedLabelName = $(checkbox).data("name");
            this.selectedLabelColor = $(checkbox).data("color");
            var fontColor = FavoriteHelper.getFontColor(this.selectedLabelColor);

            $(".favorite-selected-label-info", this.container)
                .text(this.selectedLabelName)
                .css("background-color", this.selectedLabelColor)
                .css("color", fontColor);
        });

        $("[name=favorite-label]:checked", this.container).trigger("change");

        $(".favorite-select-label-item", this.container).each((index, element) => {
            var backgroundColor = $("input", element).data("color");
            var fontColor = FavoriteHelper.getFontColor(backgroundColor);
            $(element).css("color", fontColor);
        });

        //$(".favorite-selected-label-info", this.container)
    }

    public hide() {
        $(".modal", this.container).modal("hide");
    }

    private onSaveButtonClick() {
        if (this.onSaveCallback) {
            var resultData = this.getData();
            this.onSaveCallback(resultData);
        }

        this.hide();
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