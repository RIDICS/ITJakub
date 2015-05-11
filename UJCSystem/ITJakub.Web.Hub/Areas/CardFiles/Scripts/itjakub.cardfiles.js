$(".cardfile-select-more").click(function () {
    var body = $(this).parents(".cardfile-select").children(".cardfile-select-body");
    if (body.is(":hidden")) {
        $(this).children().removeClass("glyphicon-chevron-down");
        $(this).children().addClass("glyphicon-chevron-up");
        body.slideDown();
    }
    else {
        $(this).children().removeClass("glyphicon-chevron-up");
        $(this).children().addClass("glyphicon-chevron-down");
        body.slideUp();
    }
});
$(".cardfile-clear-filter").click(function () {
    $(this).siblings(".cardfile-filter-input").val('').change();
});
$(".cardfile-filter-input").keyup(function () {
    $(this).change();
});
$(".cardfile-filter-input").change(function () {
    if ($(this).val() == '') {
        $(this).parents(".cardfile-select-body").children(".concrete-cardfile").show();
    }
    else {
        $(this).parents(".cardfile-select-body").children(".concrete-cardfile").hide().filter(':contains(' + $(this).val() + ')').show();
    }
});
$(".delete-cardfile").click(function () {
    $(this).siblings(".save-cardfile").show();
    $(this).hide();
    //TODO populate request on delete from favorites
});
$(".save-cardfile").click(function () {
    $(this).siblings(".delete-cardfile").show();
    $(this).hide();
    //TODO populate request on save to favorites
});
$(".concrete-cardfile-checkbox").click(function () {
    //TODO add cardfile to search criteria
});
$(document).ready(function () {
    var cardsCreator = new CardFileManager("div.cardfile-result-area");
    cardsCreator.makeCardFile("1", "Excerpce", "52", "netbalivy-odymovati", 22); //TODO load files and buckets by search
    cardsCreator.makeCardFile("2", "Excerpce", "71", "netbalivy-odymovati", 13);
    cardsCreator.makeCardFile("3", "Excerpce", "70", "netbalivy-odymovati", 5);
});
//# sourceMappingURL=itjakub.cardfiles.js.map