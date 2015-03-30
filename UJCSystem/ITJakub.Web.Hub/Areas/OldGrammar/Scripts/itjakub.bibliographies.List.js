$(document).ready(function () {
    var bibliographyModule = new BibliographyModule("#biblListResults", "#biblListResultsHeader");

    $('#searchButton').click(function () {
        var text = $('#searchbox').val();

        $.ajax({
            type: "GET",
            traditional: true,
            url: "/Bibliographies/Bibliographies/Search",
            data: { term: text },
            dataType: 'json',
            contentType: 'application/json',
            success: function (response) {
                bibliographyModule.showBooks(response.books);
            }
        });
    });
});
//# sourceMappingURL=itjakub.bibliographies.List.js.map
