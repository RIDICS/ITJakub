$(document).ready(function () {
    var searchBox = new SearchBox("#searchbox", "Home");
    searchBox.addDataSet("Title", "Názvy");
    searchBox.addDataSet("Author", "Autoři");
    searchBox.addDataSet("DictionaryHeadword", "Slovníková hesla");
    searchBox.create();
});
