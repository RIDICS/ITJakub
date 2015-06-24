
$(document).ready(() => {
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    var dictionarySelector = new DropDownSelect("div.dictionary-selects", getBaseUrl()+"Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();

    var editionSelector = new DropDownSelect("div.dictionary-selects", getBaseUrl() +"Dictionaries/Dictionaries/GetTextWithCategories", true, callbackDelegate);
    editionSelector.makeDropdown();

    var array = new Array();
    array.push(dictionarySelector);
    array.push(editionSelector);

    $("#searchButton").click(() => {
        for (var i = 0; i < array.length; i++) {
            var state = array[i].getState();
            showStateInAlertBox(state);
        }
    });

    var searchBox = new SearchBox("#searchbox", "Dictionaries/Dictionaries");
    searchBox.addDataSet("DictionaryEntry", "Slovníková hesla");
    searchBox.create();
});

function showStateInAlertBox(state : State) {
    var itemIds = "";
    $.each(state.SelectedItems, (index, item) => {
        itemIds = itemIds.concat(item.Id+",");
    });

    var categoriesIds = "";
    $.each(state.SelectedCategories, (index, category) => {
        categoriesIds = categoriesIds.concat(category.Id + ",");
    });

    alert("State for type: " + state.Type+"\nItems: "+itemIds+"\nCategories: "+categoriesIds);
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







