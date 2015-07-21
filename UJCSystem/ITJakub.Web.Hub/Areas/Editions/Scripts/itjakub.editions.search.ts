﻿
$(document).ready(() => {


    var bookType = $("#listResults").data("book-type");
    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", bookType);

    function pageClickCallbackForBiblModule(pageNumber: number) {
        alert("showing number: " + pageNumber);
        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/SearchEditions",
            data: { term: pageNumber }, //TODO 
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);
            }
        });
    }

    bibliographyModule.createPagination(10, pageClickCallbackForBiblModule, 500); //enable pagination

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

    var searchBox = new SearchBox("#searchbox", "Editions/Editions");
    searchBox.addDataSet("Title", "Názvy");
    searchBox.addDataSet("Author", "Autoři");
    searchBox.create();



});

