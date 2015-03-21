
$(".dictionary-select-more").click(function() {
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

$(".dictionary-clear-filter").click(function() {
    $(this).siblings(".dictionary-filter-input").val('').change();
});

$(".dictionary-filter-input").keyup(function() {
    $(this).change();
});

$(".dictionary-filter-input").change(function() {
    if ($(this).val() == '') {
        $(this).parents(".dictionary-select-body").children(".concrete-dictionary").show();
    } else {
        $(this).parents(".dictionary-select-body").children(".concrete-dictionary").hide().filter(':contains(' + $(this).val() + ')').show();
    }
});