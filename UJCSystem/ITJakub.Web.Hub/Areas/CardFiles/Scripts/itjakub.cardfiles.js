$(document).ready(function () {
    var callbackDelegate = createDelegate();
    var cardfileSelector = new DropDownSelect("div.cardfile-selects", "/CardFiles/CardFiles/CardFiles", true, callbackDelegate);
    var cardFileManager = new CardFileManager("div.cardfile-result-area");
    cardfileSelector.makeDropdown();
    $("#searchButton").click(function () {
        var noResultDiv = $("div.no-result");
        noResultDiv.hide();
        var nothingSelectedDiv = $("div.nothing-selected");
        nothingSelectedDiv.hide();
        cardFileManager.clearContainer();
        var selectedCardFiles = cardfileSelector.getState().SelectedItems;
        var searchedHeadword = $("#searchbox").val();
        if (selectedCardFiles.length === 0) {
            $(nothingSelectedDiv).show();
        }
        for (var cardFileIndex = 0; cardFileIndex < selectedCardFiles.length; cardFileIndex++) {
            var selectedCardFileItem = selectedCardFiles[cardFileIndex];
            $.ajax({
                type: "GET",
                traditional: true,
                data: { cardFileId: selectedCardFileItem.Id, headword: searchedHeadword },
                url: "/CardFiles/CardFiles/Buckets",
                dataType: 'json',
                contentType: 'application/json',
                success: function (response) {
                    var buckets = response["buckets"];
                    if (buckets.length === 0) {
                        $(noResultDiv).show();
                    }
                    for (var bucketIndex = 0; bucketIndex < buckets.length; bucketIndex++) {
                        var bucket = buckets[bucketIndex];
                        var cards = bucket["Cards"];
                        for (var cardIndex = 0; cardIndex < cards.length; cardIndex++) {
                            var card = cards[cardIndex];
                            cardFileManager.makeCardFile(selectedCardFileItem.Id, selectedCardFileItem.Name, bucket["Id"], bucket["Name"], card["Position"]);
                        }
                    }
                },
                error: function (response) {
                    //TODO resolve error
                }
            });
        }
    });
});
function createDelegate() {
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.getCategoriesFromResponseCallback = function (response) {
        return null;
    };
    callbackDelegate.getLeafItemsFromResponseCallback = function (response) {
        return response["cardFiles"];
    };
    callbackDelegate.getLeafItemIdCallback = function (item) {
        return item["Id"];
    };
    callbackDelegate.getLeafItemTextCallback = function (item) {
        return item["Name"];
    };
    callbackDelegate.getCategoryIdCallback = function (category) {
        return category["Id"];
    };
    callbackDelegate.getCategoryTextCallback = function (category) {
        return category["Name"];
    };
    callbackDelegate.getRootCategoryCallback = function (categories) {
        var rootCategory = new Object();
        rootCategory["Name"] = "Kartotéky";
        return rootCategory;
    };
    callbackDelegate.getChildCategoriesCallback = function (categories, currentCategory) {
        return null;
    };
    callbackDelegate.getChildLeafItemsCallback = function (leaftItems, currentCategory) {
        return leaftItems;
    };
    callbackDelegate.getTypeFromResponseCallback = function (response) {
        return "kartoteky";
    };
    return callbackDelegate;
}
//# sourceMappingURL=itjakub.cardfiles.js.map