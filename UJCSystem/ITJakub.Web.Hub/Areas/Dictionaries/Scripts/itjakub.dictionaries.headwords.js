$(document).ready(function () {
    var dictionariesViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription");
    $("#searchButton").click(function () {
        var query = $("#searchbox").val();
        var searchUrl = getBaseUrl() + "Dictionaries/Dictionaries/SearchHeadword";
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Dictionaries/Dictionaries/GetSearchResultCount",
            data: {
                query: query
            },
            dataType: "json",
            contentType: "application/json",
            success: function (response) {
                dictionariesViewer.createViewer(query, 190, searchUrl); //TODO
            }
        });
    });
});
//# sourceMappingURL=itjakub.dictionaries.headwords.js.map