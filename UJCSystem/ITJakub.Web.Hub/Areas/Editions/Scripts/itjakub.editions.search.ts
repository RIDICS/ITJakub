
$(document).ready(() => {

    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader");

    $('#searchButton').click(() => {
        var text = $('#searchbox').val();

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/SearchEditions",
            data: { term: text },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);

            }
        });
    });

});