﻿function createSearch() {
    var callbackDelegate = createDelegate();
    var cardfileSelector = new DropDownSelect("div.cardfile-selects", getBaseUrl() +"CardFiles/CardFiles/CardFiles", false, callbackDelegate);
    var cardFileManager = new CardFileManager("div.cardfile-result-area");
    cardfileSelector.makeDropdown();
    

    $("#searchbox").keypress((event: any) => {
        var keyCode = event.which || event.keyCode;
        if (keyCode === 13) {     //13 = Enter
            $(this.searchButton).click();
            event.preventDefault();
            event.stopPropagation();
            return false;
        }
    });

    $("#searchButton").click(() => {
        var noResultDiv = $("div.no-result");
        $(noResultDiv).hide();
        var nothingSelectedDiv = $("div.nothing-selected");
        $(nothingSelectedDiv).hide();
        var serverErrorDiv = $("div.server-error");
        $(serverErrorDiv).hide();
        var shouldShowErrorMessage = true;
        var shouldShowNoResultMessage = true;

        cardFileManager.clearContainer();
        var selectedCardFiles = cardfileSelector.getState().SelectedItems;
        var searchedHeadword = $("#searchbox").val() as string;

        if (selectedCardFiles.length === 0) {
            $(nothingSelectedDiv).show();
        } else {
            var showMessage = () => {
                if(shouldShowErrorMessage){
                    $(serverErrorDiv).show();   
                }
                if (shouldShowNoResultMessage) {
                    $(noResultDiv).show();
                }
            }
            setTimeout(showMessage, 1000);
        }

        for (var cardFileIndex = 0; cardFileIndex < selectedCardFiles.length; cardFileIndex++) {
            
            var selectedCardFileItem: Item = selectedCardFiles[cardFileIndex];
            ((selectedCardFileItem: Item) => {
                $.ajax({
                    type: "GET",
                    traditional: true,
                    data: { cardFileId: selectedCardFileItem.Id, headword: searchedHeadword } as JQuery.PlainObject,
                    url: getBaseUrl()+"CardFiles/CardFiles/Buckets",
                    dataType: "json",
                    contentType: "application/json",
                    success: (response) => {
                        shouldShowErrorMessage = false;
                        $(serverErrorDiv).hide();
                        var buckets = response["buckets"];

                        if (buckets.length !== 0) {
                            shouldShowNoResultMessage = false;
                            $(noResultDiv).hide();
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
                        shouldShowNoResultMessage = false;
                        $(noResultDiv).hide();
                    }
                });
            })(selectedCardFileItem);
        }
        
    });
};

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
        rootCategory["Name"] = "kartoteky";
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

function createListing() {
    var cardFileSelector = $("#card-file-select");
    var cardFileLoadingDiv = cardFileSelector.parent();
    var cardFileLoader = lv.create(null, "lv-dots sm lv-mid");
    cardFileLoadingDiv.append(cardFileLoader.getElement());
    var bucketSelector = $("#bucket-select");
    var bucketLoadingDiv = bucketSelector.parent();

    var cardFileManager = new CardFileManager("#cardfile-result-area");
    var cardFileIdListed: string = "";
    var cardFileNameListed: string = "";

    $.ajax({
        type: "GET",
        traditional: true,
        data: { },
        url: getBaseUrl()+"CardFiles/CardFiles/CardFiles",
        dataType: "json",
        contentType: "application/json",
        success: (response) => {
            var cardFiles = response["cardFiles"];
            for (var i = 0; i < cardFiles.length; i++) {
                var cardFile = cardFiles[i];
                var optionElement: HTMLOptionElement = document.createElement("option");
                if (i === 0) {
                    optionElement.selected = true;
                }
                optionElement.value = cardFile["Id"];
                optionElement.text = cardFile["Name"];
                $(cardFileSelector).append(optionElement);
            }

            cardFileLoader.remove();

            var cardFileId = getQueryStringParameterByName("cardFileId");
            if(cardFileId){
            $(cardFileSelector).find("option:selected").removeAttr("selected");
            $(cardFileSelector).find(`option[value = ${cardFileId}]`).prop("selected", "selected");
            }

            $(cardFileSelector).removeClass("hidden");
            $(cardFileSelector).change();
        },
        error: (response) => {
            cardFileLoader.remove();
            bootbox.alert({
                title: localization.translate("Error", "CardFiles").value,
                message: localization.translate("PermissionsError", "CardFiles").value,
                buttons: {
                    ok: {
                        className: "btn-default"
                    }
                }
            });
        }
    });

    $(cardFileSelector).change(function() {
        var optionSelected = $("option:selected", this);
        var bucketLoader = lv.create(null, "lv-dots sm lv-mid");
        cardFileIdListed = optionSelected.val() as string;
        cardFileNameListed = optionSelected.text();
        $(bucketSelector).addClass("hidden");
        $(bucketSelector).empty();
        $(bucketLoadingDiv).append(bucketLoader.getElement());

        $.ajax({
            type: "GET",
            traditional: true,
            data: { cardFileId: cardFileIdListed } as JQuery.PlainObject,
            url: getBaseUrl()+"CardFiles/CardFiles/Buckets",
            dataType: "json",
            contentType: "application/json",
            success: (response) => {
                var buckets = response["buckets"];
                for (var i = 0; i < buckets.length; i++) {
                    var bucket = buckets[i];
                    var optionElement: HTMLOptionElement = document.createElement("option");
                    if (i === 0) {
                        optionElement.selected = true;
                    }
                    optionElement.value = bucket["Id"];
                    optionElement.text = bucket["Name"];
                    $(bucketSelector).append(optionElement);
                }

                bucketLoader.remove();
                $(bucketSelector).removeClass("hidden");
                $(bucketSelector).change();
            },
            error: (response) => {
                bucketLoader.remove();
                bootbox.alert({
                    title: localization.translate("Error", "CardFiles").value,
                    message: localization.translate("PermissionsError", "CardFiles").value,
                    buttons: {
                        ok: {
                            className: "btn-default"
                        }
                    }
                });
            }
        });
    });

    $(bucketSelector).change(function() {
        var optionSelected = $("option:selected", this);
        var bucketId = optionSelected.val() as string;
        var bucketText = optionSelected.text();
        cardFileManager.clearContainer();
        cardFileManager.makeCardFile(cardFileIdListed, cardFileNameListed, bucketId, bucketText);
    });

}

function sortOrderChanged() {
    //TODO make ordering
}

function initCardList() {
    var bibliographyModule = new BibliographyModule("#cardFilesListResults", "#cardFilesResultsHeader", sortOrderChanged);

    $("#searchButton").click(() => {
        var text = $("#searchbox").val() as string;
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "CardFiles/CardFiles/SearchList",
            data: { term: text } as JQuery.PlainObject,
            dataType: "json",
            contentType: "application/json",
            success(response) {
                bibliographyModule.showBooks(response.books);
            }
        });
    });

    $("#searchbox").keypress((event: any) => {
        var keyCode = event.which || event.keyCode;
        if (keyCode === 13) {     //13 = Enter
            $(this.searchButton).click();
            event.preventDefault();
            event.stopPropagation();
            return false;
        }
    });

    $("#searchButton").click();
}
