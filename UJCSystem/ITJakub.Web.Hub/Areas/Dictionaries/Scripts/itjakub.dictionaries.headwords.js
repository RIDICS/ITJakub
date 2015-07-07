$(document).ready(function () {
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    var dictionarySelector = new DropDownSelect("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
    var dictionariesViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription");
    var bookIdList = [4]; //TODO get ids from dictionarySelector
    $("#loadButton").click(function () {
        var headwordsListUrl = getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordList";
        $.ajax({
            type: "POST",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordCount",
            data: JSON.stringify(bookIdList),
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                //todo show first page
                var resultCount = response;
                dictionariesViewer.createViewer(resultCount, headwordsListUrl, dictionarySelector);
            }
        });
    });
    //$("#searchButton").click(() => {
    //    var query = $("#searchbox").val();
    //    var searchUrl = getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwords";
    //    $.ajax({
    //        type: "GET",
    //        traditional: true,
    //        url: getBaseUrl() + "Dictionaries/Dictionaries/GetSearchResultCount",
    //        data: {
    //            query: query
    //        },
    //        dataType: "json",
    //        contentType: "application/json",
    //        success: (response) => {
    //            var resultCount = response;
    //            dictionariesViewer.createViewer(query, resultCount, searchUrl);
    //        }
    //    });
    //});
});
//# sourceMappingURL=itjakub.dictionaries.headwords.js.map