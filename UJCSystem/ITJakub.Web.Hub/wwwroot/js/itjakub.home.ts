$(document.documentElement).ready(() => {
    const searchBoxSelector = "#searchbox";
    const searchBox = new SearchBox(searchBoxSelector, "Home");
    searchBox.addDataSet("Title", localization.translate("Titles", "ItJakubJs").value);
    searchBox.create();

    var submitSearchFunction = () => {
        const searchedValue = $(searchBoxSelector).val() as string;
        if (searchedValue !== "") {
            const url = $("#searchUrl").data("search-url");
            const escapedValue = encodeURIComponent(searchedValue);
            window.location.replace(url + "?search=" + escapedValue);
        }
    }

    $(".searchbar .search").on("click", () => {
        submitSearchFunction();
    });

    $(searchBoxSelector).on("keypress", e => {
        if (e.which === 13) { // enter
            submitSearchFunction();
        }
    });
});