$(document).ready(() => {
    var callbackDelegate = createDelegate();
    var cardfileSelector = new DropDownSelect("div.cardfile-selects", "/CardFiles/CardFiles/CardFiles", true, callbackDelegate);
    var cardFileManager = new CardFileManager("div.cardfile-result-area");
    cardfileSelector.makeDropdown();

    $("#searchButton").click(() => {
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
            var selectedCardFileItem: Item = selectedCardFiles[cardFileIndex];
            $.ajax({
                type: "GET",
                traditional: true,
                data: { cardFileId: selectedCardFileItem.Id, headword: searchedHeadword },
                url: "/CardFiles/CardFiles/Buckets",
                dataType: 'json',
                contentType: 'application/json',
                success: (response) => {
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
                error: (response) => {
                    //TODO resolve error
                }
            });
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

