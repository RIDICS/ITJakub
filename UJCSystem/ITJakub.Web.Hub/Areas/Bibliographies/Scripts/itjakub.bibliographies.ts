


//TODO methods below are for testing purposes of Bibliography module
$(document).ready(function() {
    var searchPlugin = new SearchModule(new BibliographyModule("#biblListResults", "#biblListResultsHeader"));


    var arrayOfIds: string[] = new Array();
    for (var i = 0; i < 10; i++) {
        arrayOfIds.push(i.toString());
    }
    searchPlugin.getBookWithIds(arrayOfIds);


});