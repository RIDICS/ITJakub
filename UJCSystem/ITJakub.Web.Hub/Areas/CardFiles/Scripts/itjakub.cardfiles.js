function createSearch() {
    var _this = this;
    var callbackDelegate = createDelegate();
    var cardfileSelector = new DropDownSelect("div.cardfile-selects", getBaseUrl() + "CardFiles/CardFiles/CardFiles", true, callbackDelegate);
    var cardFileManager = new CardFileManager("div.cardfile-result-area");
    cardfileSelector.makeDropdown();
    $("#searchbox").keypress(function (event) {
        var keyCode = event.which || event.keyCode;
        if (keyCode === 13) {
            $(_this.searchButton).click();
            event.preventDefault();
            event.stopPropagation();
            return false;
        }
    });
    $("#searchButton").click(function () {
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
        var searchedHeadword = $("#searchbox").val();
        if (selectedCardFiles.length === 0) {
            $(nothingSelectedDiv).show();
        }
        else {
            var showMessage = function () {
                if (shouldShowErrorMessage) {
                    $(serverErrorDiv).show();
                }
                if (shouldShowNoResultMessage) {
                    $(noResultDiv).show();
                }
            };
            setTimeout(showMessage, 1000);
        }
        for (var cardFileIndex = 0; cardFileIndex < selectedCardFiles.length; cardFileIndex++) {
            var selectedCardFileItem = selectedCardFiles[cardFileIndex];
            (function (selectedCardFileItem) {
                $.ajax({
                    type: "GET",
                    traditional: true,
                    data: { cardFileId: selectedCardFileItem.Id, headword: searchedHeadword },
                    url: getBaseUrl() + "CardFiles/CardFiles/Buckets",
                    dataType: 'json',
                    contentType: 'application/json',
                    success: function (response) {
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
                    error: function (response) {
                        shouldShowNoResultMessage = false;
                        $(noResultDiv).hide();
                    }
                });
            })(selectedCardFileItem);
        }
    });
}
;
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
function createListing() {
    var cardFileSelector = $("#card-file-select");
    var cardFileLoadingDiv = $("div.card-file-select div.loading");
    var bucketSelector = $("#bucket-select");
    var bucketLoadingDiv = $("div.bucket-select div.loading");
    $(cardFileSelector).hide();
    $(bucketSelector).hide();
    var cardFileManager = new CardFileManager("div.cardfile-result-area");
    var cardFileIdListed = "";
    var cardFileNameListed = "";
    $.ajax({
        type: "GET",
        traditional: true,
        data: {},
        url: getBaseUrl() + "CardFiles/CardFiles/CardFiles",
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            var cardFiles = response["cardFiles"];
            for (var i = 0; i < cardFiles.length; i++) {
                var cardFile = cardFiles[i];
                var optionElement = document.createElement("option");
                if (i === 0) {
                    optionElement.selected = true;
                }
                optionElement.value = cardFile["Id"];
                optionElement.text = cardFile["Name"];
                $(cardFileSelector).append(optionElement);
            }
            $(cardFileLoadingDiv).hide();
            var cardFileId = getQueryStringParameterByName("cardFileId");
            if (cardFileId) {
                $(cardFileSelector).find("option:selected").removeAttr('selected');
                $(cardFileSelector).find("option[value =" + cardFileId + "]").prop('selected', 'selected');
            }
            $(cardFileSelector).show();
            $(cardFileSelector).change();
        },
        error: function (response) {
            //TODO resolve error
        }
    });
    $(cardFileSelector).change(function () {
        var optionSelected = $("option:selected", this);
        cardFileIdListed = optionSelected.val();
        cardFileNameListed = optionSelected.text();
        $(bucketSelector).empty();
        $(bucketSelector).hide();
        $(bucketLoadingDiv).show();
        $.ajax({
            type: "GET",
            traditional: true,
            data: { cardFileId: cardFileIdListed },
            url: getBaseUrl() + "CardFiles/CardFiles/Buckets",
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                var buckets = response["buckets"];
                for (var i = 0; i < buckets.length; i++) {
                    var bucket = buckets[i];
                    var optionElement = document.createElement("option");
                    if (i === 0) {
                        optionElement.selected = true;
                    }
                    optionElement.value = bucket["Id"];
                    optionElement.text = bucket["Name"];
                    $(bucketSelector).append(optionElement);
                }
                $(bucketLoadingDiv).hide();
                $(bucketSelector).show();
                $(bucketSelector).change();
            },
            error: function (response) {
                //TODO resolve error
            }
        });
    });
    $(bucketSelector).change(function () {
        var optionSelected = $("option:selected", this);
        var bucketId = optionSelected.val();
        var bucketText = optionSelected.text();
        cardFileManager.clearContainer();
        cardFileManager.makeCardFile(cardFileIdListed, cardFileNameListed, bucketId, bucketText);
    });
}
function sortOrderChanged() {
    //TODO make ordering
}
function createList() {
    var _this = this;
    var bibliographyModule = new BibliographyModule("#cardFilesListResults", "#cardFilesResultsHeader", sortOrderChanged);
    $('#searchButton').click(function () {
        var text = $('#searchbox').val();
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "CardFiles/CardFiles/SearchList",
            data: { term: text },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                bibliographyModule.showBooks(response.books);
            }
        });
    });
    $("#searchbox").keypress(function (event) {
        var keyCode = event.which || event.keyCode;
        if (keyCode === 13) {
            $(_this.searchButton).click();
            event.preventDefault();
            event.stopPropagation();
            return false;
        }
    });
    $('#searchButton').click();
}
