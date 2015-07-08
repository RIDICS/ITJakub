$(document).ready(function () {
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    var dictionarySelector = new DropDownSelect("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
    var dictionariesViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription", true);
    var bookIdList = [];
    var headwordsListUrl = getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordList";
    $.ajax({
        type: "POST",
        traditional: true,
        url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordCount",
        data: JSON.stringify(bookIdList),
        dataType: "json",
        contentType: "application/json",
        success: function (response) {
            var resultCount = response;
            dictionariesViewer.createViewer(resultCount, headwordsListUrl, bookIdList);
        }
    });
    $("#printDescription").click(function () {
        dictionariesViewer.print();
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