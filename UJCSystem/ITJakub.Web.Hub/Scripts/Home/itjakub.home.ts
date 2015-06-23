$(document).ready(() => {
    var searchBox = new SearchBox("#searchbox", "Home");
    searchBox.addDataSet("Title", "Názvy");
    searchBox.addDataSet("Author", "Autoři");
    searchBox.create();
});