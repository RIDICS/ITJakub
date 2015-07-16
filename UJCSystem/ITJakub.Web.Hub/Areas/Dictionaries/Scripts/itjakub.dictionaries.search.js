$(document).ready(function () {
    var search = new Search();
    search.makeSearch();
    $("#searchButton").click(function (event) {
        search.processSearch();
    });
});
//# sourceMappingURL=itjakub.dictionaries.search.js.map