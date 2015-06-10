$(document).ready(function () {
    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader");
    $.ajax({
        type: "GET",
        traditional: true,
        url: getBaseUrl() + "Editions/Editions/SearchEditions",
        data: {},
        dataType: "json",
        contentType: "application/json",
        success: function (response) {
            bibliographyModule.showBooks(response.books);
        }
    });
});
//# sourceMappingURL=itjakub.editions.list.js.map