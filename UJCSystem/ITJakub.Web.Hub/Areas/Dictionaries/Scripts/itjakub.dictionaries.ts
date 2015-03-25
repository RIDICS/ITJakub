
$(document).ready(function () {
    new DropDownSelect("div.dictionary-selects","/Dictionaries/Dictionaries/GetDictionariesWithCategories", true).makeDropdown();
});


$(".saved-word-area-more").click(function () {
    var area = $(".saved-word-area");
    if (!area.hasClass("uncollapsed")) {
        $(this).children().removeClass("glyphicon-collapse-down");
        $(this).children().addClass("glyphicon-collapse-up");
        area.addClass("uncollapsed");
        var actualHeight = area.height();
        var targetHeight = area.css("height", 'auto').height();
        area.height(actualHeight);
        area.animate({
            height: targetHeight
        });

    } else {
        $(this).children().removeClass("glyphicon-collapse-up");
        $(this).children().addClass("glyphicon-collapse-down");
        area.removeClass("uncollapsed");
        area.animate({
            height: "100%"
        });
    }
    
});

$(".saved-word-remove").click(function() {
    $(this).parent(".saved-word").fadeOut(function() {
        $(this).remove();
    }); //TODO populate request to remove on server
});

$(".saved-word-text").click(function () {
    alert("here should be request for new search with word: "+$(this).text());
}); //TODO populate request to add word on server







