$(document.documentElement).ready(() => {
    const searchBoxSelector = "#searchbox";
    const searchBox = new SearchBox(searchBoxSelector, "Home");
    searchBox.addDataSet("Title", localization.translate("Titles", "ItJakubJs").value);
    searchBox.create();

    $(".searchbar .search").on("click", () => {
        const searchedValue = $(searchBoxSelector).val();
        if (searchedValue !== "") {
            const url = $("#searchUrl").data("search-url");
            window.location.replace(url + "?search=" + searchedValue);
        }
    });
});