$(document).ready(function () {
    var search = new Search($("#dictionarySearchDiv"), processSearchJson, processSearchText);
    var disabledOptions = new Array();
    disabledOptions.push(10 /* Headwords */);
    disabledOptions.push(11 /* TokenDistanceHeadwords */);
    search.makeSearch(disabledOptions);
});
function processSearchResults(result) {
    alert("processed: " + result);
}
function processSearchJson(json) {
    $.ajax({
        type: "POST",
        traditional: true,
        data: JSON.stringify({ "json": json }),
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteria",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            processSearchResults(response);
        },
        error: function (response) {
        }
    });
}
function processSearchText(text) {
    $.ajax({
        type: "POST",
        traditional: true,
        data: JSON.stringify({ "text": text }),
        url: getBaseUrl() + "Dictionaries/Dictionaries/SearchCriteriaText",
        dataType: "text",
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            processSearchResults(response);
        },
        error: function (response) {
        }
    });
}
//# sourceMappingURL=itjakub.dictionaries.search.js.map