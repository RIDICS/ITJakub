$(document).ready(function () {
    var callbackDelegate = new DropDownSelectCallbackDelegate();
    var dictionarySelector = new DropDownSelect("div.dictionary-selects", getBaseUrl() + "Dictionaries/Dictionaries/GetDictionariesWithCategories", true, callbackDelegate);
    dictionarySelector.makeDropdown();
    var pageSize = 50;
    var dictionariesViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription", true);
    var bookIdList = [];
    var headwordsListUrl = getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordList";
    $.ajax({
        type: "GET",
        traditional: true,
        url: getBaseUrl() + "Dictionaries/Dictionaries/GetHeadwordCount",
        data: {
            selectedBookIds: bookIdList
        },
        dataType: "json",
        contentType: "application/json",
        success: function (response) {
            var resultCount = response;
            dictionariesViewer.createViewer(resultCount, headwordsListUrl, bookIdList, null, pageSize);
        }
    });
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
                selectedBookIds: [4, 5],
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
});
//# sourceMappingURL=itjakub.dictionaries.headwords.js.map