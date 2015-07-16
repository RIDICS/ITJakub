$(document).ready(() => {
    var search = new Search();
    search.makeSearch();

    $("#searchButton").click((event: Event) => {
        search.processSearch();
    });
});