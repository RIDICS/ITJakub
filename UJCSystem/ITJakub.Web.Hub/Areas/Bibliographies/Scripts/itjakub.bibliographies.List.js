$(document).ready(function () {
    var bibliographyModule = new BibliographyModule("#biblListResults", "#biblListResultsHeader");
    $('#searchButton').click(function () {
        var text = $('#searchbox').val();
        bibliographyModule.clearBooks();
        bibliographyModule.showLoading();
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Bibliographies/Bibliographies/SearchTerm",
            data: { term: text },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                bibliographyModule.showBooks(response.books);
            }
        });
    });
    var searchBox = new SearchBox("#searchbox", "Bibliographies/Bibliographies");
    searchBox.addDataSet("Title", "Názvy");
    searchBox.addDataSet("Author", "Autoři");
    searchBox.create();
});
//# sourceMappingURL=itjakub.bibliographies.List.js.map