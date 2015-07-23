//window.onload = () => { alert("hello from editions!"); }

function searchCallback(json: string) {
    if (typeof json === "undefined" || json === null || json === "") return;

    var bookIds = new Array();
    var categoryIds = new Array();

    $.ajax({
        type: "GET",
        traditional: true,
        url: getBaseUrl() + "Editions/Editions/AdvancedSearchResultsCount",
        data: { json: json, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
        dataType: 'json',
        contentType: 'application/json',
        success: response => {
        }

    });
}