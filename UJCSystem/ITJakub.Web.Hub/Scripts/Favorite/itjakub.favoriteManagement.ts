$(document).ready(() => {
    var favoriteManagement = new FavoriteManagement();
    favoriteManagement.init();
});

class FavoriteManagement {
    public init() {
        $("#favorite-label-color").colorpickerplus();
        $("#favorite-label-color").on("changeColor", (event, color) => {
            if (color == null) {
                $(event.target)
                    .val("#FFFFFF")
                    .css("background-color", "#FFFFFF");
            } else {
                $(event.target)
                    .val(color)
                    .css("background-color", color);
            }
        });
    }
}