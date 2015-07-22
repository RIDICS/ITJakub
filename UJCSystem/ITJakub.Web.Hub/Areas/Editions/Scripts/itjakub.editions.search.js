$(document).ready(function () {
    var bookType = $("#listResults").data("book-type");
    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", bookType);
    function editionBasicSearch(text, pageNumber) {
        if (typeof text === "undefined" || text === null || text === "")
            return;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearch",
            data: { text: text, pageNumber: pageNumber },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                bibliographyModule.showBooks(response.books);
                alert("done text");
            }
        });
    }
    function editionAdvancedSearch(json, pageNumber) {
        if (typeof json === "undefined" || json === null || json === "")
            return;
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearch",
            data: { json: json, pageNumber: pageNumber },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                bibliographyModule.showBooks(response.books);
                alert("done json");
            }
        });
    }
    var search = new Search($("#listSearchDiv")[0], editionAdvancedSearch, editionBasicSearch);
    search.makeSearch();
    function pageClickCallbackForBiblModule(pageNumber) {
        if (search.isLastQueryJson()) {
            editionAdvancedSearch(search.getLastQuery(), pageNumber);
        }
        else {
            editionBasicSearch(search.getLastQuery(), pageNumber);
        }
    }
    bibliographyModule.createPagination(10, pageClickCallbackForBiblModule, 500); //enable pagination
    //var searchBox = new SearchBox("#searchbox", "Editions/Editions");
    //searchBox.addDataSet("Title", "Názvy");
    //searchBox.addDataSet("Author", "Autoři");
    //searchBox.create();
});
//# sourceMappingURL=itjakub.editions.search.js.map