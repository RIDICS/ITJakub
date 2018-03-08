class NewFavoriteNotification {
    private container: HTMLDivElement;

    private localizationScope = "FavoriteJs";

    public show() {
        var $container = $("#favorite-change-notification");
        if ($container.length > 0) {
            $container.show();
            return;
        }

        this.container = document.createElement("div");
        $(this.container)
            .attr("id", "favorite-change-notification")
            .addClass("alert")
            .addClass("alert-warning")
            .addClass("alert-dismissible")
            .addClass("top-notification");

        var info = document.createElement("span");
        var refreshLink = document.createElement("a");
        var closeButton = document.createElement("a");
        var closeButtonContent = document.createElement("span");
        
        $(info).text(localization.translate("TagsChangedPleaseReload", this.localizationScope).value);
        $(refreshLink).text(localization.translate("ReloadPageAgain", this.localizationScope).value)
            .attr("href", "#")
            .click(() => {
                location.reload();
            });

        $(closeButtonContent)
            .html("&times;");
        $(closeButton)
            .addClass("close")
            .attr("type", "button")
            .attr("title", localization.translate("Hide", this.localizationScope).value)
            .append(closeButtonContent)
            .click(() => {
                $(this.container).hide();
            });

        $(this.container)
            .append(info)
            .append(refreshLink)
            .append(closeButton)
            .appendTo($(".module-content"));
    }
}