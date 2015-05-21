$(document).ready(function () {
    var dictionarySelector = new DropDownSelect("div.dictionary-selects", "/Dictionaries/Dictionaries/GetDictionariesWithCategories", true);
    //dictionarySelector.selectedChangedCallback = showStateInAlertBox;
    dictionarySelector.makeDropdown();
    var editionSelector = new DropDownSelect("div.dictionary-selects", "/Dictionaries/Dictionaries/GetTextWithCategories", true);
    //editionSelector.selectedChangedCallback = showStateInAlertBox;
    //selector.starSaveCategoryCallback = testCategoryCallbackMethod;
    //selector.starSaveItemCallback = testItemCallbackMethod;
    //selector.starDeleteCategoryCallback = testCategoryCallbackMethod;
    //selector.starDeleteItemCallback = testItemCallbackMethod;
    editionSelector.makeDropdown();
    var array = new Array();
    array.push(dictionarySelector);
    array.push(editionSelector);
    $("#searchButton").click(function () {
        for (var i = 0; i < array.length; i++) {
            var state = array[i].getState();
            showStateInAlertBox(state);
        }
    });
});
function showStateInAlertBox(state) {
    var itemIds = "";
    $.each(state.SelectedItemsIds, function (index, val) {
        itemIds = itemIds.concat(val + ",");
    });
    var categoriesIds = "";
    $.each(state.SelectedCategoriesIds, function (index, val) {
        categoriesIds = categoriesIds.concat(val + ",");
    });
    alert("State for type: " + state.Type + "\nItems: " + itemIds + "\nCategories: " + categoriesIds);
}
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
    }
    else {
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
    }); //TODO populate request to remove on server
});
$(".saved-word-text").click(function () {
    alert("here should be request for new search with word: " + $(this).text());
}); //TODO populate request to add word on server
//# sourceMappingURL=itjakub.dictionaries.js.map