
$(document).ready(() => {

    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader");

    $.ajax({
        type: "GET",
        traditional: true,
        url: getBaseUrl() + "Editions/Editions/SearchEditions",
        data: {},
        dataType: "json",
        contentType: "application/json",
        success: (response) => {
            bibliographyModule.showBooks(response.books);

        }
    });


});