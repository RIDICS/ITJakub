class NewFavoriteDialog {
    private container: HTMLDivElement;
    private onSaveCallback: (data: INewFavoriteItemData) => void;

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

            var labelName = $(checkbox).data("name");
            var labelColor = $(checkbox).data("color");

            $(".favorite-selected-label-info", this.container)
                .text(labelName)
                .css("background-color", labelColor);
        });
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
        var labelId: string = $("#favorite-labels").val();

        var resultData: INewFavoriteItemData = {
            itemName: itemName,
            labelId: Number(labelId)
        };
        return resultData;
    }
}

interface INewFavoriteItemData {
    labelId: number;
    itemName: string;
}