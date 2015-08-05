$(document).ready(function () {
    var searchBox = new SearchBox("#searchbox", "OldGrammar/OldGrammar");
    searchBox.addDataSet("Title", "Názvy");
    searchBox.addDataSet("Author", "Autoři");
    searchBox.create();
});
