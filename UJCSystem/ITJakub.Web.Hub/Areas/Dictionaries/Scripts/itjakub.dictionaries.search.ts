$(document).ready(() => {
    var speedAnimation = 200; //200=fast, 600=slow

    $("#advancedSearchButton").click(() => {
        var glyph = $("#advancedSearchButton .regexsearch-button-glyph");
        var regExSearchDiv = document.getElementById("regExSearchDiv");
        var searchboxTextInput = document.getElementById("searchbox");
        if (document.getElementById("regExSearchDiv").children.length === 0) {
            glyph.removeClass("glyphicon-chevron-down");
            glyph.addClass("glyphicon-chevron-up");
            var regExSearchPlugin = new RegExSearch(<HTMLDivElement>regExSearchDiv);
            regExSearchPlugin.makeRegExSearch();
            $(regExSearchDiv).hide();
            $(regExSearchDiv).slideDown(speedAnimation);
        } else if ($(regExSearchDiv).is(":hidden")) {       //show advanced search
            $(regExSearchDiv).slideDown(speedAnimation);
            $(searchboxTextInput).prop('disabled', true);
            glyph.removeClass("glyphicon-chevron-down");
            glyph.addClass("glyphicon-chevron-up");
        } else {
            $(regExSearchDiv).slideUp(speedAnimation);      //hide advanced search
            $(searchboxTextInput).prop('disabled',false);
            glyph.removeClass("glyphicon-chevron-up");
            glyph.addClass("glyphicon-chevron-down");
        }
    });
});