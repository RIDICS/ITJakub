$(document.documentElement).ready(() => {
    const searchBoxSelector = "#searchbox";
    const searchBox = new SearchBox(searchBoxSelector, "Home");
    searchBox.addDataSet("Title", localization.translate("Titles", "ItJakubJs").value);
    searchBox.create();

    $(".searchbar .search").on("click", () => {
        const searchedValue = $(searchBoxSelector).val();
        if (searchedValue !== "") {
            if (getPortalType() == PortalType.CommunityPortal) {
                window.location.replace(getBaseUrl() + "Editions/Editions/List?search=" + searchedValue);
            } else {
                window.location.replace(getBaseUrl() + "Bibliographies/Bibliographies/Search?search=" + searchedValue);
            }
        }
    });
});