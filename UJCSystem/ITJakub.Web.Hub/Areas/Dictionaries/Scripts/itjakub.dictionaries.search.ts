$(document).ready(() => {
    $("#advancedSearchButton").click(() => {
        var glyph = $("#advancedSearchButton .regexsearch-button-glyph");
        var regExSearchDiv = document.getElementById("regExSearchDiv");
        if (document.getElementById("regExSearchDiv").children.length === 0) {
            glyph.removeClass("glyphicon-chevron-down");
            glyph.addClass("glyphicon-chevron-up");
            var regExSearchPlugin = new RegExSearch(<HTMLDivElement>regExSearchDiv);
            regExSearchPlugin.makeRegExSearch();
        } else if ($(regExSearchDiv).hasClass("hidden")) {
            $(regExSearchDiv).removeClass("hidden");
            glyph.removeClass("glyphicon-chevron-down");
            glyph.addClass("glyphicon-chevron-up");
        } else {
            $(regExSearchDiv).addClass("hidden");
            glyph.removeClass("glyphicon-chevron-up");
            glyph.addClass("glyphicon-chevron-down");
        }
    });
});