$(".dictionary-select-more").click(function () {
    var body = $(this).parents(".dictionary-select").children(".dictionary-select-body");
    if (body.is(":hidden")) {
        $(this).children().removeClass("glyphicon-chevron-down");
        $(this).children().addClass("glyphicon-chevron-up");
        body.slideDown();
    } else {
        $(this).children().removeClass("glyphicon-chevron-up");
        $(this).children().addClass("glyphicon-chevron-down");
        body.slideUp();
    }
});

$(".dictionary-clear-filter").click(function () {
    $(this).siblings(".dictionary-filter-input").val('').change();
});

$(".dictionary-filter-input").keyup(function () {
    $(this).change();
});

$(".dictionary-filter-input").change(function () {
    if ($(this).val() == '') {
        $(this).parents(".dictionary-select-body").children(".concrete-dictionary").show();
    } else {
        $(this).parents(".dictionary-select-body").children(".concrete-dictionary").hide().filter(':contains(' + $(this).val() + ')').show();
    }
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

$(".saved-word-remove").click(function () {
    $(this).parent(".saved-word").fadeOut(function () {
        $(this).remove();
    }); //TODO populate request on remove to server
});

$(".saved-word-text").click(function () {
    alert("here should be request for new search with word: " + $(this).text()); //TODO populate request on server
});

$(".delete-dictionary").click(function () {
    $(this).siblings(".save-dictionary").show();
    $(this).hide();
    //TODO populate request on delete from favorites
});

$(".save-dictionary").click(function () {
    $(this).siblings(".delete-dictionary").show();
    $(this).hide();
    //TODO populate request on save to favorites
});

$(".concrete-dictionary-checkbox").click(function () {
    //TODO add dictionary to search criteria
});
//# sourceMappingURL=itjakub.dictionaries.js.map
