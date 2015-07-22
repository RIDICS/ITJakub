
$(document).ready(() => {

    var bookType = $("#listResults").data("book-type");
    var bibliographyModule = new BibliographyModule("#listResults", "#listResultsHeader", bookType);

    function editionBasicSearch(text: string, pageNumber?: number) {
        if (typeof text === "undefined" ||text === null || text === "") return;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/TextSearch",
            data: { text: text, pageNumber: pageNumber},
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);
                alert("done text");
            }
        });
    }

    function editionAdvancedSearch(json: string, pageNumber? : number) {
        if (typeof json === "undefined" || json === null || json === "") return;

        $.ajax({
            type: "GET",
            traditional: true,
            url: getBaseUrl() + "Editions/Editions/AdvancedSearch",
            data: { json: json, pageNumber: pageNumber },
            dataType: 'json',
            contentType: 'application/json',
            success: response => {
                bibliographyModule.showBooks(response.books);
                alert("done json");

            }
        });
    }

    var search = new Search(<any>$("#listSearchDiv")[0], editionAdvancedSearch, editionBasicSearch);
    search.makeSearch();

    function pageClickCallbackForBiblModule(pageNumber: number) {
        
        if (search.isLastQueryJson()) {
            editionAdvancedSearch(search.getLastQuery(), pageNumber);
        } else {
            editionBasicSearch(search.getLastQuery(), pageNumber);
        }
    }

    bibliographyModule.createPagination(10, pageClickCallbackForBiblModule, 500); //enable pagination
    

    var callbackDelegate = new DropDownSelectCallbackDelegate();
    callbackDelegate.selectedChangedCallback = (state: State) => {
        //TODO add cataegory ids and book ids to request
    };

    var editionsSelector = new DropDownSelect2("#dropdownSelectDiv", getBaseUrl() + "Editions/Editions/GetEditionsWithCategories", true, callbackDelegate);
    editionsSelector.makeDropdown();
});

