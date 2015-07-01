$(document).ready(() => {
    var dictionariesViewer = new DictionaryViewer("#headwordList", "#pagination", "#headwordDescription");

    $("#searchButton").click(() => {
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
            success: (response) => {
                dictionariesViewer.createViewer(query, 190, searchUrl); //TODO
            }
        });
    });
}); 