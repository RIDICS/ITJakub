//window.onload = () => { alert("hello from editions!"); }
//TODO methods below are for testing purposes of Bibliography module
$(document).ready(function () {
    var searchPlugin = new SearchModule(new BibliographyModule('#bibliographyList', '#sortBarBibList'));

    $('#fillBibList10').click(function () {
        var arrayOfIds = new Array();
        for (var i = 0; i < 10; i++) {
            arrayOfIds.push(i.toString());
        }
        searchPlugin.getBookWithIds(arrayOfIds);
    });

    $('#fillBibList3').click(function () {
        var arrayOfIds = new Array();
        for (var i = 0; i < 3; i++) {
            arrayOfIds.push(i.toString());
        }
        searchPlugin.getBookWithIds(arrayOfIds);
    });
    $('#fillBibList1000').click(function () {
        var arrayOfIds = new Array();
        for (var i = 0; i < 1000; i++) {
            arrayOfIds.push(i.toString());
        }
        searchPlugin.getBookWithIds(arrayOfIds);
    });

    $('#fillBibListTypeEdition').click(function () {
        searchPlugin.getBookWithType('Edition');
    });
    $('#fillBibListTypeDictionary').click(function () {
        searchPlugin.getBookWithType('Dictionary');
    });

    var readerPlugin = new ReaderModule('#Reader');
    var book = new BookInfo();
    readerPlugin.makeReader(book);
});
//# sourceMappingURL=itjakub.bibliographies.js.map
