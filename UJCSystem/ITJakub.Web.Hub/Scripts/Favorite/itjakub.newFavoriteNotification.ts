class NewFavoriteNotification {
    private container: HTMLDivElement;

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
        
        $(info).text("Přiřazené štítky se změnily. Pro správné zobrazení štítků je nutné načíst tuto stránku znovu. ");
        $(refreshLink).text("Načíst stránku znovu.")
            .attr("href", "#")
            .click(() => {
                location.reload();
            });

        $(closeButtonContent)
            .html("&times;");
        $(closeButton)
            .addClass("close")
            .attr("type", "button")
            .attr("title", "Skrýt")
            .append(closeButtonContent)
            .click(() => {
                $(this.container).hide();
            });

        $(this.container)
            .append(info)
            .append(refreshLink)
            .append(closeButton)
            .appendTo($(".container"));
    }
}