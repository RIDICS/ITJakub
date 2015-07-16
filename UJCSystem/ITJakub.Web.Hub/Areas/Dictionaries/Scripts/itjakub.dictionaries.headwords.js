$(document).ready(function () {
    var pageSize = 50;
    var headwordsListUrl = getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordList";
    var dictionariesViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription", true);
    var loadHeadwordsFunction = function (state) {
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordCount",
            data: {
                selectedBookIds: DropDownSelect.getBookIdsFromState(state),
                selectedCategoryIds: DropDownSelect.getCategoryIdsFromState(state)
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                var resultCount = response;
                dictionariesViewer.createViewer(resultCount, headwordsListUrl, state, null, pageSize);
            }
        });
    };
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = function (state) {
        //loadHeadwordsFunction(state);
    };
    // TODO add selection changed callback
    var dictionarySelector = new DropDownSelect2("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
    $("#printDescription").click(function () {
        dictionariesViewer.print();
    });
    $("#searchButton").click(function () {
        var query = $("#searchbox").val();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordPageNumber",
            data: {
                selectedBookIds: DropDownSelect.getBookIdsFromState(dictionarySelector.getState()),
                selectedCategoryIds: DropDownSelect.getCategoryIdsFromState(dictionarySelector.getState()),
                query: query,
                pageSize: pageSize
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                var resultPageNumber = response;
                dictionariesViewer.goToPage(resultPageNumber);
            }
        });
    });
    loadHeadwordsFunction(dictionarySelector.getState());
});
//# sourceMappingURL=itjakub.dictionaries.headwords.js.map