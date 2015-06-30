$(document).ready(function () {
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    var dictionarySelector = new DropDownSelect("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
    var editionSelector = new DropDownSelect("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetTextWithCategories", true, callbackDelegate);
    editionSelector.makeDropdown();
    var array = new Array();
    array.push(dictionarySelector);
    array.push(editionSelector);
    var dictionariesViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription");
    $("#searchButton").click(function () {
        //for (var i = 0; i < array.length; i++) {
        //    var state = array[i].getState();
        //    showStateInAlertBox(state);
        //}
        dictionariesViewer.search($("#searchbox").val());
    });
    $("#advancedSearchButton").click(function () {
        var glyph = $("#advancedSearchButton .regexsearch-button-glyph");
        var regExSearchDiv = document.getElementById("regExSearchDiv");
        if (document.getElementById("regExSearchDiv").children.length === 0) {
            glyph.removeClass("glyphicon-chevron-down");
            glyph.addClass("glyphicon-chevron-up");
            var regExSearchPlugin = new RegExSearch(regExSearchDiv);
            regExSearchPlugin.makeRegExSearch();
        }
        else if ($(regExSearchDiv).hasClass("hidden")) {
            $(regExSearchDiv).removeClass("hidden");
            glyph.removeClass("glyphicon-chevron-down");
            glyph.addClass("glyphicon-chevron-up");
        }
        else {
            $(regExSearchDiv).addClass("hidden");
            glyph.removeClass("glyphicon-chevron-up");
            glyph.addClass("glyphicon-chevron-down");
        }
    });
    var searchBox = new SearchBox("#searchbox", "Dictionaries/Dictionaries");
    searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla");
    searchBox.create();
});
function showStateInAlertBox(state) {
    var itemIds = "";
    $.each(state.SelectedItems, function (index, item) {
        itemIds = itemIds.concat(item.Id + ",");
    });
    var categoriesIds = "";
    $.each(state.SelectedCategories, function (index, category) {
        categoriesIds = categoriesIds.concat(category.Id + ",");
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