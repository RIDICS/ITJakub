$(document.documentElement).ready(() => {
    var searchBox = new SearchBox("#searchbox", "Home");
    searchBox.addDataSet("Title", localization.translate("Titles", "ItJakubJs").value);
    searchBox.addDataSet("Author", localization.translate("Authors", "ItJakubJs").value);
    searchBox.addDataSet("DictionaryHeadword", localization.translate("DictionaryHeadword", "ItJakubJs").value);
    searchBox.create();
});