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
    callbackDelegate.getCategoriesFromResponseCallback = (response) => {
        return null;
    };

    callbackDelegate.getLeafItemsFromResponseCallback = (response) => {
        return response["cardFiles"];
    };

    callbackDelegate.getLeafItemIdCallback = (item) => {
        return item["Id"];
    };

    callbackDelegate.getLeafItemTextCallback = (item) => {
        return item["Name"];
    };

    callbackDelegate.getCategoryIdCallback = (category): string => {
        return category["Id"];
    };

    callbackDelegate.getCategoryTextCallback = (category): string => {
        return category["Name"];
    };

    callbackDelegate.getRootCategoryCallback = (categories): any => {
        var rootCategory = new Object();
        rootCategory["Name"] = "Kartotéky";
        return rootCategory;
    };

    callbackDelegate.getChildCategoriesCallback = (categories, currentCategory): Array<any> => {
        return null;
    }

    callbackDelegate.getChildLeafItemsCallback = (leaftItems, currentCategory): Array<any> => {
        return leaftItems;
    }

    callbackDelegate.getTypeFromResponseCallback = (response) => {
        return "kartoteky";
    }

    return callbackDelegate;
}

//TODO this method is just showcase what is result of dropdown select menu. Remove this method in future
function showStateInAlertBoxCardFile(state: State) {
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
        data: { },
        url: "/CardFiles/CardFiles/CardFiles",
        dataType: 'json',
        contentType: 'application/json',
        success: (response) => {
            var cardsCreator = new CardFileManager("div.cardfile-result-area");
            var cardFiles = response["cardFiles"];
            for (var i = 0; i < 3; i++) {
                var cardFile = cardFiles[i];
                var buckets = getBuckets(cardFile["Id"]);
                var firstBucket = buckets[0];
                cardsCreator.makeCardFile(cardFile["Id"], cardFile["Name"], firstBucket["Id"], firstBucket["Name"]); 
            }
        },
        error: (response) => {
            //TODO resolve error
        }
    });
}

function getBuckets(cardFileId) {
    var response = $.ajax({
        async: false,
        type: "GET",
        traditional: true,
        data: { cardFileId : cardFileId},
        url: "/CardFiles/CardFiles/Buckets",
        dataType: 'json',
        contentType: 'application/json'
    });

    return response.responseJSON["buckets"];
}

