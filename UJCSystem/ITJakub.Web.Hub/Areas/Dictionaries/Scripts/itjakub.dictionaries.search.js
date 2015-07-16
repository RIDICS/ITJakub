$(document).ready(function () {
    var speedAnimation = 200; //200=fast, 600=slow
    $("#advancedSearchButton").click(function () {
        var glyph = $("#advancedSearchButton .regexsearch-button-glyph");
        var regExSearchDiv = document.getElementById("regExSearchDiv");
        if (document.getElementById("regExSearchDiv").children.length === 0) {
            glyph.removeClass("glyphicon-chevron-down");
            glyph.addClass("glyphicon-chevron-up");
            var regExSearchPlugin = new RegExSearch(regExSearchDiv);
            regExSearchPlugin.makeRegExSearch();
            $(regExSearchDiv).hide();
            $(regExSearchDiv).slideDown(speedAnimation);
        }
        else if ($(regExSearchDiv).is(":hidden")) {
            $(regExSearchDiv).slideDown(speedAnimation);
            glyph.removeClass("glyphicon-chevron-down");
            glyph.addClass("glyphicon-chevron-up");
        }
        else {
            $(regExSearchDiv).slideUp(speedAnimation);
            glyph.removeClass("glyphicon-chevron-up");
            glyph.addClass("glyphicon-chevron-down");
        }
    });
});
//# sourceMappingURL=itjakub.dictionaries.search.js.map