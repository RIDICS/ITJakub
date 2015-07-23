//window.onload = () => { alert("hello from editions!"); }
function searchCallback(json) {
    if (typeof json === "undefined" || json === null || json === "")
        return;
    var bookIds = new Array();
    var categoryIds = new Array();
    $.ajax({
        type: "GET",
        traditional: true,
        url: getBaseUrl() + "Editions/Editions/AdvancedSearchResultsCount",
        data: { json: json, selectedBookIds: bookIds, selectedCategoryIds: categoryIds },
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
        }
    });
}
//# sourceMappingURL=itjakub.editions.js.map