$(document).ready(function () {
    var bookType = $("#listResults").data("book-type");
    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", bookType);
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