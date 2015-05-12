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
    downloadCardFiles();
});
function downloadCardFiles() {
    $.ajax({
        type: "GET",
        traditional: true,
        data: {},
        url: "/CardFiles/CardFiles/CardFiles",
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            var cardsCreator = new CardFileManager("div.cardfile-result-area");
            var cardFiles = response["cardFiles"];
            for (var i = 0; i < 3; i++) {
                var cardFile = cardFiles[i];
                var buckets = getBuckets(cardFile["Id"]);
                var firstBucket = buckets[0];
                cardsCreator.makeCardFile(cardFile["Id"], cardFile["Name"], firstBucket["Id"], firstBucket["Name"]);
            }
        },
        error: function (response) {
            //TODO resolve error
        }
    });
}
function getBuckets(cardFileId) {
    var response = $.ajax({
        async: false,
        type: "GET",
        traditional: true,
        data: { cardFileId: cardFileId },
        url: "/CardFiles/CardFiles/Buckets",
        dataType: 'json',
        contentType: 'application/json'
    });
    return response.responseJSON["buckets"];
}
//# sourceMappingURL=itjakub.cardfiles.js.map