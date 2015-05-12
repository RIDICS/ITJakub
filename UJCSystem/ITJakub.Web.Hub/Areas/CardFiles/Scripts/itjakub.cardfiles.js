$(document).ready(function () {
    downloadCardFiles();
    var callbackDelegate = createDelegate();
    var cardfileSelector = new DropDownSelect("div.cardfile-selects", "/CardFiles/CardFiles/CardFiles", true, callbackDelegate);
    cardfileSelector.makeDropdown();
    var array = new Array();
    array.push(cardfileSelector);
    $("#searchButton").click(function () {
        for (var i = 0; i < array.length; i++) {
            var state = array[i].getState();
            showStateInAlertBoxCardFile(state);
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
//TODO this method is just showcase what is result of dropdown select menu. Remove this method in future
function showStateInAlertBoxCardFile(state) {
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