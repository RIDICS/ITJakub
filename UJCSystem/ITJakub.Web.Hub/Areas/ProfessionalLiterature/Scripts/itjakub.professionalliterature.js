$(document).ready(function () {
    var searchBox = new SearchBox("#searchbox", "ProfessionalLiterature/ProfessionalLiterature");
    searchBox.addDataSet("Title", "Názvy");
    searchBox.addDataSet("Author", "Autoři");
    searchBox.create();
});
