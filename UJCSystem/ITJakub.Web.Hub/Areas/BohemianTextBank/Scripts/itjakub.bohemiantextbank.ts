$(document).ready(() => {
    var searchBox = new SearchBox("#searchbox", "BohemianTextBank/BohemianTextBank");
    searchBox.addDataSet("Title", "Názvy");
    searchBox.addDataSet("Author", "Autoři");
    searchBox.create();
});